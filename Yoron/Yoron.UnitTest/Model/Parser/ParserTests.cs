using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yoron.Model.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Yoron.Model.Parser.Tests
{
    [TestClass()]
    public class ParserTests
    {
        [TestMethod()]
        public void 標準出力_文字列()
        {
            var lambda = Parser.GetLambdaExpression("Writeln(\"Hello World\")");
            var a = lambda.Body as BlockExpression;
            Assert.AreEqual(a.Expressions.Count, 1);
        }

        [TestMethod()]
        public void 標準入力()
        {
            var lambda = Parser.GetLambdaExpression("var s = Readln()");
            var a = lambda.Body as BlockExpression;
            Assert.AreEqual(a.Expressions.Count, 1);
        }

        [TestMethod()]
        public void 標準出力_int32()
        {
            var lambda = Parser.GetLambdaExpression("Writeln(2)");
            var a = lambda.Body as BlockExpression;
            Assert.AreEqual(a.Expressions.Count, 1);
        }

        [TestMethod()]
        public void 標準出力_bool()
        {
            var lambda = Parser.GetLambdaExpression("Writeln(true);Writeln(false);");
            var a = lambda.Body as BlockExpression;
            Assert.AreEqual(a.Expressions.Count, 2);
        }

        [TestMethod]
        public void 標準出力_整数_n行()
        {
            var rnd = new Random();
            var i = rnd.Next(10000);
            var stringBuilder = Enumerable.Range(0, i)
                .Aggregate(new StringBuilder(), (sb, x) => sb.Append($"Writeln({x})\r\n"));
            var lambda = Parser.GetLambdaExpression(stringBuilder.ToString());
            var a = lambda.Body as BlockExpression;
            Assert.AreEqual(a.Expressions.Count, i);
        }

        [TestMethod()]
        public void 変数宣言_出力()
        {
            var lambda = Parser.GetLambdaExpression("var x = 1;Writeln(x)");
            var a = lambda.Body as BlockExpression;
            Assert.AreEqual(a.Expressions.Count, 2);
        }

        [TestMethod()]
        public void 変数宣言_整数加算_出力()
        {
            var lambda = Parser.GetLambdaExpression("var x = 1 + 1;Writeln(x)");
            var a = lambda.Body as BlockExpression;
            Assert.AreEqual(a.Expressions.Count, 2);
        }

        [TestMethod()]
        public void 変数宣言_整数加算項３つ_出力()
        {
            var lambda = Parser.GetLambdaExpression("var x = 1 + 1 + 1;Writeln(x)");
            var a = lambda.Body as BlockExpression;
            Assert.AreEqual(a.Expressions.Count, 2);
        }

        [TestMethod()]
        public void 変数宣言_整数減算_出力()
        {
            var lambda = Parser.GetLambdaExpression("var x = 1 - 1;Writeln(x)");
            var a = lambda.Body as BlockExpression;
            Assert.AreEqual(a.Expressions.Count, 2);
        }

        [TestMethod()]
        public void 変数宣言_整数乗算_出力()
        {
            var lambda = Parser.GetLambdaExpression("var x = 1 * 1;Writeln(x)");
            var a = lambda.Body as BlockExpression;
            Assert.AreEqual(a.Expressions.Count, 2);
        }

        [TestMethod()]
        public void 変数宣言_整数除算_出力()
        {
            var lambda = Parser.GetLambdaExpression("var x = 4 / 2;Writeln(x)");
            var a = lambda.Body as BlockExpression;
            Assert.AreEqual(a.Expressions.Count, 2);
        }

        [TestMethod()]
        public void 変数宣言_整数剰余_出力()
        {
            var lambda = Parser.GetLambdaExpression("var x = 4 % 2;Writeln(x)");
            var a = lambda.Body as BlockExpression;
            Assert.AreEqual(a.Expressions.Count, 2);
        }

        [TestMethod()]
        public void 変数宣言_等値_出力()
        {
            var lambda = Parser.GetLambdaExpression("var x = 1 == 1;var y = 1 != 1;Writeln(x);Writeln(y);");
            var a = lambda.Body as BlockExpression;
            Assert.AreEqual(a.Expressions.Count, 4);
        }

        [TestMethod()]
        public void 変数宣言_比較()
        {
            var lambda = Parser.GetLambdaExpression("var a = 2 > 1;var b = 2 < 1;var c = 2 >= 1;var d = 2 <= 1;");
            var a = lambda.Body as BlockExpression;
            Assert.AreEqual(a.Expressions.Count, 4);
        }

        [TestMethod()]
        public void 変数宣言_条件ANDOR()
        {
            var lambda = Parser.GetLambdaExpression("var a = true && true;var b = true || false;");
            var a = lambda.Body as BlockExpression;
            Assert.AreEqual(a.Expressions.Count, 2);
        }

        [TestMethod()]
        public void 変数宣言_加算代入()
        {
            var lambda = Parser.GetLambdaExpression("var a = 1;a += 3;");
            var a = lambda.Body as BlockExpression;
            Assert.AreEqual(a.Expressions.Count, 2);
        }

        [TestMethod()]
        public void 変数宣言_if文()
        {
            var lambda = Parser.GetLambdaExpression("var a = 1;if(a == 1){var x = \"aは1です。\"; Writeln(x);}");
            var a = lambda.Body as BlockExpression;
            Assert.AreEqual(a.Expressions.Count, 2);
        }

        [TestMethod()]
        public void 変数宣言_while文()
        {
            var lambda = Parser.GetLambdaExpression("var a = 1;while(a < 100){Writeln(a); a += 1;}");
            var a = lambda.Body as BlockExpression;
            Assert.AreEqual(a.Expressions.Count, 2);
        }
    }
}