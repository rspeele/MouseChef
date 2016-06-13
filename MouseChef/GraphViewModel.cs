using System;
using System.Linq;
using MouseChef.Analysis;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace MouseChef
{
    public class GraphViewModel
    {
        public GraphViewModel(Graphable xStats, Graphable yStats)
        {
            Plot = new PlotModel();
            FillPlot(Plot, xStats, yStats);
            Title = $"{yStats} over {xStats}";
        }

        private static LinearAxis MakeAxis(AxisPosition position, string title, IStats stats)
            => new LinearAxis
            {
                Position = position,
                MinorGridlineStyle = LineStyle.Solid,
                MajorGridlineStyle = LineStyle.Solid,
                Minimum = Math.Min(stats.ExpectedMinimum, stats.MinValue),
                Maximum = Math.Max(stats.ExpectedMaximum, stats.MaxValue),
                MinorStep = stats.MinorStep ?? double.NaN,
                MajorStep = stats.MajorStep,
                Title = title,
            };

        private static void FillPlot(PlotModel plot, Graphable xGraph, Graphable yGraph)
        {
            var xStats = xGraph.GetStats();
            var yStats = yGraph.GetStats();
            var xAxis = MakeAxis(AxisPosition.Bottom, xGraph.ToString(), xStats);
            var yAxis = MakeAxis(AxisPosition.Left, yGraph.ToString(), yStats);
            plot.Axes.Add(xAxis);
            plot.Axes.Add(yAxis);
            var series = new LineSeries();
            plot.Series.Add(series);
            var points = xStats.DataPoints.Concat(yStats.DataPoints)
                .Select(p => new
                {
                    x = xStats.ValueAt(p.Time),
                    y = yStats.ValueAt(p.Time),
                })
                .OrderBy(p => p.x);
            foreach (var point in points)
            {
                series.Points.Add(new DataPoint
                    ( point.x
                    , point.y
                    ));
            }
            plot.InvalidatePlot(false);
        }

        public string Title { get; }
        public PlotModel Plot { get; }
    }
}
