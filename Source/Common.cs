using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zerogic
{
    /// <summary>Provide common static methods</summary>
    public static class Common
    {
        /// <summary>Deep copy of this dictionary</summary>
        /// <param name="source">Source dictionary</param>
        /// <returns>Deep copied dictionary</returns>
        public static Dictionary<string, bool> DeepCopy(this Dictionary<string, bool> source)
        {
            Dictionary<string, bool> newDict = new Dictionary<string, bool>();
            foreach (KeyValuePair<string, bool> pair in source)
                newDict.Add(pair.Key, pair.Value);
            return newDict;
        }
    }
}
