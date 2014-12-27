//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Medallion.Parse
//{
//    public abstract class Symbol
//    {
//        internal Symbol(SymbolType type) 
//        {
//            this.Type = type;
//        }

//        public SymbolType Type { get; private set; } 
//        public abstract int Position { get; }
//        public abstract int Length { get; }

//        public abstract IReadOnlyList<Symbol> Children { get; }

//        #region ---- Token ----
//        public static Symbol Token(SymbolType type, string text)
//        {
//            Throw.IfNull(type, "type");
//            Throw.IfNullOrEmpty(text, "text");

//            return new Token(type, position, length);
//        }

//        private sealed class Token : Symbol
//        {
//            public Token(SymbolType type, int position, int length)
//                : base(type)
//            {
//                this.position = position;
//                this.length = length;
//            }

//            private readonly int position;
//            public override int Position { get { return this.position; } }

//            private readonly int length;
//            public override int Length { get { return this.length; } }

//            public override IReadOnlyList<Symbol> Children
//            {
//                get { return Empty<Symbol>.Array; }
//            }
//        }
//        #endregion

//        #region ---- NonTerminal ----
//        public static Symbol Create(SymbolType type, IEnumerable<Symbol> children)
//        {
//            Throw.IfNull(type, "type");
//            Throw.IfNull(children, "children");

//            var childrenList = new List<Symbol>();
//            foreach (Symbol child in children) 
//            {
//                Throw.IfNull(child, "children: all must be non-null");
//                childrenList.Add(child);
//            }

//            return CreateUnchecked(type, childrenList);
//        }

//        internal static Symbol CreateUnchecked(SymbolType type, IReadOnlyList<Symbol> children)
//        {

//        }

//        private sealed class NonTerminal : Symbol
//        {
//            public NonTerminal(SymbolType type, IReadOnlyList<Symbol> children)
//                : base(type)
//            {

//            }

//            public override int Position
//            {
//                get 
//                {
//                    return this.
//                }
//            }

//            public override int Length
//            {
//                get { return this.children.Sum(ch => ch.Length); }
//            }

//            private readonly IReadOnlyList<Symbol> children;
//            public override IReadOnlyList<Symbol> Children
//            {
//                get { return this.children; }
//            }
//        }
//        #endregion
//    }
//}
