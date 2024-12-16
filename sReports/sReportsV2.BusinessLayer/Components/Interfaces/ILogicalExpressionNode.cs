namespace sReportsV2.BusinessLayer.Components.Interfaces
{
    public interface ILogicalExpressionNode
    {
        object Accept(ILogicalExpressionNodeVisitor visitor);
    }
}
