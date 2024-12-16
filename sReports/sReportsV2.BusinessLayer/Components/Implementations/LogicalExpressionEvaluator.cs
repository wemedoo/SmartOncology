using sReportsV2.BusinessLayer.Components.Interfaces;
using sReportsV2.BusinessLayer.Helpers;
using sReportsV2.Common.Constants;
using sReportsV2.DTOs.DTOs.FieldInstance.DataIn;
using sReportsV2.DTOs.DTOs.FieldInstance.DataOut;
using sReportsV2.DTOs.DTOs.FormInstance.DataIn;
using sReportsV2.DTOs.Field.DataIn;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace sReportsV2.BusinessLayer.Components.Implementations
{
    public class LogicalExpressionEvaluator : ILogicalExpressionNodeVisitor
    {
        private readonly FieldInstanceDependencyDataIn _dependencyData;
        private readonly Dictionary<ExpressionTokenType, Func<OperandValue, OperandValue, object>> _binaryFunctions;

        public LogicalExpressionEvaluator(FieldInstanceDependencyDataIn dependencyData)
        {
            this._dependencyData = dependencyData;
            this._binaryFunctions = new Dictionary<ExpressionTokenType, Func<OperandValue, OperandValue, object>> {
                { ExpressionTokenType.Plus, (left, right) => left.Plus(right) },
                { ExpressionTokenType.Minus, (left, right) => left.Minus(right) },
                { ExpressionTokenType.Multiply, (left, right) => left.Multiply(right) },
                { ExpressionTokenType.Divide, (left, right) => left.Divide(right) },
                { ExpressionTokenType.Equal, (left, right) => left.Equal(right) },
                { ExpressionTokenType.NotEqual, (left, right) => left.NotEqual(right) },
                { ExpressionTokenType.Great, (left, right) => left.Great(right) },
                { ExpressionTokenType.GreatOrEqual, (left, right) => left.GreatOrEqual(right) },
                { ExpressionTokenType.Less, (left, right) => left.Less(right) },
                { ExpressionTokenType.LessOrEqual, (left, right) => left.Less(right) },
                { ExpressionTokenType.And, (left, right) => left.And(right) },
                { ExpressionTokenType.Or, (left, right) => left.Or(right) },
            };
        }

        #region Operations
        public object VisitUnaryOperation(ExpressionToken op, ILogicalExpressionNode node)
        {
            OperandValue nodeResult = (OperandValue)node.Accept(this);
            if (IsResultUndefined(nodeResult))
            {
                return new OperandBooleanValue(LogicalExpresionResult.UNDEFINED);
            }

            return EvalBinaryOperation(op, new OperandNumericValue(0), nodeResult);
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
            OperandValue leftRawResult = (OperandValue)left.Accept(this);
            if (IsResult(leftRawResult, LogicalExpresionResult.FALSE) && op.Type == ExpressionTokenType.And)
            {
                return leftRawResult;
            }
            if (IsResult(leftRawResult, LogicalExpresionResult.TRUE) && op.Type == ExpressionTokenType.Or)
            {
                return leftRawResult;
            }
            OperandValue rightRawResult = (OperandValue)right.Accept(this);

            return EvalBinaryOperation(op, leftRawResult, rightRawResult);
        }

        private object VisitBinaryOperation(ExpressionToken op, ILogicalExpressionNode left, ILogicalExpressionNode right)
        {
            OperandValue leftResult = (OperandValue)left.Accept(this);
            if (IsResultUndefined(leftResult))
            {
                return leftResult;
            }
            OperandValue rightResult = (OperandValue)right.Accept(this);
            if (IsResultUndefined(rightResult))
            {
                return rightResult;
            }
            return EvalBinaryOperation(op, leftResult, rightResult);
        }

        private object EvalBinaryOperation(ExpressionToken op, OperandValue leftResult, OperandValue rightResult)
        {
            ValidateOperandResultType(op, leftResult, rightResult);
            if (_binaryFunctions.TryGetValue(op.Type, out Func<OperandValue, OperandValue, object> function))
            {
                return function(leftResult, rightResult);
            }
            else
            {
                throw new InvalidEvaluationException(string.Format("Token of type {0} cannot be evaluated.", op.Type.ToString()));
            }
        }

        private void ValidateOperandResultType(ExpressionToken op, OperandValue leftResult, OperandValue rightResult)
        {
            if (leftResult.GetType() != rightResult.GetType())
            {
                throw new InvalidEvaluationException($"For {leftResult.GetOperand()} {op.Value} {rightResult.GetOperand()}, there is type dismatch of operands, {{{leftResult.GetType().Name}}} of one side and {{{rightResult.GetType().Name}}} on the other side");
            }
        }

        private bool IsResultUndefined(OperandValue operand)
        {
            return IsResult(operand, LogicalExpresionResult.UNDEFINED);
        }

        private bool IsResult(OperandValue operand, LogicalExpresionResult result)
        {
            return operand is OperandBooleanValue operandBoolean && operandBoolean.Value == result;
        }

        #endregion /Operations

        #region Operands
        public object VisitNumericOperand(ExpressionToken numeric)
        {
            return new OperandNumericValue(decimal.Parse(numeric.Value));
        }

        public object VisitStringLiteralOperand(ExpressionToken stringLiteral)
        {
            bool isDate = DateTime.TryParse(stringLiteral.Value, out DateTime parsedDate);
            if (isDate)
            {
                return CreaateDateOperandDateValue(stringLiteral.Value);
            }
            else
            {
                return new OperandStringValue(stringLiteral.Value);
            }
        }

        public object VisitVariableOperand(ExpressionToken variable)
        {
            string variableName = variable.Value;
            DependentOnFieldInfoDataIn dependentOnFieldInfoData = 
                this._dependencyData.DependentOnFieldInfos.Find(x => x.Variable == variableName) ?? throw new InvalidEvaluationException($"Cannot find dependent field instance with variable [{variableName}]");
            FieldInstanceDTO fieldInstanceDataIn = GetFieldInstance(dependentOnFieldInfoData) ?? throw new InvalidEvaluationException($"Cannot find field instance with field-id \"{dependentOnFieldInfoData.FieldId}\"");

            if (dependentOnFieldInfoData.VariableAssignedToOption())
            {
                return VisitStringLiteralOperand(new ExpressionToken(ExpressionTokenType.StringLiteral, dependentOnFieldInfoData.FieldValueId));
            }
            else
            {
                string value = GetFieldInstanceValue(fieldInstanceDataIn, dependentOnFieldInfoData);
                return VisitVariableOperand(fieldInstanceDataIn.Type, value);
            }
        }

        private string GetFieldInstanceValue(FieldInstanceDTO fieldInstanceDataIn, DependentOnFieldInfoDataIn dependentOnFieldInfoData)
        {
            string fieldInstanceValue = string.Empty;
            if (fieldInstanceDataIn.FlatValues.Count > 0 && !fieldInstanceDataIn.IsSpecialValue)
            {
                switch (fieldInstanceDataIn.Type)
                {
                    case FieldTypes.Coded:
                        fieldInstanceValue = fieldInstanceDataIn.FlatValueLabel;
                        break;
                    case FieldTypes.Checkbox:
                        string selectedOptionId = this._dependencyData.DependentOnFieldInfos
                            .Find(x => x.FieldId == dependentOnFieldInfoData.FieldId && !string.IsNullOrEmpty(x.FieldValueId))?
                            .FieldValueId;
                        fieldInstanceValue = fieldInstanceDataIn.FlatValues.Find(fV => fV == selectedOptionId);
                        break;
                    case FieldTypes.Date:
                        fieldInstanceValue = TransformDateInUpcomingSReportsFormat(fieldInstanceDataIn.FlatValues[0]);
                        break;
                    case FieldTypes.Datetime:
                        fieldInstanceValue = TransforDateTimeInUpcomingSReportsFormat(fieldInstanceDataIn.FlatValues[0]);
                        break;
                    default:
                        fieldInstanceValue = fieldInstanceDataIn.FlatValues[0];
                        break;
                }
            }
            return fieldInstanceValue;
        }

        private OperandDateValue CreaateDateOperandDateValue(string dateString)
        {
            if (DateTime.TryParseExact(dateString, $"MM/dd/yyyy {DateConstants.TimeFormatDisplay}", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDateTimme))
            {
                return new OperandDateValue(TransformDateInSReportsFormat(parsedDateTimme, true), true);
            }
            else if (DateTime.TryParseExact(dateString, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return new OperandDateValue(TransformDateInSReportsFormat(parsedDate, false), false);
            }
            else
            {
                throw new InvalidEvaluationException($"Unrecognizable date(-time) format, value: {dateString}");
            }
        }

        private DateTime TransformDateInSReportsFormat(DateTime dateTime, bool addMinutes)
        {
            DateTime result = addMinutes
                ? new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, DateTimeKind.Local)
                  : new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Local);

            return result;
        }

        private string TransformDateInUpcomingSReportsFormat(string dateString)
        {
            string result = string.Empty;
            if (DateTime.TryParse(dateString, out DateTime date))
            {
                result = $"{date.Day:00}/{date.Month:00}/{date.Year}";
            }
            return result;
        }

        private string TransforDateTimeInUpcomingSReportsFormat(string dateTimeString)
        {
            string result = string.Empty;
            string datetimeInUtc = dateTimeString.Length >= 16 ? dateTimeString.Substring(0, 16) : string.Empty;
            if (DateTime.TryParse(datetimeInUtc, out DateTime dateTime))
            {
                result = $"{dateTime.Day:00}/{dateTime.Month:00}/{dateTime.Year} {dateTime.Hour:00}:{dateTime.Minute:00}";
            }
            return result;
        }

        private object VisitVariableOperand(string fieldType, string value)
        {
            switch (fieldType)
            {
                case FieldTypes.Number:
                case FieldTypes.Calculative:
                    return string.IsNullOrEmpty(value) ?
                        new OperandBooleanValue(LogicalExpresionResult.UNDEFINED)
                        : VisitNumericOperand(new ExpressionToken(ExpressionTokenType.Number, value));
                case FieldTypes.Regex:
                case FieldTypes.Email:
                case FieldTypes.Coded:
                case FieldTypes.Select:
                case FieldTypes.Radio:
                case FieldTypes.Checkbox:
                case FieldTypes.Datetime:
                case FieldTypes.Date:
                case FieldTypes.LongText:
                case FieldTypes.Text:
                    return string.IsNullOrEmpty(value) ?
                        new OperandBooleanValue(LogicalExpresionResult.UNDEFINED)
                        : VisitStringLiteralOperand(new ExpressionToken(ExpressionTokenType.StringLiteral, value));
                default:
                    throw new InvalidEvaluationException($"{fieldType} cannot be included in dependency formula");
            }
        }

        private FieldInstanceDTO GetFieldInstance(DependentOnFieldInfoDataIn dependentOnFieldInfoData)
        {
            FieldInstanceDTO fieldInstanceDataIn = null;
            if (this._dependencyData.IsChildDependentFieldSetRepetitive)
            {
                fieldInstanceDataIn = this._dependencyData.FieldInstancesInFormula.Find(x => x.FieldId == dependentOnFieldInfoData.FieldId && x.FieldSetInstanceRepetitionId == this._dependencyData.ChildFieldSetInstanceRepetitionId);
            }
            if (fieldInstanceDataIn == null)
            {
                fieldInstanceDataIn = this._dependencyData.FieldInstancesInFormula.Find(x => x.FieldId == dependentOnFieldInfoData.FieldId);
            }

            return fieldInstanceDataIn;
        }
        #endregion /Operands
    }

    #region Exceptions
    [Serializable]
    public class InvalidEvaluationException : Exception
    {
        public InvalidEvaluationException()
        {
        }

        public InvalidEvaluationException(string message) : base(message)
        {
        }
    }
    #endregion /Exceptions
}
