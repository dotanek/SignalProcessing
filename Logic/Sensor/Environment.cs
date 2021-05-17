using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalProcessing.Logic.Sensor
{
    public class PhysicalEnvironment
    {
        public double TimeUnit { get; set; } // s
        public double DisperseVelocity { get; set; }  // m/s
        public double ObjectStartingDistance { get; set; }
        public double ObjectVelocity { get; set; }  // m/s
        public double ReportPeriodStart { get; set; }
        public double ReportPeriodEnd { get; set; }
        public Antenna Antenna { get; set; }

        public PhysicalEnvironment()
        {
            TimeUnit = 1;
            ReportPeriodStart = 0;
            ReportPeriodEnd = 20;
            DisperseVelocity = 1000; // Speed of sound m/s

            ObjectVelocity = 10;
            ObjectStartingDistance = 50;

            Antenna = new Antenna();
        }

        public double MeasureDistance(double time)
        {
            double objectDistance = ObjectStartingDistance + time * ObjectVelocity;
            double signalDelay = objectDistance / DisperseVelocity * 2;

            Antenna.SampleSignals(time,signalDelay);
            Antenna.CorrelateSignals();

            double measuredDelay = Antenna.MeasureDelay();
            double measuredDistance = (measuredDelay * DisperseVelocity) / 2;

            return measuredDistance;
        }

        public List<Tuple<double,double>> MeasureDistanceOnTimePeriod()
        {
            List<Tuple<double, double>> measurements = new List<Tuple<double, double>>();

            for (double t = ReportPeriodStart; t <= ReportPeriodEnd; t += TimeUnit)
            {
                double distance = MeasureDistance(t);
                measurements.Add(new Tuple<double,double>(t,distance));
            }

            return measurements;
        }
    }
}
