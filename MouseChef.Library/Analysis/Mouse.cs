using System.Text.RegularExpressions;
using MouseChef.Input;
using MouseChef.USB;

namespace MouseChef.Analysis
{
    public class Mouse
    {
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

        public string Name { get; }
        public string UsbVendorName { get; }
        public string UsbProductName { get; }

        public string UsbVendor => $"{Info.UsbVendorId:X4}: {UsbVendorName ?? "(unknown)"}";
        public string UsbProduct => $"{Info.UsbProductId:X4}: {UsbProductName ?? "(unknown)"}";

        public override string ToString() => Name;
    }
}