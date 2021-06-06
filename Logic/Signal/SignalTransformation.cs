using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Windows;
using SignalProcessing.Model;

namespace SignalProcessing.Logic
{
    public class SignalTransformation
    {
        public enum Type
        {
            DFT,
            InverseDFT,
            DITFFT,
            InverseDITFFT,
            WaveletTransformDB4,
            InverseWaveletTransformDB4
        }
        
        public static List<Complex> DiscreteFourierTransformation(List<double> signalValues)
        {
            var points = RealToComplex(signalValues);
            var result = new List<Complex>();
            for (var i = 0; i < points.Count; i++)
            {
                Complex complex = 0;

                for (var j = 0; j < points.Count; j++)
                    complex += new Complex(points[j].Real, points[j].Imaginary) * TwiddleFactor(i, j, points.Count);

                result.Add(complex / points.Count);
            }

            return result;
        }
        
        public static List<double> InverseDiscreteFourierTransformation(List<Complex> complexSignalValues)
        {
            return complexSignalValues.Select(
                (complex, i) => complexSignalValues.Select(
                    (c, j) => (c * InverseTwiddleFactor(i, j, complexSignalValues.Count)).Real
                    ).Sum()
                ).ToList();
        }
        
        public static List<Complex> FastFourierTransformation(List<double> realPoints)
        {
            var points = RealToComplex(realPoints);

            var transformed = SwitchSamples(points);

            return transformed.Select(c => c / points.Count).ToList();
        }

        public static List<double> InverseFastFourierTransformation(List<Complex> points)
        {
            var transformed = SwitchSamples(points, true);

            return transformed.Select(c => c.Real).ToList();
        }

        private static List<Complex> SwitchSamples(List<Complex> points, bool reverse = false)
        {
            if (points.Count < 2)
                return points;

            var odd = new List<Complex>();
            var even = new List<Complex>();

            for (var i = 0; i < points.Count / 2; i++)
            {
                even.Add(points[i * 2]);
                odd.Add(points[i * 2 + 1]);
            }

            var result = Connect(SwitchSamples(even, reverse), SwitchSamples(odd, reverse), reverse);

            return result;
        }

        private static List<Complex> Connect(List<Complex> evenPoints, List<Complex> oddPoints, bool reverse)
        {
            var result = new List<Complex>();
            var resultRight = new List<Complex>();

            for (var i = 0; i < oddPoints.Count; i++)
            {
                if (!reverse)
                {
                    result.Add(evenPoints[i] + TwiddleFactor(i, 1, oddPoints.Count * 2) * oddPoints[i]);
                    resultRight.Add(evenPoints[i] - TwiddleFactor(i, 1, oddPoints.Count * 2) * oddPoints[i]);
                }
                else
                {
                    result.Add(evenPoints[i] + InverseTwiddleFactor(i, 1, oddPoints.Count * 2) * oddPoints[i]);
                    resultRight.Add(evenPoints[i] - InverseTwiddleFactor(i, 1, oddPoints.Count * 2) * oddPoints[i]);
                }
            }

            result.AddRange(resultRight);

            return result;
        }
        
        public static Signal WaveletTransformation(Signal signal)
        {
            var hCore = new Signal(0, 0, 0, true, 0, H.Select(c => new Point(0, c)).ToList());
            var gCore = new Signal(0, 0, 0, true, 0, G.Select(c => new Point(0, c)).ToList());
            var hSamples = SignalOperations.Convolute(signal, hCore, signal.Values.Count, hCore.Values.Count);
            var gSamples = SignalOperations.Convolute(signal, gCore, signal.Values.Count, gCore.Values.Count);

            var hHalf = new List<double>();
            var gHalf = new List<double>();

            for (var i = 0; i < hSamples.Values.Count; i++)
            {
                if (i % 2 == 0)
                    hHalf.Add(hSamples.Values[i].Y);

                else
                    gHalf.Add(gSamples.Values[i].Y);
            }

            signal.ComplexValue = gHalf.Select((t, i) => new Complex(hHalf[i], t)).ToList();
            signal.Values = signal.Values.Select(v => new Point(v.X, 0)).ToList();
            return signal;
        }

        public static Signal InverseWaveletTransformation(Signal signal)
        {
            var hReversed = new List<double>(H);
            var gReversed = new List<double>(G);
            hReversed.Reverse();
            gReversed.Reverse();
            var hCore = new Signal(0, 0, 0, true, 0, hReversed.Select(c => new Point(0, c)).ToList());
            var gCore = new Signal(0, 0, 0, true, 0, gReversed.Select(c => new Point(0, c)).ToList());
            
            var hSamples = new List<double>();
            var gSamples = new List<double>();

            foreach (var t in signal.ComplexValue)
            {
                hSamples.Add(t.Real);
                hSamples.Add(0);

                gSamples.Add(0);
                gSamples.Add(t.Imaginary);
            }
            var hResult = new Signal(signal.StartTime, signal.Duration, signal.Period, signal.Discrete, signal.Frequency, signal.Values.Select((v, i) => new Point(v.X, hSamples[i])).ToList());
            var gResult = new Signal(signal.StartTime, signal.Duration, signal.Period, signal.Discrete, signal.Frequency, signal.Values.Select((v, i) => new Point(v.X, gSamples[i])).ToList());
            hResult = SignalOperations.Convolute(hResult, hCore, hResult.Values.Count, hCore.Values.Count);
            gResult = SignalOperations.Convolute(gResult, gCore, gResult.Values.Count, gCore.Values.Count);
            return SignalOperations.Add(hResult, gResult);
        }

        private static List<Complex> RealToComplex(List<double> real)
        {
            return real.Select(number => new Complex(number, 0)).ToList();
        }

        private static Complex TwiddleFactor(int m, int n, int N)
        {
            return Complex.Exp(new Complex(0, -2 * Math.PI * m * n / N));
        }

        private static Complex InverseTwiddleFactor(int m, int n, int N)
        {
            return Complex.Exp(new Complex(0, 2 * Math.PI * m * n / N));
        }
        
        private static List<double> H = new List<double>
        {
            (1 + Math.Sqrt(3)) / (4 * Math.Sqrt(2)),
            (3 + Math.Sqrt(3)) / (4 * Math.Sqrt(2)),
            (3 - Math.Sqrt(3)) / (4 * Math.Sqrt(2)),
            (1 - Math.Sqrt(3)) / (4 * Math.Sqrt(2))
        };

        private static List<double> G = new List<double>
        {
            H[3],
            -H[2],
            H[1],
            -H[0]
        };
    }
}