using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MouseChef.USB
{
    internal class UsbIdsFile
    {
        private class Vendor
        {
            public Vendor(string name)
            {
                Name = name;
            }

            public Dictionary<int, string> Products { get; } = new Dictionary<int, string>();
            public string Name { get; }
        }
        private readonly Dictionary<int, Vendor> _vendorMap = new Dictionary<int, Vendor>();

        public bool Lookup(int vendorId, int productId, out string vendorName, out string productName)
        {
            Vendor vendor;
            if (!_vendorMap.TryGetValue(vendorId, out vendor))
            {
                vendorName = null;
                productName = null;
                return false;
            }
            vendorName = vendor.Name;
            vendor.Products.TryGetValue(productId, out productName);
            return true;
        }

        private static readonly Regex RelevantLine = new Regex(@"^(?<tab>\t)?(?<id>[0-9a-fA-F]{4})\s+(?<name>.+)$");

        public void Load(IEnumerable<string> lines)
        {
            Vendor currentVendor = null;
            foreach (var line in lines.Where(l => !string.IsNullOrWhiteSpace(l)))
            {
                var match = RelevantLine.Match(line);
                if (!match.Success)
                {
                    currentVendor = null;
                    continue;
                }
                var id = int.Parse(match.Groups["id"].Value, NumberStyles.HexNumber);
                var name = match.Groups["name"].Value;
                if (!match.Groups["tab"].Success)
                {
                    var vendor = new Vendor(name);
                    _vendorMap.Add(id, vendor);
                    currentVendor = vendor;
                }
                else
                {
                    currentVendor?.Products.Add(id, name);
                }
            }
        }

        private static IEnumerable<string> Lines(TextReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }

        public void Load(TextReader reader) => Load(Lines(reader));

        public void Load(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                Load(reader);
            }
        }
    }
}
