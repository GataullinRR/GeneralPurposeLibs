using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Utilities.Extensions;
using Vectors;

namespace MyMath
{
    public static class Statistic
    {
        static public double CalcPirsonCorrelation(List<V2> points)
        {
            double avgX = points.Average(p => p.X);
            double avgY = points.Average(p => p.Y);
            double numerator = points.Select(p => (p.X - avgX) * (p.Y - avgY)).Sum();
            double sumOf_SqrOf_X_Sub_AvgX = points.Select(p => p.X - avgX).PowEach(2).Sum();
            double sumOf_SqrOf_Y_Sub_AvgY = points.Select(p => p.Y - avgY).PowEach(2).Sum();
            double denumerator = (sumOf_SqrOf_X_Sub_AvgX * sumOf_SqrOf_Y_Sub_AvgY).Root(2);

            return numerator / denumerator;
        }

        static public double CalcPirsonCorrelation(IList<double> x, IList<double> y)
        {
            double avgX = x.Average();
            double avgY = y.Average();
            double numerator = 0;
            double sumOf_SqrOf_X_Sub_AvgX = 0;
            double sumOf_SqrOf_Y_Sub_AvgY = 0;
            for (int i = 0; i < x.Count; i++)
            {
                numerator += (x[i] - avgX) * (y[i] - avgY);
                sumOf_SqrOf_X_Sub_AvgX += (x[i] - avgX).Pow(2);
                sumOf_SqrOf_Y_Sub_AvgY += (y[i] - avgY).Pow(2);
            }
            double denumerator = (sumOf_SqrOf_X_Sub_AvgX * sumOf_SqrOf_Y_Sub_AvgY).Root(2);

            return numerator / denumerator;
        }

        public static double Median(params double[] sourceNumbers)
        {
            //Framework 2.0 version of this method. there is an easier way in F4        
            if (sourceNumbers == null || sourceNumbers.Length == 0)
                throw new System.Exception("Median of empty array not defined.");

            //make sure the list is sorted, but use a new array
            double[] sortedPNumbers = (double[])sourceNumbers.Clone();
            Array.Sort(sortedPNumbers);

            //get the median
            int size = sortedPNumbers.Length;
            int mid = size / 2;
            double median = size % 2 != 0 ? sortedPNumbers[mid] : (sortedPNumbers[mid] + sortedPNumbers[mid - 1]) / 2;
            return median;
        }

        static public double Mx(params double[] values)
        {
            double[] valusesDistinct = values
                .Distinct()
                .ToArray();

            return valusesDistinct
                .Aggregate(0D, (acc, next) => acc + next * values.Count(v => v == next) / values.Count());
        }

        static public double Mx2(params double[] values)
        {
            double[] valusesDistinct = values
                .Distinct()
                .ToArray();

            return valusesDistinct
                .Aggregate(0D, (acc, next) => acc + Math.Pow(next, 2) * values.Count(v => v == next) / values.Count());
        }

        static public double Dx(params double[] values)
        {
            return -Math.Pow(Mx(values), 2) + Mx2(values);
        }
    }
}
