using LiveCharts;
using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalProcessing.Model
{
    class Signal
    {
        public double StartTime { get; }
        public double Duration { get; }
        public double Period { get; }
        public List<ObservablePoint> Values { get; }

        public Signal(double startTime, double period, List<ObservablePoint> values)
        {
            StartTime = startTime;
            Period = period;
            Values = values;
        }
    }
}
