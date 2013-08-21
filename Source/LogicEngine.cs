using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zerogic
{
    /// <summary>Provide logic calculation of proposition</summary>
    public class LogicEngine
    {
        /// <summary>Store list of true assignment table</summary>
        private List<Dictionary<string, bool>> trueList;

        /// <summary>Calculation engine</summary>
        private CalcEngine calcEng;

        /// <summary>Make new logic engine</summary>
        public LogicEngine()
        {
            calcEng = new CalcEngine();
        }

        /// <summary>Convert given proposition to negative normal form</summary>
        /// <param name="expr">Proposition to convert</param>
        /// <exception cref="InvaildPropositionException"></exception>
        /// <returns>NNF proposition</returns>
        public Proposition ConvertToNNF(Proposition expr)
        {
            if (expr == null || !expr.IsVaild()) throw new InvalidPropositionException("INVAILD");

            if (expr.op == Operator.Atom) return expr;
            else if (expr.op == Operator.Or) // S(P|Q) => S(P)|S(Q)
                return new Proposition(Operator.Or,
                    ConvertToNNF(expr.left),
                    ConvertToNNF(expr.right));
            else if (expr.op == Operator.And) // S(P&Q) => S(P)&S(Q)
                return new Proposition(Operator.And,
                    ConvertToNNF(expr.left),
                    ConvertToNNF(expr.right));
            else if (expr.op == Operator.Imply) // S(P->Q) => S(~P)|S(Q)
                return new Proposition(Operator.Or,
                    ConvertToNNF(new Proposition(Operator.Not, expr.left)),
                    ConvertToNNF(expr.right));
            else if (expr.op == Operator.IfOnlyIf) // S(P<->Q) => S(P->Q)&S(Q->P)
                return new Proposition(Operator.And,
                    ConvertToNNF(new Proposition(Operator.Imply, expr.left, expr.right)),
                    ConvertToNNF(new Proposition(Operator.Imply, expr.right, expr.left)));
            else if (expr.op == Operator.Not) // S(~P) =>
            {
                Proposition nexpr = expr.left;
                if (nexpr.op == Operator.Atom) // S(T) => F, S(F) => T, S(~P) => ~P
                {
                    if (nexpr.atom.Symbol == "T") return new Proposition(Operator.Atom, new Atom("F"));
                    else if (nexpr.atom.Symbol == "F") return new Proposition(Operator.Atom, new Atom("T"));
                    else return expr;
                }

                if (nexpr.op == Operator.Imply) // S(P->Q) => S(~P)|S(Q)
                    nexpr = new Proposition(Operator.Or,
                        ConvertToNNF(new Proposition(Operator.Not, nexpr.left)),
                        ConvertToNNF(nexpr.right));
                else if (nexpr.op == Operator.IfOnlyIf) // S(P<->Q) => S(P->Q)&S(Q->P)
                    nexpr = new Proposition(Operator.And,
                        ConvertToNNF(new Proposition(Operator.Imply, nexpr.left, nexpr.right)),
                        ConvertToNNF(new Proposition(Operator.Imply, nexpr.right, nexpr.left)));

                if (nexpr.op == Operator.Or) // S(~(P|Q)) => ~S(P)&~S(Q)
                    return new Proposition(Operator.And,
                        ConvertToNNF(new Proposition(Operator.Not, nexpr.left)),
                        ConvertToNNF(new Proposition(Operator.Not, nexpr.right)));
                else if (nexpr.op == Operator.And) // S(~(P&Q)) => ~S(P)|~S(Q)
                    return new Proposition(Operator.Or,
                        ConvertToNNF(new Proposition(Operator.Not, nexpr.left)),
                        ConvertToNNF(new Proposition(Operator.Not, nexpr.right)));
                else if (nexpr.op == Operator.Not) // S(~(~P))) => S(P)
                    return ConvertToNNF(nexpr.left);
            }
            return null;
        }

        /// <summary>Convert given NNF proposition to CNF proposition</summary>
        /// <param name="expr">NNF proposition to convert</param>
        /// <exception cref="PropositionNotSupportException"></exception>
        /// <returns>CNF proposition</returns>
        public Proposition ConvertToCNF(Proposition expr)
        {
            if (expr == null || !expr.IsNNF()) throw new PropositionNotSupportedException("NOT_NNF");

            if (expr.op == Operator.Atom) return expr;
            else if (expr.op == Operator.And) // S(P&Q) => S(P)&S(Q)
                return new Proposition(Operator.And,
                    ConvertToCNF(expr.left),
                    ConvertToCNF(expr.right));
            else if (expr.op == Operator.Or) // S(P|Q) => 
            {
                Proposition c1 = ConvertToCNF(expr.left);
                Proposition c2 = ConvertToCNF(expr.right);

                if (c1.op != Operator.And && c2.op != Operator.And) // S(P|Q) => S(P)|S(Q)
                    return new Proposition(Operator.Or, c1, c2);

                if (c1.op != Operator.And)
                {
                    Proposition tmp = c1; c1 = c2; c2 = tmp; // S((A&B)|C) => S(A|C)&S(B|C)
                }
                return new Proposition(Operator.And,
                    ConvertToCNF(new Proposition(Operator.Or, c1.left, c2)),
                    ConvertToCNF(new Proposition(Operator.Or, c1.right, c2)));
            }
            else if (expr.op == Operator.Not) // S(P) => S(P)
            {
                if (expr.left.op == Operator.Atom) // Must Be Unit According to Def. of NNF
                    return expr;
            }
            return null;
        }

        /// <summary>Check whether give proposition is tautology</summary>
        /// <param name="expr">Proposition to check</param>
        /// <exception cref="InvaildPropositionException"></exception>
        /// <returns>True if tautology</returns>
        public bool IsTautology(Proposition expr)
        {
            if (expr == null || !expr.IsVaild()) throw new InvalidPropositionException("INVAILD");
            if (!expr.IsCNF()) expr = ConvertToCNF(ConvertToNNF(expr));
            return IsTautologyCore(expr);
        }

        /// <summary>Tautology core engine</summary>
        /// <param name="expr">Proposition to check</param>
        /// <returns>True if tautology</returns>
        private bool IsTautologyCore(Proposition expr)
        {
            if (expr.op == Operator.Atom)
            {
                if (expr.atom.Symbol == "T") return true;
                else return false;
            }
            else if (expr.op == Operator.Not)
            {
                if (expr.atom.Symbol == "F") return true;
                else return false;
            }
            else if (expr.op == Operator.And)
                return IsTautologyCore(expr.left) && IsTautologyCore(expr.right);
            else if (expr.op == Operator.Or)
                return expr.IsTautologyClause();
            else 
                return false;
        }

        /// <summary>Check whether give proposition is contradiction</summary>
        /// <param name="expr">Proposition to check</param>
        /// <exception cref="InvaildPropositionException"></exception>
        /// <returns>True if contradiction</returns>
        public bool IsContradiction(Proposition expr)
        {
            if (expr == null || !expr.IsVaild()) throw new InvalidPropositionException("INVAILD");
            return (GetTrueAssignmentList(expr).Count == 0);
        }

        /// <summary>Find assignment list which each assignment table make give proposition true</summary>
        /// <param name="expr">Proposition to get true assignment list</param>
        /// <exception cref="InvaildPropositionException"></exception>
        /// <returns>Assignment list</returns>
        public List<Dictionary<string, bool>> GetTrueAssignmentList(Proposition expr)
        {
            if (expr == null || !expr.IsVaild()) throw new InvalidPropositionException("INVAILD");

            trueList = new List<Dictionary<string, bool>>();
            GetTrueAssignmentListCore(expr, new Dictionary<string, bool>());
            return trueList;
        }

        /// <summary>Build all possible assignment and calculate</summary>
        /// <param name="expr">Proposition to calculate</param>
        /// <param name="dict">Assignment list</param>
        private void GetTrueAssignmentListCore(Proposition expr, Dictionary<string, bool> dict)
        {
            if (expr.GetAtomList().Count == dict.Count)
            {
                bool value; string result;
                calcEng.TryEvaluate(out value, out result, expr, dict);
                if (value) trueList.Add(dict.DeepCopy());
            }
            else
            {
                string target = expr.GetAtomList().Except(dict.Keys.ToList()).First();
                Dictionary<string, bool> dict1 = dict.DeepCopy(), dict2 = dict.DeepCopy();
                dict1.Add(target, false);
                dict2.Add(target, true);

                GetTrueAssignmentListCore(expr, dict1);
                GetTrueAssignmentListCore(expr, dict2);
            }
        }
    }
}