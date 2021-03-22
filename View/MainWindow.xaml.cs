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
using SignalProcessing.View;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SignalProcessing
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public CartesianChart SignalChart;
        public CartesianChart Histogram;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        public MainWindow()
        {
            InitializeComponent();
            SignalComboBox.ItemsSource = Enum.GetValues(typeof(SignalGenerator.Type)).Cast<SignalGenerator.Type>();
        }

        public void ShowChart(object obj, RoutedEventArgs routedEventArgs)
        {

            
          
            SignalChart = (CartesianChart)FindName("SignalChart");
            Histogram = (CartesianChart)FindName("Histogram");

            SignalGenerator signalGenerator = new SignalGenerator
            {
                Period = Math.PI,
                Frequency = 100,
                Duration = 10
            };
            signalGenerator.JumpTime = 5;
            
            Signal signal = signalGenerator.Generate((SignalGenerator.Type)SignalComboBox.SelectionBoxItem);

            Stream stream = File.Open("Signal.sig", FileMode.Create);
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            binaryFormatter.Serialize(stream, signal);

            stream.Close();

            signal = null;

            stream = File.Open("Signal.sig", FileMode.Open);

            signal = (Signal)binaryFormatter.Deserialize(stream);

            Window chartWindow = new ChartWindow(signal);
            chartWindow.Show();
        }
    }
}
