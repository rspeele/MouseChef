using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using MouseChef.Input;
using MouseChef.USB;

namespace MouseChef.Analysis
{
    public class Mouse : INotifyPropertyChanged
    {
        private readonly PointStats _pollingRate = new PointStats("Polling Rate", 0, 0.0, 1000.0, 100.0);
        public DeviceInfoEvent Info { get; }

        public Mouse(DeviceInfoEvent info)
        {
            Info = info;
            Name = Regex.Replace(Info?.Description ?? "", "^.*;", "");
            string vendor, device;
            USBInfo.Lookup(info.UsbVendorId, info.UsbProductId, out vendor, out device);
            UsbVendorName = vendor;
            UsbProductName = device;
        }

        private TimeSpan? _lastMove;
        internal void OnMove(Move move)
        {
            if (_lastMove == null)
            {
                _lastMove = move.Time;
                return;
            }
            var delta = move.Time - _lastMove.Value;
            _lastMove = move.Time;
            if (delta == TimeSpan.Zero) return;
            if (delta > TimeSpan.FromSeconds(0.1)) return; // no mouse polls that infrequently
            var rate = 1.0 / delta.TotalSeconds;
            _pollingRate.AddPoint(move.Time, rate);
            OnPropertyChanged(nameof(PollingRate));
        }

        public string Name { get; }
        public string UsbVendorName { get; }
        public string UsbProductName { get; }

        public string UsbVendor => $"{Info.UsbVendorId:X4}: {UsbVendorName ?? "(unknown)"}";
        public string UsbProduct => $"{Info.UsbProductId:X4}: {UsbProductName ?? "(unknown)"}";

        public override string ToString() => Name;

        public IStats PollingRate => _pollingRate;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}