using System.Collections.Generic;
using MouseChef.Input;

namespace MouseChef.Analysis
{
    public class EventProcessor
    {
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
            Moves.Add(new Move(mouse, evt.Us, evt.Dx, evt.Dy));
            return newMouse;
        }

        public void Reset()
        {
            _deviceInfos.Clear();
            _mice.Clear();
            Moves.Clear();
        }
    }
}
