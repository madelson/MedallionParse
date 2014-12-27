using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.Parse
{
    internal sealed class GrammarInfo
    {
        private readonly IReadOnlyCollection<Production> productions;
        private readonly Lazy<IReadOnlySet<SymbolType>> types, nullableSet, terminals, nonTerminals;
        private readonly Lazy<IReadOnlyDictionary<SymbolType, IReadOnlySet<SymbolType>>> firstSets, followSets;

        public GrammarInfo(IReadOnlyCollection<Production> productions)
        {
            this.productions = productions;

            this.types = new Lazy<IReadOnlySet<SymbolType>>(
                () => this.productions
                    .Select(p => p.Produced)
                    .Concat(this.productions.SelectMany(p => p.Components))
                    .ToSet()                    
            );

            this.nonTerminals = new Lazy<IReadOnlySet<SymbolType>>(
                () => this.productions.Select(p => p.Produced).ToSet()  
            );

            this.terminals = new Lazy<IReadOnlySet<SymbolType>>(
                () => this.Types.Except(this.NonTerminals).ToSet()
            );

            this.nullableSet = new Lazy<IReadOnlySet<SymbolType>>(this.ComputeNullableSet);
            this.firstSets = new Lazy<IReadOnlyDictionary<SymbolType,IReadOnlySet<SymbolType>>>(this.ComputeFirstSets);
            this.followSets = new Lazy<IReadOnlyDictionary<SymbolType, IReadOnlySet<SymbolType>>>(this.ComputeFollowSets);
        }

        public IReadOnlySet<SymbolType> Types { get { return this.types.Value; } }
        public IReadOnlySet<SymbolType> NonTerminals { get { return this.nonTerminals.Value; } }
        public IReadOnlySet<SymbolType> Terminals { get { return this.terminals.Value; } }
        public IReadOnlySet<SymbolType> NullableSet { get { return this.nullableSet.Value; } }
        public IReadOnlyDictionary<SymbolType, IReadOnlySet<SymbolType>> FirstSets { get { return this.firstSets.Value; } }
        public IReadOnlyDictionary<SymbolType, IReadOnlySet<SymbolType>> FollowSets { get { return this.followSets.Value; } }

        public IReadOnlySet<SymbolType> FirstSet(IEnumerable<SymbolType> symbolTypes)
        {
            Set<SymbolType> result = null;
            foreach (var symbolType in symbolTypes)
            {
                result = result ?? new Set<SymbolType>();
                result.AddRange(this.FirstSets[symbolType]);
                if (!this.NullableSet.Contains(symbolType))
                {
                    break;
                }
            }

            return result ?? Empty<SymbolType>.Set;
        }

        private IReadOnlySet<SymbolType> ComputeNullableSet()
        {
            var nullableSet = new Set<SymbolType>();
		    bool changed;
		    do {
			    changed = false;
                foreach (var production in this.productions)
                {
                    if (production.Components.All(nullableSet.Contains))
                    {
                        changed |= nullableSet.Add(production.Produced);
                    }
                }
		    } while (changed);

            return nullableSet;
        }

        private IReadOnlyDictionary<SymbolType, IReadOnlySet<SymbolType>> ComputeFirstSets()
        {
            var firstSets = new Dictionary<SymbolType, Set<SymbolType>>();
            
            // terminals first sets are themselves
            this.Terminals.ForEach(t => firstSets.Add(t, new Set<SymbolType> { t }));

            // non-terminals initialize to empty
            this.NonTerminals.ForEach(t => firstSets.Add(t, new Set<SymbolType>()));
            
            bool changed;
		    do {
			    changed = false;
			    foreach (var production in this.productions)
                {
				    foreach (SymbolType type in production.Components) 
                    {
					    changed |= firstSets[production.Produced].AddRange(firstSets[type]);
                        if (!this.NullableSet.Contains(type))
                        {
                            break;
                        }
				    }
                }
		    } while (changed);

            return firstSets.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.As<IReadOnlySet<SymbolType>>());
        }

        private IReadOnlyDictionary<SymbolType, IReadOnlySet<SymbolType>> ComputeFollowSets() 
        {
            // initialize as empty
            var followSets = this.Types.ToDictionary(t => t, t => new Set<SymbolType>());

            bool changed;
		    do 
            {
			    changed = false;
			    foreach (var production in this.productions) 
                {
				    var tailStillNullable = true;
				    for (int i = production.Components.Count - 1; i >= 0; i--) 
                    {
					    // if y_i+1 to end are nullable, follow(y_i) U=
					    // follow(x)
					    if (tailStillNullable) 
                        {
						    changed |= followSets[production.Components[i]].AddRange(followSets[production.Produced]);
						    tailStillNullable = this.NullableSet.Contains(production.Components[i]);
					    }

					    foreach (var follower in production.Components.Skip(i)) 
                        {
						    changed |= followSets[production.Components[i]].AddRange(this.FirstSets[follower]);
						    if (!this.NullableSet.Contains(follower))
                            {
							    break;
                            }
					    }
				    }
			    }
		    } while (changed);

            return followSets.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.As<IReadOnlySet<SymbolType>>());
        }
    }
}
