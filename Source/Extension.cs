using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zerogic.Extension
{
    /// NOTE: Following class contaions extension methods of string and expression
    /// They can be used to make your code short, but try-catch blocks are highly recommended

    /// <summary>Store extension methods</summary>
    public static class Core
    {
        /// <summary>Used in logic calculation</summary>
        private static LogicEngine logEngine = new LogicEngine();
        /// <summary>Used in evalution calculation</summary>
        private static CalcEngine calcEngine = new CalcEngine();
        /// <summary>Used in parsing</summary>
        private static ParseEngine parseEngine = new ParseEngine();

        /// <summary>Parse string to proposition object</summary>
        /// <exception cref="ParseException"></exception>
        /// <returns>Proposition</returns>
        public static Proposition ToProposition(this string str)
        {
            Proposition expr; string result;
            if (parseEngine.TryParse(out expr, out result, str)) return expr;
            throw new ParseException(result);
        }

        /// <summary>Evaluate with given data</summary>
        /// <param name="data">Data to evaulate</param>
        /// <returns>Evaluation result</returns>
        public static bool Eval(this Proposition expr, Dictionary<string, bool> data)
        {
            bool value; string error;
            calcEngine.TryEvaluate(out value, out error, expr, data);
            return value;
        }

        /// <summary>Convert to NNF proposition</summary>
        /// <exception cref="InvaildPropositionException"></exception>
        /// <returns>NNF proposition</returns>
        public static Proposition ToNNF(this Proposition expr)
        {
            if (expr.IsNNF()) return expr;
            return logEngine.ConvertToNNF(expr);
        }

        /// <summary>Convert to CNF proposition</summary>
        /// <exception cref="InvaildPropositionException"></exception>
        /// <returns>CNF proposition</returns>
        public static Proposition ToCNF(this Proposition expr)
        {
            if (expr.IsCNF()) return expr;
            if (expr.IsNNF()) return logEngine.ConvertToCNF(expr);
            else return logEngine.ConvertToCNF(logEngine.ConvertToNNF(expr));
        }

        /// <summary>Check whether give proposition is tautology</summary>
        /// <exception cref="InvaildPropositionException"></exception>
        /// <returns>True if tautology</returns>
        public static bool IsTautology(this Proposition expr)
        {
            return logEngine.IsTautology(expr);
        }

        /// <summary>Check whether give proposition is contradiction</summary>
        /// <exception cref="InvaildPropositionException"></exception>
        /// <returns>True if contradiction</returns>
        public static bool IsContradiction(this Proposition expr)
        {
            return logEngine.IsContradiction(expr);
        }

        /// <summary>Find assignment list which each assignment table make give proposition true</summary>
        /// <exception cref="InvaildPropositionException"></exception>
        /// <returns>Assignment list</returns>
        public static List<Dictionary<string, bool>> GetTrueAssignmentList(this Proposition expr)
        {
            return logEngine.GetTrueAssignmentList(expr);
        }
    }
}
