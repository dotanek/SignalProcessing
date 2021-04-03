using System;
using System.Linq;
using SignalProcessing.Model;

namespace SignalProcessing.Logic
{
    public static class SimilarityMetric
    {
        public static double ComputeMSE(Signal sourceSignal, Signal reconstructedSignal)
        {
            var error = sourceSignal.Values.Select((t, i) => Math.Pow((t.Y - reconstructedSignal.Values[i].Y), 2))
                .Sum();
            error /= sourceSignal.Values.Count;
            return error;
        }

        public static double ComputeSNR(Signal sourceSignal, Signal reconstructedSignal)
        {
            var sum = sourceSignal.Values.Select(e => Math.Pow(e.Y, 2)).Sum();
            var se = sourceSignal.Values.Select((t, i) => Math.Pow((t.Y - reconstructedSignal.Values[i].Y), 2))
                .Sum();
            var snr = 10 * Math.Log10(sum / se);
            return snr;
        }
        
        public static double ComputePSNR(Signal sourceSignal, Signal reconstructedSignal)
        {
            var max = sourceSignal.Values.Select(e => e.Y).Max();
            var psnr = 10 * Math.Log10(max / ComputeMSE(sourceSignal, reconstructedSignal));
            return psnr;
        }
        
        public static double ComputeMD(Signal sourceSignal, Signal reconstructedSignal)
        {
            var max = sourceSignal.Values
                .Select((point, index) => Math.Abs(point.Y - reconstructedSignal.Values[index].Y)).Max();
            
            return max;
        }

        public static double ComputeENOB(Signal sourceSignal, Signal reconstructedSignal)
        {
            return (ComputeSNR(sourceSignal, reconstructedSignal) - 1.76) / 6.02;
        }
    }
}