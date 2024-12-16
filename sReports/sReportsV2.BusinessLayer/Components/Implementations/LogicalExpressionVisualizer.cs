using sReportsV2.BusinessLayer.Components.Interfaces;
using sReportsV2.BusinessLayer.Helpers;
using System.Collections.Generic;
using System.Text;

namespace sReportsV2.BusinessLayer.Components.Implementations
{
    public class LogicalExpressionVisualizer : ILogicalExpressionNodeVisitor
    {
        public enum BranchOriental
        {
            None,
            Left,
            Right
        }

        const char SPACE = ' ';
        const char L_TURN = '┌';
        const char V_PIPE = '│';
        const char R_TURN = '└';
        const string TAB = "    ";
        const string H_PIPE = "──";

        private readonly StringBuilder sb;
        private readonly Stack<string> indentStack;
        private readonly Stack<BranchOriental> orientalStack;

        public LogicalExpressionVisualizer()
        {
            this.sb = new StringBuilder();

            this.indentStack = new Stack<string>();
            this.orientalStack = new Stack<BranchOriental>();

            this.indentStack.Push(string.Empty);
            this.orientalStack.Push(BranchOriental.None);
        }

        public object VisitUnaryOperation(ExpressionToken op, ILogicalExpressionNode node)
        {
            BranchOriental legacyOriental = this.orientalStack.Pop();
            string legacyIndent = this.indentStack.Pop();

            if (legacyOriental == BranchOriental.Left)
            {
                sb.AppendLine(ReplaceLastChar(legacyIndent, L_TURN) + H_PIPE + " (" + op.ToString() + ")");
            }
            else
            {
                sb.AppendLine(ReplaceLastChar(legacyIndent, R_TURN) + H_PIPE + " (" + op.ToString() + ")");
            }

            if (legacyOriental == BranchOriental.Right)
            {
                this.indentStack.Push(ReplaceLastChar(legacyIndent, SPACE) + TAB + R_TURN);
                this.orientalStack.Push(BranchOriental.Right);
                node.Accept(this);
            }
            else
            {
                this.indentStack.Push(ReplaceLastChar(legacyIndent, V_PIPE) + TAB + R_TURN);
                this.orientalStack.Push(BranchOriental.Right);
                node.Accept(this);
            }

            return this.sb.ToString();
        }

        public object VisitBinaryArithmeticOperation(ExpressionToken op, ILogicalExpressionNode left, ILogicalExpressionNode right)
        {
            return VisitBinaryOperation(op, left, right);
        }

        public object VisitBinaryComparisonOperation(ExpressionToken op, ILogicalExpressionNode left, ILogicalExpressionNode right)
        {
            return VisitBinaryOperation(op, left, right);
        }

        public object VisitBinaryLogicalOperation(ExpressionToken op, ILogicalExpressionNode left, ILogicalExpressionNode right)
        {
            return VisitBinaryOperation(op, left, right);
        }

        public object VisitNumericOperand(ExpressionToken numeric)
        {
            return VisitOperand(numeric);
        }

        public object VisitStringLiteralOperand(ExpressionToken stringLiteral)
        {
            return VisitOperand(stringLiteral);
        }

        public object VisitVariableOperand(ExpressionToken variable)
        {
            return VisitOperand(variable);
        }

        private object VisitBinaryOperation(ExpressionToken op, ILogicalExpressionNode left, ILogicalExpressionNode right)
        {
            BranchOriental legacyOriental = this.orientalStack.Pop();
            string legacyIndent = this.indentStack.Pop();

            if (legacyOriental == BranchOriental.Left)
            {
                this.indentStack.Push(ReplaceLastChar(legacyIndent, SPACE) + TAB + L_TURN);
                this.orientalStack.Push(BranchOriental.Left);
                left.Accept(this);
            }
            else
            {
                this.indentStack.Push(ReplaceLastChar(legacyIndent, V_PIPE) + TAB + V_PIPE);
                this.orientalStack.Push(BranchOriental.Left);
                left.Accept(this);
            }

            if (legacyOriental == BranchOriental.Left)
            {
                sb.AppendLine(ReplaceLastChar(legacyIndent, L_TURN) + H_PIPE + " (" + op.ToString() + ")");
            }
            else
            {
                sb.AppendLine(ReplaceLastChar(legacyIndent, R_TURN) + H_PIPE + " (" + op.ToString() + ")");
            }

            if (legacyOriental == BranchOriental.Right)
            {
                this.indentStack.Push(ReplaceLastChar(legacyIndent, SPACE) + TAB + R_TURN);
                this.orientalStack.Push(BranchOriental.Right);
                right.Accept(this);
            }
            else
            {
                this.indentStack.Push(ReplaceLastChar(legacyIndent, V_PIPE) + TAB + V_PIPE);
                this.orientalStack.Push(BranchOriental.Right);
                right.Accept(this);
            }

            return this.sb.ToString();
        }

        private object VisitOperand(ExpressionToken operand)
        {
            BranchOriental legacyOriental = this.orientalStack.Pop();
            string legacyIndent = this.indentStack.Pop();

            string wrapping = operand.Type == ExpressionTokenType.StringLiteral ? "\"" : string.Empty;
            string operandFormated = string.Format("{0}{1}{2}", wrapping, operand.ToString(), wrapping);

            if (legacyOriental == BranchOriental.Left)
            {
                sb.AppendLine(ReplaceLastChar(legacyIndent, L_TURN) + H_PIPE + "  " + operandFormated);
            }
            else
            {
                sb.AppendLine(ReplaceLastChar(legacyIndent, R_TURN) + H_PIPE + "  " + operandFormated);
            }

            return this.sb.ToString();
        }

        private static string ReplaceLastChar(string str, char rep = ' ')
        {
            if (!string.IsNullOrEmpty(str))
            {
                return str.Substring(0, str.Length - 1) + rep.ToString();
            }
            else
            {
                return "";
            }
        }
    }
}
