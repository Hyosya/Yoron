using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yoron.Model.Lexer;
using Yoron.Model.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoron.Model.Lexer.Tests
{
    [TestClass()]
    public class LexerTests
    {
        [TestMethod]
        public void 左括弧()
        {
            var tokenList = Lexer.GetTokenList("(");
            Assert.IsTrue(tokenList[0] is LeftParenthesis && tokenList[0].RawValue == "(");
        }

        [TestMethod]
        public void 右括弧()
        {
            var tokenList = Lexer.GetTokenList(")");
            Assert.IsTrue(tokenList[0] is RightParenthesis && tokenList[0].RawValue == ")");
        }

        [TestMethod()]
        public void Intリテラル()
        {
            var tokenList = Lexer.GetTokenList("123");
            Assert.IsTrue(tokenList[0] is Int32Literal && tokenList[0].RawValue == "123");
        }

        [TestMethod()]
        public void 文字列リテラル()
        {
            var tokenList = Lexer.GetTokenList("\"文字列\"");
            Assert.IsTrue(tokenList[0] is StringLiteral && tokenList[0].RawValue == "文字列");
        }

        [TestMethod()]
        public void boolリテラル()
        {
            var tokenList = Lexer.GetTokenList("true;false;");
            Assert.AreEqual(tokenList.Count(i => i is BoolLiteral), 2);
        }

        [TestMethod]
        public void 行末()
        {
            var tokenlist = Lexer.GetTokenList(";\r\n");
            Assert.IsTrue(tokenlist.Count > 0);
            Assert.IsTrue(tokenlist.All(i => i is EndOfLine));
        }


        [TestMethod()]
        public void 標準出力_文字列()
        {
            var tokenList = Lexer.GetTokenList("Writeln(\"Hello World\")");
            Assert.IsTrue(tokenList.Any(i => i is StandardOutput) && tokenList.Any(i => i is StringLiteral));
        }

        [TestMethod()]
        public void 標準入力()
        {
            var tokenList = Lexer.GetTokenList("var s = Readln()");
            Assert.IsTrue(tokenList.Any(i => i is StandardInput) && tokenList.Any(i => i is VarDeclaration));
        }

        [TestMethod]
        public void 標準出力2行()
        {
            var tokenList = Lexer.GetTokenList("Writeln(\"Hello\")\r\nWriteln(\"Hello\")");
            Assert.AreEqual(tokenList.Count(i => i is StandardOutput), 2);
            Assert.AreEqual(tokenList.Count(i => i is StringLiteral), 2);
            Assert.IsTrue(tokenList.Any(i => i is EndOfLine));
        }

        [TestMethod]
        public void 変数_var_int代入()
        {
            var tokenList = Lexer.GetTokenList("var x = 1");
            Assert.AreEqual(tokenList.Count(t => t is Int32Literal), 1);
            Assert.AreEqual(tokenList.Count(t => t is Assign), 1);
            Assert.AreEqual(tokenList.Count(t => t is VarDeclaration), 1);
        }

        [TestMethod]
        public void 加算代入()
        {
            var tokenList = Lexer.GetTokenList("x += 1");
            Assert.AreEqual(tokenList.Count(t => t is Int32Literal), 1);
            Assert.AreEqual(tokenList.Count(t => t is AddAssign), 1);
            Assert.AreEqual(tokenList.Count(t => t is Identifier), 1);
        }

        [TestMethod]
        public void 変数出力()
        {
            var tokenList = Lexer.GetTokenList("Writeln(x)");
            Assert.AreEqual(tokenList.Count(t => t is StandardOutput), 1);
            Assert.AreEqual(tokenList.Count(t => t is Identifier), 1);
        }

        [TestMethod]
        public void 整数足し算()
        {
            var tokenList = Lexer.GetTokenList("1 + 1");
            Assert.AreEqual(tokenList.Count(t => t is Int32Literal), 2);
            Assert.AreEqual(tokenList.Count(t => t is PlusOperator), 1);
        }

        [TestMethod]
        public void 整数引き算()
        {
            var tokenList = Lexer.GetTokenList("1 - 1");
            Assert.AreEqual(tokenList.Count(t => t is Int32Literal), 2);
            Assert.AreEqual(tokenList.Count(t => t is MinusOperator), 1);
        }

        [TestMethod]
        public void 整数掛け算()
        {
            var tokenList = Lexer.GetTokenList("2 * 2");
            Assert.AreEqual(tokenList.Count(t => t is Int32Literal), 2);
            Assert.AreEqual(tokenList.Count(t => t is MultiplyOperator), 1);
        }

        [TestMethod]
        public void 整数割り算()
        {
            var tokenList = Lexer.GetTokenList("2 / 2");
            Assert.AreEqual(tokenList.Count(t => t is Int32Literal), 2);
            Assert.AreEqual(tokenList.Count(t => t is DivideOperator), 1);
        }

        [TestMethod]
        public void 整数剰余()
        {
            var tokenList = Lexer.GetTokenList("2 % 2");
            Assert.AreEqual(tokenList.Count(t => t is Int32Literal), 2);
            Assert.AreEqual(tokenList.Count(t => t is ModuloOperator), 1);
        }

        [TestMethod]
        public void イコール()
        {
            var tokenList = Lexer.GetTokenList("2 == 2");
            Assert.AreEqual(tokenList.Count(t => t is Int32Literal), 2);
            Assert.AreEqual(tokenList.Count(t => t is EqualOperator), 1);
        }

        [TestMethod]
        public void ノットイコール()
        {
            var tokenList = Lexer.GetTokenList("2 != 2");
            Assert.AreEqual(tokenList.Count(t => t is Int32Literal), 2);
            Assert.AreEqual(tokenList.Count(t => t is NotEqualOperator), 1);
        }

        [TestMethod]
        public void 大なり()
        {
            var tokenList = Lexer.GetTokenList("2 > 1");
            Assert.AreEqual(tokenList.Count(t => t is Int32Literal), 2);
            Assert.AreEqual(tokenList.Count(t => t is GreaterThanOperator), 1);
        }

        [TestMethod]
        public void 小なり()
        {
            var tokenList = Lexer.GetTokenList("2 < 1");
            Assert.AreEqual(tokenList.Count(t => t is Int32Literal), 2);
            Assert.AreEqual(tokenList.Count(t => t is LessThanOperator), 1);
        }

        [TestMethod]
        public void 大なりイコール()
        {
            var tokenList = Lexer.GetTokenList("2 >= 1");
            Assert.AreEqual(tokenList.Count, 4);
            Assert.AreEqual(tokenList.Count(t => t is Int32Literal), 2);
            Assert.AreEqual(tokenList.Count(t => t is GreaterThanOrEqualOperator), 1);
        }

        [TestMethod]
        public void 小なりイコール()
        {
            var tokenList = Lexer.GetTokenList("2 <= 1");
            Assert.AreEqual(tokenList.Count, 4);
            Assert.AreEqual(tokenList.Count(t => t is Int32Literal), 2);
            Assert.AreEqual(tokenList.Count(t => t is LessThanOrEqualOperator), 1);
        }

        [TestMethod]
        public void 条件AND()
        {
            var tokenList = Lexer.GetTokenList("true && false");
            Assert.AreEqual(tokenList.Count, 4);
            Assert.AreEqual(tokenList.Count(t => t is BoolLiteral), 2);
            Assert.AreEqual(tokenList.Count(t => t is AndAlsoOperator), 1);
        }

        [TestMethod]
        public void 条件OR()
        {
            var tokenList = Lexer.GetTokenList("true || false");
            Assert.AreEqual(tokenList.Count, 4);
            Assert.AreEqual(tokenList.Count(t => t is BoolLiteral), 2);
            Assert.AreEqual(tokenList.Count(t => t is OrElseOperator), 1);
        }

        [TestMethod]
        public void If文()
        {
            var tokenList = Lexer.GetTokenList("if (true) { Writeln(true) }");
            Assert.AreEqual(tokenList.Count(t => t is BoolLiteral), 2);
            Assert.AreEqual(tokenList.Count(t => t is IfKeyword), 1);
            Assert.AreEqual(tokenList.Count(t => t is BlockBegin), 1);
            Assert.AreEqual(tokenList.Count(t => t is BlockEnd), 1);
        }

        [TestMethod]
        public void While文()
        {
            var tokenList = Lexer.GetTokenList("while (true) { Writeln(true) }");
            Assert.AreEqual(tokenList.Count(t => t is BoolLiteral), 2);
            Assert.AreEqual(tokenList.Count(t => t is WhileKeyword), 1);
            Assert.AreEqual(tokenList.Count(t => t is BlockBegin), 1);
            Assert.AreEqual(tokenList.Count(t => t is BlockEnd), 1);
        }

        [TestMethod]
        public void Breakキーワード()
        {
            var tokenList = Lexer.GetTokenList("break");
            Assert.AreEqual(tokenList.Count(t => t is BreakKeyword), 1);
        }

        [TestMethod]
        public void Continueキーワード()
        {
            var tokenList = Lexer.GetTokenList("continue");
            Assert.AreEqual(tokenList.Count(t => t is ContinueKeyword), 1);
        }
    }
}