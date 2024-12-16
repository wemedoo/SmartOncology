using Hl7.Fhir.Model;
using sReportsV2.BusinessLayer.Components.Interfaces;
using sReportsV2.BusinessLayer.Helpers;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Helpers;
using sReportsV2.DTOs.Field.DataOut;
using sReportsV2.DTOs.Form.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Configurations;

namespace sReportsV2.BusinessLayer.Components.Implementations
{
    public class LogicalExpressionFHIRAnalyzer : ILogicalExpressionNodeVisitor
    {
        private readonly List<FieldDataOut> _fields;
        private readonly DependentOnInfoDataOut _dependentOnInfoDataOut;
        private readonly List<ExpressionTokenType> _visitedLogicalOperators;
        private readonly string _host;

        public LogicalExpressionFHIRAnalyzer(List<FieldDataOut> fields, DependentOnInfoDataOut dependentOnInfoDataOut, string host)
        {
            _fields = fields;
            _dependentOnInfoDataOut = dependentOnInfoDataOut;
            _visitedLogicalOperators = new List<ExpressionTokenType>();
            _host = host;
        }

        public object VisitUnaryOperation(ExpressionToken op, ILogicalExpressionNode node)
        {
            var result = node.Accept(this);
            if (result is FhirSimpleOperand fhirSimpleOperand)
            {
                string valueWithSign = $"-{fhirSimpleOperand.Value}";
                fhirSimpleOperand.Value = valueWithSign;
                fhirSimpleOperand.Label = valueWithSign;
                return result;
            }
            throw new NotImplementedException($"Unary operation returns unexpected result. Instead of FhirSimpleOperand, node: {node}; result: {result}");
        }

        public object VisitBinaryArithmeticOperation(ExpressionToken op, ILogicalExpressionNode left, ILogicalExpressionNode right)
        {
            if (op != null && right != null)
            {
                throw new NotImplementedException("Binary arithmetic operation is not supported");
            }
            return left.Accept(this);
        }

        public object VisitBinaryComparisonOperation(ExpressionToken op, ILogicalExpressionNode left, ILogicalExpressionNode right)
        {
            object leftResult = left.Accept(this);
            object rigthResult = right.Accept(this);

            if (leftResult == null || right == null)
            {
                throw new NotImplementedException("Either left or right operand get null as result in comparison operation");
            }

            Questionnaire.EnableWhenComponent enableWhen;
            if (leftResult is FhirFieldOperand fieldOperandL && rigthResult is FhirSimpleOperand simpleOperandR)
            {
                enableWhen = fieldOperandL.CreateFhirDependency(op.Type, simpleOperandR);
            }
            else if (leftResult is FhirSimpleOperand simpleOperandL && rigthResult is FhirFieldOperand fieldOperandR)
            {
                enableWhen = fieldOperandR.CreateFhirDependency(op.Type, simpleOperandL);
            }
            else
            {
                throw new NotImplementedException($"Invalid operands in comparison operation, left: {left}, right: {right}");
            }

            return new List<Questionnaire.EnableWhenComponent> { enableWhen };
        }

        public object VisitBinaryLogicalOperation(ExpressionToken op, ILogicalExpressionNode left, ILogicalExpressionNode right)
        {
            List<Questionnaire.EnableWhenComponent> fhirDependencies = new List<Questionnaire.EnableWhenComponent>();

            AddVisitedOperator(op);
            if (left != null)
            {
                List<Questionnaire.EnableWhenComponent> leftDependencies = (List<Questionnaire.EnableWhenComponent>) left.Accept(this);
                fhirDependencies.AddRange(leftDependencies);
            }
            if (right != null)
            {
                List<Questionnaire.EnableWhenComponent> rightDependencies = (List<Questionnaire.EnableWhenComponent>)right.Accept(this);
                fhirDependencies.AddRange(rightDependencies);
            }

            return fhirDependencies;
        }

        public object VisitNumericOperand(ExpressionToken numeric)
        {
            return new FhirSimpleOperand(numeric.Value);
        }

