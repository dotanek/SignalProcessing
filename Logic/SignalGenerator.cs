using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalProcessing.Logic
{
    class SignalGenerator
    {
        private double Amplitude;
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

        public SignalGenerator(double amplitude)
        {
            Amplitude = amplitude;
            Rand = new Random();
        }

        public delegate double Generator(double time);

        public List<double> Generate(double startTime, double duration, double period, Type type)
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

            for (double t = startTime; t < startTime + duration; t += period)
            {
                values.Add(generator(t));
            }

            return values;
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
            return 0;
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
