using System.Linq;
using MouseChef.Analysis;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace MouseChef
{
    public class GraphViewModel
    {
        public GraphViewModel(IStats xStats, IStats yStats)
        {
            Plot = new PlotModel();
            FillPlot(Plot, xStats, yStats);
        }

        private static LinearAxis MakeAxis(AxisPosition position, string title)
            => new LinearAxis
            {
                Position = position,
                MinorGridlineStyle = LineStyle.Solid,
                MajorGridlineStyle = LineStyle.Solid,
                Title = title
            };

        private static void FillPlot(PlotModel plot, IStats xStats, IStats yStats)
        {
            var xAxis = MakeAxis(AxisPosition.Bottom, xStats.Description);
            var yAxis = MakeAxis(AxisPosition.Left, yStats.Description);
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

        public PlotModel Plot { get; }
    }
}
