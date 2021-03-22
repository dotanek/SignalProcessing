using LiveCharts;
using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalProcessing.Model
{
    public class Signal
    {
        public double StartTime { get; }
        public double Duration { get; }
        public double Period { get; }
        public bool Discrete { get;  }
        public int SampleAmount { get; set; }
        public List<ObservablePoint> Values { get; }

        public Signal(double startTime, double duration, double period, bool discrete, int sampleAmount, List<ObservablePoint> values)
        {
            StartTime = startTime;
            Period = period;
            Duration = duration;
            Values = values;
            Discrete = discrete;
            SampleAmount = sampleAmount;
        }

        public double Average()
        {
            if (Discrete)
            {
            }
            return Values.Sum(e => e.Y) / Values.Count;
        }

        public double AbsoluteAverage()
        {
            return Values.Sum(e => Math.Abs(e.Y)) / Values.Count;
        }
        public double RootMeanSquare()
        {
            return 0d;
        }

        public double Variation()
        {
            return 0d;
        }

        public double AveragePower()
        {
            return 0d;
        }

        public List<ObservablePoint> GetHistogramPlot(int sectionAmount)
        {
            double minY = Values.Min(v => v.Y);
            double maxY = Values.Max(v => v.Y);

            double sectionRange = (maxY - minY) / sectionAmount;
            List<ObservablePoint> valueFrequencies = new List<ObservablePoint>();

            for (int i = 0; i < sectionAmount; i++)
            {
                valueFrequencies.Add(
                    new ObservablePoint
                    {
                        X = minY + i * sectionRange,
                        Y = 0
                    }
                );
            }

            foreach (ObservablePoint value in Values)
            {
                ObservablePoint valueFrequency = valueFrequencies.FirstOrDefault(vf => value.Y <= vf.X + sectionRange);
                if (valueFrequency != null)
                {
                    valueFrequency.Y += 1;
                }
            }

            foreach (ObservablePoint valueFrequency in valueFrequencies) // Moving label to the middle of the section.
            {
                valueFrequency.X += sectionRange / 2;
            }

            return valueFrequencies;
        }
    }
}
