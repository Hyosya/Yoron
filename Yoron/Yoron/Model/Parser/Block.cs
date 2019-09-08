using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yoron.Model.Lexer.Tokens;

namespace Yoron.Model.Parser
{
    /// <summary>
    /// メソッドブロックやif文などのブロックを表現。
    /// </summary>
    public class Block
    {
        private Dictionary<string, Expression> ExistLocalVariables { get; set; }

        private List<Token> TokenList { get; set; }

        private List<Statement> Statements { get; set; } = new List<Statement>();

        private Block(Dictionary<string, Expression> localVariables, List<Token> tokens)
        {
            ExistLocalVariables = localVariables;
            TokenList = tokens;
        }

        /// <summary>
        /// ローカル変数用ディクショナリとトークン列を使用してブロックを生成。
        /// </summary>
        /// <param name="localVariables"></param>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public static Block NewBlock(Dictionary<string, Expression> localVariables, List<Token> tokens)
        {
            var block = new Block(localVariables, tokens);
            block.TokenToStatements();
            return block;
        }

        /// <summary>
        /// ブロック内の文たちをまとめてBlockExpressionを生成。
        /// </summary>
        /// <returns></returns>
        public BlockExpression ToBlockExpression()
        {
            var expList = Statements.Select(i => i.ToExpression()).ToList();
            var variables = Statements
                .Where(s => s is VarStatement)
                .Select(i => (i as VarStatement).NewVariable)
                .ToList();
            BlockExpression block;
            if (variables.Any())
            {
                block = Expression.Block(variables, expList);
            }
            else
            {
                block = Expression.Block(expList);
            }
            return block;
        }
        /// <summary>
        /// このブロック内の文を返す
        /// </summary>
        public List<Statement> GetStatements() => Statements;

        /// <summary>
        /// BlockExpressionをxpression<Action>に変換。
        /// </summary>
        /// <returns></returns>
        public Expression<Action> ToLambdaExpression()
        {
            var block = ToBlockExpression();
            return Expression.Lambda<Action>(block);
        }

        private void TokenToStatements()
        {
            while (TokenList.Count > 0)
                switch (TokenList[0])
                {
                    case EndOfLine _:
                    case BlockEnd _:
                        TokenList.RemoveAt(0);
                        break;

                    case VarDeclaration _:
                        var varList = TokenList.Take(TokenList.FindIndex(t => t is EndOfLine) + 1).ToList();
                        TokenList.RemoveRange(0, varList.Count);
                        Statements.Add(new VarStatement(ExistLocalVariables, varList));
                        break;

                    case IfKeyword _:
                        var ifCondition = TokenList.Skip(1).TakeWhile(t => !(t is BlockBegin)).ToList();
                        var ifBlock = ParseBlock(TokenList.Skip(TokenList.FindIndex(t => t is BlockBegin)).ToList());
                        TokenList.RemoveRange(0, ifCondition.Count + ifBlock.Count + 2);
                        Statements.Add(new IfStatement(ExistLocalVariables, ifCondition, ifBlock));
                        break;

                    case WhileKeyword _:
                        var whileCondition = TokenList.Skip(1).TakeWhile(t => !(t is BlockBegin)).ToList();
                        var whileBlock = ParseBlock(TokenList.Skip(TokenList.FindIndex(t => t is BlockBegin)).ToList());
                        TokenList.RemoveRange(0, whileCondition.Count + whileBlock.Count + 2);
                        Statements.Add(new WhileStatement(ExistLocalVariables, whileCondition, whileBlock));
                        break;

                    case BreakKeyword _:
                        TokenList.RemoveAt(0);
                        Statements.Add(new BreakStatement(ExistLocalVariables));
                        break;

                    case ContinueKeyword _:
                        TokenList.RemoveAt(0);
                        Statements.Add(new ContinueStatement(ExistLocalVariables));
                        break;

                    default:
                        var list = TokenList.Take(TokenList.FindIndex(t => t is EndOfLine) + 1).ToList();
                        TokenList.RemoveRange(0, list.Count);
                        Statements.Add(new Statement(ExistLocalVariables, list));
                        break;
                }
        }

        private List<Token> ParseBlock(List<Token> tokens)
        {
            var block = new List<Token>();
            var cnt = 0;
            foreach (var item in tokens)
            {
                if (item is BlockBegin) cnt++;
                if (item is BlockEnd) cnt--;
                block.Add(item);
                if (cnt == 0) break;
            }
            block.RemoveAt(0);
            block.Remove(block.Last());
            return block;
        }
    }
}
