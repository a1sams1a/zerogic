using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zerogic
{
    /// <summary>Represent invaild proposition exception</summary>
    public class InvalidPropositionException: Exception
    {
        /// <summary>Represent user-friendly message of invaild proposition</summary>
        public const string REASON_INVAILD = "올바르지 않은 논리식입니다";

        /// <summary>Make default exception object</summary>
        public InvalidPropositionException() : base() { }

        /// <summary>Make exception object with reason</summary>
        /// <param name="reason">Reason of exception</param>
        public InvalidPropositionException(string reason) : base(ConvertToMessage(reason)) { }

        /// <summary>Get error reason to user-friendly message</summary>
        /// <param name="reason">Error reason</param>
        /// <returns>User-friendly message</returns>
        private static string ConvertToMessage(string reason)
        {
            if (reason == "INVAILD") return REASON_INVAILD;
            else return reason;
        }
    }
}
