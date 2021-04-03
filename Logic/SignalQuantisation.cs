using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using SignalProcessing.Model;

namespace SignalProcessing.Logic
{
    public class SignalQuantisation
    {
        private readonly int _numberOfLevels;

        public SignalQuantisation(int bits)
        {
            _numberOfLevels = (int) Math.Pow(2, bits);
        }

        public Signal Quantise(Signal source)
        {
            var levels = GetLevels(source);

            var values = source.Values.Select(e => e.Y)
                .Select(value =>
                    (int) Math.Floor( (value - levels.First()) / (levels.Last() - levels.First()) * (levels.Count -1) ))
                .Select(levelIndex => levels[levelIndex]).ToList();
            
            var quantised = new Signal(source.StartTime, source.Duration, source.Period, source.Discrete, source.Frequency, source.Values);
            quantised.Values = quantised.Values.Select((p, i) => new Point(p.X, values[i])).ToList();

            return quantised;
        }

        private List<double> GetLevels(Signal source)
        {
            List<double> levels = new List<double>();
            double min = source.Values.Min(e => e.Y);
            double max = source.Values.Max(e => e.Y);

            double step = (max - min) / (_numberOfLevels - 1);

            for (int i = 0; i < _numberOfLevels; i++)
            {
                levels.Add(i * step + min);
            }

            return levels;
        }
        
        
    }
}