using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zerogic
{
    /// <summary>Provide calculation of proposition</summary>
    public class CalcEngine
    {
        /// <summary>Represent assignment table</summary>
        private Dictionary<string, bool> evalData;

        /// <summary>Evaluate proposition with given data</summary>
        /// <param name="value">Result of evaluation</param>
        /// <param name="error">Error Message</param>
        /// <param name="expr">Proposition to evaluate</param>
        /// <param name="data">Assignment table</param>
        /// <returns>True if successed evaluate</returns>
        public bool TryEvaluate(out bool value, out string error, Proposition expr, Dictionary<string, bool> data)
        {
            value = false; error = "";
            if (expr == null || !expr.IsVaild())
            {
                error = InvalidPropositionException.REASON_INVAILD;
                return false;
            }

            List<string> atomList = expr.GetAtomList();
            List<string> keyList = data.Keys.ToList();
            if (atomList.Intersect(keyList).Count() != atomList.Count)
            {
                error = "모든 atom에 대해 적절한 대입값이 없습니다.";
                return false;
            }

            evalData = data;
            value = Evaluate(expr);
            return true;
        }

        /// <summary>Evaluate at given proposition</summary>
        /// <param name="expr">Proposition to evaluate</param>
        /// <returns>Result of evaluation</returns>
        private bool Evaluate(Proposition expr)
        {
            switch (expr.op)
            {
                case Operator.Atom:
                    string s = expr.atom.Symbol;
                    if (s == ParseEngine.SYMBOL_T) return true;
                    else if (s == ParseEngine.SYMBOL_F) return false;
                    return evalData[s];
                case Operator.Or:
                    return Evaluate(expr.left) || Evaluate(expr.right);
                case Operator.And:
                    return Evaluate(expr.left) && Evaluate(expr.right);
                case Operator.Not:
                    return !Evaluate(expr.left);
                case Operator.Imply:
                    return !Evaluate(expr.left) || Evaluate(expr.right);
                case Operator.IfOnlyIf:
                    return !(Evaluate(expr.left) ^ Evaluate(expr.right));
                default:
                    return false;
            }
        }
    }
}
