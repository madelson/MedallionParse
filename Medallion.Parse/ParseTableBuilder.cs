using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.Parse
{
    /// <summary>
    /// Contructs the LL(1) parse table for a grammar
    /// </summary>
    internal static class ParseTableBuilder
    {
        public static IReadOnlyDictionary<SymbolType, IReadOnlyDictionary<SymbolType, IReadOnlyList<Production>>> BuildLookahead1Table(Grammar grammar)
        {
            // from http://en.wikipedia.org/wiki/LL_parser
            // T[A, a] contains A -> w if
            // - a is in first(w)
            // - OR w is nullable and a is in follow(A)

            var result = grammar.ProductionsByType.ToDictionary(
                g => g.Key,
                g => g.SelectMany(p =>
                        // anything in first(components) 
                        grammar.Info.FirstSet(p.Components)
                            // plus, if p is nullable, anything in the follow of produced
                            .Concat(
                                p.Components.All(grammar.Info.NullableSet.Contains)
                                    ? grammar.Info.FollowSets[p.Produced]
                                    : Enumerable.Empty<SymbolType>()
                            ),
                        (production, type) => new { production, type }
                    )
                    .GroupBy(t => t.type)
                    .ToDictionary(gg => gg.Key, gg => gg.Select(t => t.production).ToArray().As<IReadOnlyList<Production>>())
                    .As<IReadOnlyDictionary<SymbolType, IReadOnlyList<Production>>>()
            );
            return result;
        }
    }
}
