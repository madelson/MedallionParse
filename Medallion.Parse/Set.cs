using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.Parse
{
    internal interface IReadOnlySet<T> : IReadOnlyCollection<T> 
    {
        bool Contains(T value);
    }

    internal sealed class Set<T> : HashSet<T>, IReadOnlySet<T>
    {
        public Set(IEqualityComparer<T> comparer = null) 
            : base(comparer ?? EqualityComparer<T>.Default)    
        {
        }

        public Set(IEnumerable<T> items, IEqualityComparer<T> comparer = null)
            : base(items, comparer ?? EqualityComparer<T>.Default)
        {   
        }
    }
}
