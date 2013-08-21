using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zerogic
{
    /// <summary>Represent parse exception</summary>
    public class ParseException : Exception
    {
        /// <summary>Represent user-friendly message of not perfect parsingn</summary>
        public const string REASON_NOT_READ_ALL = "완벽하게 파싱되지 않았습니다.";
        /// <summary>Represent user-friendly message of given string is null</summary>
        public const string REASON_NULL_STRING = "빈 문자열입니다.";
        /// <summary>Represent user-friendly message of not permitted char</summary>
        public const string REASON_NOT_PERMITTED = "인덱스 {0}의 기호는 허용되지 않는 기호입니다.";
        /// <summary>Represent user-friendly message of length of atom is bigger than 1</summary>
        public const string REASON_ATOM_LENGTH = "인덱스{0}부터 시작하는 토큰의 길이가 1보다 큽니다.";
        /// <summary>Represent user-friendly message of need close parenthesis</summary>
        public const string REASON_NOT_MATCH_PARN = "인덱스 {0}에 닫는 괄호가 필요합니다.";
        /// <summary>Represent user-friendly message of not suitable operator</summary>
        public const string REASON_NOT_OPERATOR = "{0}은 올바른 연산자가 아닙니다.";
        /// <summary>Represent user-friendly message of need string at the end of string</summary>
        public const string REASON_NEED_AT_END = "식의 끝에 {0}이 필요합니다.";

        /// <summary>Make default exception object</summary>
        public ParseException() : base() { }

        /// <summary>Make exception object with reason</summary>
        /// <param name="reason">Reason of exception</param>
        public ParseException(string reason) : base(ConvertToMessage(reason, "")) { }

        /// <summary>Make exception object with reason and detail reason</summary>
        /// <param name="reason">Reason of exception</param>
        /// <param name="more">Detail reason of exception</param>
        public ParseException(string reason, string more) : base(ConvertToMessage(reason, more)) { }

        /// <summary>Get error reason to user-friendly message</summary>
        /// <param name="reason">Error reason</param>
        /// <returns>User-friendly message</returns>
        private static string ConvertToMessage(string reason, string more)
        {
            if (reason == "NOT_READ_ALL") return REASON_NOT_READ_ALL;
            else if (reason == "NULL_STRING") return REASON_NULL_STRING;
            else if (reason == "NOT_PERMITTED") return string.Format(REASON_NOT_PERMITTED, more);
            else if (reason == "ATOM_LENGTH") return string.Format(REASON_ATOM_LENGTH, more);
            else if (reason == "NOT_MATCH_PARN") return string.Format(REASON_NOT_MATCH_PARN, more);
            else if (reason == "NOT_OPERATOR") return string.Format(REASON_NOT_OPERATOR, more);
            else if (reason == "NEED_AT_END") return string.Format(REASON_NEED_AT_END, more);
            else return reason;
        }
    }
}
