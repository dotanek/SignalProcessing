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
                for (double j = 0; j < 4; j ++)
                {
                    reconstructedValues.Add(new Point(source.Values[i].X + j * step, source.Values[i].Y));
                }
            }

            output.Values = reconstructedValues;
            return output;
        }

        public Signal SincInterpolation(Signal source, int range)
        {
            Signal output = new Signal(source.StartTime, source.Duration, source.Period, source.Discrete, source.Frequency, source.Values);
            List<Point> reconstructedValues = new List<Point>();
            double step = 1.0 / source.Frequency / 4.0;
            if (range > source.Values.Count)
            {
                range = source.Values.Count;
            }
            for (double t = source.Values.First().X; reconstructedValues.Count < source.Values.Count*4; t += step)
            {
                var sum = 0.0;
                int currentSampleIndex = reconstructedValues.Count / 4;
                if (currentSampleIndex + range > source.Values.Count)
                {
                    currentSampleIndex = source.Values.Count - range;
                }
                for (int j = currentSampleIndex; j < currentSampleIndex + range; j++)
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