using System;
using MouseChef.Analysis;

namespace MouseChef
{
    public class Graphable
    {
        private readonly string _name;
        public Graphable(string name, Func<IStats> getStats)
        {
            _name = name;
            GetStats = getStats;
        }

        public Func<IStats> GetStats { get; }
        public override string ToString() => _name;
    }
}
