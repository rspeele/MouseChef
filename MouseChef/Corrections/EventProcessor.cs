using System.Collections.Generic;
using MouseChef.Input;

namespace MouseChef.Corrections
{
    public class EventProcessor : IEventProcessor
    {
        private readonly List<Move> _moves = new List<Move>();
        private readonly Dictionary<int, DeviceInfoEvent> _deviceInfos = new Dictionary<int, DeviceInfoEvent>();
        private readonly Dictionary<int, Mouse> _mice = new Dictionary<int, Mouse>();

        public void DeviceInfo(DeviceInfoEvent evt) => _deviceInfos[evt.DeviceId] = evt;

        public void Move(MoveEvent evt)
        {
            Mouse mouse;
            if (!_mice.TryGetValue(evt.DeviceId, out mouse))
            {
                mouse = new Mouse(_deviceInfos[evt.DeviceId]);
                _mice[evt.DeviceId] = mouse;
            }
            _moves.Add(new Move(mouse, evt.Us, evt.Dx, evt.Dy));
        }
    }
}
