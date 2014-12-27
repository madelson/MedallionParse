using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.Parse.Builders
{
    public sealed class GrammarBuilder
    {
        private readonly List<Production> productions = new List<Production>();

        public SymbolTypeBuilder Define(SymbolType symbolType)
        {
            Throw.IfNull(symbolType, "symbolType");

            return new SymbolTypeBuilder(this, symbolType);
        }

        internal void Add(Production production)
        {
            Throw.IfNull(production, "production");
            Throw.If(this.productions.Contains(production), "production: cannot add the same rule to a grammar twice!");

            this.productions.Add(production);
        }

        internal Grammar ToGrammar()
        {
            // todo some verification?

            return new Grammar(this.productions);
        }
    }
}
