using System;
using System.Collections.Generic;
using System.Linq;
using MouseChef.Analysis;
using MouseChef.Analysis.Analyzers;
using MouseChef.Input;
using MouseChef.Models;
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
        private readonly EventProcessor _eventProcessor = new EventProcessor();
        private readonly Dictionary<Mouse, LineSeries> _series = new Dictionary<Mouse, LineSeries>();

        private static IAnalyzer[] Analzyers() => new IAnalyzer[]
        {
            new LagAnalyzer(),
            new AngleAnalyzer(),
            new DPIAnalyzer(), 
        };

        public MultiAnalyzerModel MultiAnalyzer { get; } = new MultiAnalyzerModel
            (Analzyers().Select(a => new AnalyzerModel(a)));

        public MainViewModel()
        {
            Plot.Axes.Add(MakeAxis(AxisPosition.Bottom, "x"));
            Plot.Axes.Add(MakeAxis(AxisPosition.Left, "y"));
            Plot.Series.Add(_mouse);

            _inputReader = new InputReader(this);
        }

        public PlotModel Plot { get; set; } = new PlotModel();

        public void DeviceInfo(DeviceInfoEvent evt) => _eventProcessor.DeviceInfo(evt);

        public LineSeries SeriesForMouse(Mouse mouse)
        {
            LineSeries found;
            if (_series.TryGetValue(mouse, out found)) return found;
            found = new LineSeries
            {
                DataFieldX = "X",
                DataFieldY = "Y",
            };
            Plot.Series.Add(found);
            _series[mouse] = found;
            return found;
        }

        private static IEnumerable<Vec> MovesToPositions(IEnumerable<Move> moves)
        {
            var pos = new Vec(0, 0);
            yield return pos;
            foreach (var move in moves)
            {
                pos += move.D;
                yield return pos;
            }
        }

        public void Move(MoveEvent evt)
        {
            _eventProcessor.Move(evt);
            var moves = MultiAnalyzer.Update(_eventProcessor.Moves);
            lock (Plot.SyncRoot)
            {
                foreach (var mouse in _eventProcessor.Mice)
                {
                    var series = SeriesForMouse(mouse);
                    series.ItemsSource = MovesToPositions(moves.Where(m => m.Mouse == mouse));
                }
            }
            Plot.InvalidatePlot(updateData: true);
        }

        public void Dispose() => _inputReader.Dispose();
    }
}
