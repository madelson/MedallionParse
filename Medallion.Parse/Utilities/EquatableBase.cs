using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.Parse.Utilities
{
    /// <summary>
    /// A base class which simplifies the implementation of a class with value equality semantics
    /// </summary>
    public abstract class EquatableBase<T> where T : EquatableBase<T>
    {
        // forbid external inheritors
        internal EquatableBase() { }

        protected abstract bool EqualsInternal(T that);
        protected abstract int GetHashCodeInternal();

        public sealed override bool Equals(object obj)
        {
            return this == (obj as T);
        }

        private int hash;
        public sealed override int GetHashCode()
        {
            if (this.hash == 0)
            {
                this.hash = this.GetHashCodeInternal();
            }
            return this.hash;
        }

        public static bool operator ==(EquatableBase<T> @this, T that)
        {
            if (ReferenceEquals(@this, that))
            {
                return true;
            }
            if (@this == null || that == null || @this.GetHashCode() != that.GetHashCode())
            {
                return false;
            }

            return @this.EqualsInternal(that);
        }
        
        public static bool operator !=(EquatableBase<T> @this, T that) 
        {
            return !(@this == that);
        }
    }
}
