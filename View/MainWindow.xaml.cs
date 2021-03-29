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
        private Signal _generatedSignal;
        private Signal _loadedSignal1;
        private Signal _loadedSignal2;
        private Signal _resultSignal;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        public MainWindow()
        {
            InitializeComponent();
            SignalChart.Hoverable = false;
            HistogramChart.Hoverable = false;

            SignalChart.AxisY.Add(new Axis
            {
                LabelFormatter = (x) => string.Format("{0:0.000}", x),
            });
            HistogramChart.AxisY.Add(new Axis
            {
                LabelFormatter = (x) => string.Format("{0000:0000}", x),
            });
            SignalComboBox.ItemsSource = Enum.GetValues(typeof(SignalGenerator.Type)).Cast<SignalGenerator.Type>();
        }

        public void GenerateChart(object obj, RoutedEventArgs routedEventArgs)
        {
            ReadyText.Text = "Generating...";
            SignalGenerator signalGenerator = new SignalGenerator();
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

            _generatedSignal = signalGenerator.Generate((SignalGenerator.Type)SignalComboBox.SelectionBoxItem);
            ShowGenerated.IsEnabled = true;
            SaveSignalToFile(_generatedSignal,"generated");
            ReadyText.Text = "Ready";

            //Window chartWindow = new ChartWindow(signal, sectionAmount);
            //chartWindow.Show();
        }

        public void ShowGeneratedSignal(object obj, RoutedEventArgs routedEventArgs)
        {
            ShowCharts(_generatedSignal);
        }
        public void ShowCharts(Signal signal)
        {
            SignalChart.Series.Clear();
            SignalChart.Series = new SeriesCollection
            {
               new LineSeries
               {
                   Values = new ChartValues<ObservablePoint>(signal.Values),
                   PointGeometry = null,
               },
            };

            int histogramSections = int.TryParse(HistogramSections.Text, out histogramSections) ? int.Parse(HistogramSections.Text) : 10;

            List<ObservablePoint> histogramPlot = signal.GetHistogramPlot(histogramSections);
            List<double> Y = histogramPlot.Select(e => e.Y).ToList();
            List<string> X = histogramPlot.Select(e => string.Format("{0:0.00}", e.X)).ToList();

            HistogramChart.AxisX.Clear();
            HistogramChart.AxisX.Add(
                new Axis
                {
                    Labels = X,
                    Separator = new LiveCharts.Wpf.Separator
                    {
                        Step = 1
                    }
                }
            );

            HistogramChart.Series.Clear();
            HistogramChart.Series = new SeriesCollection
            {
               new ColumnSeries
               {
                   Values = new ChartValues<double>(Y),
                   PointGeometry = null
               }
            };

            SignalTextValues.Text = signal.ToString();

            var sb = new StringBuilder();
            sb.Append("Average = ").Append(Math.Round(signal.Average(), 6));
            sb.Append("\nAbsoluteAverage = ").Append(Math.Round(signal.AbsoluteAverage(), 6));
            sb.Append("\nRootMeanSquare = ").Append(Math.Round(signal.RootMeanSquare(), 6));
            sb.Append("\nVariation = ").Append(Math.Round(signal.Variation(), 6));
            sb.Append("\nAveragePower = ").Append(Math.Round(signal.AveragePower(), 6));

            SignalTextAverages.Text = sb.ToString();
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
        public void ShowSignal1(object obj, RoutedEventArgs routedEventArgs)
        {
            ShowCharts(_loadedSignal1);
        }

        public void LoadSignal2(object obj, RoutedEventArgs routedEventArgs)
        {
            var (signalFromFile, fileName) = LoadSignalFromFile();
            if (signalFromFile == null) return;
            _loadedSignal2 = signalFromFile;
            SignalName2.Text = fileName;
        }

        public void ShowSignal2(object obj, RoutedEventArgs routedEventArgs)
        {
            ShowCharts(_loadedSignal2);
        }

        public (Signal, String) LoadSignalFromFile()
        {
            ReadyText.Text = "Loading...";
            var dlg = new Microsoft.Win32.OpenFileDialog {DefaultExt = ".sig"};
            var result = dlg.ShowDialog();
            if (result != true) return (null, "");
            var filename = dlg.FileName;
            var binaryFormatter = new BinaryFormatter();
            Stream stream = File.Open(filename, FileMode.Open);
            var signal = (Signal)binaryFormatter.Deserialize(stream);
            //Window chartWindow = new ChartWindow(signal, int.TryParse(HistogramInterval.Text, out _) ? int.Parse(HistogramInterval.Text) : 15);
            //chartWindow.Show();
            ReadyText.Text = "Ready";
            return (signal, dlg.SafeFileName);
        }

        public void ShowResultSignal(object obj, RoutedEventArgs routedEventArgs)
        {
            ShowCharts(_resultSignal);
        }

        public void AddSignals(object obj, RoutedEventArgs routedEventArgs)
        {
            ReadyText.Text = "Adding...";
            if (_loadedSignal1 == null && _loadedSignal2 == null) return;
            _resultSignal = SignalOperations.Add(_loadedSignal1, _loadedSignal2);
            //Window chartWindow = new ChartWindow(sum, int.TryParse(HistogramInterval.Text, out _) ? int.Parse(HistogramInterval.Text) : 15);
            //chartWindow.Show();
            SaveSignalToFile(_resultSignal, "ADD");
            ReadyText.Text = "Ready";
        }
        
        public void SubtractSignals(object obj, RoutedEventArgs routedEventArgs)
        {
            ReadyText.Text = "Subtracting...";
            if (_loadedSignal1 == null && _loadedSignal2 == null) return;
            _resultSignal = SignalOperations.Subtract(_loadedSignal1, _loadedSignal2);
            //Window chartWindow = new ChartWindow(difference, int.TryParse(HistogramInterval.Text, out _) ? int.Parse(HistogramInterval.Text) : 15);
            //chartWindow.Show();
            SaveSignalToFile(_resultSignal, "SUBTRACT");
            ReadyText.Text = "Ready";
        }
        
        public void MultiplySignals(object obj, RoutedEventArgs routedEventArgs)
        {
            ReadyText.Text = "Multiplying...";
            if (_loadedSignal1 == null && _loadedSignal2 == null) return;
            _resultSignal = SignalOperations.Multiply(_loadedSignal1, _loadedSignal2);
            //Window chartWindow = new ChartWindow(product, int.TryParse(HistogramInterval.Text, out _) ? int.Parse(HistogramInterval.Text) : 15);
            //chartWindow.Show();
            SaveSignalToFile(_resultSignal, "MULTIPLY");
            ReadyText.Text = "Ready";
        }
        
        public void DivideSignals(object obj, RoutedEventArgs routedEventArgs)
        {
            if (_loadedSignal1 == null && _loadedSignal2 == null) return;
            ReadyText.Text = "Dividing...";
            _resultSignal = SignalOperations.Divide(_loadedSignal1, _loadedSignal2);
            //Window chartWindow = new ChartWindow(quotient, int.TryParse(HistogramInterval.Text, out _) ? int.Parse(HistogramInterval.Text) : 15);
            //chartWindow.Show();
            SaveSignalToFile(_resultSignal, "DIVIDE");
            ReadyText.Text = "Ready";
        }
    }
}    
