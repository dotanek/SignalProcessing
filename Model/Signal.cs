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
        public List<double> Values { get; }

        public Signal(double startTime, double period, List<double> values)
        {
            StartTime = startTime;
            Period = period;
            Values = values;
        }

        public ChartValues<ObservablePoint> GetPlottableValues()
        {
            ChartValues<ObservablePoint> chartValues = new ChartValues<ObservablePoint>();

            for (int i = 0; i < Values.Count; i++)
            {
                chartValues.Add(
                    new ObservablePoint
                    {
                        X = StartTime + (i * Period),
                        Y = Values.ElementAt(i)
                    }
                );
            }

            return chartValues;
        }
    }
}
