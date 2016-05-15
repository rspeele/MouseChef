using System.Windows.Media;
using MouseChef.Input;

namespace MouseChef.Corrections
{
    public class Mouse
    {
        private static int _nextColor = 0;
        private static readonly Color[] Colors =
        {
            Color.FromRgb(255, 0, 0),
            Color.FromRgb(0, 200, 0),
            Color.FromRgb(0, 0, 255),
            Color.FromRgb(255, 128, 0),

            Color.FromRgb(0, 128, 255),
            Color.FromRgb(128, 0, 255),
            Color.FromRgb(0, 200, 255),
            Color.FromRgb(0, 128, 128),

            Color.FromRgb(128, 0, 0),
            Color.FromRgb(128, 128, 0),
            Color.FromRgb(255, 0, 128),
            Color.FromRgb(0, 0, 0),
        };
        private readonly DeviceInfoEvent _info;

        public Mouse(DeviceInfoEvent info)
        {
            _info = info;
            Color = Colors[_nextColor++ % Colors.Length];
        }

        public Color Color { get; }
    }
}