        public object VisitStringLiteralOperand(ExpressionToken stringLiteral)
        {
            return new FhirSimpleOperand(stringLiteral.Value);
        }

        public object VisitVariableOperand(ExpressionToken variable)
        {
            DependentOnFieldInfoDataOut dependentOnFieldInfo = _dependentOnInfoDataOut.GetDependentOnFieldInfoByVariable(variable.Value) ?? throw new NotImplementedException($"Could not find variable {variable.Value}");
            FieldDataOut field = _fields.Find(f => f.Id == dependentOnFieldInfo.FieldId);
            if (!string.IsNullOrEmpty(dependentOnFieldInfo.FieldValueId))
            {
                if (field is FieldSelectableDataOut fieldSelectable)
                {
                    FormFieldValueDataOut formFieldOption = fieldSelectable.GetOption(dependentOnFieldInfo.FieldValueId) 
                        ?? throw new NotImplementedException($"Selected options does not exist variable: {variable.Value}, field: {field.Label}");
                    return new FhirSimpleOperand(
                            formFieldOption.ThesaurusId.ToString(),
                            formFieldOption.Label,
                            dependentOnFieldInfo.FieldId
                        );
                }
                else
                {
                    throw new NotImplementedException($"{field.Label} is not selectable");
                }
            }
            else
            {
                return new FhirFieldOperand(field, _host);
            }
        }

        public Questionnaire.EnableWhenBehavior GetLogicalOperator()
        {
            if (this._visitedLogicalOperators.Count == 0)
            {
                this._visitedLogicalOperators.Add(ExpressionTokenType.Or);
            }
            ExpressionTokenType operatorTokenType = this._visitedLogicalOperators.First();
            if (this._visitedLogicalOperators.TrueForAll(op => op == operatorTokenType))
            {
                if (operatorTokenType == ExpressionTokenType.And)
                {
                    return Questionnaire.EnableWhenBehavior.All;
                }
                else if(operatorTokenType == ExpressionTokenType.Or)
                {
                    return Questionnaire.EnableWhenBehavior.Any;
                }
            }
            throw new NotImplementedException($"All operands should be of the same type (And/Or) but [{string.Join(",", this._visitedLogicalOperators)}] found");
        }

        private void AddVisitedOperator(ExpressionToken operatorToken)
        {
            if (operatorToken != null)
            {
                this._visitedLogicalOperators.Add(operatorToken.Type);
            }
        }
    }

    public abstract class FhirOperand
    {
        public string FieldId { get; set; }

        protected FhirOperand(string fieldId)
        {
            FieldId = fieldId;
        }
    }

    public class FhirSimpleOperand : FhirOperand 
    {
        public string Label { get; set; }
        public string Value { get; set; }

        public FhirSimpleOperand(string value, string label, string fieldId) : base(fieldId)
        {
            Value = value;
            Label = label;
        }

        public FhirSimpleOperand(string value) : this(value, value, null)  {}
    }

    public class FhirFieldOperand : FhirOperand
    {
        public FieldDataOut Field { get; set; }
        private readonly string _thesaurusUrlTemplate;


        public FhirFieldOperand(FieldDataOut field, string host) : base(field.Id)
        {
            this.Field = field;
            this._thesaurusUrlTemplate = host.GetPreviewThesaurusUrlTemplate();
        }

        public Questionnaire.EnableWhenComponent CreateFhirDependency(ExpressionTokenType expressionTokenType, FhirSimpleOperand fhirSimpleOperand)
        {
            Questionnaire.EnableWhenComponent enableWhen = new Questionnaire.EnableWhenComponent
            {
                Operator = GetOperator(expressionTokenType),
                Question = this.FieldId,
                Answer = GetAnswer(fhirSimpleOperand)
            };

            return enableWhen;
        }

