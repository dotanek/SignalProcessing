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
        private Signal _loadedSignal1;
        private Signal _loadedSignal2;
        
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

        public void GenerateChart(object obj, RoutedEventArgs routedEventArgs)
        {

            SignalGenerator signalGenerator = new SignalGenerator
            {
                Period = Math.PI,
                Frequency = 100,
                Duration = 10
            };
            double temporaryInputValue;
            if (double.TryParse(InputAmplitude.Text, out temporaryInputValue))
            {
                signalGenerator.Amplitude = double.Parse(InputAmplitude.Text);
            }
            if (double.TryParse(InputStartTime.Text, out temporaryInputValue))
            {
                signalGenerator.StartTime = double.Parse(InputStartTime.Text);
            }
            if (double.TryParse(InputDuration.Text, out temporaryInputValue))
            {
                signalGenerator.Duration = double.Parse(InputDuration.Text);
            }
            if (double.TryParse(InputPeriod.Text, out temporaryInputValue))
            {
                signalGenerator.Period = double.Parse(InputPeriod.Text);
            }
            if (double.TryParse(InputFillFactor.Text, out temporaryInputValue))
            {
                signalGenerator.FillFactor = double.Parse(InputFillFactor.Text);
            }
            if (double.TryParse(InputJumpTime.Text, out temporaryInputValue))
            {
                signalGenerator.JumpTime = double.Parse(InputJumpTime.Text);
            }
            if (double.TryParse(InputProbability.Text, out temporaryInputValue))
            {
                signalGenerator.Probability = double.Parse(InputProbability.Text);
            }
            if (double.TryParse(InputFrequency.Text, out temporaryInputValue))
            {
                signalGenerator.Frequency = double.Parse(InputFrequency.Text);
            }

            int sectionAmount = 15;
            sectionAmount = int.TryParse(HistogramInterval.Text, out sectionAmount) ? int.Parse(HistogramInterval.Text) : 15;
            Signal signal = signalGenerator.Generate((SignalGenerator.Type)SignalComboBox.SelectionBoxItem);

            SaveSignalToFile(signal,"generated");
           
            Window chartWindow = new ChartWindow(signal, sectionAmount);
            chartWindow.Show();
        }

        public void SaveSignalToFile(Signal signal, String prefix)
        {
            Stream stream = File.Open("signals/"+prefix+"_signal_@"+DateTime.Now.ToString("T").Replace(':','-')+".sig", FileMode.Create);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(stream, signal);
            stream.Close();
        }
        public void LoadSignal1(object obj, RoutedEventArgs routedEventArgs)
        {
            var (signalFromFile, fileName) = LoadSignalFromFile();
            if (signalFromFile == null) return;
            _loadedSignal1 = signalFromFile;
            SignalName1.Text = fileName;
        }
        
        public void LoadSignal2(object obj, RoutedEventArgs routedEventArgs)
        {
            var (signalFromFile, fileName) = LoadSignalFromFile();
            if (signalFromFile == null) return;
            _loadedSignal2 = signalFromFile;
            SignalName2.Text = fileName;
        }

        public (Signal, String) LoadSignalFromFile()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog {DefaultExt = ".sig"};
            var result = dlg.ShowDialog();
            if (result != true) return (null, "");
            var filename = dlg.FileName;
            var binaryFormatter = new BinaryFormatter();
            Stream stream = File.Open(filename, FileMode.Open);
            var signal = (Signal)binaryFormatter.Deserialize(stream);
            Window chartWindow = new ChartWindow(signal, int.TryParse(HistogramInterval.Text, out _) ? int.Parse(HistogramInterval.Text) : 15);
            chartWindow.Show();
            return (signal, dlg.SafeFileName);
        }
        
        public void AddSignals(object obj, RoutedEventArgs routedEventArgs)
        {
            if (_loadedSignal1 == null && _loadedSignal2 == null) return;
            Signal sum = SignalOperations.Add(_loadedSignal1, _loadedSignal2);
            Window chartWindow = new ChartWindow(sum, int.TryParse(HistogramInterval.Text, out _) ? int.Parse(HistogramInterval.Text) : 15);
            chartWindow.Show();
            SaveSignalToFile(sum,"ADD");
        }
        
        public void SubtractSignals(object obj, RoutedEventArgs routedEventArgs)
        {
            if (_loadedSignal1 == null && _loadedSignal2 == null) return;
            Signal difference = SignalOperations.Subtract(_loadedSignal1, _loadedSignal2);
            Window chartWindow = new ChartWindow(difference, int.TryParse(HistogramInterval.Text, out _) ? int.Parse(HistogramInterval.Text) : 15);
            chartWindow.Show();
            SaveSignalToFile(difference,"SUBTRACT");
        }
        
        public void MultiplySignals(object obj, RoutedEventArgs routedEventArgs)
        {
            if (_loadedSignal1 == null && _loadedSignal2 == null) return;
            Signal product = SignalOperations.Multiply(_loadedSignal1, _loadedSignal2);
            Window chartWindow = new ChartWindow(product, int.TryParse(HistogramInterval.Text, out _) ? int.Parse(HistogramInterval.Text) : 15);
            chartWindow.Show();
            SaveSignalToFile(product,"MULTIPLY");
        }
        
        public void DivideSignals(object obj, RoutedEventArgs routedEventArgs)
        {
            if (_loadedSignal1 == null && _loadedSignal2 == null) return;
            Signal quotient = SignalOperations.Divide(_loadedSignal1, _loadedSignal2);
            Window chartWindow = new ChartWindow(quotient, int.TryParse(HistogramInterval.Text, out _) ? int.Parse(HistogramInterval.Text) : 15);
            chartWindow.Show();
            SaveSignalToFile(quotient,"DIVIDE");
        }
        
    }
}    
