using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoron.Model.Lexer.Tokens;

namespace Yoron.Model.Lexer
{
    public static class Lexer
    {
        /// <summary>
        /// ソースコードからトークン列を生成する。
        /// </summary>
        /// <param name="source">ソースコード</param>
        /// <returns></returns>
        public static List<Token> GetTokenList(string source)
        {
            var list = Analysis(source).ToList();
            return list;
        }

        /// <summary>
        /// ソースコードを解析してトークン列を生成
        /// </summary>
        /// <param name="targets"></param>
        /// <returns></returns>
        private static IEnumerable<Token> Analysis(string targets)
        {
            if (targets.EndsWith(";") == false || targets.EndsWith("\r\n") == false) targets += "\r\n";
            while (targets.Length > 0)
            {
                //スペースとタブは変換しない
                if (targets[0] == ' ' || targets[0] == '\t')
                {
                    targets = targets.Remove(0, 1);
                    continue;
                }

                if (targets[0] == '\"')
                {
                    var literal = targets
                        .Skip(1)
                        .TakeWhile(c => c != '\"')
                        .Aggregate(new StringBuilder(), (sb, c) => sb.Append(c))
                        .ToString();
                    targets = targets.Remove(0, literal.Length + 2);
                    yield return new StringLiteral(literal);
                    continue;
                }

                if (LeftParenthesis.IsMatch(targets[0]))
                {
                    targets = targets.Remove(0, 1);
                    yield return new LeftParenthesis();
                    continue;
                }

                if (RightParenthesis.IsMatch(targets[0]))
                {
                    targets = targets.Remove(0, 1);
                    yield return new RightParenthesis();
                    continue;
                }

                if (BlockBegin.IsMatch(targets[0]))
                {
                    targets = targets.Remove(0, 1);
                    yield return new BlockBegin();
                    continue;
                }

                if (BlockEnd.IsMatch(targets[0]))
                {
                    targets = targets.Remove(0, 1);
                    yield return new BlockEnd();
                    continue;
                }

                if (targets.StartsWith("Writeln"))
                {
                    targets = targets.Remove(0, 7);
                    yield return new StandardOutput();
                    continue;
                }

                if (targets.StartsWith("Readln()"))
                {
                    targets = targets.Remove(0, 8);
                    yield return new StandardInput();
                    continue;
                }

                if (Int32Literal.SpecifiedCollection.Contains(targets[0]))
                {
                    var value = targets.TakeWhile(c => Int32Literal.SpecifiedCollection.Contains(c)).ToArray();
                    targets = targets.Remove(0, value.Length);
                    yield return new Int32Literal(new string(value));
                    continue;
                }

                if (EndOfLine.IsMatch(targets[0].ToString()) || EndOfLine.IsMatch(new string(targets.Take(2).ToArray())))
                {
                    var value = targets.TakeWhile(i => i == ';' || i == '\r' || i == '\n').Count();
                    targets = targets.Remove(0, value);
                    yield return new EndOfLine();
                    continue;
                }

                if (targets.Length > 1 && Assign.IsMatch(targets[0]) && !Assign.IsMatch(targets[1]))
                {
                    targets = targets.Remove(0, 1);
                    yield return new Assign();
                    continue;
                }

                if (targets.Length > 1 && AddAssign.IsMatch(targets[0], targets[1]))
                {
                    targets = targets.Remove(0, 2);
                    yield return new AddAssign();
                    continue;
                }

                if (targets.StartsWith("var "))
                {
                    var varVariableName = targets.Skip(4)
                        .TakeWhile(c => c != ' ' && c != '\t' && c != '=')
                        .Aggregate(new StringBuilder(), (sb, c) => sb.Append(c))
                        .ToString();
                    if (!(JudgeWord(varVariableName) is Identifier)) throw new Exception("変数にキーワードが使われています。");
                    targets = targets.Remove(0, 4 + varVariableName.Length);
                    yield return new VarDeclaration(varVariableName);
                    continue;
                }

                if (PlusOperator.IsMatch(targets[0]))
                {
                    targets = targets.Remove(0, 1);
                    yield return new PlusOperator();
                    continue;
                }

                if (MinusOperator.IsMatch(targets[0]))
                {
                    targets = targets.Remove(0, 1);
                    yield return new MinusOperator();
                    continue;
                }

                if (MultiplyOperator.IsMatch(targets[0]))
                {
                    targets = targets.Remove(0, 1);
                    yield return new MultiplyOperator();
                    continue;
                }

                if (DivideOperator.IsMatch(targets[0]))
                {
                    targets = targets.Remove(0, 1);
                    yield return new DivideOperator();
                    continue;
                }

                if (ModuloOperator.IsMatch(targets[0]))
                {
                    targets = targets.Remove(0, 1);
                    yield return new ModuloOperator();
                    continue;
                }

                if (targets.Length > 1 && targets[0] == '=' && targets[1] == '=')
                {
                    targets = targets.Remove(0, 2);
                    yield return new EqualOperator();
                    continue;
                }

                if (targets.Length > 1 && targets[0] == '!' && targets[1] == '=')
                {
                    targets = targets.Remove(0, 2);
                    yield return new NotEqualOperator();
                    continue;
                }

                if (targets.Length > 1 && targets[0] == '>')
                {
                    if (targets[1] == '=')
                    {
                        targets = targets.Remove(0, 2);
                        yield return new GreaterThanOrEqualOperator();
                        continue;
                    }
                    targets = targets.Remove(0, 1);
                    yield return new GreaterThanOperator();
                    continue;
                }

                if (targets.Length > 1 && targets[0] == '<')
                {
                    if (targets[1] == '=')
                    {
                        targets = targets.Remove(0, 2);
                        yield return new LessThanOrEqualOperator();
                        continue;
                    }
                    targets = targets.Remove(0, 1);
                    yield return new LessThanOperator();
                    continue;
                }

                if (targets.Length > 1 && targets[0] == '&' && targets[1] == '&')
                {
                    targets = targets.Remove(0, 2);
                    yield return new AndAlsoOperator();
                    continue;
                }

                if (targets.Length > 1 && targets[0] == '|' && targets[1] == '|')
                {
                    targets = targets.Remove(0, 2);
                    yield return new OrElseOperator();
                    continue;
                }

                var word = targets.TakeWhile(IdentifierTake)
                      .Aggregate(new StringBuilder(), (sb, c) => sb.Append(c))
                      .ToString();
                if (word.Length == 0) throw new Exception("変数名なのに文字数がゼロ");
                targets = targets.Remove(0, word.Length);
                yield return JudgeWord(word);
            }
        }

        /// <summary>
        /// キーワードと変数名を識別する
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private static Token JudgeWord(string word)
        {
            if (BoolLiteral.SpecifiedCollection.Contains(word)) return new BoolLiteral(word);
            if (word.Length == 2 && IfKeyword.IsMatch(word)) return new IfKeyword();
            if (word.Length == 5 && WhileKeyword.IsMatch(word)) return new WhileKeyword();
            if (word.Length == 5 && BreakKeyword.IsMatch(word)) return new BreakKeyword();
            if (word.Length == 8 && ContinueKeyword.IsMatch(word)) return new ContinueKeyword();
            return new Identifier(word);
        }

        /// <summary>
        /// 変数名を決めるための条件
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static bool IdentifierTake(char c)
        {
            return !Assign.IsMatch(c)
                && !EndOfLine.IsMatch(c.ToString())
                && !LeftParenthesis.IsMatch(c)
                && !RightParenthesis.IsMatch(c)
                && c != ' '
                && c != '\t';
        }
    }
}
