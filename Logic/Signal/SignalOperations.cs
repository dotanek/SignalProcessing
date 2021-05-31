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
    public static class SignalOperations
    {
        public static Signal Add(Signal s1, Signal s2)
        {
            if (!CheckCompatibility(s1, s2))
            {
                throw new Exception("Incompatible signals.");
            }

            List<Point> values = new List<Point>();

            for (int i = 0; i < s1.Values.Count; i++)
            {
                values.Add(
                    new Point
                    {
                        X = s1.Values[i].X,
                        Y = s1.Values[i].Y + s2.Values[i].Y
                    }
                );
            }

            return new Signal(s1.StartTime, s1.Duration,s1.Period, s1.Discrete, s1.Frequency, values);
        }

        public static Signal Subtract(Signal s1, Signal s2)
        {
            if (!CheckCompatibility(s1, s2))
            {
                throw new Exception("Incompatible signals.");
            }

            List<Point> values = new List<Point>();

            for (int i = 0; i < s1.Values.Count; i++)
            {
                values.Add(
                    new Point
                    {
                        X = s1.Values[i].X,
                        Y = s1.Values[i].Y - s2.Values[i].Y
                    }
                );
            }

            return new Signal(s1.StartTime, s1.Duration, s1.Period, s1.Discrete, s1.Frequency, values);
        }

        public static Signal Multiply(Signal s1, Signal s2)
        {
            if (!CheckCompatibility(s1, s2))
            {
                throw new Exception("Incompatible signals.");
            }

            List<Point> values = new List<Point>();

            for (int i = 0; i < s1.Values.Count; i++)
            {
                values.Add(
                    new Point
                    {
                        X = s1.Values[i].X,
                        Y = s1.Values[i].Y * s2.Values[i].Y
                    }
                );
            }

            return new Signal(s1.StartTime, s1.Duration, s1.Period, s1.Discrete, s1.Frequency, values);
        }

        public static Signal Divide(Signal s1, Signal s2)
        {
            if (!CheckCompatibility(s1, s2))
            {
                throw new Exception("Incompatible signals.");
            }

            List<Point> values = new List<Point>();

            for (int i = 0; i < s1.Values.Count; i++)
            {
                values.Add(
                    new Point
                    {
                        X = s1.Values[i].X,
                        Y = s2.Values[i].Y != 0 ? s1.Values[i].Y / s2.Values[i].Y : s1.Values[i].Y
                    }
                );
            }

            return new Signal(s1.StartTime, s1.Duration, s1.Period, s1.Discrete, s1.Frequency, values);
        }

        public static Signal Convolute(Signal s1, Signal s2, int s1SampleAmount, int s2SampleAmount)
        {
            // Only taking as many samples as there is available.
            List<Point> v1 = s1SampleAmount < s1.Values.Count ? s1.Values.GetRange(0, s1SampleAmount) : new List<Point>(s1.Values);
            List<Point> v2 = s2SampleAmount < s2.Values.Count ? s2.Values.GetRange(0, s2SampleAmount) : new List<Point>(s2.Values);
            v2.Reverse();
            List<Point> v3 = new List<Point>();

            int v3SampleAmount = v1.Count + v2.Count - 1;

            // Padding up with zeroes.
            for (int i = 0; i < v2.Count - 1; i++)
            {
                v1.Insert(0, new Point(0, 0));
                v1.Add(new Point(0, 0));
            }

            double step = 1 / s1.Frequency;

            for (int i = 0; i < v3SampleAmount; i++)
            {
                v3.Add(new Point(i*step,ProductSum(v1,v2,i)));
            }

            return new Signal(s1.StartTime,s1.StartTime + v3.Count * step, s1.Period, s1.Discrete, s1.Frequency, v3);
        }

        public static Signal Correlate(Signal s1, Signal s2, int s1SampleAmount, int s2SampleAmount)
        {
            // Only taking as many samples as there is available.
            List<Point> v1 = s1SampleAmount < s1.Values.Count ? s1.Values.GetRange(0, s1SampleAmount) : new List<Point>(s1.Values);
            List<Point> v2 = s2SampleAmount < s2.Values.Count ? s2.Values.GetRange(0, s2SampleAmount) : new List<Point>(s2.Values);
            List<Point> v3 = new List<Point>();

            int v3SampleAmount = v1.Count + v2.Count - 1;

            // Padding up with zeroes.
            for (int i = 0; i < v2.Count - 1; i++)
            {
                v1.Insert(0, new Point(0, 0));
                v1.Add(new Point(0, 0));
            }

            double step = 1 / s1.Frequency;

            for (int i = 0; i < v3SampleAmount; i++)
            {
                v3.Add(new Point(i * step, ProductSum(v1, v2, i)));
            }

            return new Signal(s1.StartTime, s1.StartTime + v3.Count * step, s1.Period, s1.Discrete, s1.Frequency, v3);
        }

        private static double ProductSum(List<Point> v1, List<Point> v2, int index)
        {
            double sum = 0.0;

            for (int i = 0; i < v2.Count; i++)
            {
                sum += v2.ElementAt(i).Y * v1.ElementAt(i + index).Y;
            }

            return sum;
        }

        private static bool CheckCompatibility(Signal s1, Signal s2)
        {
            if (s1.StartTime != s2.StartTime) return false;
            if (s1.Duration != s2.Duration) return false;
            if (s1.Frequency != s2.Frequency) return false;
            return true;
        }
    }
}
