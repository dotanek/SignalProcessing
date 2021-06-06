using LiveCharts;
using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;

namespace SignalProcessing.Model
{
    [Serializable]
    public class Signal : ISerializable
    {
        public double StartTime { get; }
        public double Duration { get; }
        public double Period { get; }
        public bool Discrete { get; set; }
        public double Frequency { get; }
        public List<Point> Values { get; set; }
        
        public List<Complex> ComplexValue { get; set; }

        public Signal(double startTime, double duration, double period, bool discrete, double frequency, List<Point> values)
        {
            StartTime = startTime;
            Period = period;
            Duration = duration;
            Values = values;
            Discrete = discrete;
            Frequency = frequency;
            ComplexValue = new List<Complex>();
        }

        public double Average()
        {
            if (Discrete)
            {
                return Values.Sum(e => e.Y) / Values.Count;
            }
             
            double step = 1.0 / Frequency;

            return step * Values.Sum(e => e.Y) / Duration;
        }

        public double AbsoluteAverage()
        {
            if (Discrete)
            {
                return Values.Sum(e => Math.Abs(e.Y)) / Values.Count;
            }

            double step = 1.0 / Frequency;

            return step * Values.Sum(e => Math.Abs(e.Y)) / Duration;
        }
        public double RootMeanSquare()
        {
            return Math.Sqrt(AveragePower());
        }

        public double Variation()
        {
            double average = Average();

            if (Discrete)
            {
                return Values.Sum(e => (e.Y - average)*(e.Y - average)) / Values.Count;
            }

            double step = 1.0 / Frequency;

            return step * Values.Sum(e => (e.Y - average) * (e.Y - average)) / Duration;
        }

        public double AveragePower()
        {
            if (Discrete)
            {
                return Values.Sum(e => e.Y * e.Y) / Values.Count;
            }

            double step = 1.0 / Frequency;

            return step * Values.Sum(e => e.Y * e.Y) / Duration;
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

            foreach (Point value in Values)
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

        // Serialization
        public Signal(SerializationInfo info, StreamingContext context)
        {
            StartTime = (double)info.GetValue("StartTime", typeof(double));
            Period = (double)info.GetValue("Period", typeof(double));
            Frequency = (double)info.GetValue("Frequency", typeof(double));

            List<double> values = (List<double>)info.GetValue("Values", typeof(List<double>));
            //List<Complex> complexValues = (List<Complex>)info.GetValue("ComplexValue", typeof(List<Complex>));
            ComplexValue = (List<Complex>)info.GetValue("ComplexValue", typeof(List<Complex>));
            Values = new List<Point>();
            double step = 1.0 / Frequency;
            for (int i = 0; i < values.Count; i++)
            {
                Values.Add(
                    new Point
                    {
                        X = StartTime + i * step,
                        Y = values.ElementAt(i)
                    }
                );
            }

            Duration = Values.Count * step;
            Discrete = true;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("StartTime", StartTime);
            info.AddValue("Period", Period);
            info.AddValue("Frequency", Frequency);
            info.AddValue("Values", Values.Select(v => v.Y).ToList());
            info.AddValue("ComplexValue", ComplexValue);
        }

        override 
        public String ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("StartTime: " + StartTime);
            stringBuilder.AppendLine("Period: " + Period);
            stringBuilder.AppendLine("Frequency: " + Frequency);
            stringBuilder.AppendLine("Values:");
            foreach (var point in Values)
            {
                stringBuilder.AppendLine(Math.Round(point.Y, 4).ToString("0.0000"));
            }

            return stringBuilder.ToString();
        }
    }
}
