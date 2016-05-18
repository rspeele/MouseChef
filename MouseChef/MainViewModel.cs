using OxyPlot;
using OxyPlot.Axes;

namespace MouseChef
{
    public class MainViewModel
    {
        private const double MaxRange = 100 * 1000;
        private const double DefaultZoom = 50;
        private const double TickSize = 1000;

        private static LinearAxis MakeAxis(AxisPosition position, string title)
            => new LinearAxis
            {
                Position = position,
                Minimum = -MaxRange / DefaultZoom,
                Maximum = MaxRange / DefaultZoom,
                AbsoluteMinimum = -MaxRange,
                AbsoluteMaximum = MaxRange,
                MinorStep = TickSize,
                MajorStep = TickSize * 10,
                MinorGridlineStyle = LineStyle.Solid,
                MajorGridlineStyle = LineStyle.Solid,
                Title = title
            };

        public MainViewModel()
        {
            Plot.Axes.Add(MakeAxis(AxisPosition.Bottom, "x"));
            Plot.Axes.Add(MakeAxis(AxisPosition.Left, "y"));
        }

        public PlotModel Plot { get; set; } = new PlotModel();
    }
}
