using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.Parse
{
    internal enum GeneratedSymbolType : byte
    {
        OptionChoice,
        Tuple,
        List,

        Eof,
        Error,
        Unrecognized,

        Prefix,
    }

    public sealed class SymbolType
    {
        public SymbolType(string name)
        {
            Throw.IfNullOrEmpty(name, "name");

            this.Name = name;
        }

        internal static SymbolType PrefixType(SymbolType producedType, IEnumerable<SymbolType> productionPrefix)
        {
            Throw.IfNull(producedType, "producedType");
            Throw.IfNull(productionPrefix, "productionPrefix");
            var prefix = productionPrefix.ToArray();
            Throw.If(prefix.Length == 0, "productionPrefix: must be populated");
            Throw.If(prefix.Contains(null), "productionPrefix: must not contain nulls");

            return new SymbolType(string.Format("prefix_for_{0}_of_{1}", producedType.Name, productionPrefix.ToDelimitedString("_")))
            {
                GeneratedType = GeneratedSymbolType.Prefix,
                ForType = producedType,
                Prefix = prefix,
            };
        }

        internal GeneratedSymbolType? GeneratedType { get; private set; }

        internal SymbolType ForType { get; private set; }
        internal IReadOnlyList<SymbolType> Prefix { get; private set; }

        public string Name { get; private set; }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            var that = obj as SymbolType;
            return that != null 
                && that.GetHashCode() == this.GetHashCode()
                && that.Name == this.Name
                && Equals(that.ForType, this.ForType)
                && SequenceComparer<SymbolType>.Default.Equals(that.Prefix, this.Prefix);
        }

        private int hash;
        public override int GetHashCode()
        {
            if (this.hash == 0)
            {
                int hash = this.Name.GetHashCode();
                if (this.GeneratedType.HasValue)
                {
                    hash = Hash.Combine(hash, this.GeneratedType.Value.GetHashCode());
                }
                if (this.ForType != null)
                {
                    hash = Hash.Combine(hash, this.ForType.GetHashCode());
                }
                if (this.Prefix != null)
                {
                    hash = Hash.Combine(hash, SequenceComparer<SymbolType>.Default.GetHashCode(this.Prefix));
                }

                this.hash = hash;
            }

            return this.hash;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
