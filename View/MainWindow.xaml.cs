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
        public SeriesCollection seriesCollection { get; set; }
        private Signal _generatedSignal;
        private Signal _reconstructedSignal;
        private Signal _quantised;
        private Signal _groundTruthSignal;
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
            ReconstructorComboBox.Items.Add("Zero Order Hold");
            ReconstructorComboBox.Items.Add("Sinc Interpolation");

            seriesCollection = new SeriesCollection();
            seriesCollection.Add(new ScatterSeries
                {Fill = Brushes.Blue, Stroke = Brushes.Blue});
            seriesCollection.Add(new LineSeries
                {PointGeometry = null, Fill = Brushes.Transparent, Stroke = Brushes.Brown});
            seriesCollection.Add(new LineSeries
                {PointGeometry = null, Fill = Brushes.Transparent, Stroke = Brushes.Gray});
            SignalChart.Series = seriesCollection;
            seriesCollection.Add(new ScatterSeries
                {Fill = Brushes.Red, Stroke = Brushes.Red});
        }

        public void GenerateChart(object obj, RoutedEventArgs routedEventArgs)
        {
            ReadyText.Text = "Generating...";
            SignalGenerator signalGenerator = new SignalGenerator();
            SignalGenerator secondarySignalGenerator = null;
            double temporaryInputValue;
            double secondarySignalPeriod = 0.0;
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
            else if (InputPeriod.Text.Contains("/"))
            {
                var freqParams = InputPeriod.Text.Split('/');
                signalGenerator.Period = double.Parse(freqParams[0]) / double.Parse(freqParams[1]);
            }

            if (double.TryParse(InputSecondaryPeriod.Text, out temporaryInputValue))
            {
                secondarySignalPeriod = double.Parse(InputSecondaryPeriod.Text);
            }
            else if (InputSecondaryPeriod.Text.Contains("/"))
            {
                var freqParams = InputSecondaryPeriod.Text.Split('/');
                secondarySignalPeriod = double.Parse(freqParams[0]) / double.Parse(freqParams[1]);
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


            _generatedSignal = signalGenerator.Generate((SignalGenerator.Type) SignalComboBox.SelectionBoxItem);
            var keepFrequency = signalGenerator.Frequency;
            signalGenerator.Frequency *= 4;
            var generatedCompareSignal =
                signalGenerator.Generate((SignalGenerator.Type) SignalComboBox.SelectionBoxItem);
            _groundTruthSignal = generatedCompareSignal;
            signalGenerator.Frequency = keepFrequency;
            if (secondarySignalPeriod != 0.0)
            {
                secondarySignalGenerator = new SignalGenerator(signalGenerator) {Period = secondarySignalPeriod};
                _generatedSignal = SignalOperations.Add(_generatedSignal,
                    secondarySignalGenerator.Generate((SignalGenerator.Type) SignalComboBox.SelectionBoxItem));
                
                secondarySignalGenerator.Frequency *= 4;
                generatedCompareSignal = SignalOperations.Add(generatedCompareSignal, secondarySignalGenerator.Generate((SignalGenerator.Type) SignalComboBox.SelectionBoxItem));
                _groundTruthSignal = generatedCompareSignal;

            }
            int quantisationBits = int.TryParse(QuantizationBits.Text, out quantisationBits)
                ? int.Parse(QuantizationBits.Text)
                : 8;
            var quantisation = new SignalQuantisation(quantisationBits);
            _quantised = quantisation.Quantise(_generatedSignal);
            var signalReconstructor = new SignalReconstructor();
            _reconstructedSignal = ReconstructorComboBox.SelectedIndex == 0
                ? signalReconstructor.ZeroOrderHold(_quantised)
                : signalReconstructor.SincInterpolation(_quantised, int.Parse(SincRange.Text));
            
            var sizeDifference = generatedCompareSignal.Values.Count - _reconstructedSignal.Values.Count;
            if (sizeDifference > 0)
            {
                generatedCompareSignal.Values.RemoveRange(generatedCompareSignal.Values.Count - sizeDifference,
                    sizeDifference);
            }
            else
            {
                sizeDifference = Math.Abs(sizeDifference);
                _reconstructedSignal.Values.RemoveRange(_reconstructedSignal.Values.Count - sizeDifference,
                    sizeDifference);
            }

            SignalTextAverages.Text = "";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("====================");
            sb.AppendLine("Quantisation-Source");
            sb.AppendLine("MSE: " + Math.Round(SimilarityMetric.ComputeMSE(_generatedSignal, _quantised), 6));
            sb.AppendLine("SNR: " + Math.Round(SimilarityMetric.ComputeSNR(_generatedSignal, _quantised), 6));
            sb.AppendLine("PSNR: " + Math.Round(SimilarityMetric.ComputePSNR(_generatedSignal, _quantised), 6));
            sb.AppendLine("MD: " + Math.Round(SimilarityMetric.ComputeMD(_generatedSignal, _quantised), 6));
            sb.AppendLine("ENOB: " +
                          Math.Round(SimilarityMetric.ComputeENOB(_generatedSignal, _quantised), 6));
            sb.AppendLine("====================");
            sb.AppendLine("Reconstruction-Source");
            sb.AppendLine("MSE: " +
                          Math.Round(SimilarityMetric.ComputeMSE(generatedCompareSignal, _reconstructedSignal), 6));
            sb.AppendLine("SNR: " +
                          Math.Round(SimilarityMetric.ComputeSNR(generatedCompareSignal, _reconstructedSignal), 6));
            sb.AppendLine("PSNR: " +
                          Math.Round(SimilarityMetric.ComputePSNR(generatedCompareSignal, _reconstructedSignal), 6));
            sb.AppendLine("MD: " +
                          Math.Round(SimilarityMetric.ComputeMD(generatedCompareSignal, _reconstructedSignal), 6));
            sb.AppendLine("ENOB: " +
                          Math.Round(SimilarityMetric.ComputeENOB(generatedCompareSignal, _reconstructedSignal), 6));
            sb.AppendLine("====================");
       //     latex purpose
       //     sb.AppendLine("\\hline");
       //     sb.AppendLine("\\"+"textbf MSE &" + Math.Round(SimilarityMetric.ComputeMSE(_generatedSignal, quantised), 6)+" &"+Math.Round(SimilarityMetric.ComputeMSE(generatedCompareSignal, _reconstructedSignal), 6)+" \\\\");
       //     sb.AppendLine("\\hline");
       //     sb.AppendLine("\\"+"textbf SNR &" + Math.Round(SimilarityMetric.ComputeSNR(_generatedSignal, quantised), 6)+" &"+Math.Round(SimilarityMetric.ComputeSNR(generatedCompareSignal, _reconstructedSignal), 6)+" \\\\");
       //     sb.AppendLine("\\hline");
       //     sb.AppendLine("\\"+"textbf PSNR &" + Math.Round(SimilarityMetric.ComputePSNR(_generatedSignal, quantised), 6)+" &"+Math.Round(SimilarityMetric.ComputePSNR(generatedCompareSignal, _reconstructedSignal), 6)+" \\\\");
       //     sb.AppendLine("\\hline");
       //     sb.AppendLine("\\"+"textbf MD &" + Math.Round(SimilarityMetric.ComputeMD(_generatedSignal, quantised), 6)+" &"+Math.Round(SimilarityMetric.ComputeMD(generatedCompareSignal, _reconstructedSignal), 6)+" \\\\");
       //     sb.AppendLine("\\hline");
       //     sb.AppendLine("\\"+"textbf ENOB &" + Math.Round(SimilarityMetric.ComputeENOB(_generatedSignal, quantised), 6)+" &"+Math.Round(SimilarityMetric.ComputeENOB(generatedCompareSignal, _reconstructedSignal), 6)+" \\\\");
       //     sb.AppendLine("\\hline");
            
            
            SignalTextAverages.Text += sb.ToString();

            ShowGenerated.IsEnabled = true;
            SaveSignalToFile(_generatedSignal, "generated");
            ReadyText.Text = "Ready";
        }

        public void ShowGeneratedSignal(object obj, RoutedEventArgs routedEventArgs)
        {
            ShowCharts(_generatedSignal);
            ShowReconstructionChart(_reconstructedSignal, _quantised);
            ShowGroundTruthSignal(_groundTruthSignal);
        }

        public void ShowCharts(Signal signal)
        {
            seriesCollection[0].Values = new ChartValues<ObservablePoint>();
            signal.Values.ForEach(e => seriesCollection[0].Values.Add(new ObservablePoint(e.X, e.Y)));

            int histogramSections = int.TryParse(HistogramSections.Text, out histogramSections)
                ? int.Parse(HistogramSections.Text)
                : 10;

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

            SignalTextAverages.Text += sb.ToString();
        }

        public void ShowReconstructionChart(Signal signal, Signal quantised)
        {
            seriesCollection[1].Values = new ChartValues<ObservablePoint>();
            seriesCollection[3].Values = new ChartValues<ObservablePoint>();
            signal.Values.ForEach(e => seriesCollection[1].Values.Add(new ObservablePoint(e.X, e.Y)));
            quantised.Values.ForEach(e => seriesCollection[3].Values.Add(new ObservablePoint(e.X, e.Y)));
        }

        public void ShowGroundTruthSignal(Signal signal)
        {
            seriesCollection[2].Values = new ChartValues<ObservablePoint>();
            seriesCollection[2].Values.AddRange(signal.Values.Select(e => new ObservablePoint(e.X, e.Y)).ToList());
        }

        public void SaveSignalToFile(Signal signal, String prefix)
        {
            Stream stream =
                File.Open("signals/" + prefix + "_signal_@" + DateTime.Now.ToString("T").Replace(':', '-') + ".sig",
                    FileMode.Create);
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
            var signal = (Signal) binaryFormatter.Deserialize(stream);
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