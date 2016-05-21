using System;
using MouseChef.Input;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace MouseChef
{
    public class MainViewModel : IEventProcessor, IDisposable
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

        private readonly InputReader _inputReader;
        private readonly LineSeries _mouse = new LineSeries();

        public MainViewModel()
        {
            Plot.Axes.Add(MakeAxis(AxisPosition.Bottom, "x"));
            Plot.Axes.Add(MakeAxis(AxisPosition.Left, "y"));
            Plot.Series.Add(_mouse);

            _inputReader = new InputReader(this);
        }

        public PlotModel Plot { get; set; } = new PlotModel();

        public void DeviceInfo(DeviceInfoEvent evt)
        {
        }

        public void Move(MoveEvent evt)
        {
            lock (Plot.SyncRoot)
            {
                var lastPoint = _mouse.Points.Count > 0 ? _mouse.Points[_mouse.Points.Count - 1] : new DataPoint(0, 0);
                _mouse.Points.Add(new DataPoint(lastPoint.X + evt.Dx, lastPoint.Y - evt.Dy));
            }
            Plot.InvalidatePlot(updateData: false);
        }

        public void Dispose() => _inputReader.Dispose();
    }
}
