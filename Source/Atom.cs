using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zerogic
{
    /// <summary>Represent one atom</summary>
    public class Atom
    {
        /// <summary>Represent atom symbol</summary>
        public readonly string Symbol;

        /// <summary>Make atom with symbol</summary>
        /// <param name="symbol">Symbol of atom</param>
        public Atom(string symbol)
        {
            this.Symbol = symbol;
        }

        /// <summary>Convert atom to normal string</summary>
        /// <returns>String of atom</returns>
        public override string ToString()
        {
            return Symbol;
        }
    }
}
