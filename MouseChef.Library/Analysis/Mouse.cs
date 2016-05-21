using System.Drawing;
using MouseChef.Input;

namespace MouseChef.Analysis
{
    public class Mouse
    {
        private static int _nextColor = 0;
        private static readonly Color[] Colors =
        {
            Color.FromArgb(255, 0, 0),
            Color.FromArgb(0, 200, 0),
            Color.FromArgb(0, 0, 255),
            Color.FromArgb(255, 128, 0),

            Color.FromArgb(0, 128, 255),
            Color.FromArgb(128, 0, 255),
            Color.FromArgb(0, 200, 255),
            Color.FromArgb(0, 128, 128),

            Color.FromArgb(128, 0, 0),
            Color.FromArgb(128, 128, 0),
            Color.FromArgb(255, 0, 128),
            Color.FromArgb(0, 0, 0),
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