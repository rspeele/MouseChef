namespace MouseChef.USB
{
    public static class USBInfo
    {
        private const string ResourcePath = nameof(MouseChef) + ".USB.usb.ids";
        private static UsbIdsFile LoadFileFromResource()
        {
            var file = new UsbIdsFile();
            using (var resource = typeof(USBInfo).Assembly.GetManifestResourceStream(ResourcePath))
            {
                file.Load(resource);
            }
            return file;
        }
        private static readonly UsbIdsFile File = LoadFileFromResource();

        public static bool Lookup(int vendorId, int productId, out string vendorName, out string productName)
            => File.Lookup(vendorId, productId, out vendorName, out productName);
    }
}
