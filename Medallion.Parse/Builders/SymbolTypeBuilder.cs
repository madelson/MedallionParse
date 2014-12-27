using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.Parse.Builders
{
    public sealed class SymbolTypeBuilder
    {
        private readonly GrammarBuilder builder;
        private readonly SymbolType symbolType;

        internal SymbolTypeBuilder(GrammarBuilder builder, SymbolType symbolType)
        {
            this.builder = builder;
            this.symbolType = symbolType;
        }

        public ContinuingBuilder As(params SymbolType[] componentTypes)
        {
            return this.As(componentTypes.AsEnumerable());
        }

        public ContinuingBuilder As(IEnumerable<SymbolType> componentTypes)
        {
            Throw.IfNull(componentTypes, "componentTypes");
            var componentTypesArray = componentTypes.ToArray();
            Throw.IfContainsNull(componentTypesArray, "componentTypes");

            this.builder.Add(new Production(this.symbolType, componentTypesArray));
            return new ContinuingBuilder(this);
        }

        public sealed class ContinuingBuilder
        {
            private readonly SymbolTypeBuilder builder;

            internal ContinuingBuilder(SymbolTypeBuilder builder) 
            {
                this.builder = builder;
            }

            public ContinuingBuilder OrAs(params SymbolType[] componentTypes)
            {
                return this.OrAs(componentTypes.AsEnumerable());
            }

            public ContinuingBuilder OrAs(IEnumerable<SymbolType> componentTypes)
            {
                return this.builder.As(componentTypes);
            }
        }
    }
}
