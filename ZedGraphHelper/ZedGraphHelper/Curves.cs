    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
//using System.Windows.Media;
using Utilities.Types;
    using Vectors;
using System.Drawing;

namespace ZedGraphHelper
{
    public abstract class CurveBase
    {
        public int Count { get; }
        public CurveBase(int count)
        {
            Count = count;
        }
    }

    public abstract class CurveBase<T> : CurveBase
    {
        public IEnumerable<T> Points { get; }

        public CurveBase(IEnumerable<T> points) : base(points.Count())
        {
            Points = points ?? throw new ArgumentNullException(nameof(points));
        }
    }

    public class Curve : CurveBase<double>
    {
        public Curve(IEnumerable<double> points)
            : this(points, null)
        {

        }
        public Curve(IEnumerable<double> points, Color? color)
            : base(points)
        {
            Color = color;
        }

        public Color? Color { get; }
    }

    public class Histogram : CurveBase<double>
    {
        public Histogram(IEnumerable<double> ys, double binWidth)
            : this(ys, binWidth, null)
        {

        }
        public Histogram(IEnumerable<double> ys, double binWidth, Color? color)
            : base(ys)
        {
            BinWidth = binWidth;
            Color = color;
        }

        public double BinWidth { get; }
        public Color? Color { get; }
    }

    public class HBarCurve : CurveBase<Interval>
    {
        public HBarCurve(IEnumerable<Interval> points)
            : this(points, 1, null)
        {

        }
        public HBarCurve(IEnumerable<Interval> points, double height, Color? color)
            : base(points)
        {
            Height = height;
            Color = color;
        }
        public HBarCurve(IEnumerable<Interval> points, double height)
            : this(points, height, null)
        {
            Height = height;
        }

        public Color? Color { get; }
        public double Height { get; }
    }

    public class Graphic : IEnumerable<CurveBase>
    {
        public static implicit operator Graphic(CurveBase curve)
        {
            var g = new Graphic();
            g.Curves.Add(curve);

            return g;
        }

        public void Add(CurveBase curve)
        {
            Curves.Add(curve);
        }

        public IEnumerator<CurveBase> GetEnumerator()
        {
            return Curves.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Curves.GetEnumerator();
        }

        public Enumerable<CurveBase> Curves { get; } = new Enumerable<CurveBase>();
    }
}
