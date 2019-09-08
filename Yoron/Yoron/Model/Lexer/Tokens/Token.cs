using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoron.Model.Lexer.Tokens
{
    /// <summary>
    /// トークンのスーパークラス
    /// </summary>
    public abstract class Token
    {
        protected Token(in string value)
        {
            RawValue = value;
        }

        protected Token(char value)
        {
            RawValue = value.ToString();
        }

        public string RawValue { get; }
    }

    /// <summary>
    /// メソッド呼び出し
    /// </summary>
    public class MethodCall : Token
    {
        public MethodCall(in string value) : base(value) { }
    }

    /// <summary>
    /// 文字列リテラル
    /// </summary>
    public class StringLiteral : Token
    {
        public StringLiteral(in string value) : base(value) { }
    }

    /// <summary>
    /// 左括弧
    /// </summary>
    public class LeftParenthesis : Token
    {
        private const char Specified = '(';
        public LeftParenthesis() : base(Specified) { }

        public static bool IsMatch(char source) => source == Specified;
    }

    /// <summary>
    /// 右括弧
    /// </summary>
    public class RightParenthesis : Token
    {
        private const char Specified = ')';

        public RightParenthesis() : base(Specified) { }

        public static bool IsMatch(char source) => source == Specified;
    }

    /// <summary>
    /// ブロック開始
    /// </summary>
    public class BlockBegin : Token
    {
        private const char Specified = '{';

        public BlockBegin() : base(Specified) { }

        public static bool IsMatch(char source) => source == Specified;
    }

    /// <summary>
    /// ブロック終了
    /// </summary>
    public class BlockEnd : Token
    {
        private const char Specified = '}';

        public BlockEnd() : base(Specified) { }

        public static bool IsMatch(char source) => source == Specified;
    }

    /// <summary>
    /// int32リテラル
    /// </summary>
    public class Int32Literal : Token
    {
        public static IReadOnlyCollection<char> SpecifiedCollection { get; } = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };

        public Int32Literal(in string value) : base(value) { }
    }

    /// <summary>
    /// boolリテラル
    /// </summary>
    public class BoolLiteral : Token
    {
        public static IReadOnlyCollection<string> SpecifiedCollection { get; } = new List<string> { "true", "false" };

        public BoolLiteral(in string value) : base(value) { }
    }

    /// <summary>
    /// 行末
    /// </summary>
    public class EndOfLine : Token
    {
        private const string CRLF = "\r\n";

        private const string SemiColon = ";";

        public EndOfLine() : base("") { }

        public static bool IsMatch(in string value) => value == CRLF || value == SemiColon || CRLF.Contains(value);
    }

    /// <summary>
    /// ローカル変数
    /// </summary>
    public class LocalVariable : Token
    {
        public LocalVariable(in string variableName) : base(variableName) { }
    }

    /// <summary>
    /// varによる型推論
    /// </summary>
    public class VarDeclaration : Token
    {
        public VarDeclaration(in string variableName) : base(variableName) { }
    }

    /// <summary>
    /// 識別子(変数など)
    /// </summary>
    public class Identifier : Token
    {
        public Identifier(in string value) : base(value) { }
    }

    /// <summary>
    /// if文のキーワード
    /// </summary>
    public class IfKeyword : Token
    {
        private const string Specification = "if";

        public IfKeyword() : base(Specification) { }

        public static bool IsMatch(in string value) => value == Specification;
    }

    /// <summary>
    /// whileのキーワード
    /// </summary>
    public class WhileKeyword : Token
    {
        private const string Specification = "while";

        public WhileKeyword() : base(Specification) { }

        public static bool IsMatch(in string value) => value == Specification;
    }

    /// <summary>
    /// breakのキーワード
    /// </summary>
    public class BreakKeyword : Token
    {
        private const string Specification = "break";

        public BreakKeyword() : base(Specification) { }

        public static bool IsMatch(in string value) => value == Specification;
    }

    /// <summary>
    /// continueのキーワード
    /// </summary>
    public class ContinueKeyword : Token
    {
        private const string Specification = "continue";

        public ContinueKeyword() : base(Specification) { }

        public static bool IsMatch(in string value) => value == Specification;
    }
}
