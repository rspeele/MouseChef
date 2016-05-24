using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Win32;
using MouseChef.Analysis;
using MouseChef.Analysis.Analyzers;
using MouseChef.Input;
using MouseChef.Models;
using Newtonsoft.Json;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Mouse = MouseChef.Analysis.Mouse;

namespace MouseChef
{
    public class MainViewModel : IEventProcessor, IDisposable, INotifyPropertyChanged
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

        private readonly EventProcessor _eventProcessor = new EventProcessor();
        private readonly Dictionary<Mouse, LineSeries> _series = new Dictionary<Mouse, LineSeries>();
        private InputReader _reader;

        private static IEnumerable<IAnalyzer> Analzyers() => new IAnalyzer[]
        {
            new LagAnalyzer(),
            new AngleAnalyzer(),
            new DPIAnalyzer(), 
        };

        public MultiAnalyzerModel MultiAnalyzer { get; } = new MultiAnalyzerModel
            (Analzyers().Select(a => new AnalyzerModel(a)));

        private void Reset()
        {
            _eventProcessor.Reset();
            _series.Clear();
            MultiAnalyzer.Reset();
            BaselineMouse.Reset();
            SubjectMouse.Reset();
            Plot.Series.Clear();
            Plot.InvalidatePlot(updateData: true);
        }

        public MainViewModel()
        {
            Plot.Axes.Add(MakeAxis(AxisPosition.Bottom, "x"));
            Plot.Axes.Add(MakeAxis(AxisPosition.Left, "y"));

            BaselineMouse = new MouseInfoViewModel(MultiAnalyzer, isBaseline: true) { Caption = "Baseline Mouse" };
            SubjectMouse = new MouseInfoViewModel(MultiAnalyzer, isBaseline: false) { Caption = "Subject Mouse" };
            Reset();
        }

        public MouseInfoViewModel BaselineMouse { get; set; }
        public MouseInfoViewModel SubjectMouse { get; set; }

        public PlotModel Plot { get; set; } = new PlotModel();

        public bool Recording
        {
            get { return _recording; }
            set
            {
                if (_recording == value) return;
                _recording = value; OnPropertyChanged();
            }
        }

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

        private int _bufferCount = 0;
        private bool _recording;

        public void Move(MoveEvent evt)
        {
            if (_eventProcessor.Move(evt))
            {
                if (BaselineMouse.SelectedMouse == null)
                {
                    BaselineMouse.SelectedMouse = _eventProcessor.Mice.FirstOrDefault();
                }
                if (SubjectMouse.SelectedMouse == null)
                {
                    SubjectMouse.SelectedMouse = _eventProcessor.Mice.FirstOrDefault(m => m != BaselineMouse.SelectedMouse);
                }
                SubjectMouse.MouseOptions = _eventProcessor.Mice.ToList();
                BaselineMouse.MouseOptions = _eventProcessor.Mice.ToList();
            }
            if (_bufferCount++ < 100) return;
            _bufferCount = 0;
            var moves = MultiAnalyzer.Update(_eventProcessor.Moves);
            foreach (var mouse in _eventProcessor.Mice)
            {
                var series = SeriesForMouse(mouse);
                series.ItemsSource = MovesToPositions(moves.Where(m => m.Mouse == mouse));
            }
            Plot.InvalidatePlot(updateData: true);
        }

        private void StopRecording()
        {
            if (_reader == null) return;
            _reader.Dispose();
            _reader = null;
            Recording = false;
        }

        private void StartRecording()
        {
            StopRecording();
            Reset();
            _reader = new InputReader(this);
            Recording = true;
        }

        private void OpenFile()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Mouse Logs (*.json)|*.json",
                CheckFileExists = true,
                CheckPathExists = true,
            };
            if (dialog.ShowDialog() != true) return;
            MoveEvent lastMove = null;
            using (var reader = new StreamReader(File.OpenRead(dialog.FileName)))
            {
                string line;
                while (null != (line = reader.ReadLine()))
                {
                    var evt = JsonConvert.DeserializeObject<Event>(line);
                    switch (evt.Type)
                    {
                        case EventType.DeviceInfo:
                            _eventProcessor.DeviceInfo(evt.DeviceInfo);
                            break;
                        case EventType.Move:
                            if (lastMove != null)
                            {
                                _eventProcessor.Move(lastMove);
                            }
                            lastMove = evt.Move;
                            break;
                    }
                }
            }
            if (lastMove != null)
            {
                Move(lastMove);
            }
        }

        private void SaveFile()
        {
            throw new NotImplementedException();
        }

        public ICommand StartRecordingCommand => new Command(StartRecording);
        public ICommand StopRecordingCommand => new Command(StopRecording);
        public ICommand OpenFileCommand => new Command(OpenFile);
        public ICommand SaveFileCommand => new Command(SaveFile);

        public void Dispose() => StopRecording();
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
