using LiveCharts.Defaults;
using SignalProcessing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            List<ObservablePoint> values = new List<ObservablePoint>();

            for (int i = 0; i < s1.Values.Count; i++)
            {
                values.Add(
                    new ObservablePoint
                    {
                        X = s1.Values[i].X,
                        Y = s1.Values[i].Y + s2.Values[i].Y
                    }
                );
            }

            return new Signal(s1.StartTime, s1.Duration,s1.Period, s1.Discrete, s1.SampleAmount, values);
        }

        public static Signal Subtract(Signal s1, Signal s2)
        {
            if (!CheckCompatibility(s1, s2))
            {
                throw new Exception("Incompatible signals.");
            }

            List<ObservablePoint> values = new List<ObservablePoint>();

            for (int i = 0; i < s1.Values.Count; i++)
            {
                values.Add(
                    new ObservablePoint
                    {
                        X = s1.Values[i].X,
                        Y = s1.Values[i].Y - s2.Values[i].Y
                    }
                );
            }

            return new Signal(s1.StartTime, s1.Duration, s1.Period, s1.Discrete, s1.SampleAmount, values);
        }

        public static Signal Multiply(Signal s1, Signal s2)
        {
            if (!CheckCompatibility(s1, s2))
            {
                throw new Exception("Incompatible signals.");
            }

            List<ObservablePoint> values = new List<ObservablePoint>();

            for (int i = 0; i < s1.Values.Count; i++)
            {
                values.Add(
                    new ObservablePoint
                    {
                        X = s1.Values[i].X,
                        Y = s1.Values[i].Y * s2.Values[i].Y
                    }
                );
            }

            return new Signal(s1.StartTime, s1.Duration, s1.Period, s1.Discrete, s1.SampleAmount, values);
        }

        public static Signal Divide(Signal s1, Signal s2)
        {
            if (!CheckCompatibility(s1, s2))
            {
                throw new Exception("Incompatible signals.");
            }

            List<ObservablePoint> values = new List<ObservablePoint>();

            for (int i = 0; i < s1.Values.Count; i++)
            {
                values.Add(
                    new ObservablePoint
                    {
                        X = s1.Values[i].X,
                        Y = s2.Values[i].Y != 0 ? s1.Values[i].Y / s2.Values[i].Y : s1.Values[i].Y
                    }
                );
            }

            return new Signal(s1.StartTime, s1.Duration, s1.Period, s1.Discrete, s1.SampleAmount, values);
        }

        private static bool CheckCompatibility(Signal s1, Signal s2)
        {
            if (s1.StartTime != s2.StartTime) return false;
            if (s1.Duration != s2.Duration) return false;
            if (s1.SampleAmount != s2.SampleAmount) return false;
            return true;
        }
    }
}
