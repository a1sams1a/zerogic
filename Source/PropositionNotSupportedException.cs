using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zerogic
{
    /// <summary>Represent proposition not supported exception</summary>
    public class PropositionNotSupportedException : Exception
    {
        /// <summary>Represent user-friendly message of not clause proposition</summary>
        public const string REASON_NOT_CLAUSE = "주어진 논리식이 Clause가 아닙니다.";
        /// <summary>Represent user-friendly message of not CNF proposition</summary>
        public const string REASON_NOT_CNF = "주어진 논리식이 CNF가 아닙니다.";
        /// <summary>Represent user-friendly message of not NNF proposition</summary>
        public const string REASON_NOT_NNF = "주어진 논리식이 NNF가 아닙니다.";

        /// <summary>Make default exception object</summary>
        public PropositionNotSupportedException() : base() { }

        /// <summary>Make exception object with reason</summary>
        /// <param name="reason">Reason of exception</param>
        public PropositionNotSupportedException(string reason) : base(ConvertToMessage(reason)) { }

        /// <summary>Get error reason to user-friendly message</summary>
        /// <param name="reason">Error reason</param>
        /// <returns>User-friendly message</returns>
        private static string ConvertToMessage(string reason)
        {
            if (reason == "NOT_CLAUSE") return REASON_NOT_CLAUSE;
            else if (reason == "NOT_CNF") return REASON_NOT_CNF;
            else if (reason == "NOT_NNF") return REASON_NOT_NNF;
            else return reason;
        }
    }
}
