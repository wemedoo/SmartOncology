using sReportsV2.DTOs.DTOs.FieldInstance.DataOut;
using System;
using System.Linq;

namespace sReportsV2.BusinessLayer.Helpers
{
    public abstract class OperandValue
    {
        public abstract OperandValue Plus(OperandValue rightOperandValue);
        public abstract OperandValue Minus(OperandValue rightOperandValue);
        public abstract OperandValue Multiply(OperandValue rightOperandValue);
        public abstract OperandValue Divide(OperandValue rightOperandValue);
        public abstract OperandValue Equal(OperandValue rightOperandValue);
        public abstract OperandValue NotEqual(OperandValue rightOperandValue);
        public abstract OperandValue Great(OperandValue rightOperandValue);
        public abstract OperandValue GreatOrEqual(OperandValue rightOperandValue);
        public abstract OperandValue Less(OperandValue rightOperandValue);
        public abstract OperandValue LessOrEqual(OperandValue rightOperandValue);
        public abstract OperandValue And(OperandValue rightOperandValue);
        public abstract OperandValue Or(OperandValue rightOperandValue);
        public abstract string GetOperand();

        protected OperandBooleanValue ParseBooleanValue(bool result)
        {
            return new OperandBooleanValue(result ? LogicalExpresionResult.TRUE : LogicalExpresionResult.FALSE);
        }
    }

    public class OperandNumericValue : OperandValue
    {
        public decimal Value { get; set; }

        public OperandNumericValue(decimal value)
        {
            Value = value;
        }

        public override string GetOperand()
        {
            return $"{Value}";
        }

        public override OperandValue Plus(OperandValue rightOperandValue)
        {
            return new OperandNumericValue(Value + ((OperandNumericValue)rightOperandValue).Value);
        }

        public override OperandValue Minus(OperandValue rightOperandValue)
        {
            return new OperandNumericValue(Value - ((OperandNumericValue)rightOperandValue).Value);
        }

        public override OperandValue Multiply(OperandValue rightOperandValue)
        {
            return new OperandNumericValue(Value * ((OperandNumericValue)rightOperandValue).Value);
        }

        public override OperandValue Divide(OperandValue rightOperandValue)
        {
            return new OperandNumericValue(Value - ((OperandNumericValue)rightOperandValue).Value);
        }

        public override OperandValue Equal(OperandValue rightOperandValue)
        {
            return ParseBooleanValue(Value == ((OperandNumericValue)rightOperandValue).Value);
        }

        public override OperandValue NotEqual(OperandValue rightOperandValue)
        {
            return ParseBooleanValue(Value != ((OperandNumericValue)rightOperandValue).Value);
        }

        public override OperandValue Great(OperandValue rightOperandValue)
        {
            return ParseBooleanValue(Value > ((OperandNumericValue)rightOperandValue).Value);
        }

        public override OperandValue GreatOrEqual(OperandValue rightOperandValue)
        {
            return ParseBooleanValue(Value >= ((OperandNumericValue)rightOperandValue).Value);
        }

        public override OperandValue Less(OperandValue rightOperandValue)
        {
            return ParseBooleanValue(Value < ((OperandNumericValue)rightOperandValue).Value);
        }

        public override OperandValue LessOrEqual(OperandValue rightOperandValue)
        {
            return ParseBooleanValue(Value <= ((OperandNumericValue)rightOperandValue).Value);
        }

        public override OperandValue And(OperandValue rightOperandValue)
        {
            throw new NotImplementedException();
        }

        public override OperandValue Or(OperandValue rightOperandValue)
        {
            throw new NotImplementedException();
        }
    }

    public class OperandStringValue : OperandValue
    {
        public string Value { get; set; }

        public OperandStringValue(string value)
        {
            Value = value;
        }

        public override string GetOperand()
        {
            return $@"""{Value}""";
        }

        public override OperandValue Equal(OperandValue rightOperandValue)
        {
            return ParseBooleanValue(Value == ((OperandStringValue)rightOperandValue).Value);
        }

        public override OperandValue NotEqual(OperandValue rightOperandValue)
        {
            return ParseBooleanValue(Value != ((OperandStringValue)rightOperandValue).Value);
        }

        public override OperandValue Plus(OperandValue rightOperandValue)
        {
            throw new NotImplementedException();
        }

        public override OperandValue Minus(OperandValue rightOperandValue)
        {
            throw new NotImplementedException();
        }

        public override OperandValue Multiply(OperandValue rightOperandValue)
        {
            throw new NotImplementedException();
        }

        public override OperandValue Divide(OperandValue rightOperandValue)
        {
            throw new NotImplementedException();
        }

        public override OperandValue Great(OperandValue rightOperandValue)
        {
            throw new NotImplementedException();
        }

        public override OperandValue GreatOrEqual(OperandValue rightOperandValue)
        {
            throw new NotImplementedException();
        }

        public override OperandValue Less(OperandValue rightOperandValue)
        {
            throw new NotImplementedException();
        }

        public override OperandValue LessOrEqual(OperandValue rightOperandValue)
        {
            throw new NotImplementedException();
        }

        public override OperandValue And(OperandValue rightOperandValue)
        {
            throw new NotImplementedException();
        }

        public override OperandValue Or(OperandValue rightOperandValue)
        {
            throw new NotImplementedException();
        }
    }

    public class OperandDateValue : OperandValue
    {
        public DateTime Value { get; set; }
        public bool AddMinutes { get; set; }

        public OperandDateValue(DateTime value, bool addMinutes)
        {
            Value = value;
            AddMinutes = addMinutes;
        }

        public override string GetOperand()
        {
            return $"{Value}";
        }

