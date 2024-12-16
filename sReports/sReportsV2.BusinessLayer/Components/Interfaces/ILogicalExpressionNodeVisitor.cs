using sReportsV2.BusinessLayer.Helpers;

namespace sReportsV2.BusinessLayer.Components.Interfaces
{
    public interface ILogicalExpressionNodeVisitor
    {
        object VisitNumericOperand(ExpressionToken numeric);
        object VisitVariableOperand(ExpressionToken variable);
        object VisitStringLiteralOperand(ExpressionToken stringLiteral);
        object VisitUnaryOperation(ExpressionToken op, ILogicalExpressionNode node);
        object VisitBinaryArithmeticOperation(ExpressionToken op, ILogicalExpressionNode left, ILogicalExpressionNode right);
        object VisitBinaryComparisonOperation(ExpressionToken op, ILogicalExpressionNode left, ILogicalExpressionNode right);
        object VisitBinaryLogicalOperation(ExpressionToken op, ILogicalExpressionNode left, ILogicalExpressionNode right);
    }
}
