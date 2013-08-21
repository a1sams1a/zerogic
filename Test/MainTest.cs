using System;
using System.IO;
using Zerogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZerogicTest
{
    [TestClass]
    public class MainTest
    {
        [TestMethod]
        public void TestMain()
        {

        }

        [TestMethod]
        public void RunMain()
        {
            Proposition expr;
            string result;

            //string s = "( p -> ( q & r ) ) <-> y";
            //string s = "(A ->( B -> C )) <-> ( C -> D ) & ( D -> E) ";
            //string s = "p<->p";
            //string s = "p <-> a & b";
            //string s = "((p | q) & (~p | r)) -> (q | r)";
            //string s = "(P|(S&T))|R";
            //string s = "(A&B)|(~A)|(~B)";
            //string s = "(~p)<->(p->F)";
            //string s = "(~((a&b)|c)&d)|(((a&b)|c)&(~d))";
            string s = "(a&b)|(c&d)|(e&f)";
            ParseEngine parseEngine = new ParseEngine();
            parseEngine.TryParse(out expr, out result, s);

            LogicEngine logicEng = new LogicEngine();
            Proposition nnfExpr = logicEng.ConvertToNNF(expr);

            Proposition cnfExpr = logicEng.ConvertToCNF(nnfExpr);
            bool b = logicEng.IsTautology(cnfExpr);
            WriteFile(cnfExpr.ToString());
        }


        private void WriteFile(string content)
        {
            File.WriteAllText("..\\..\\RunResult.txt", content);
        }
    }
}
