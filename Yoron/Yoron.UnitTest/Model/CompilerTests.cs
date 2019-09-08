using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yoron.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Yoron.Model.Tests
{
    [TestClass()]
    public class CompilerTests
    {
        private const string TestFileName = "Test";
        private const string OutputFileName = "Test.exe";

        [TestMethod()]
        public void 実行ファイル生成()
        {
            Compiler.Compile("Writeln(\"Hello World\")", TestFileName);
            Assert.IsTrue(File.Exists("Test.exe"));
        }

        [TestMethod]
        public async Task 標準入力_標準出力()
        {
            Compiler.Compile("var s = Readln();Writeln(s);", TestFileName);
            var outputValue = await GetOutputStringFromChildProcess("Hello", OutputFileName);
            Assert.AreEqual(outputValue, "Hello\r\n");
        }

        [TestMethod()]
        public async Task 文字列リテラル出力()
        {
            Compiler.Compile("Writeln(\"Hello World\")", TestFileName);
            var outputValue = await GetOutputStringFromChildProcess(OutputFileName);
            Assert.AreEqual(outputValue, "Hello World\r\n");
        }

        [TestMethod()]
        public async Task 整数出力()
        {
            Compiler.Compile("Writeln(2)", TestFileName);
            var outputValue = await GetOutputStringFromChildProcess(OutputFileName);
            Assert.AreEqual(outputValue, "2\r\n");
        }

        [TestMethod()]
        public async Task Bool出力()
        {
            Compiler.Compile("Writeln(true);Writeln(false);", TestFileName);
            var outputValue = await GetOutputStringFromChildProcess(OutputFileName);
            Assert.AreEqual(outputValue, "True\r\nFalse\r\n");
        }

        [TestMethod]
        public async Task 整数変数出力()
        {
            Compiler.Compile("var x = 2;Writeln(x)", TestFileName);
            var outputValue = await GetOutputStringFromChildProcess(OutputFileName);
            Assert.AreEqual(outputValue, "2\r\n");
        }

        [TestMethod]
        public async Task 整数加減算出力()
        {
            Compiler.Compile("var x = 3 + 2 - 4;var y = 5 - 1 + 3;Writeln(x);Writeln(y)", TestFileName);
            var outputValue = await GetOutputStringFromChildProcess(OutputFileName);
            Assert.AreEqual(outputValue, "1\r\n7\r\n");
        }

        [TestMethod]
        public async Task 整数乗除算出力()
        {
            Compiler.Compile("var x = 3 * 4;var y = 8 * 8 / 32;Writeln(x);Writeln(y)", TestFileName);
            var outputValue = await GetOutputStringFromChildProcess(OutputFileName);
            Assert.AreEqual(outputValue, "12\r\n2\r\n");
        }

        [TestMethod]
        public async Task 整数剰余算出力()
        {
            Compiler.Compile("var x = 6 % 4;var y = 2 + 4 * 5 % 2;Writeln(x);Writeln(y)", TestFileName);
            var outputValue = await GetOutputStringFromChildProcess(OutputFileName);
            Assert.AreEqual(outputValue, "2\r\n2\r\n");
        }

        [TestMethod]
        public async Task 整数文字列等値演算出力()
        {
            Compiler.Compile("var a = \"A\";var b = \"B\";var x = 2;var y = 2;Writeln(a != b);Writeln(x != y)", TestFileName);

            var outputValue = await GetOutputStringFromChildProcess(OutputFileName);
            Assert.AreEqual(outputValue, "True\r\nFalse\r\n");
        }

        [TestMethod]
        public async Task 整数比較演算出力()
        {
            Compiler.Compile("var x = 2;var y = 3;Writeln(x > y);Writeln(x < y);Writeln(x >= y);Writeln(x <= y);", TestFileName);

            var outputValue = await GetOutputStringFromChildProcess(OutputFileName);
            Assert.AreEqual(outputValue, "False\r\nTrue\r\nFalse\r\nTrue\r\n");
        }

        [TestMethod]
        public async Task 条件ANDOR出力()
        {
            Compiler.Compile("Writeln(true && false);Writeln(true || false);", TestFileName);

            var outputValue = await GetOutputStringFromChildProcess(OutputFileName);
            Assert.AreEqual(outputValue, "False\r\nTrue\r\n");
        }

        [TestMethod]
        public async Task 加算代入出力()
        {
            Compiler.Compile("var a = 1;a += 4;Writeln(a);", TestFileName);

            var outputValue = await GetOutputStringFromChildProcess(OutputFileName);
            Assert.AreEqual(outputValue, "5\r\n");
        }

        [TestMethod]
        public async Task 条件分岐出力()
        {
            Compiler.Compile("var a = 1;if a == 1 { Writeln(a);}", TestFileName);

            var outputValue = await GetOutputStringFromChildProcess(OutputFileName);
            Assert.AreEqual(outputValue, "1\r\n");
        }

        [TestMethod]
        public async Task ループ出力()
        {
            var source = @"
var a = 1
while a < 3
{
    Writeln(a)
    a += 1
}
";
            Compiler.Compile(source, TestFileName);

            var outputValue = await GetOutputStringFromChildProcess(OutputFileName);
            Assert.AreEqual(outputValue, "1\r\n2\r\n");
        }

        [TestMethod]
        public async Task FizzBuzz()
        {
            var source = @"
var a = 0
while (a < 100)
{
    a +=1
    var modThreeIsZero = a % 3 == 0
    var modFiveIsZero = a % 5 == 0

    if modThreeIsZero || modFiveIsZero
    {        
        if (modThreeIsZero && modFiveIsZero)
        {
            Writeln(""FizzBuzz"")
            continue
        }

        if modThreeIsZero
        {
            Writeln(""Fizz"")
            continue
        }

        if (modFiveIsZero)
        {
            Writeln(""Buzz"")
            continue
        }
    }
    Writeln(a)
}
";
            Compiler.Compile(source, TestFileName);

            var outputValue = await GetOutputStringFromChildProcess(OutputFileName);
            var collect = GetFizzBuzz1To100();
            Assert.AreEqual(outputValue, collect);
        }

        [TestMethod]
        public async Task 文字列結合()
        {
            var source = "var a = \"Hello\";a = a + \" World\";Writeln(a);";
            Compiler.Compile(source, TestFileName);
            var outputValue = await GetOutputStringFromChildProcess(OutputFileName);
            Assert.AreEqual(outputValue, "Hello World\r\n");
        }

        private string GetFizzBuzz1To100()
        {
            return Enumerable
                 .Range(1, 100)
                 .Aggregate(new StringBuilder(), (sb, i) =>
                  {
                      var a = i % 3 == 0;
                      var b = i % 5 == 0;
                      if (a && b)
                      {
                          return sb.Append("FizzBuzz\r\n");
                      }
                      else if (a)
                      {
                          return sb.Append("Fizz\r\n");
                      }
                      else if (b)
                      {
                          return sb.Append("Buzz\r\n");
                      }
                      return sb.Append(i.ToString() + "\r\n");
                  }).ToString();
        }

        private static async Task<string> GetOutputStringFromChildProcess(string OutputFileName)
        {
            var psi = new ProcessStartInfo(OutputFileName)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            using (var child = Process.Start(psi))
            {
                var stdout = await child.StandardOutput.ReadToEndAsync();
                child.WaitForExit();
                return stdout;
            }
        }

        private static async Task<string> GetOutputStringFromChildProcess(string stdInputValue, string OutputFileName)
        {
            var psi = new ProcessStartInfo(OutputFileName)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
            };

            using (var child = Process.Start(psi))
            {
                await child.StandardInput.WriteLineAsync(stdInputValue);
                var stdout = await child.StandardOutput.ReadToEndAsync();
                child.WaitForExit();
                return stdout;
            }
        }
    }
}