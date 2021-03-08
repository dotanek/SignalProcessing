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

        public MainWindow()
        {
            InitializeComponent();
            Chart = (CartesianChart)FindName("Test");

            SignalGenerator signalGenerator = new SignalGenerator(2,0,10,0.1);
            Signal signal = signalGenerator.Generate(SignalGenerator.Type.Sinusoidal);

           /* Chart.AxisY.Clear();
            Chart.AxisY.Add(
                new Axis
                {
                    MinValue = signal.Values.Min() - 1,
                    MaxValue = signal.Values.Max() + 1
                }
             );*/

            Chart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Values = signal.GetPlottableValues()
                }
            };
        }
    }
}
