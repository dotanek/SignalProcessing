using LiveCharts.Defaults;
using SignalProcessing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SignalProcessing.Logic
{
    public class Antenna
    {
        // Sensor
        public double SignalPeriod { get; set; }
        public double SamplingFrequency { get; set; }
        public double BufforSize { get; set; }

        // Buffers
        public List<double> BaseSignalSamples;
        public List<double> FeedbackSignalSamples;
        public List<double> CorrelatedSignalSamples;

        public Antenna()
        {
            BaseSignalSamples = new List<double>();
            FeedbackSignalSamples = new List<double>();
            CorrelatedSignalSamples = new List<double>();

            SignalPeriod = 1;
            SamplingFrequency = 100;
            BufforSize = 1000;
        }

        public void SampleSignals(double time, double delay)
        {
            BaseSignalSamples.Clear();
            FeedbackSignalSamples.Clear();


            double step = 1.0 / SamplingFrequency;

            for (int i = 0; i < BufforSize; i++)
            {
                BaseSignalSamples.Add(GetProbeSample(time + i * step));
                FeedbackSignalSamples.Add(GetProbeSample(time + i * step - delay));
            }
        }

        public void CorrelateSignals()
        {
            CorrelatedSignalSamples.Clear();
            List<double> PaddedFeedbackSignal = new List<double>(FeedbackSignalSamples);

            for (int i = 0; i < BufforSize - 1; i++)
            {
                PaddedFeedbackSignal.Insert(0, 0);
                PaddedFeedbackSignal.Add(0);
            }

            for (int i = 0; i < BufforSize * 2 - 1; i++)
            {
                double sum = 0.0;

                for (int j = 0; j < BufforSize; j++)
                {
                    sum += BaseSignalSamples.ElementAt(j) * PaddedFeedbackSignal.ElementAt(j + i);
                }

                CorrelatedSignalSamples.Add(sum);
            }
        }

        public double MeasureDelay()
        {
            List<double> correlationSignalSamplesHalf = CorrelatedSignalSamples.Skip(CorrelatedSignalSamples.Count / 2).ToList();

            int maxIndex = 0;

            for (int i = 1; i < correlationSignalSamplesHalf.Count; i++)
            {
                if (correlationSignalSamplesHalf.ElementAt(i) > correlationSignalSamplesHalf.ElementAt(maxIndex))
                {
                    maxIndex = i;
                }
            }

            return maxIndex * (1 / SamplingFrequency);
        }

        public Signal GetCorrelationAsSignal()
        {
            List<Point> values = new List<Point>();
            double step = 1 / SamplingFrequency;

            for (int i = 0; i < CorrelatedSignalSamples.Count; i++)
            {
                values.Add(new Point(i * step, CorrelatedSignalSamples.ElementAt(i)));
            }

            return new Signal(0, 0, 0, true, SamplingFrequency, values);
        }

        private double GetProbeSample(double time)
        {
            double sin1Sample = Math.Sin((2 * Math.PI / SignalPeriod) * time);
            double sin2Sample = Math.Sin((2 * Math.PI / (SignalPeriod/2)) * time);
            double sin3Sample = Math.Sin((2 * Math.PI / (SignalPeriod/4)) * time);

            return sin1Sample + sin2Sample + sin3Sample;
        }
    }
}
