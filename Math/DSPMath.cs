using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Utilities.Extensions;
using Vectors;

namespace MyMath
{
    public static class DSPMath
    {
        public static double CalcTHDf(List<V2> magnitudeSpectrum, int mainHarmonicIndex)
        {
            double thdTmp = 0;
            for (int i = 1; i < magnitudeSpectrum.Count; i++)
            {
                if (i != mainHarmonicIndex)
                {
                    thdTmp += magnitudeSpectrum[i].Y.Pow(2);
                }
            }
            double mainHarmonicAmp = magnitudeSpectrum[mainHarmonicIndex].Y;
            double thd_f = thdTmp.Root(2) / mainHarmonicAmp;
            return thd_f;
        }

        public static double CalcTHDr(double thd_f)
        {
            return thd_f / (1 + thd_f.Pow(2)).Root(2);
        }
    }
}
