using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yoron.Model.Lexer.Tokens;

namespace Yoron.Model.Parser
{
    public class Statement
    {
        protected Dictionary<string, Expression> ParentLocalVariables { get; set; }

        protected IReadOnlyList<Token> TokenList { get; set; }

        public Statement(Dictionary<string, Expression> localVariables, List<Token> tokens)
        {
            ParentLocalVariables = localVariables;
            TokenList = tokens;
        }

        protected Statement(Dictionary<string, Expression> localVariables, Token token)
        {
            ParentLocalVariables = localVariables;
        }

        public virtual Expression ToExpression()
        {
            var listQueue = ConvertToRPN(TokenList);
            var exp = MakeExpression(listQueue);
            return exp;
        }

        /// <summary>
        /// 操車場アルゴリズムを使って逆ポーランド記法に変換。
        /// </summary>
        /// <param name="tokenList"></param>
        /// <returns></returns>
        protected Queue<Token> ConvertToRPN(IReadOnlyList<Token> tokenList)
        {
            var inputQueue = new Queue<Token>(tokenList);
            var opeStack = new Stack<Token>();
            var outputQueue = new Queue<Token>();
            while (inputQueue.Count > 0 || opeStack.Count > 0)
            {
                if (inputQueue.Count == 0)
                {
                    outputQueue.Enqueue(opeStack.Pop());
                    continue;
                }

                var target = inputQueue.Dequeue();
                if (target is Operator ope && !(target is StandardOutput) && !(target is StandardInput))
                {
                    while (opeStack.Any() && opeStack.Peek() is Operator topOperator
                        && ((ope.Associativity == Associativity.Left && ope <= topOperator) || ope < topOperator))
                    {
                        outputQueue.Enqueue(opeStack.Pop());
                    }

                    opeStack.Push(target);
                    continue;
                }

                if (target is StandardOutput)
                {
                    opeStack.Push(target);
                    continue;
                }

                if (target is StandardInput)
                {
                    opeStack.Push(target);
                    continue;
                }

                if (target is LeftParenthesis)
                {
                    opeStack.Push(target);
                    continue;
                }

                if (target is RightParenthesis)
                {
                    if (opeStack.Any(i => i is LeftParenthesis) == false) throw new Exception("構文エラー 括弧");
                    while (!(opeStack.Peek() is LeftParenthesis))
                    {
                        outputQueue.Enqueue(opeStack.Pop());
                    }
                    opeStack.Pop();
                    continue;
                }

                if (target is EndOfLine || target is BlockEnd)
                {
                    while (opeStack.Count > 0)
                    {
                        outputQueue.Enqueue(opeStack.Pop());
                    }
                    break;
                }

                outputQueue.Enqueue(target);
            }
            return outputQueue;
        }

        /// <summary>
        /// 逆ポーランドのトークン列を解釈してExpressionに。
        /// </summary>
        /// <param name="tokenQueue"></param>
        /// <returns></returns>
        protected Expression MakeExpression(Queue<Token> tokenQueue)
        {
            var expStack = new Stack<Expression>();
            while (tokenQueue.Count > 0)
            {
                var target = tokenQueue.Dequeue();
                switch (target)
                {
                    case StringLiteral stringLiteral:
                        expStack.Push(Expression.Constant(stringLiteral.RawValue, typeof(string)));
                        break;

                    case Int32Literal int32Literal:
                        expStack.Push(Expression.Constant(int.Parse(int32Literal.RawValue), typeof(int)));
                        break;

                    case BoolLiteral boolLiteral:
                        expStack.Push(Expression.Constant(bool.Parse(boolLiteral.RawValue), typeof(bool)));
                        break;

                    case StandardOutput _:
                        var outputValue = expStack.Pop();
                        if (outputValue.Type == typeof(int) || outputValue.Type == typeof(string))
                        {
                            return Expression.Call(typeof(Console).GetMethod("WriteLine", new[] { outputValue.Type }), outputValue);
                        }
                        var obj = Expression.Convert(outputValue, typeof(object));
                        expStack.Push(Expression.Call(typeof(Console).GetMethod("WriteLine", new[] { obj.Type }), obj));
                        break;

                    case StandardInput _:
                        var readExp = Expression.Call(typeof(Console).GetMethod("ReadLine", Type.EmptyTypes));
                        expStack.Push(readExp);
                        break;

                    case Operator ope:
                        MakeOperatorExpression(expStack, ope);
                        break;

                    case Identifier identifier:
                        if (ParentLocalVariables.TryGetValue(identifier.RawValue, out var exp) == false) throw new Exception("宣言されていない変数を使用");
                        expStack.Push(exp);
                        break;

                    case BlockBegin _:
                    case BlockEnd _:
                        break;

                    default:
                        throw new Exception("未定義トークン");
                }
            }
            return expStack.Pop();
        }

