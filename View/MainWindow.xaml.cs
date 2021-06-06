using SignalProcessing.Logic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.IO;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using SignalProcessing.Logic.Sensor;
using static SignalProcessing.Logic.SignalFilter;

namespace SignalProcessing
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SeriesCollection seriesCollection { get; set; }
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
            TransformationComboBox.ItemsSource =
                Enum.GetValues(typeof(SignalTransformation.Type)).Cast<SignalTransformation.Type>();
/*
            seriesCollection = new SeriesCollection();
            seriesCollection.Add(new LineSeries
                {PointGeometry = null, Fill = Brushes.Transparent, Stroke = Brushes.DarkBlue});
            seriesCollection.Add(new LineSeries
            { PointGeometry = null, Fill = Brushes.Transparent, Stroke = Brushes.DarkGreen });*/
            /*seriesCollection.Add(new LineSeries
                {PointGeometry = null, Fill = Brushes.Transparent, Stroke = Brushes.Brown});
            seriesCollection.Add(new LineSeries
                {PointGeometry = null, Fill = Brushes.Transparent, Stroke = Brushes.Gray});
            seriesCollection.Add(new ScatterSeries
                {Fill = Brushes.Red, Stroke = Brushes.Red});
            SignalChart.Series = seriesCollection;
            */
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


            ShowGenerated.IsEnabled = true;
            SaveSignalToFile(_generatedSignal, "generated");
            ReadyText.Text = "Ready";
        }

        public void ShowGeneratedSignal(object obj, RoutedEventArgs routedEventArgs)
        {
            ShowCharts(_generatedSignal);
        }

        public void ShowW1(object obj, RoutedEventArgs routedEventArgs)
        {
            var real = _generatedSignal.ComplexValue.Select(c => c.Real).ToList();
            var imaginary = _generatedSignal.ComplexValue.Select(c => c.Imaginary).ToList();
            SignalChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    PointGeometry = null,
                    StrokeThickness = 3,
                    Values = new ChartValues<double>(real),
                    Fill = Brushes.Transparent,
                    Stroke = Brushes.Black
                },
                new ScatterSeries
                {
                    PointGeometry = new EllipseGeometry(),
                    StrokeThickness = 5,
                    Values = new ChartValues<double>(real),
                    Stroke = Brushes.Brown
                }
            };
            HistogramChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    PointGeometry = null,
                    StrokeThickness = 3,
                    Values = new ChartValues<double>(imaginary),
                    Fill = Brushes.Transparent,
                    Stroke = Brushes.Black
                },
                new ScatterSeries
                {
                    PointGeometry = new EllipseGeometry(),
                    StrokeThickness = 5,
                    Values = new ChartValues<double>(imaginary),
                    Stroke = Brushes.Brown
                }
            };
        }

        public void ShowW2(object obj, RoutedEventArgs routedEventArgs)
        {
            var magnitude = _generatedSignal.ComplexValue.Select(c => c.Magnitude).ToList();
            var phase = _generatedSignal.ComplexValue.Select(c => c.Phase).ToList();
            SignalChart.Series = new SeriesCollection
            {
                new LineSeries()
                {
                    PointGeometry = null,
                    StrokeThickness = 3,
                    Values = new ChartValues<double>(magnitude),
                    Fill = Brushes.Transparent,
                    Stroke = Brushes.Black
                },
                new ScatterSeries()
                {
                    PointGeometry = new EllipseGeometry(),
                    StrokeThickness = 5,
                    Values = new ChartValues<double>(magnitude),
                    Stroke = Brushes.Brown
                }
            };

            HistogramChart.Series = new SeriesCollection
            {
                new LineSeries()
                {
                    PointGeometry = null,
                    StrokeThickness = 3,
                    Values = new ChartValues<double>(phase),
                    Fill = Brushes.Transparent,
                    Stroke = Brushes.Black
                },
                new ScatterSeries()
                {
                    PointGeometry = new EllipseGeometry(),
                    StrokeThickness = 5,
                    Values = new ChartValues<double>(phase),
                    Stroke = Brushes.Brown
                }
            };
        }

        public void Transform(object obj, RoutedEventArgs routedEventArgs)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            switch (TransformationComboBox.SelectionBoxItem)
            {
                case SignalTransformation.Type.DFT:
                {
                    _generatedSignal.ComplexValue =
                        SignalTransformation.DiscreteFourierTransformation(_generatedSignal.Values.Select(x => x.Y)
                            .ToList());
                    timer.Stop();
                    _generatedSignal.Values = _generatedSignal.Values.Select(e => new Point(e.X, 0)).ToList();
                    SaveSignalToFile(_generatedSignal, "DFT");
                    break;
                }
                case SignalTransformation.Type.InverseDFT:
                {
                    var values =
                        SignalTransformation.InverseDiscreteFourierTransformation(_generatedSignal.ComplexValue);
                    timer.Stop();
                    _generatedSignal.Values = values.Select((v, i) => new Point(_generatedSignal.Values[i].X, v)).ToList();
                    break;
                }
                case SignalTransformation.Type.DITFFT:
                {
                    _generatedSignal.ComplexValue =
                        SignalTransformation.FastFourierTransformation(_generatedSignal.Values.Select(x => x.Y)
                            .ToList());
                    timer.Stop();
                    _generatedSignal.Values = _generatedSignal.Values.Select(e => new Point(e.X, 0)).ToList();
                    SaveSignalToFile(_generatedSignal, "DITFFT");
                    break;
                }
                case SignalTransformation.Type.InverseDITFFT:
                {
                    var values =
                        SignalTransformation.InverseFastFourierTransformation(_generatedSignal.ComplexValue);
                    timer.Stop();
                    _generatedSignal.Values = values.Select((v, i) => new Point(_generatedSignal.Values[i].X, v)).ToList();
                    break;
                }
                case SignalTransformation.Type.WaveletTransformDB4:
                {
                    _generatedSignal = SignalTransformation.WaveletTransformation(_generatedSignal);
                    timer.Stop();
                    SaveSignalToFile(_generatedSignal, "WAVE");
                    break;
                }
                case SignalTransformation.Type.InverseWaveletTransformDB4:
                {
                    _generatedSignal = SignalTransformation.InverseWaveletTransformation(_generatedSignal);
                    timer.Stop();
                    break;
                }
            }
            
            TransformationTime.Text = "Transformation time: " + timer.Elapsed.TotalMilliseconds +" ms";
        }

        public void ShowCharts(Signal signal)
        {
            var values = new ChartValues<ObservablePoint>();
            values.AddRange(signal.Values.Select(e => new ObservablePoint(e.X, e.Y)).ToList());
            
            SignalChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    PointGeometry = null,
                    StrokeThickness = 3,
                    Values = values,
                    Fill = Brushes.Transparent,
                    Stroke = Brushes.Black
                },
                new ScatterSeries()
                {
                    PointGeometry = new EllipseGeometry(),
                    StrokeThickness = 5,
                    Values = values,
                    Stroke = Brushes.Brown
                }
            };
            HistogramChart.Series.Clear();
         //   int histogramSections = int.TryParse(HistogramSections.Text, out histogramSections)
         //       ? int.Parse(HistogramSections.Text)
         //       : 10;
         //   List<ObservablePoint> histogramPlot = signal.GetHistogramPlot(histogramSections);
         //   List<double> Y = histogramPlot.Select(e => e.Y).ToList();
         //   List<string> X = histogramPlot.Select(e => string.Format("{0:0.00}", e.X)).ToList();
         //   HistogramChart.AxisX.Clear();
         //   HistogramChart.AxisX.Add(
         //       new Axis
         //       {
         //           Labels = X,
         //           Separator = new LiveCharts.Wpf.Separator
         //           {
         //               Step = 1
         //           }
         //       }
         //   );
         //   HistogramChart.Series.Clear();
         //   HistogramChart.Series = new SeriesCollection
         //   {
         //       new ColumnSeries
         //       {
         //           Values = new ChartValues<double>(Y),
         //           PointGeometry = null
         //       }
         //   };
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
            _generatedSignal = signalFromFile;
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
            ReadyText.Text = "Ready";
            return (signal, dlg.SafeFileName);
        }

        public void AddSignals(object obj, RoutedEventArgs routedEventArgs)
        {
            ReadyText.Text = "Adding...";
            if (_loadedSignal1 == null && _loadedSignal2 == null) return;
            _resultSignal = SignalOperations.Add(_loadedSignal1, _loadedSignal2);
            SaveSignalToFile(_resultSignal, "ADD");
            ReadyText.Text = "Ready";
        }

        public void SubtractSignals(object obj, RoutedEventArgs routedEventArgs)
        {
            ReadyText.Text = "Subtracting...";
            if (_loadedSignal1 == null && _loadedSignal2 == null) return;
            _resultSignal = SignalOperations.Subtract(_loadedSignal1, _loadedSignal2);
            SaveSignalToFile(_resultSignal, "SUBTRACT");
            ReadyText.Text = "Ready";
        }

        public void MultiplySignals(object obj, RoutedEventArgs routedEventArgs)
        {
            ReadyText.Text = "Multiplying...";
            if (_loadedSignal1 == null && _loadedSignal2 == null) return;
            _resultSignal = SignalOperations.Multiply(_loadedSignal1, _loadedSignal2);
            SaveSignalToFile(_resultSignal, "MULTIPLY");
            ReadyText.Text = "Ready";
        }

        public void DivideSignals(object obj, RoutedEventArgs routedEventArgs)
        {
            if (_loadedSignal1 == null && _loadedSignal2 == null) return;
            ReadyText.Text = "Dividing...";
            _resultSignal = SignalOperations.Divide(_loadedSignal1, _loadedSignal2);
            SaveSignalToFile(_resultSignal, "DIVIDE");
            ReadyText.Text = "Ready";
        }

        public void ConvoluteSignals(object obj, RoutedEventArgs routedEventArgs)
        {
            if (_loadedSignal1 == null && _loadedSignal2 == null) return;
            ReadyText.Text = "Convoluting...";

            int s1SampleAmount = 0;
            int s2SampleAmount = 0;
            int temporaryInputValue;

            _resultSignal = SignalOperations.Convolute(_loadedSignal1, _loadedSignal2, s1SampleAmount, s2SampleAmount);
            SaveSignalToFile(_resultSignal, "CONVOLUTE");
            ReadyText.Text = "Ready";
        }
    }
}