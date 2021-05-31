using SignalProcessing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SignalProcessing.Logic
{
    class SignalFilter
    {
        public int M { get; set; }
        public double F0 { get; set; }
        public double Fp { get; set; }
        
        public List<double> Coefficients { get; set; }

        public FilterType Filter { get; set; }
        public WindowType Window { get; set; }

        public enum FilterType
        {
            Lowpass,
            Bandpass
        }

        public enum WindowType
        {
            Rectangular,
            Hamming
        }

        public SignalFilter()
        {
            M = 100;
            F0 = 5;
            Fp = 10;
            Filter = FilterType.Bandpass;
            Window = WindowType.Hamming;
        }

        public void Generate()
        {
            switch (Filter)
            {
                case FilterType.Lowpass: GenerateLowPass();  break;
                case FilterType.Bandpass: GenerateBandPass();  break;
            }

            switch (Window)
            {
                case WindowType.Rectangular: ApplyHanningWindow(); break;
            }
        }

        private void GenerateLowPass()
        {
            List<double> coefficients = new List<double>();

            double K = Fp / F0;

            int center = (M - 1) / 2;

            for (int n = 0; n < M; n++)
            {
                if (n == center)
                {
                    coefficients.Add(2.0 / K);
                }
                else
                {
                    coefficients.Add(Math.Sin(2 * Math.PI * (n - center) / K) / (Math.PI * (n - center)));
                }
            }

            Coefficients = coefficients;
        }

        private void GenerateBandPass()
        {
            List<double> coefficients = new List<double>();

            GenerateLowPass();

            for (int n = 0; n < Coefficients.Count; n++)
            {
                coefficients.Add(Coefficients[n] * 2 * Math.Sin(Math.PI * n / 2));
            }

            Coefficients = coefficients;
        }

        private void ApplyHanningWindow()
        {
            List<double> coefficients = new List<double>();

            for (int n = 0; n < Coefficients.Count; n++)
            {
                coefficients.Add(Coefficients[n] * (0.5 - 0.5 * Math.Cos(2 * Math.PI * n / M)));
            }

            Coefficients = coefficients;
        }

        public Signal GetAsSignal()
        {
            return new Signal(0,0,0,true,0,Coefficients.Select(c => new Point(0,c)).ToList());
        }
    }
}
