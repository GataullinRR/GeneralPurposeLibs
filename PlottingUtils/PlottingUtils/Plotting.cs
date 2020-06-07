using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Types;
using Utilities.Extensions;
using ZedGraph;
using System.Drawing;
using Vectors;

namespace PlottingUtils
{
    public static class Plotting
    {
        public static void Plot(ZedGraphControl graph, IEnumerable<double> x, IEnumerable<Graphic> ys)
        {
            graph.MasterPane.InnerPaneGap = 0;
            graph.MasterPane.Title.IsVisible = false;
            graph.IsSynchronizeXAxes = true;
            x = x ??
                Enumerable.Range(0, ys.EmptyToNull()?.Max(y => y.Curves.Max(c => c.Count)) ?? 0).ToDoubles();
            GraphPane pane = null;
            var j = 0;
            foreach (var graphic in ys)
            {
                pane = j == 0 ? graph.GraphPane : new GraphPane();
                if (j != 0)
                {
                    graph.MasterPane.Add(pane);
                }
                j++;

                pane.XAxis.Scale.Max = x.Max();
                pane.YAxis.Scale.Min = 0;
                pane.Title.IsVisible = false;
                pane.XAxis.Title.IsVisible = false;
                pane.YAxis.Title.IsVisible = false;
                pane.Border.IsVisible = false;
                var colors = new[] { Color.Black, Color.Blue, Color.Red };
                var i = 0;
                foreach (var y in graphic.Curves)
                {
                    if (y is Curve curve)
                    {
                        pane.AddCurve(null, x.ToArray(), curve.Points.ToArray(), curve.Color ?? colors[i], SymbolType.None);
                    }
                    else if (y is HBarCurve curve2)
                    {
                        var rects = new List<PolyObj>();
                        var hBarHeight = curve2.Height;
                        for (int k = 0; k < curve2.Points.Count(); k++)
                        {
                            Interval interval = curve2.Points.ElementAt(k);

                            var rect = new PolyObj();
                            rect.Points = new[]
                                {
                                        new PointD(interval.From, 0),
                                        new PointD(interval.From, hBarHeight),
                                        new PointD(interval.To, hBarHeight),
                                        new PointD(interval.To, 0)
                                    };
                            rect.Fill = new Fill(curve2.Color ?? colors[i]);
                            rect.IsClippedToChartRect = true;
                            rect.ZOrder = ZOrder.E_BehindCurves;

                            rects.Add(rect);
                        }

                        pane.GraphObjList.AddRange(rects.Cast<GraphObj>());
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                    i++;
                }
            }

            graph.AxisChange();
            using (Graphics g = graph.ParentForm.CreateGraphics())
            {
                graph.MasterPane.SetLayout(g, PaneLayout.SingleColumn);
            }
            graph.Invalidate();
        }
    }
}
