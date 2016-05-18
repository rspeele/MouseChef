using OxyPlot;
using OxyPlot.Axes;

namespace MouseChef
{
    public class MainViewModel
    {
        private const double MaxRange = 500 * 1000;
        private const double TickSize = 1000;

        public MainViewModel()
        {
            Plot.Axes.Add
                (new LinearAxis
                {
                    Minimum = -MaxRange / 2,
                    Maximum = MaxRange / 2,
                    AbsoluteMinimum = -MaxRange,
                    AbsoluteMaximum = MaxRange,
                    MajorStep = TickSize,
                    Title = "x"
                });
            Plot.Axes.Add
                (new LinearAxis
                {
                    Minimum = -MaxRange / 2,
                    Maximum = MaxRange / 2,
                    AbsoluteMinimum = -MaxRange,
                    AbsoluteMaximum = MaxRange,
                    MajorStep = TickSize,
                    Title = "y"
                });
        }

        public PlotModel Plot { get; set; }
    }
}
