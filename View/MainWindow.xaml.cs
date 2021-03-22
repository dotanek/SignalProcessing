using SignalProcessing.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using LiveCharts;
using LiveCharts.Wpf;
using SignalProcessing.Model;
using LiveCharts.Defaults;

namespace SignalProcessing
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public CartesianChart Chart;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        public MainWindow()
        {
            InitializeComponent();
            Chart = (CartesianChart)FindName("Test");

            SignalGenerator signalGenerator = new SignalGenerator
            {
                Period = 1
            };
            signalGenerator.JumpTime = 2.5;
            Signal signal = signalGenerator.Generate(SignalGenerator.Type.Triangular);
            
            Signal signal2 = signalGenerator.Generate(SignalGenerator.Type.Sinusoidal);
            Signal signal3 = SignalOperations.Add(signal, signal2);

            signal2.Average();

            Chart.AxisY.Clear();
            Chart.AxisY.Add(
                new Axis
                {
                    /*MinValue = 0,*/
                }
            );

            double separator = (signal.Values.Max(v => v.Y) - signal.Values.Min(v => v.Y)) / 15;

            Chart.AxisX.Clear();
            Chart.AxisX.Add(
                new Axis
                {
                    Separator = new LiveCharts.Wpf.Separator
                    {
                        Step = separator
                    },
                }
            );

            Chart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<ObservablePoint>(signal3.Values),
                    PointGeometry = null,
                }
            };

            /*Chart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Values = new ChartValues<ObservablePoint>(signal.GetHistogramPlot(15)),
                    PointGeometry = null,
                }
            };*/
        }
    }
}
