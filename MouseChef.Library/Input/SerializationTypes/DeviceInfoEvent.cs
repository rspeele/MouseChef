namespace MouseChef.Input
{
    public class DeviceInfoEvent
    {
        public int DeviceId { get; set; }
        public int UsbVendorId { get; set; }
        public int UsbProductId { get; set; }
        public string Driver { get; set; }
        public string Description { get; set; }
    }
}