        protected void MakeOperatorExpression(Stack<Expression> expStack, Operator ope)
        {
            Expression getExpression(Expression right, Expression left, Func<Expression, Expression, Expression> func) => func(left, right);
            switch (ope)
            {
                case MultiplyOperator _:
                    expStack.Push(getExpression(expStack.Pop(), expStack.Pop(), Expression.Multiply));
                    break;

                case DivideOperator _:
                    expStack.Push(getExpression(expStack.Pop(), expStack.Pop(), Expression.Divide));
                    break;

                case ModuloOperator _:
                    expStack.Push(getExpression(expStack.Pop(), expStack.Pop(), Expression.Modulo));
                    break;

                case PlusOperator _:
                    var right = expStack.Pop();
                    var left = expStack.Pop();
                    var stringType = typeof(string);
                    if (right.Type == stringType && left.Type == stringType)
                    {
                        var methodInfo = stringType.GetMethod("Concat", new[] { stringType, stringType });
                        var call = Expression.Call(methodInfo, left, right);
                        expStack.Push(call);
                        break;
                    }
                    expStack.Push(Expression.Add(left, right));
                    break;

                case MinusOperator _:
                    expStack.Push(getExpression(expStack.Pop(), expStack.Pop(), Expression.Subtract));
                    break;

                case EqualOperator _:
                    expStack.Push(getExpression(expStack.Pop(), expStack.Pop(), Expression.Equal));
                    break;

                case NotEqualOperator _:
                    expStack.Push(getExpression(expStack.Pop(), expStack.Pop(), Expression.NotEqual));
                    break;

                case GreaterThanOperator _:
                    expStack.Push(getExpression(expStack.Pop(), expStack.Pop(), Expression.GreaterThan));
                    break;

                case GreaterThanOrEqualOperator _:
                    expStack.Push(getExpression(expStack.Pop(), expStack.Pop(), Expression.GreaterThanOrEqual));
                    break;

                case LessThanOperator _:
                    expStack.Push(getExpression(expStack.Pop(), expStack.Pop(), Expression.LessThan));
                    break;

                case LessThanOrEqualOperator _:
                    expStack.Push(getExpression(expStack.Pop(), expStack.Pop(), Expression.LessThanOrEqual));
                    break;

                case AndAlsoOperator _:
                    expStack.Push(getExpression(expStack.Pop(), expStack.Pop(), Expression.AndAlso));
                    break;

                case OrElseOperator _:
                    expStack.Push(getExpression(expStack.Pop(), expStack.Pop(), Expression.OrElse));
                    break;

                case Assign _:
                    expStack.Push(getExpression(expStack.Pop(), expStack.Pop(), Expression.Assign));
                    break;

                case AddAssign _:
                    expStack.Push(getExpression(expStack.Pop(), expStack.Pop(), Expression.AddAssign));
                    break;


                default:
                    throw new Exception("未定義トークン");
            }
        }
    }

