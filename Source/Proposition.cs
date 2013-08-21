using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zerogic
{
    /// <summary>Represent one proposition</summary>
    public class Proposition
    {
        /// <summary>Represent left sub-proposition</summary>
        public readonly Proposition left;
        /// <summary>Represent right sub-proposition</summary>
        public readonly Proposition right;
        /// <summary>Represent operator</summary>
        public readonly Operator op;
        /// <summary>Represent atom</summary>
        public readonly Atom atom;

        /// <summary>Make proposition with operator and atom</summary>
        /// <param name="op">Operator of proposition(Atom Expected)</param>
        /// <param name="atom">Atom of proposition</param>
        public Proposition(Operator op, Atom atom) : this(op, atom, null, null) { }

        /// <summary>Make proposition with operator and one sub-proposition</summary>
        /// <param name="op">Operator of proposition(Not Expected)</param>
        /// <param name="expr1">Sub-proposition of proposiiton</param>
        public Proposition(Operator op, Proposition expr1) : this(op, null, expr1, null) { }

        /// <summary>Make proposiiton with operator and two sub-proposition</summary>
        /// <param name="op">Operator of proposition</param>
        /// <param name="expr1">First sub-proposition of proposition</param>
        /// <param name="expr2">Second sub-proposition of proposition</param>
        public Proposition(Operator op, Proposition expr1, Proposition expr2) : this(op, null, expr1, expr2) { }

        /// <summary>Make proposiiton with operator, atom and two sub-proposition</summary>
        /// <param name="op">Operator of proposition</param>
        /// <param name="atom">Atom of proposition</param>
        /// <param name="expr1">First sub-proposition of proposition</param>
        /// <param name="expr2">Second sub-proposition of proposition</param>
        public Proposition(Operator op, Atom atom, Proposition expr1, Proposition expr2)
        {
            this.left = expr1;
            this.right = expr2;
            this.atom = atom;
            this.op = op;
        }

        /// <summary>Check this proposition is vaild</summary>
        /// <returns>True if vaild</returns>
        public bool IsVaild()
        {
            if (op == Operator.Null) return false;
            else if (op == Operator.Atom) return (atom != null);
            else if (op == Operator.Not) return (left != null && left.IsVaild());
            else if (op == Operator.Or || op == Operator.And || op == Operator.Imply || op == Operator.IfOnlyIf)
                return (left != null && right != null && left.IsVaild() && right.IsVaild());
            else return false;
        }

        /// <summary>Check this proposition is negative normal form</summary>
        /// <returns>True if NNF</returns>
        public bool IsNNF()
        {
            if (!IsVaild()) return false;

            if (op == Operator.Atom) return true;
            else if (op == Operator.Imply || op == Operator.IfOnlyIf) return false;
            else if (op == Operator.Not) return (left.op == Operator.Atom);
            else if (op == Operator.Or || op == Operator.And) return (left.IsNNF() && right.IsNNF());
            else return false;
        }

        /// <summary>Check this proposition is conjunction normal form</summary>
        /// <returns>True if CNF</returns>
        public bool IsCNF()
        {
            if (!IsNNF()) return false;

            if (op == Operator.Atom) return true;
            else if (op == Operator.Not) return (left.op == Operator.Atom);
            else if (op == Operator.And) return (left.IsCNF() && right.IsCNF());
            else if (op == Operator.Or) return this.IsClause();
            else return false;
        }

        /// <summary>Check this proposition is clause</summary>
        /// <returns>True if Clause</returns>
        public bool IsClause()
        {
            if (!IsVaild()) return false;

            if (op == Operator.Atom) return true;
            else if (op == Operator.Imply || op == Operator.IfOnlyIf || op == Operator.And) return false;
            else if (op == Operator.Not) return (left.op == Operator.Atom);
            else if (op == Operator.Or) return (left.IsClause() && right.IsClause());
            else return false;
        }

        /// <summary>Check this clause is tautology</summary>
        /// <exception cref="PropositionNotSupportedException"></exception>
        /// <returns>True if tautology</returns>
        public bool IsTautologyClause()
        {
            if (!IsClause()) throw new PropositionNotSupportedException("NOT_CLAUSE");

            List<string> posList = GetPositiveAtomList();
            List<string> negList = GetNegativeAtomList();
            if (posList.Contains(ParseEngine.SYMBOL_T)) return true;
            return (posList.Intersect(negList).Count() != 0);
        }

        /// <summary>Get list of atom string in proposition</summary>
        /// <exception cref="InvaildPropositionException"></exception>
        /// <returns>List of atom string(Excluded T, F)</returns>
        public List<string> GetAtomList()
        {
            if (!IsVaild()) throw new InvalidPropositionException("INVAILD");

            string str = ToString();
            List<string> list = new List<string>();
            for (int i = 0; i < str.Length; i++)
                if (char.IsUpper(str[i]) && !list.Contains(str[i] + ""))
                    list.Add(str[i] + "");
            list.Remove(ParseEngine.SYMBOL_F + "");
            list.Remove(ParseEngine.SYMBOL_T + "");
            return list;
        }

        /// <summary>Get list of positive atom string in NNF proposition</summary>
        /// <exception cref="PropositionNotSupportedException"></exception>
        /// <returns>List of positive atom string(Include T)</returns>
        public List<string> GetPositiveAtomList()
        {
            if (!IsNNF()) throw new PropositionNotSupportedException("NOT_NNF");

            List<string> posList = new List<string>();
            if (op == Operator.Atom)
            {
                if (atom.Symbol != ParseEngine.SYMBOL_F) posList.Add(atom.Symbol);
            }
            else if (op == Operator.Not)
            {
                if (left.atom.Symbol == ParseEngine.SYMBOL_F) posList.Add(ParseEngine.SYMBOL_T);
            }
            else if (op == Operator.And || op == Operator.Or)
                posList.AddRange(left.GetPositiveAtomList().Union(right.GetPositiveAtomList()));
            return posList;
        }

        /// <summary>Get list of negative atom string in NNF proposition</summary>
        /// <exception cref="PropositionNotSupportedException"></exception>
        /// <returns>List of negative atom string(Include F)</returns>
        public List<string> GetNegativeAtomList()
        {
            if (!IsNNF()) throw new PropositionNotSupportedException("NOT_NNF");

            List<string> negList = new List<string>();
            if (op == Operator.Atom)
            {
                if (atom.Symbol == ParseEngine.SYMBOL_F) negList.Add(ParseEngine.SYMBOL_F);
            }
            else if (op == Operator.Not)
            {
                if (left.atom.Symbol != ParseEngine.SYMBOL_F) negList.Add(left.atom.Symbol);
            }
            else if (op == Operator.And || op == Operator.Or)
                negList.AddRange(left.GetNegativeAtomList().Union(right.GetNegativeAtomList()));
            return negList;
        }

        /// <summary>Convert proposition to normal string</summary>
        /// <returns>String of proposition</returns>
        public override string ToString()
        {
            if (!IsVaild()) return InvalidPropositionException.REASON_INVAILD;

            if (op == Operator.IfOnlyIf) return "(" + left.ToString() + ")<->(" + right.ToString() + ")";
            else if (op == Operator.Imply) return "(" + left.ToString() + ")->(" + right.ToString() + ")";
            else if (op == Operator.And) return "(" + left.ToString() + ")&(" + right.ToString() + ")";
            else if (op == Operator.Or) return "(" + left.ToString() + ")|(" + right.ToString() + ")";
            else if (op == Operator.Not) return "~(" + left.ToString() + ")";
            else if (op == Operator.Atom) return atom.ToString();
            else return "";
        }
    }
}