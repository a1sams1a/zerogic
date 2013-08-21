using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zerogic
{
    /// <summary>Represent operator of zeroth-order logic</summary>
    public enum Operator
    {
        Null,               // Invaild
        Atom,               // Atom
        Not,                // One sub-proposition
        And, Or,            // Two sub-proposition, NNF
        Imply, IfOnlyIf     // Two sub-proposition
    }
}
