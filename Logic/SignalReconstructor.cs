using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using SignalProcessing.Model;

namespace SignalProcessing.Logic
{
    public class SignalReconstructor
    {
        
        public Signal ZeroOrderHold(Signal source)
        {
            Signal output = new Signal(source.StartTime, source.Duration, source.Period, source.Discrete, source.Frequency, source.Values);
            List<Point> reconstructedValues = new List<Point>();
            double step = 1.0 / source.Frequency / 4.0;
            for (int i = 0; i < source.Values.Count - 1; i++)
            {
                for (double j = source.Values[i].X; j < source.Values[i + 1].X; j += step)
                {
                    reconstructedValues.Add(new Point(j, source.Values[i].Y));
                }
            }

            output.Values = reconstructedValues;
            return output;
        }

        public Signal SincInterpolation(Signal source)
        {
            Signal output = new Signal(source.StartTime, source.Duration, source.Period, source.Discrete, source.Frequency, source.Values);
            List<Point> reconstructedValues = new List<Point>();
            double step = 1.0 / source.Frequency / 4.0;

            for (double t = source.Values.First().X; reconstructedValues.Count < source.Values.Count*4; t += step)
            {
                var sum = 0.0;
                for (int j = 0; j < source.Values.Count; j++)
                {
                    sum += source.Values[j].Y * Sinc(t / (step*4) - j);
                }
                reconstructedValues.Add(new Point(t, sum));
            }
            
            output.Values = reconstructedValues;
            return output;
        }

        private double Sinc(double t)
        {
            if (t == 0)
            {
                return 1;
            }
            return Math.Sin(Math.PI * t) / Math.PI / t;
        }
            
        
    }
}