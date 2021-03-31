using LiveCharts.Defaults;
using SignalProcessing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalProcessing.Logic
{
    class SignalGenerator
    {
        public double Amplitude { get; set; }
        public double StartTime { get; set; }
        public double Duration { get; set; }
        public double Period { get; set; }
        public double FillFactor { get; set; }
        public double JumpTime { get; set; } // Used for both Jump and Impulse signals.
        public double Probability { get; set; }
        public double Frequency { get; set; }

        private Random Random; 

        public enum Type
        {
            UniformDistributionNoice, // S1
            GaussianNoice, // S2
            Sinusoidal, // S3
            OneHalfRectSinusoidal, // S4
            TwoHalfRectSinusoidal, // S5
            Rectangular, // S6
            SymetricRectangular, // S7
            Triangular, // S8
            UnitJump, // S9
            UnitImpulse, // S10
            ImpulseNoice // S11
        }

        public SignalGenerator() {
            Random = new Random();
            Amplitude = 1d;
            StartTime = 0d;
            Duration = 10d;
            Period = 1d;
            FillFactor = 0.5d;
            JumpTime = 5d;
            Probability = 0.5d;
            Frequency = 10d;
        }

        public delegate double Generator(double time);

        public Signal Generate(Type type)
        {
            Generator generator = null;

            bool discrete = false;

            switch (type)
            {
                case Type.UniformDistributionNoice: generator = UniformDistributionNoice; break;
                case Type.GaussianNoice: generator = GaussianNoice; break;
                case Type.Sinusoidal: generator = Sinusoidal; break;
                case Type.OneHalfRectSinusoidal: generator = OneHalfRectSinusoidal; break;
                case Type.TwoHalfRectSinusoidal: generator = TwoHalfRectSinusoidal; break;
                case Type.Rectangular: generator = Rectangular; break;
                case Type.SymetricRectangular: generator = SymetricRectangular; break;
                case Type.Triangular: generator = Triangular; break;
                case Type.UnitJump: generator = UnitJump; break;
                case Type.UnitImpulse: generator = UnitImpulse; discrete = true; break;
                case Type.ImpulseNoice: generator = ImpulseNoice; discrete = true; break;
                default: throw new Exception("Unknown signal type.");
            }

            List<ObservablePoint> values = new List<ObservablePoint>();

            double step = 1.0 / Frequency;

            for (double t = StartTime; t <= StartTime + Duration; t += step)
            {
                values.Add(
                    new ObservablePoint
                    {
                        X = t,
                        Y = generator(t)
                    }
                );
            }

            return new Signal(StartTime, Duration, Period, discrete, Frequency, values);
        }

        // Signal generators.

        private double UniformDistributionNoice(double time)
        {
            return Random.NextDouble() * 2 * Amplitude - Amplitude;
        }

        private double GaussianNoice(double time) // Inspired by the double dice throw distribution.
        {
            double multiplier = 2.0 * Amplitude / 5.0; 
            double random = 0;

            for (int i = 0; i < 5; i++)
            {
                random += Random.NextDouble() * multiplier;
            }

            return random - Amplitude;
        }

        private double Sinusoidal(double time)
        {
            return Amplitude * Math.Sin((2 * Math.PI / Period) * (time - StartTime));
        }

        private double OneHalfRectSinusoidal(double time)
        {
            return Amplitude / 2.0 * (Sinusoidal(time) + Math.Abs(Sinusoidal(time)));
        }

        private double TwoHalfRectSinusoidal(double time)
        {
            return Amplitude * Math.Abs(Sinusoidal(time));
        }

        private double Rectangular(double time)
        {
            int k = (int)(time / Period);

            if (time > k * Period + StartTime && time < k * Period + StartTime + (FillFactor * Period))
            {
                return Amplitude;
            }

            return 0;
        }

        private double SymetricRectangular(double time)
        {
            int k = (int)(time / Period);

            if (time >= k * Period + StartTime && time < k * Period + StartTime + (FillFactor * Period))
            {
                return Amplitude;
            }

            return -Amplitude;
        }

        private double Triangular(double time)
        {
            int k = (int)(time / Period);

            if (time >= k * Period + StartTime && time < k * Period + StartTime + (FillFactor * Period))
            {
                return Amplitude / (FillFactor * Period) * (time - k * Period - StartTime);
            }

            return -Amplitude / (Period * (1.0 - FillFactor)) * (time - k * Period - StartTime) + Amplitude / (1.0 - FillFactor);
        }

        private double UnitJump(double time)
        {
            if (time > JumpTime)
            {
                return Amplitude;
            }

            if (time == JumpTime)
            {
                return Amplitude / 2.0;
            }

            return 0;
        }

        private double UnitImpulse(double time)
        {
            double step = 1.0 / Frequency;
            if (JumpTime >= time && JumpTime < time + step)
            {
                return Amplitude;
            }

            return 0;
        }

        private double ImpulseNoice(double time)
        {
            double random = Random.NextDouble();

            if (random <= Probability)
            {
                return Amplitude;
            }

            return 0;
        }
    }
}