        private Questionnaire.QuestionnaireItemOperator GetOperator(ExpressionTokenType expressionTokenType)
        {
            switch (expressionTokenType)
            {
                case ExpressionTokenType.Equal: return Questionnaire.QuestionnaireItemOperator.Equal;
                case ExpressionTokenType.NotEqual: return Questionnaire.QuestionnaireItemOperator.NotEqual;
                case ExpressionTokenType.Great: return Questionnaire.QuestionnaireItemOperator.GreaterThan;
                case ExpressionTokenType.GreatOrEqual: return Questionnaire.QuestionnaireItemOperator.GreaterOrEqual;
                case ExpressionTokenType.Less: return Questionnaire.QuestionnaireItemOperator.LessThan;
                case ExpressionTokenType.LessOrEqual: return Questionnaire.QuestionnaireItemOperator.LessOrEqual;
                default:
                    throw new NotImplementedException($"Unexpected token {expressionTokenType} occurred in comparison operation");
            }
        }

        private Element GetAnswer(FhirSimpleOperand fhirSimpleOperand)
        {
            switch (this.Field.Type)
            {
                case FieldTypes.Select:
                case FieldTypes.Radio:
                case FieldTypes.Checkbox: return GetSelectableAnswer(fhirSimpleOperand);
                case FieldTypes.Number: return GetNumericAnswer(fhirSimpleOperand);
                case FieldTypes.Datetime:
                case FieldTypes.Date: return GetDateAnswer(fhirSimpleOperand);
                default:
                    return new FhirString(fhirSimpleOperand.Value);
            }

        }

        private Element GetNumericAnswer(FhirSimpleOperand fhirSimpleOperand)
        {
            if (this.Field is FieldNumericDataOut fieldNumeric)
            {
                bool isInteger = fieldNumeric.Step.HasValue && fieldNumeric.Step.Value % 1 == 0;
                if (isInteger)
                {
                    if (int.TryParse(fhirSimpleOperand.Value, out int parsedInt))
                    {
                        return new Integer(parsedInt);
                    }
                }
                else
                {
                    if (decimal.TryParse(fhirSimpleOperand.Value, out decimal parsedDecimal))
                    {
                        return new FhirDecimal(parsedDecimal);
                    }
                }
            }

            throw new NotImplementedException($"GetNumericAnswer ends with error, fhirSimpleOperand value is {fhirSimpleOperand.Value} and field (type: {this.Field.Type}, id: {this.FieldId})"); 
        }

        private Element GetDateAnswer(FhirSimpleOperand fhirSimpleOperand)
        {
            if (this.Field is FieldDateDataOut _ && DateTime.TryParse(fhirSimpleOperand.Value, out DateTime parsedDate))
            {
                return new Date(parsedDate.GetDateTimeDisplay(DateConstants.UTCDatePartFormat, excludeTimePart: true));
            }
            else if (this.Field is FieldDatetimeDataOut _ && DateTime.TryParse(fhirSimpleOperand.Value, out DateTime dateTime))
            {
                string processedValue = dateTime.ToString("yyyy-MM-ddTHH:mm") + GlobalConfig.GetUserOffset(isOffsetForFormInstance: true);
                return new FhirDateTime(processedValue);
            }

            throw new NotImplementedException($"GetDateAnswer ends with error. fhirSimpleOperand value is {fhirSimpleOperand.Value} and field (type: {this.Field.Type}, id: {this.FieldId})");
        }

        private Element GetSelectableAnswer(FhirSimpleOperand fhirSimpleOperand)
        {
            if (this.FieldId == fhirSimpleOperand.FieldId)
            {
                return CreateCode(fhirSimpleOperand.Value, fhirSimpleOperand.Label);
            }
            else
            { 
                throw new NotImplementedException($"GetSelectableAnswer ends with error. option does not belong to the selectable field. fhirSimpleOperand value is {fhirSimpleOperand.Value} and field (type: {this.Field.Type}, id: {this.FieldId})");
            }
        }

        private Coding CreateCode(string code, string display)
        {
            return new Coding(_thesaurusUrlTemplate + code, code, display);
        }
    }
}