    public class VarStatement : Statement
    {
        public VarStatement(Dictionary<string, Expression> localVariables, List<Token> tokens) : base(localVariables, tokens) { }

        public ParameterExpression NewVariable { get; private set; }

        public override Expression ToExpression()
        {
            var rightTokens = ConvertToRPN(TokenList.Skip(2).ToList());
            var rightExp = MakeExpression(rightTokens);
            var left = Expression.Parameter(rightExp.Type, TokenList[0].RawValue);
            ParentLocalVariables.Add(left.Name, left);
            NewVariable = left;
            return Expression.Assign(left, rightExp);
        }
    }

    public class IfStatement : Statement
    {
        private IReadOnlyList<Token> Condition { get; set; }

        private Block Block { get; set; }

        public IfStatement(Dictionary<string, Expression> localVariables, List<Token> condition, List<Token> block) : base(localVariables, block)
        {
            Condition = condition;
            Block = Block.NewBlock(localVariables, block);
        }

        public override Expression ToExpression()
        {
            var condQueue = ConvertToRPN(Condition);
            var condExp = MakeExpression(condQueue);
            var blockExp = Block.ToBlockExpression();
            return Expression.IfThen(condExp, blockExp);
        }
    }

    public class WhileStatement : Statement
    {
        private IReadOnlyList<Token> Condition { get; set; }

        private Block Block { get; set; }

        private LabelTarget BreakTarget { get; }

        private LabelTarget ContinueTarget { get; }

        public WhileStatement(Dictionary<string, Expression> localVariables, List<Token> condition, List<Token> block) : base(localVariables, block)
        {
            BreakTarget = Expression.Label();
            ContinueTarget = Expression.Label();
            Condition = condition;
            Block = Block.NewBlock(localVariables, block);
        }

        public override Expression ToExpression()
        {
            var condQueue = ConvertToRPN(Condition);
            var condExp = Expression.Not(MakeExpression(condQueue));
            var breakExp = Expression.IfThen(condExp, Expression.Break(BreakTarget));

            var blockStatement = Block.GetStatements();
            var blockExpList = blockStatement.Select(i => i.ToExpression()).ToList();
            var visitor = new BreakContinueVisitor(BreakTarget, ContinueTarget);
            blockExpList = blockExpList.Select(i => visitor.Visit(i)).ToList();

            blockExpList.Insert(0, breakExp);

            var blockVariables = blockStatement.Where(s => s is VarStatement).Select(s => (s as VarStatement).NewVariable).ToList();
            var blockExp = blockVariables.Any() ? Expression.Block(blockVariables, blockExpList) : Expression.Block(blockExpList);
            return Expression.Loop(blockExp, BreakTarget, ContinueTarget);
        }

        private class BreakContinueVisitor : ExpressionVisitor
        {
            private LabelTarget BreakTarget { get; }

            private LabelTarget ContinueTarget { get; }

            public BreakContinueVisitor(LabelTarget breakTarget, LabelTarget continueTarget)
            {
                BreakTarget = breakTarget;
                ContinueTarget = continueTarget;
            }

            protected override Expression VisitGoto(GotoExpression node)
            {
                if (node.Target.Name == "Break")
                {
                    return node.Update(BreakTarget, null);
                }
                else if (node.Target.Name == "Continue")
                {
                    return node.Update(ContinueTarget, null);
                }
                return base.VisitGoto(node);
            }
        }
    }

    public class BreakStatement : Statement
    {
        public BreakStatement(Dictionary<string, Expression> localVariables) : base(localVariables, new BreakKeyword())
        {

        }

        public override Expression ToExpression() => Expression.Break(Expression.Label("Break"));
    }

    public class ContinueStatement : Statement
    {
        public ContinueStatement(Dictionary<string, Expression> localVariables) : base(localVariables, new ContinueKeyword())
        {

        }

        public override Expression ToExpression() => Expression.Break(Expression.Label("Continue"));
    }
}
