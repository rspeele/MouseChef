using System.Collections.Generic;
using System.IO;
using System.Text;
using MouseChef.Input;
using Newtonsoft.Json;

namespace MouseChef.Analysis
{
    public class EventHistory
    {
        private readonly List<Event> _events = new List<Event>();
        public List<Move> Moves { get; } = new List<Move>();
        private readonly Dictionary<int, DeviceInfoEvent> _deviceInfos = new Dictionary<int, DeviceInfoEvent>();
        private readonly Dictionary<int, Mouse> _mice = new Dictionary<int, Mouse>();
        public IEnumerable<Mouse> Mice => _mice.Values;

        public void DeviceInfo(DeviceInfoEvent evt) => _deviceInfos[evt.DeviceId] = evt;

        public bool Move(MoveEvent evt)
        {
            var newMouse = false;
            Mouse mouse;
            if (!_mice.TryGetValue(evt.DeviceId, out mouse))
            {
                mouse = new Mouse(_deviceInfos[evt.DeviceId]);
                _mice[evt.DeviceId] = mouse;
                newMouse = true;
            }
            var move = new Move(mouse, evt.Us, evt.Dx, evt.Dy);
            Moves.Add(move);
            mouse.OnMove(move);
            return newMouse;
        }

        public void StoreEvent(Event evt) => _events.Add(evt);

        public void Reset()
        {
            _events.Clear();
            _deviceInfos.Clear();
            _mice.Clear();
            Moves.Clear();
        }

        public void Save(Stream stream)
        {
            using (var writer = new StreamWriter(stream, Encoding.UTF8, 0x1000, leaveOpen: true))
            {
                foreach (var evt in _events)
                {
                    writer.WriteLine(JsonConvert.SerializeObject(evt, Formatting.None));
                }
            }
        }
    }
}
