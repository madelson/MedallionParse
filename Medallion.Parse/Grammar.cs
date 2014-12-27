using Medallion.Parse.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.Parse
{
    internal sealed class Grammar
    {
        private readonly Lazy<GrammarInfo> info;

        public Grammar(IEnumerable<Production> productions)
        {
            this.Productions = productions.ToArray();
            this.info = new Lazy<GrammarInfo>(() => new GrammarInfo(this.Productions));
        }

        public IReadOnlyList<Production> Productions { get; private set; }

        private ILookup<SymbolType, Production> productionsByType;
        public ILookup<SymbolType, Production> ProductionsByType
        {
            get { return this.productionsByType ?? (this.productionsByType = this.Productions.ToLookup(p => p.Produced)); }
        }

        public GrammarInfo Info { get { return this.info.Value; } }
    }
}
