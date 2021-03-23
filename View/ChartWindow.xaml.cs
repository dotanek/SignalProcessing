using System;
using System.Linq;
using System.Text;
using System.Windows;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using SignalProcessing.Model;

namespace SignalProcessing.View
{
    public partial class ChartWindow : Window
    {

        public ChartWindow(Signal signal, int sectionAmount = 15)
        {
            InitializeComponent();
            var sb = new StringBuilder();
            sb.Append("Average = ").Append(Math.Round( signal.Average(),6));
            sb.Append("\nAbsoluteAverage = ").Append(Math.Round(signal.AbsoluteAverage(),6));
            sb.Append("\nRootMeanSquare = ").Append(Math.Round(signal.RootMeanSquare(),6));
            sb.Append("\nVariation = ").Append(Math.Round(signal.Variation(),6));
            sb.Append("\nAveragePower = ").Append(Math.Round(signal.AveragePower(),6));
            sb.Append("\n================\n").Append(signal.ToString());
            SignalText.Text = sb.ToString();
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
                    Values = new ChartValues<ObservablePoint>(signal.GetHistogramPlot(sectionAmount)),
                    PointGeometry = null,
                }
            };
        }


    }
}