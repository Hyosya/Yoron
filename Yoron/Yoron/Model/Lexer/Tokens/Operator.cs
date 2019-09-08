using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoron.Model.Lexer.Tokens
{
    /// <summary>
    /// 演算子の結合性
    /// </summary>
    public enum Associativity : short
    {
        Left,
        Right,
    }

    /// <summary>
    /// 演算子のスーパークラス
    /// </summary>
    public abstract class Operator : Token
    {
        private OperatorPriority Priority { get; }

        public Associativity Associativity { get; protected set; }

        protected Operator(in string operatorString, OperatorPriority priority) : base(operatorString) { Priority = priority; }

        protected Operator(char operatorString, OperatorPriority priority) : base(operatorString) { Priority = priority; }

        public static bool operator <(Operator left, Operator right) => left.Priority < right.Priority;

        public static bool operator >(Operator left, Operator right) => left.Priority > right.Priority;

        public static bool operator <=(Operator left, Operator right) => left.Priority <= right.Priority;

        public static bool operator >=(Operator left, Operator right) => left.Priority >= right.Priority;

        /// <summary>
        /// 演算子の優先順位
        /// 値が大きいと優先順位が高い
        /// </summary>
        protected enum OperatorPriority
        {
            代入_関数,
            条件OR,
            条件AND,
            等値,
            比較,
            加減算,
            乗除算,
        }
    }

    /// <summary>
    /// 標準出力
    /// 演算子でいいのだろうか？
    /// </summary>
    public class StandardOutput : Operator
    {
        private const string Specified = "Writeln";

        public StandardOutput() : base(Specified, OperatorPriority.代入_関数)
        {
            Associativity = Associativity.Right;
        }

        public static bool IsMatch(in string source) => source == Specified;
    }

    /// <summary>
    /// 標準入力
    /// </summary>
    public class StandardInput : Operator
    {
        private const string Specified = "Readln";

        public StandardInput() : base(Specified, OperatorPriority.代入_関数)
        {
            Associativity = Associativity.Right;
        }

        public static bool IsMatch(in string source) => source == Specified;
    }

    /// <summary>
    /// 代入演算子
    /// </summary>
    public class Assign : Operator
    {
        private const char Specified = '=';

        public Assign() : base(Specified.ToString(), OperatorPriority.代入_関数) { }

        public static bool IsMatch(char value) => Specified == value;
    }

    /// <summary>
    /// 加算代入演算子
    /// </summary>
    public class AddAssign : Operator
    {
        private const string Specified = "+=";

        public AddAssign() : base(Specified.ToString(), OperatorPriority.代入_関数) { }

        public static bool IsMatch(char first, char second) => Specified[0] == first && Specified[1] == second;
    }

    /// <summary>
    /// 加算演算子
    /// </summary>
    public class PlusOperator : Operator
    {
        private const char Specified = '+';

        public PlusOperator() : base(Specified.ToString(), OperatorPriority.加減算) { }

        public static bool IsMatch(char value) => Specified == value;
    }

    /// <summary>
    /// 減算演算子
    /// </summary>
    public class MinusOperator : Operator
    {
        private const char Specified = '-';

        public MinusOperator() : base(Specified.ToString(), OperatorPriority.加減算) { }

        public static bool IsMatch(char value) => Specified == value;
    }

    /// <summary>
    /// 乗算演算子
    /// </summary>
    public class MultiplyOperator : Operator
    {
        private const char Specified = '*';

        public MultiplyOperator() : base(Specified.ToString(), OperatorPriority.乗除算) { }

        public static bool IsMatch(char value) => Specified == value;
    }

    /// <summary>
    /// 除算演算子
    /// </summary>
    public class DivideOperator : Operator
    {
        private const char Specified = '/';

        public DivideOperator() : base(Specified.ToString(), OperatorPriority.乗除算) { }

        public static bool IsMatch(char value) => Specified == value;
    }

    /// <summary>
    /// 剰余演算子
    /// </summary>
    public class ModuloOperator : Operator
    {
        private const char Specified = '%';

        public ModuloOperator() : base(Specified.ToString(), OperatorPriority.乗除算) { }

        public static bool IsMatch(char value) => Specified == value;
    }

    /// <summary>
    /// イコール演算子
    /// </summary>
    public class EqualOperator : Operator
    {
        private const string Specified = "==";

        public EqualOperator() : base(Specified, OperatorPriority.等値) { }

        public static bool IsMatch(in string value) => Specified == value;
    }

    /// <summary>
    /// ノットイコール演算子
    /// </summary>
    public class NotEqualOperator : Operator
    {
        private const string Specified = "!=";

        public NotEqualOperator() : base(Specified, OperatorPriority.等値) { }

        public static bool IsMatch(in string value) => Specified == value;
    }

    /// <summary>
    /// 大なり演算子
    /// </summary>
    public class GreaterThanOperator : Operator
    {
        private const char Specified = '>';

        public GreaterThanOperator() : base(Specified, OperatorPriority.比較) { }

        public static bool IsMatch(char value) => Specified == value;
    }

    /// <summary>
    /// 小なり演算子
    /// </summary>
    public class LessThanOperator : Operator
    {
        private const char Specified = '<';

        public LessThanOperator() : base(Specified, OperatorPriority.比較) { }

        public static bool IsMatch(char value) => Specified == value;
    }

    /// <summary>
    /// 大なりイコール演算子
    /// </summary>
    public class GreaterThanOrEqualOperator : Operator
    {
        private const string Specified = ">=";

        public GreaterThanOrEqualOperator() : base(Specified, OperatorPriority.比較) { }

        public static bool IsMatch(in string value) => Specified == value;
    }

    /// <summary>
    /// 小なりイコール演算子
    /// </summary>
    public class LessThanOrEqualOperator : Operator
    {
        private const string Specified = "<=";

        public LessThanOrEqualOperator() : base(Specified, OperatorPriority.比較) { }

        public static bool IsMatch(in string value) => Specified == value;
    }

    /// <summary>
    /// 条件AND演算子
    /// </summary>
    public class AndAlsoOperator : Operator
    {
        private const string Specified = "&&";

        public AndAlsoOperator() : base(Specified, OperatorPriority.条件AND) { }

        public static bool IsMatch(in string value) => Specified == value;
    }

    /// <summary>
    /// 条件OR演算子
    /// </summary>
    public class OrElseOperator : Operator
    {
        private const string Specified = "||";

        public OrElseOperator() : base(Specified, OperatorPriority.条件OR) { }

        public static bool IsMatch(in string value) => Specified == value;
    }
}
