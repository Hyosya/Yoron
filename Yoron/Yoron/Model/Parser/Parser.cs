using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yoron.Model.Lexer.Tokens;

namespace Yoron.Model.Parser
{
    public static class Parser
    {
        public static LambdaExpression GetLambdaExpression(string source)
        {
            var tokenList = Lexer.Lexer.GetTokenList(source);
            var methodBlock = Block.NewBlock(new Dictionary<string, Expression>(), tokenList);
            var lambda = methodBlock.ToLambdaExpression();
            return lambda;
        }
    }
}