        public override OperandValue Equal(OperandValue rightOperandValue)
        {
            (DateTime leftOperand, DateTime rightOperand) = PrepareOperands(rightOperandValue);
            return ParseBooleanValue(leftOperand == rightOperand);
        }

        public override OperandValue NotEqual(OperandValue rightOperandValue)
        {
            (DateTime leftOperand, DateTime rightOperand) = PrepareOperands(rightOperandValue);
            return ParseBooleanValue(leftOperand != rightOperand);
        }

        public override OperandValue Great(OperandValue rightOperandValue)
        {
            (DateTime leftOperand, DateTime rightOperand) = PrepareOperands(rightOperandValue);
            return ParseBooleanValue(leftOperand > rightOperand);
        }

        public override OperandValue GreatOrEqual(OperandValue rightOperandValue)
        {
            (DateTime leftOperand, DateTime rightOperand) = PrepareOperands(rightOperandValue);
            return ParseBooleanValue(leftOperand >= rightOperand);
        }

        public override OperandValue Less(OperandValue rightOperandValue)
        {
            (DateTime leftOperand, DateTime rightOperand) = PrepareOperands(rightOperandValue);
            return ParseBooleanValue(leftOperand < rightOperand);
        }

        public override OperandValue LessOrEqual(OperandValue rightOperandValue)
        {
            (DateTime leftOperand, DateTime rightOperand) = PrepareOperands(rightOperandValue);
            return ParseBooleanValue(leftOperand <= rightOperand);
        }

        public override OperandValue Plus(OperandValue rightOperandValue)
        {
            throw new NotImplementedException();
        }

        public override OperandValue Minus(OperandValue rightOperandValue)
        {
            throw new NotImplementedException();
        }

        public override OperandValue Multiply(OperandValue rightOperandValue)
        {
            throw new NotImplementedException();
        }

        public override OperandValue Divide(OperandValue rightOperandValue)
        {
            throw new NotImplementedException();
        }

        public override OperandValue And(OperandValue rightOperandValue)
        {
            throw new NotImplementedException();
        }

        public override OperandValue Or(OperandValue rightOperandValue)
        {
            throw new NotImplementedException();
        }

        private (DateTime, DateTime) PrepareOperands(OperandValue rightOperandValue)
        {
            OperandDateValue rightOperandDate = (OperandDateValue)rightOperandValue;
            if (AddMinutes && rightOperandDate.AddMinutes)
            {
                return (Value, rightOperandDate.Value);
            }
            else
            {
                return (Value.Date, rightOperandDate.Value.Date);
            }
        }
    }

    public class OperandBooleanValue : OperandValue
    {
        public LogicalExpresionResult Value { get; set; }

        public OperandBooleanValue(LogicalExpresionResult value)
        {
            Value = value;
        }

        public override string GetOperand()
        {
            return $"{Value}";
        }

        public override OperandValue And(OperandValue rightOperandValue)
        {
            OperandBooleanValue rightOperandBooleanValue = (OperandBooleanValue)rightOperandValue;
            if (AnyConditionEqualTo(LogicalExpresionResult.FALSE, this, rightOperandBooleanValue))
            {
                return new OperandBooleanValue(LogicalExpresionResult.FALSE);
            }
            else if (AnyConditionEqualTo(LogicalExpresionResult.UNDEFINED, this, rightOperandBooleanValue))
            {
                return new OperandBooleanValue(LogicalExpresionResult.UNDEFINED);
            }
            else
            {
                return new OperandBooleanValue(LogicalExpresionResult.TRUE);
            }
        }

        public override OperandValue Or(OperandValue rightOperandValue)
        {
            OperandBooleanValue rightOperandBooleanValue = (OperandBooleanValue)rightOperandValue;
            if (AnyConditionEqualTo(LogicalExpresionResult.TRUE, this, rightOperandBooleanValue))
            {
                return new OperandBooleanValue(LogicalExpresionResult.TRUE);
            }
            else if (AnyConditionEqualTo(LogicalExpresionResult.UNDEFINED, this, rightOperandBooleanValue))
            {
                return new OperandBooleanValue(LogicalExpresionResult.UNDEFINED);
            }
            else
            {
                return new OperandBooleanValue(LogicalExpresionResult.FALSE);
            }
        }

        public override OperandValue Plus(OperandValue rightOperandValue)
        {
            throw new NotImplementedException();
        }

        public override OperandValue Minus(OperandValue rightOperandValue)
        {
            throw new NotImplementedException();
        }

        public override OperandValue Multiply(OperandValue rightOperandValue)
        {
            throw new NotImplementedException();
        }

        public override OperandValue Divide(OperandValue rightOperandValue)
        {
            throw new NotImplementedException();
        }

        public override OperandValue Equal(OperandValue rightOperandValue)
        {
            throw new NotImplementedException();
        }

        public override OperandValue NotEqual(OperandValue rightOperandValue)
        {
            throw new NotImplementedException();
        }

        public override OperandValue Great(OperandValue rightOperandValue)
        {
            throw new NotImplementedException();
        }

        public override OperandValue GreatOrEqual(OperandValue rightOperandValue)
        {
            throw new NotImplementedException();
        }

        public override OperandValue Less(OperandValue rightOperandValue)
        {
            throw new NotImplementedException();
        }

        public override OperandValue LessOrEqual(OperandValue rightOperandValue)
        {
            throw new NotImplementedException();
        }

        private bool AnyConditionEqualTo(LogicalExpresionResult logicalExpresionResult, params OperandBooleanValue[] operandsResult)
        {
            return operandsResult.ToList().Exists(x => x.Value == logicalExpresionResult);
        }
    }
}
