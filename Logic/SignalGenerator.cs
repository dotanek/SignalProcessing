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

        private Random Rand; 

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

        public SignalGenerator(double amplitude, double startTime, double duration, double period)
        {
            Rand = new Random();
            Amplitude = amplitude;
            StartTime = startTime;
            Duration = duration;
            Period = period;
        }

        public delegate double Generator(double time);

        public Signal Generate(Type type)
        {
            Generator generator = null;

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
                case Type.UnitImpulse: generator = UnitImpulse; break;
                case Type.ImpulseNoice: generator = ImpulseNoice; break;
                default: throw new Exception("Unknown signal type.");
            }

            List<double> values = new List<double>();

            for (double t = StartTime; t <= StartTime + Duration; t += Period)
            {
                values.Add(generator(t));
            }

            return new Signal(StartTime, Period, values);
        }

        // Signal generators.

        private double UniformDistributionNoice(double time)
        {
            return Rand.NextDouble() * 2 * Amplitude - Amplitude;
        }

        private double GaussianNoice(double time)
        {
            return 0;
        }

        private double Sinusoidal(double time)
        {
            double test = (2 * Math.PI / Period) * (time - StartTime);
            double test2 = Math.Sin(test);
            return Amplitude * test2;
        }

        private double OneHalfRectSinusoidal(double time)
        {
            return 0;
        }

        private double TwoHalfRectSinusoidal(double time)
        {
            return 0;
        }

        private double Rectangular(double time)
        {
            return 0;
        }

        private double SymetricRectangular(double time)
        {
            return 0;
        }

        private double Triangular(double time)
        {
            return 0;
        }

        private double UnitJump(double time)
        {
            return 0;
        }

        private double UnitImpulse(double time)
        {
            return 0;
        }

        private double ImpulseNoice(double time)
        {
            return 0;
        }
    }
}
