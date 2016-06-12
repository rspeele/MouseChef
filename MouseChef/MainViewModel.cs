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
using Xceed.Wpf.Toolkit;
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

        private readonly EventHistory _eventHistory = new EventHistory();
        private readonly Dictionary<Mouse, LineSeries> _series = new Dictionary<Mouse, LineSeries>();
        private InputReader _reader;

        private static IEnumerable<IAnalyzer> Analzyers() => new IAnalyzer[]
        {
            new LagAnalyzer(),
            new AngleAnalyzer(),
            new DPIAnalyzer(),
            new BaselineSpeedAnalyzer(),
        };

        public MultiAnalyzerModel MultiAnalyzer { get; } = new MultiAnalyzerModel
            (Analzyers().Select(a => new AnalyzerModel(a)));

        public List<Graphable> Graphables { get; }

        private void Reset()
        {
            _eventHistory.Reset();
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

            foreach (var analyzer in MultiAnalyzer.Analyzers)
            {
                analyzer.PropertyChanged += AnalyzerOnPropertyChanged;
            }

            BaselineMouse = new MouseInfoViewModel(MultiAnalyzer, isBaseline: true) { Caption = "Baseline Mouse" };
            BaselineMouse.PropertyChanged += MouseInfoOnPropertyChanged;
            SubjectMouse = new MouseInfoViewModel(MultiAnalyzer, isBaseline: false) { Caption = "Subject Mouse" };
            SubjectMouse.PropertyChanged += MouseInfoOnPropertyChanged;
            Graphables = MultiAnalyzer.Analyzers.Select(a => new Graphable(a.Analyzer.Name, () => a.LatestStats))
                .Concat(new[] { new Graphable("Time (s)", () => new Seconds()) })
                .ToList();
            SelectedX = Graphables.LastOrDefault();
            SelectedY = Graphables.FirstOrDefault();
            Reset();
        }

        private void MouseInfoOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MouseInfoViewModel.SelectedMouse))
            {
                UpdateAnalysis();
            }
        }

        private static readonly string[] UpdateOnAnalyzerProperties =
        {
            nameof(AnalyzerModel.OverrideFactor),
            nameof(AnalyzerModel.FactorMode),
        };
        private void AnalyzerOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (UpdateOnAnalyzerProperties.Contains(e.PropertyName))
            {
                UpdateAnalysis();
            }
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

        public void StoreEvent(Event evt) => _eventHistory.StoreEvent(evt);

        public void DeviceInfo(DeviceInfoEvent evt) => _eventHistory.DeviceInfo(evt);

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

        private int _bufferCount;
        private bool _recording;

        public void Move(MoveEvent evt) => Move(evt, bufferUpdate: 25);

        private void UpdateAnalysis()
        {
            _bufferCount = 0;
            var moves = MultiAnalyzer.Update(_eventHistory.Moves);
            foreach (var mouse in _eventHistory.Mice)
            {
                var series = SeriesForMouse(mouse);
                series.ItemsSource = MovesToPositions(moves.Where(m => m.Mouse == mouse));
            }
            Plot.InvalidatePlot(updateData: true);
        }

        public void Move(MoveEvent evt, int bufferUpdate)
        {
            if (_eventHistory.Move(evt))
            {
                if (BaselineMouse.SelectedMouse == null)
                {
                    BaselineMouse.SelectedMouse = _eventHistory.Mice.FirstOrDefault();
                }
                if (SubjectMouse.SelectedMouse == null)
                {
                    SubjectMouse.SelectedMouse = _eventHistory.Mice.FirstOrDefault(m => m != BaselineMouse.SelectedMouse);
                }
                SubjectMouse.MouseOptions = _eventHistory.Mice.ToList();
                BaselineMouse.MouseOptions = _eventHistory.Mice.ToList();
            }
            if (_bufferCount++ < bufferUpdate) return;
            UpdateAnalysis();
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
            StopRecording();
            Reset();
            using (var reader = new StreamReader(File.OpenRead(dialog.FileName)))
            {
                string line;
                while (null != (line = reader.ReadLine()))
                {
                    var evt = JsonConvert.DeserializeObject<Event>(line);
                    switch (evt.Type)
                    {
                        case EventType.DeviceInfo:
                            DeviceInfo(evt.DeviceInfo);
                            break;
                        case EventType.Move:
                            Move(evt.Move, bufferUpdate: int.MaxValue);
                            break;
                    }
                }
            }
            UpdateAnalysis();
        }

        private void SaveFile()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Mouse Logs (*.json)|*.json",
                CheckPathExists = true,
            };
            if (dialog.ShowDialog() != true) return;
            try
            {
                using (var stream = File.Create(dialog.FileName))
                {
                    _eventHistory.Save(stream);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public Graphable SelectedX { get; set; }
        public Graphable SelectedY { get; set; }

        private void OpenGraph()
        {
            if (SelectedX != null && SelectedY != null)
            {
                new GraphWindow(SelectedX, SelectedY).ShowDialog();
            }
        }

        public ICommand StartRecordingCommand => new Command(StartRecording);
        public ICommand StopRecordingCommand => new Command(StopRecording);
        public ICommand ToggleRecordingCommand => new Command(() =>
        {
            if (Recording) StopRecording();
            else StartRecording();
        });
        public ICommand OpenFileCommand => new Command(OpenFile);
        public ICommand SaveFileCommand => new Command(SaveFile);
        public ICommand OpenGraphCommand => new Command(OpenGraph);

        public void Dispose() => StopRecording();
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
