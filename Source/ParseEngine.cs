using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zerogic
{
    /// <summary>Provide parsing methods</summary>
    /// Credit: Grant Malcolm, http://cgi.csc.liv.ac.uk/~grant/Teaching/COMP213/Code/Parser.java
    public class ParseEngine
    {
        #region Constants

        /// <summary>Symbol of left parenthesis</summary>
        public const string SYMBOL_LP = "(";
        /// <summary>Symbol of right parenthesis</summary>
        public const string SYMBOL_RP = ")";
        /// <summary>Symbol of and operator</summary>
        public const string SYMBOL_AND = "&";
        /// <summary>Symbol of or operator</summary>
        public const string SYMBOL_OR = "|";
        /// <summary>Symbol of imply operator</summary>
        public const string SYMBOL_IMP = "->";
        /// <summary>Symbol of ifandonlyif operator</summary>
        public const string SYMBOL_IFF = "<->";
        /// <summary>Symbol of not operator</summary>
        public const string SYMBOL_NOT = "~";
        /// <summary>Symbol of true</summary>
        public const string SYMBOL_T = "T";
        /// <summary>Symbol of false</summary>
        public const string SYMBOL_F = "F";

        /// <summary>Priority of proposition</summary>
        private const int PREC_MAX = 10;
        /// <summary>Priority of ifandonlyif operator</summary>
        private const int PREC_IFF = 8;
        /// <summary>Priority of imply operator</summary>
        private const int PREC_IMP = 7;
        /// <summary>Priority of or operator</summary>
        private const int PREC_OR = 5;
        /// <summary>Priority of and operator</summary>
        private const int PREC_AND = 4;
        /// <summary>Priority of not operator</summary>
        private const int PREC_NOT = 1;

        #endregion

        /// <summary>Store parse string</summary>
        private char[] input;
        /// <summary>Store current position in parsing steps</summary>
        private int index;
        /// <summary>Store position of SYMBOL_RP in parsing steps</summary>
        private int backIndex;

        /// <summary>Parse string to proposition object</summary>
        /// <param name="expr">Parsed proposition object</param>
        /// <param name="result">Result of parsing</param>
        /// <param name="str">String to parse</param>
        /// <returns>True if success parsing</returns>
        public bool TryParse(out Proposition expr, out string result, string str)
        {
            expr = null; result="";
            backIndex = 0;
            index = 0;

            try
            {
                input = Normalize(str).ToCharArray();
                expr = ParseWithPrec(PREC_MAX);
            }
            catch (ParseException e)
            {
                result = e.Message;
                return false;
            }

            if (index != input.Length)
            {
                result = ParseException.REASON_NOT_READ_ALL; return false;
            }
            return true;
        }

        /// <summary>Normalize string to parse</summary>
        /// <param name="str">String to normalize</param>
        /// <returns>Normalized string</returns>
        private string Normalize(string str)
        {
            str = str.Trim();
            if (str.Length == 0) throw new ParseException("NULL_STRING");

            str = str.ToUpper();

            if (str.IndexOf("0") != -1 || str.IndexOf("1") != -1)
                throw new ParseException("NOT_PERMITTED", "-1");

            str = str.Replace(SYMBOL_IFF, "1").Replace(SYMBOL_IMP, "0").Replace(" ", "");

            bool isAtom = false;
            for (int i = 0; i < str.Length; i++)
            {
                if (isAtom && char.IsUpper(str[i]))
                    throw new ParseException("ATOM_LENGTH", (i - 1).ToString());
                if (char.IsUpper(str[i]))
                {
                    isAtom = true;
                    continue;
                }
                isAtom = false;
                if ("" + str[i] != SYMBOL_LP && "" + str[i] != SYMBOL_RP && "" + str[i] != SYMBOL_AND &&
                    "" + str[i] != SYMBOL_AND && "" + str[i] != SYMBOL_OR && "" + str[i] != SYMBOL_NOT &&
                    "" + str[i] != "0" && "" + str[i] != "1")
                    throw new ParseException("NOT_PERMITTED", i.ToString());
            }

            string newStr = "";
            for (int i = 0; i < str.Length; i++) newStr += str[i] + " ";
            newStr = newStr.Substring(0, newStr.Length - 1).Replace("1", SYMBOL_IFF).Replace("0", SYMBOL_IMP);
            return newStr;
        }

        /// <summary>Parse from current position with given priority</summary>
        /// <param name="prec">Priority</param>
        /// <returns>Parsed proposition</returns>
        private Proposition ParseWithPrec(int prec)
        {
            Proposition expr1, expr2;
            expr1 = GetNextExpression();

            backIndex = index;

            while (index != input.Length)
            {
                string s = GetNextToken();

                if (s == SYMBOL_RP)
                {
                    index = backIndex;
                    return expr1;
                }

                Operator op = GetOperator(s);
                int pr = GetPrecedence(op);

                if (pr > prec)
                {
                    index = backIndex;
                    return expr1;
                }
                else
                {
                    expr2 = ParseWithPrec(pr);
                    expr1 = new Proposition(op, expr1, expr2);
                }
            }
            return expr1;
        }

        /// <summary>Find next proposition from current position</summary>
        /// <returns>Proposition</returns>
        private Proposition GetNextExpression()
        {
            string s = GetNextToken();
            if (s == SYMBOL_LP)
            {
                Proposition expr = ParseWithPrec(PREC_MAX);
                if (!MoveNext(SYMBOL_RP)) throw new ParseException("NOT_MATCH_PARN", index.ToString());
                return expr;
            }
            else if (s == SYMBOL_NOT)
            {
                Proposition expr = ParseWithPrec(PREC_NOT);
                return new Proposition(Operator.Not, expr);
            }
            else
                return new Proposition(Operator.Atom, new Atom(s));
        }

        /// <summary>Get operator of give string</summary>
        /// <param name="s">String to convert</param>
        /// <returns>Operator</returns>
        private Operator GetOperator(string s)
        {
            if (s == SYMBOL_AND)
                return Operator.And;
            else if (s == SYMBOL_OR)
                return Operator.Or;
            else if (s == SYMBOL_IMP)
                return Operator.Imply;
            else if (s == SYMBOL_IFF)
                return Operator.IfOnlyIf;
            else
                throw new ParseException("NOT_OPERATOR", s);
        }

        /// <summary>Get priority of given operator</summary>
        /// <param name="op">Operator</param>
        /// <returns>Priority</returns>
        private int GetPrecedence(Operator op)
        {
            switch (op)
            {
                case Operator.Imply: return PREC_IMP;
                case Operator.IfOnlyIf: return PREC_IFF;
                case Operator.Or: return PREC_OR;
                case Operator.And: return PREC_AND;
                case Operator.Not: return PREC_NOT;
                default: return 0;
            }
        }

        /// <summary>Find next token from current position</summary>
        /// <returns>String</returns>
        private string GetNextToken()
        {
            if ("" + input[index] == SYMBOL_LP)
            {
                index++;
                SkipWhiteSpace();
                return SYMBOL_LP + "";
            }
            else if ("" + input[index] == SYMBOL_RP)
            {
                index++;
                SkipWhiteSpace();
                return SYMBOL_RP + "";
            }
            return GetNextBlock();
        }

        /// <summary>Move to next position with given string</summary>
        /// <param name="s">String to move</param>
        /// <returns>True if success moving</returns>
        private bool MoveNext(string s)
        {
            if (index + s.Length > input.Length)
                throw new ParseException("NEED_AT_END", s);

            string t = new string(input, index, s.Length);
            index += s.Length;
            SkipWhiteSpace();
            return t == s;
        }

        /// <summary>Read until whitespace appear</summary>
        /// <returns>String</returns>
        private string GetNextBlock()
        {
            int start = index;

            while (index != input.Length && !char.IsWhiteSpace(input[index]))
                index++;

            string s = new string(input, start, index - start);
            SkipWhiteSpace();
            return s;
        }

        /// <summary>Skip whitespace and move cursor</summary>
        private void SkipWhiteSpace()
        {
            while (index != input.Length && char.IsWhiteSpace(input[index]))
                index++;
        }
    }
}
