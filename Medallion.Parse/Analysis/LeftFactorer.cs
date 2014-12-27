using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.Parse.Analysis
{
    /// <summary>
    /// Disambiguates by combining productions with common prefixes
    /// 
    /// Left factoring from http://goose.ycp.edu/~dhovemey/fall2012/cs340/lecture/lecture05.html
    /// </summary>
    internal static class LeftFactorer
    {
        public static Grammar LeftFactor(Grammar grammar)
        {
            var productions = grammar.Productions.ToList();
            var changed = false;
            while (TryLeftFactorOne(productions))
            {
                changed = true;
            }

            return changed ? new Grammar(productions) : grammar;
        }

        private static bool TryLeftFactorOne(List<Production> productions)
        {
            // find a set of productions with a common lead symbol
            var grouping = productions.Where(s => s.Components.Count > 0)
                .GroupBy(g => g.Produced)
                .SelectMany(g => g.GroupBy(s => s.Components[0]))
                .FirstOrDefault(g => g.Count() > 1);

            // if no such set exists, return
            if (grouping == null)
            {
                return false;
            }

            // find the longest common prefix among the set
            var groupingArray = grouping.ToArray();
            var longestCommonPrefix = Enumerable.Range(1, groupingArray.Min(p => p.Components.Count))
                .Reverse()
                .Select(i => groupingArray[0].Components.Take(i))
                .First(prefix => groupingArray.All(p => p.Components.StartsWith(prefix)));

            // rewrite
            // TODO this is just wrong. We should make a suffix type SUFFIX, such that for A -> a b c and A -> a b d, we get
            // A -> a b SUFFIX, SUFFIX -> c, SUFFIX -> d
            var prefixType = SymbolType.PrefixType(grouping.Key, longestCommonPrefix);
            for (var i = 0; i < productions.Count; ++i)
            {
                if (groupingArray.Contains(productions[i]))
                {
                    // replace at same precedence
                    productions[i] = new Production(prefixType, prefixType.Prefix.Concat(productions[i].Components.Skip(prefixType.Prefix.Count)));
                }
            }
            productions.Add(new Production(prefixType, prefixType.Prefix));

            return true;
        }
    }
}
