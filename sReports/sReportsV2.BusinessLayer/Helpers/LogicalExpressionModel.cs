using sReportsV2.BusinessLayer.Components.Interfaces;

namespace sReportsV2.BusinessLayer.Helpers
{
    public abstract class LogicalExpression : ILogicalExpressionNode
    {
        public abstract object Accept(ILogicalExpressionNodeVisitor visitor);
        public virtual string GetName()
        {
            return this.GetType().Name;
        }

        public abstract string PrintExpression();
    }

    #region Operands
    public abstract class Operand : LogicalExpression
    {
        public ExpressionToken ExpressionToken { get; private set; }

        protected Operand(ExpressionToken expressionToken)
        {
            this.ExpressionToken = expressionToken;
        }

        public override string PrintExpression()
        {
            return GetOperand();
        }

        protected abstract string GetOperand();
    }

    public class NumericOperand : Operand
    {
        public NumericOperand(ExpressionToken expressionToken) : base(expressionToken)
        {
        }

        public override object Accept(ILogicalExpressionNodeVisitor visitor)
        {
            return visitor.VisitNumericOperand(ExpressionToken);
        }

        protected override string GetOperand()
        {
            return ExpressionToken.Value;
        }

    }

    public class VariableOperand : Operand
    {
        public VariableOperand(ExpressionToken expressionToken) : base(expressionToken)
        {
        }

        public override object Accept(ILogicalExpressionNodeVisitor visitor)
        {
            return visitor.VisitVariableOperand(ExpressionToken);
        }

        protected override string GetOperand()
        {
            return $"[{ExpressionToken.Value}]";
        }
    }

    public class StringLiteralOperand : Operand
    {
        public StringLiteralOperand(ExpressionToken expressionToken) : base(expressionToken)
        {
        }

        public override object Accept(ILogicalExpressionNodeVisitor visitor)
        {
            return visitor.VisitStringLiteralOperand(ExpressionToken);
        }

        protected override string GetOperand()
        {
            return $@"""{ExpressionToken.Value}""";
        }
    }
    #endregion /Operands

    #region Operations
    public class UnaryOperation : LogicalExpression
    {
        public ExpressionToken Operator { get; private set; }
        public LogicalExpression Node { get; private set; }

        public UnaryOperation(ExpressionToken op, LogicalExpression node)
        {
            this.Operator = op;
            this.Node = node;
        }

        public override string GetName()
        {
            return $"{Operator.Type} ({Node.GetName()})";
        }

        public override string PrintExpression()
        {
            return $"{Operator.Value} ({Node.PrintExpression()})";
        }

        override public object Accept(ILogicalExpressionNodeVisitor visitor)
        {
            return visitor.VisitUnaryOperation(this.Operator, this.Node);
        }
    }

    public abstract class BinaryOperation : LogicalExpression
    {
        public ExpressionToken Operator { get; private set; }
        public LogicalExpression Left { get; private set; }
        public LogicalExpression Right { get; private set; }

        protected BinaryOperation(ExpressionToken op, LogicalExpression left, LogicalExpression right)
        {
            this.Operator = op;
            this.Left = left;
            this.Right = right;
        }

        public override string GetName()
        {
            return $"{Left.GetName()} {Operator.Type} {Right.GetName()}";
        }

        public override string PrintExpression()
        {
            return $"{Left.PrintExpression()} {Operator.Value} {Right.PrintExpression()}";
        }
    }

    public class BinaryArithmeticOperation : BinaryOperation
    {
        public BinaryArithmeticOperation(ExpressionToken op, LogicalExpression left, LogicalExpression right) : base(op, left, right)
        {
        }

        public override object Accept(ILogicalExpressionNodeVisitor visitor)
        {
            return visitor.VisitBinaryArithmeticOperation(this.Operator, this.Left, this.Right);
        }
    }

    public class BinaryComparisonOperation : BinaryOperation
    {
        public BinaryComparisonOperation(ExpressionToken op, LogicalExpression left, LogicalExpression right) : base(op, left, right)
        {
        }

        public override object Accept(ILogicalExpressionNodeVisitor visitor)
        {
            return visitor.VisitBinaryComparisonOperation(this.Operator, this.Left, this.Right);
        }
    }

    public class BinaryLogicalOperation : BinaryOperation
    {
        public BinaryLogicalOperation(ExpressionToken op, LogicalExpression left, LogicalExpression right) : base(op, left, right)
        {
        }

        public override object Accept(ILogicalExpressionNodeVisitor visitor)
        {
            return visitor.VisitBinaryLogicalOperation(this.Operator, this.Left, this.Right);
        }
    }
    #endregion /Operations

    #region Tokens
    public class ExpressionToken
    {
        public ExpressionTokenType Type { get; private set; }
        public string Value { get; private set; }

        public ExpressionToken(ExpressionTokenType type, string value)
        {
            this.Type = type;
            this.Value = value;
        }

        public static ExpressionToken None()
        {
            return new ExpressionToken(ExpressionTokenType.None, "");
        }

        public override string ToString()
        {
            return this.Value;
        }
    }

    public enum ExpressionTokenType
    {
        None,
        Plus,
        Minus,
        Multiply,
        Divide,
        Number,
        LeftParenthesis,
        RightParenthesis,
        And,
        Or,
        Equal,
        NotEqual,
        Less,
        LessOrEqual,
        Great,
        GreatOrEqual,
        LeftBracket,
        RightBracket,
        Variable,
        Quote,
        StringLiteral,
    }
    #endregion /Tokens
}
