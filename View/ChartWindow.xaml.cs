using System.Linq;
using System.Windows;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using SignalProcessing.Model;

namespace SignalProcessing.View
{
    public partial class ChartWindow : Window
    {

        public ChartWindow(Signal signal)
        {
            InitializeComponent();
            
            SignalChart.AxisY.Clear();
            SignalChart.AxisY.Add(
                new Axis
                {
                    /*MinValue = 0,*/
                }
            );
            
            Histogram.AxisY.Clear();
            Histogram.AxisY.Add(
                new Axis
                {
                    /*MinValue = 0,*/
                }
            );

            double separator = (signal.Values.Max(v => v.Y) - signal.Values.Min(v => v.Y)) / 15;

            SignalChart.AxisX.Clear();
            SignalChart.AxisX.Add(
                new Axis
                {
                    Separator = new LiveCharts.Wpf.Separator
                    {
                        Step = separator
                    },
                }
            );

            SignalChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<ObservablePoint>(signal.Values),
                    PointGeometry = null,
                }
            };

            Histogram.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Values = new ChartValues<ObservablePoint>(signal.GetHistogramPlot(15)),
                    PointGeometry = null,
                }
            };
        }


    }
}