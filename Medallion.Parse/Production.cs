using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.Parse
{
    internal enum ProductionKind
    {
        Normal,
        Option,
        Choice,
        Tuple,
        List,
    }

    //http://goose.ycp.edu/~dhovemey/fall2012/cs340/lecture/lecture05.html
    internal sealed class Production
    {
        public Production(SymbolType produced, IEnumerable<SymbolType> components)
        {
            this.Produced = produced;
            this.Components = components.ToArray();
        }

        public SymbolType Produced { get; private set; }
        public IReadOnlyList<SymbolType> Components { get; private set; }

        private int hash;
        public override int GetHashCode()
        {
            return this.hash != 0
                ? this.hash
                : this.hash = Hash.Combine(this.Produced.GetHashCode(), this.Components.Select(t => t.GetHashCode()).Aggregate(Hash.Combine));
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            var that = obj as Production;
            return that != null
                && that.hash == this.hash
                && Equals(that.Produced, this.Produced)
                && that.Components.SequenceEqual(this.Components);
        }
    }
}
