using sReportsV2.BusinessLayer.Helpers;
using sReportsV2.DTOs.Field.DataIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sReportsV2.BusinessLayer.Components.Implementations
{
    public class LogicalExpressionParser
    {
        private readonly string _text;

        private ExpressionToken _curToken;
        private int _curPos;
        private readonly int _charCount;
        private char _curChar;

        private Dictionary<string, ExpressionTokenType> _comparisonOperators;
        private Dictionary<string, ExpressionTokenType> _arithmeticOperators;

        private List<string> _variablesFromFormula;
        private readonly DependentOnInfoDataIn _dependentOnInfo;

        public LogicalExpressionParser(DependentOnInfoDataIn dependentOnInfo)
        {
            this._text = string.IsNullOrEmpty(dependentOnInfo.Formula) ? string.Empty : dependentOnInfo.Formula;
            this._dependentOnInfo = dependentOnInfo;
            this._charCount = this._text.Length;

            this.SetInitialPropertiesToDefault();
        }

        public LogicalExpression Parse()
        {
            this.NextToken();
            LogicalExpression node = this.GrabLogicalExpression();
            this.ExpectToken(ExpressionTokenType.None);
            return node;
        }

        public void CheckIfAllVariablesAreIncluded()
        {
            List<string> variablesFromInput = this._dependentOnInfo.GetFormulaVariables();
            if (variablesFromInput.Count != _variablesFromFormula.Count
                || !variablesFromInput.TrueForAll(_variablesFromFormula.Contains))
            {
                throw new InvalidFormulaException("Not all variables are included in formula");
            }
        }

        #region Logical Rules
        private LogicalExpression GrabLogicalExpression()
        {
            LogicalExpression left = this.GrabOrTerm();

            while (IsCurrentTokenTypeEqualTo(ExpressionTokenType.Or))
            {
                ExpressionToken op = this._curToken;
                this.NextToken();
                LogicalExpression right = this.GrabOrTerm();
                left = new BinaryLogicalOperation(op, left, right);
            }

            return left;
        }

        private LogicalExpression GrabOrTerm()
        {
            LogicalExpression left = this.GrabAndTerm();

            while (IsCurrentTokenTypeEqualTo(ExpressionTokenType.And))
            {
                ExpressionToken op = this._curToken;
                this.NextToken();
                LogicalExpression right = this.GrabAndTerm();
                left = new BinaryLogicalOperation(op, left, right);
            }

            return left;
        }

        private LogicalExpression GrabAndTerm()
        {
            LogicalExpression left = this.GrabLogicalTerm();

            if (!CurrentTokenIsComparisonOperator() && !IsBlankCharacter() && !ReachEnd())
            {
                ThrowUnexpectedTokenError($"{{{string.Join(", ", this._comparisonOperators.Values)}}}");
            }

            while (CurrentTokenIsComparisonOperator())
            {
                ExpressionToken op = this._curToken;
                this.NextToken();
                LogicalExpression right = this.GrabLogicalTerm();
                ValidateComparisonOperation(left, op, right);
                left = new BinaryComparisonOperation(op, left, right);
            }

            return left;
        }

        private LogicalExpression GrabLogicalTerm()
        {
            if (IsCurrentTokenTypeEqualTo(ExpressionTokenType.LeftParenthesis))
            {
                LogicalExpression node = this.GrabLogicalBracketExpr();
                return node;
            }
            else
            {
                return this.GrabArithmeticExpresion();
            }
        }

        private LogicalExpression GrabLogicalBracketExpr()
        {
            this.ExpectToken(ExpressionTokenType.LeftParenthesis);
            this.NextToken();
            LogicalExpression node = this.GrabLogicalExpression();
            this.ExpectToken(ExpressionTokenType.RightParenthesis);
            this.NextToken();
            return node;
        }

        #endregion /Logical Rules

        #region Arithmetic Rules
        private LogicalExpression GrabArithmeticExpresion()
        {
            LogicalExpression left = this.GrabArithmeticTerm();

            while (IsCurrentTokenTypeEqualTo(ExpressionTokenType.Plus)
                || IsCurrentTokenTypeEqualTo(ExpressionTokenType.Minus))
            {
                ExpressionToken op = this._curToken;
                this.NextToken();
                LogicalExpression right = this.GrabArithmeticTerm();
                ValidateArithmeticOperation(left, op, right);
                left = new BinaryArithmeticOperation(op, left, right);
            }

            return left;
        }

        private LogicalExpression GrabArithmeticTerm()
        {
            LogicalExpression left = this.GrabArithmeticFactor();

            while (IsCurrentTokenTypeEqualTo(ExpressionTokenType.Multiply)
                || IsCurrentTokenTypeEqualTo(ExpressionTokenType.Divide))
            {
                ExpressionToken op = this._curToken;
                this.NextToken();
                LogicalExpression right = this.GrabArithmeticFactor();
                ValidateArithmeticOperation(left, op, right);
                left = new BinaryArithmeticOperation(op, left, right);
            }

            return left;
        }

        private LogicalExpression GrabArithmeticFactor()
        {
            if (IsCurrentTokenTypeEqualTo(ExpressionTokenType.Plus)
                || IsCurrentTokenTypeEqualTo(ExpressionTokenType.Minus))
            {
                LogicalExpression node = this.GrabArithmeticUnaryExpression();
                return node;
            }
            else if (IsCurrentTokenTypeEqualTo(ExpressionTokenType.LeftParenthesis))
            {
                LogicalExpression node = this.GrabArithmeticBracketExpression();
                return node;
            }
            else if (IsCurrentTokenTypeEqualTo(ExpressionTokenType.LeftBracket))
            {
                LogicalExpression node = this.GrabVariable();
                return node;
            }
            else if (IsCurrentTokenTypeEqualTo(ExpressionTokenType.Quote))
            {
                LogicalExpression node = this.GrabStringLiteral();
                return node;
            }
            else
            {
                ExpressionToken token = this.ExpectToken(ExpressionTokenType.Number);
                this.NextToken();
                return new NumericOperand(token);
            }
        }

        private LogicalExpression GrabArithmeticUnaryExpression()
        {
            ExpressionToken op;

            if (IsCurrentTokenTypeEqualTo(ExpressionTokenType.Plus))
            {
                op = this.ExpectToken(ExpressionTokenType.Plus);
            }
            else
            {
                op = this.ExpectToken(ExpressionTokenType.Minus);
            }

            this.NextToken();

            if (IsCurrentTokenTypeEqualTo(ExpressionTokenType.Plus)
                || IsCurrentTokenTypeEqualTo(ExpressionTokenType.Minus))
            {
                LogicalExpression expr = this.GrabArithmeticUnaryExpression();
                return new UnaryOperation(op, expr);
            }
            else
            {
                LogicalExpression expr = this.GrabArithmeticFactor();
                ValidateArithmeticOperation(null, op, expr);
                return new UnaryOperation(op, expr);
            }
        }

        private LogicalExpression GrabArithmeticBracketExpression()
        {
            this.ExpectToken(ExpressionTokenType.LeftParenthesis);
            this.NextToken();
            LogicalExpression node = this.GrabArithmeticExpresion();
            this.ExpectToken(ExpressionTokenType.RightParenthesis);
            this.NextToken();
            return node;
        }

        private LogicalExpression GrabVariable()
        {
            this.ExpectToken(ExpressionTokenType.LeftBracket);
            this.NextToken();
            ExpressionToken token = this.ExpectToken(ExpressionTokenType.Variable);
            AddVariable(token.Value);
            LogicalExpression node = new VariableOperand(token);
            this.NextToken();
            this.ExpectToken(ExpressionTokenType.RightBracket);
            this.NextToken();
            return node;
        }

        private LogicalExpression GrabStringLiteral()
        {
            this.ExpectToken(ExpressionTokenType.Quote);
            this.NextToken();
            ExpressionToken token = this.ExpectToken(ExpressionTokenType.StringLiteral);
            LogicalExpression node = new StringLiteralOperand(token);
            this.NextToken();
            this.ExpectToken(ExpressionTokenType.Quote);
            this.NextToken();
            return node;
        }
        #endregion /Arithmetic Rules

        #region Next Token Methods
        private void NextToken()
        {
            if (CheckIfNextTokenReachedEnd())
            {
                return;
            }

            if (EatBlankCharacters())
            {
                return;
            }

            if (CheckIfNextTokenIsLogicalOperator())
            {
                return;
            }

            if (CheckIfNextTokenIsParenthesis())
            {
                return;
            }

            if (CheckIfNextTokenIsComparisonOperator())
            {
                return;
            }

            if (CheckIfNextTokenIsArithmeticOperator())
            {
                return;
            }

            if (CheckIfNextTokenIsQuote())
            {
                return;
            }

            if (CheckIfNextTokenIsStringLiteral())
            {
                return;
            }

            if (CheckIfNextTokenIsBracket())
            {
                return;
            }

            if (CheckIfNextTokenIsVariable())
            {
                return;
            }

            if (CheckIfNextTokenIsNumber())
            {
                return;
            }

            ThrowUnexpectedSymbolError();
        }

        private bool CheckIfNextTokenReachedEnd()
        {
            bool reachEnd = this.ReachEnd();
            if (reachEnd)
            {
                this._curToken = ExpressionToken.None();
            }
            return reachEnd;
        }

        private bool EatBlankCharacters()
        {
            bool reachEnd = false;
            if (IsBlankCharacter())
            {
                while (!ReachEnd() && IsBlankCharacter())
                {
                    this.Advance();
                }

                if (ReachEnd())
                {
                    this._curToken = ExpressionToken.None();
                    reachEnd = true;
                }
            }
            return reachEnd;
        }

        private bool CheckIfNextTokenIsLogicalOperator()
        {
            bool isLogicalOperator = false;
            if (this._curChar == 'a' || this._curChar == 'A')
            {
                string possibleOperatorFromInput = new string(new char[]
                            {
                            this._curChar,
                            GetNextCharacter(1),
                            GetNextCharacter(2)
                            });
                isLogicalOperator = IsNextLogicalOperator(possibleOperatorFromInput, ExpressionTokenType.And.ToString(), ExpressionTokenType.And);
            }

            if (this._curChar == '&')
            {
                string possibleOperatorFromInput = new string(new char[]
                            {
                            this._curChar,
                            GetNextCharacter(1)
                            });
                isLogicalOperator = IsNextLogicalOperator(possibleOperatorFromInput, "&&", ExpressionTokenType.And);
            }

            if (this._curChar == 'o' || this._curChar == 'O')
            {
                string possibleOperatorFromInput = new string(new char[]
                            {
                            this._curChar,
                            GetNextCharacter(1)
                            });
                isLogicalOperator = IsNextLogicalOperator(possibleOperatorFromInput, ExpressionTokenType.Or.ToString(), ExpressionTokenType.Or);
            }

            if (this._curChar == '|')
            {
                string possibleOperatorFromInput = new string(new char[]
                            {
                            this._curChar,
                            GetNextCharacter(1)
                            });
                isLogicalOperator = IsNextLogicalOperator(possibleOperatorFromInput, "||", ExpressionTokenType.Or);
            }

            return isLogicalOperator;
        }

        private bool IsNextLogicalOperator(string possibleOperatorFromInput, string operatorToCheckWith, ExpressionTokenType expressionTokenType)
        {
            bool isLogicalOperator = EqualsToOperator(operatorToCheckWith, possibleOperatorFromInput);
            if (isLogicalOperator)
            {
                SetCurrentTokenAndMovePosition(expressionTokenType, possibleOperatorFromInput);
            }

            return isLogicalOperator;
        }

        private bool CheckIfNextTokenIsParenthesis()
        {
            bool isParenthesis = IsNextCharacterEqualTo('(', ExpressionTokenType.LeftParenthesis);
            return isParenthesis ? isParenthesis : IsNextCharacterEqualTo(')', ExpressionTokenType.RightParenthesis);
        }

        private bool CheckIfNextTokenIsBracket()
        {
            bool isBracket = IsNextCharacterEqualTo('[', ExpressionTokenType.LeftBracket);
            return isBracket ? isBracket : IsNextCharacterEqualTo(']', ExpressionTokenType.RightBracket);
        }

        private bool IsNextCharacterEqualTo(char charToCompareWith, ExpressionTokenType expressionTokenType)
        {
            bool isParenthesis = this._curChar == charToCompareWith;
            if (isParenthesis)
            {
                SetCurrentTokenAndMovePosition(expressionTokenType, this._curChar.ToString());
            }

            return isParenthesis;
        }

        private bool CheckIfNextTokenIsComparisonOperator()
        {
            bool isComparisonOperator = false;
            if (this._comparisonOperators.Keys.FirstOrDefault(comparisonOperator => comparisonOperator.StartsWith(this._curChar.ToString())) != null)
            {
                string operatorFromInput = string.Empty;
                char nextCharacter = GetNextCharacter(1);
                if (this._curChar == '<' || this._curChar == '>')
                {
                    operatorFromInput = this._curChar.ToString();
                    if (nextCharacter == '=')
                    {
                        operatorFromInput += nextCharacter;
                    }
                }
                else
                {
                    operatorFromInput = new string(new char[]
                    {
                        this._curChar,
                        nextCharacter
                    });
                }

                isComparisonOperator = this._comparisonOperators.TryGetValue(operatorFromInput, out ExpressionTokenType parsedToken);
                if (isComparisonOperator)
                {
                    SetCurrentTokenAndMovePosition(parsedToken, operatorFromInput);
                }
            }

            return isComparisonOperator;
        }

        private bool CheckIfNextTokenIsArithmeticOperator()
        {
            string currentChar = this._curChar.ToString();
            bool isArithmeticOperator = this._arithmeticOperators.TryGetValue(currentChar, out ExpressionTokenType parsedToken);
            if (isArithmeticOperator)
            {
                SetCurrentTokenAndMovePosition(parsedToken, currentChar);
            }

            return isArithmeticOperator;
        }

        private char GetNextCharacter(int successorNumber)
        {
            char nextCharacter = '0';
            int inputLength = _text.Length;
            int successorIndex = _curPos + successorNumber;
            if (successorIndex < inputLength)
            {
                nextCharacter = _text[successorIndex];
            }
            return nextCharacter;
        }

        private bool EqualsToOperator(string operatorToCompareWith, string extractedInput)
        {
            return operatorToCompareWith.ToUpper().Equals(extractedInput.ToUpper());
        }

        private bool CheckIfNextTokenIsNumber()
        {
            bool isNumber = IsDigit();
            StringBuilder textBuilder = new StringBuilder();

            if (isNumber)
            {
                while (IsDigit())
                {
                    textBuilder.Append(_curChar.ToString());
                    this.Advance();
                }

                if (this._curChar == '.')
                {
                    textBuilder.Append(_curChar.ToString());
                    this.Advance();

                    if (IsDigit())
                    {
                        while (IsDigit())
                        {
                            textBuilder.Append(_curChar.ToString());
                            this.Advance();
                        }
                    }
                    else
                    {
                        ThrowUnexpectedSymbolError();
                    }
                }

                string num = textBuilder.ToString();
                this._curToken = new ExpressionToken(ExpressionTokenType.Number, num);
            }

            return isNumber;
        }

        private bool CheckIfNextTokenIsVariable()
        {
            bool isVariable = IsCharacter();
            StringBuilder textBuilder = new StringBuilder();

            if (isVariable)
            {
                while (IsCharacter())
                {
                    textBuilder.Append(_curChar.ToString());
                    this.Advance();
                }

                this._curToken = new ExpressionToken(ExpressionTokenType.Variable, textBuilder.ToString());
            }

            return isVariable;
        }

        private bool CheckIfNextTokenIsQuote()
        {
            bool isQoute = IsQuote();

            if (isQoute)
            {
                SetCurrentTokenAndMovePosition(ExpressionTokenType.Quote, this._curChar.ToString());
            }

            return isQoute;
        }

        private bool CheckIfNextTokenIsStringLiteral()
        {
            bool isStringLiteral = IsCurrentTokenTypeEqualTo(ExpressionTokenType.Quote);
            StringBuilder textBuilder = new StringBuilder();

            if (isStringLiteral)
            {
                while (!IsQuote() && !ReachEnd())
                {
                    textBuilder.Append(this._curChar.ToString());
                    this.Advance();
                }

                string stringLiteral = textBuilder.ToString();
                this._curToken = new ExpressionToken(ExpressionTokenType.StringLiteral, stringLiteral);
            }

            return isStringLiteral;
        }
        #endregion /Next Token Methods

        #region Common Methods
        private void SetInitialPropertiesToDefault()
        {
            this._variablesFromFormula = new List<string>();
            this._curToken = ExpressionToken.None();
            this._curPos = -1;
            this.Advance();

            InitializeOperatorsLists();
        }

        private void InitializeOperatorsLists()
        {
            this._comparisonOperators = new Dictionary<string, ExpressionTokenType>()
            {
                {"==", ExpressionTokenType.Equal },
                {"!=", ExpressionTokenType.NotEqual },
                {"<", ExpressionTokenType.Less },
                {"<=", ExpressionTokenType.LessOrEqual },
                {">", ExpressionTokenType.Great },
                { ">=", ExpressionTokenType.GreatOrEqual }
            };

            this._arithmeticOperators = new Dictionary<string, ExpressionTokenType>()
            {
                {"+", ExpressionTokenType.Plus },
                {"-", ExpressionTokenType.Minus },
                {"*", ExpressionTokenType.Multiply },
                {"/", ExpressionTokenType.Divide }
            };
        }

        private ExpressionToken ExpectToken(ExpressionTokenType tokenType)
        {

            if (IsCurrentTokenTypeEqualTo(tokenType))
            {
                return this._curToken;
            }
            else
            {
                ThrowUnexpectedTokenError(tokenType.ToString());
                return null;
            }
        }

        private bool IsCurrentTokenTypeEqualTo(ExpressionTokenType tokenType)
        {
            return this._curToken.Type == tokenType;
        }

        private bool CurrentTokenIsComparisonOperator()
        {
            return this._comparisonOperators.Values.ToList().Contains(this._curToken.Type);
        }

        private void Advance(int step = 1)
        {
            this._curPos += step;

            if (this._curPos < this._charCount)
            {
                this._curChar = this._text[this._curPos];
            }
            else
            {
                this._curChar = char.MinValue;
            }
        }

        private void SetCurrentTokenAndMovePosition(ExpressionTokenType expressionTokenType, string extractedToken)
        {
            this._curToken = new ExpressionToken(expressionTokenType, extractedToken);
            this.Advance(extractedToken.Length);
        }

        private bool IsDigit()
        {
            return this._curChar >= '0' && this._curChar <= '9';
        }
        private bool IsCharacter()
        {
            return char.IsLetter(this._curChar);
        }

        private bool IsQuote()
        {
            return this._curChar == '\'' || this._curChar == '"';
        }

        private bool ReachEnd()
        {
            return this._curChar == char.MinValue;
        }

        private bool IsBlankCharacter()
        {
            return this._curChar == ' ';
        }

        private void ThrowUnexpectedSymbolError()
        {
            throw new InvalidFormulaException(string.Format("Invalid syntax at position {0}. Unexpected symbol {1}.", this._curPos, this._curChar));
        }

        private void ThrowUnexpectedTokenError(string exptectedTokenType)
        {
            throw new InvalidFormulaException(string.Format("Invalid syntax at position {0}. Expected {1} but {2} is given.", this._curPos, exptectedTokenType, this._curToken.Type));
        }

        private void ValidateArithmeticOperation(LogicalExpression left, ExpressionToken op, LogicalExpression right)
        {
            if (left is StringLiteralOperand || right is StringLiteralOperand)
            {
                ThrowInvalidOperationException(left, op, right, false);
            }

            if (left is VariableOperand leftVariableOperand && right is VariableOperand rightVariableOperand)
            {
                DependentOnFieldInfoDataIn dependentOnFieldInfoLeft = this._dependentOnInfo.GetDependentOnFieldInfoByVariable(leftVariableOperand.ExpressionToken.Value);
                DependentOnFieldInfoDataIn dependentOnFieldInfoRigth = this._dependentOnInfo.GetDependentOnFieldInfoByVariable(rightVariableOperand.ExpressionToken.Value);
                if (dependentOnFieldInfoLeft != null && !dependentOnFieldInfoLeft.IsNumeric() && dependentOnFieldInfoRigth != null && !dependentOnFieldInfoRigth.IsNumeric())
                {
                    ThrowInvalidOperationException(left, op, right, false, dependentOnFieldInfoLeft.FieldType);
                }
            }
        }

        private void ValidateComparisonOperation(LogicalExpression left, ExpressionToken op, LogicalExpression right)
        {
            if (left is StringLiteralOperand)
            {
                ValidateStringInComparisonOperation(left, op, right, false);
            }
            else if (left is NumericOperand)
            {
                ValidateNumericInComparisonOperation(left, op, right, false);
            }
            else if (left is BinaryComparisonOperation binaryComparisonOperationLeft)
            {
                ThrowInvalidNestingOperation(binaryComparisonOperationLeft, op, false);
            }
            else if (right is StringLiteralOperand)
            {
                ValidateStringInComparisonOperation(right, op, left, true);
            }
            else if (right is NumericOperand)
            {
                ValidateNumericInComparisonOperation(right, op, left, true);
            }
            else if (right is BinaryComparisonOperation binaryComparisonOperationRigth)
            {
                ThrowInvalidNestingOperation(binaryComparisonOperationRigth, op, true);
            }
        }

        private void ValidateNumericInComparisonOperation(LogicalExpression firstNumericOperand, ExpressionToken op, LogicalExpression secondOperand, bool reverseOperands)
        {
            if (secondOperand is VariableOperand variableOperand)
            {
                DependentOnFieldInfoDataIn dependentOnFieldInfo = this._dependentOnInfo.GetDependentOnFieldInfoByVariable(variableOperand.ExpressionToken.Value);
                if (dependentOnFieldInfo != null && !dependentOnFieldInfo.IsNumeric())
                {
                    ThrowInvalidOperationException(firstNumericOperand, op, secondOperand, reverseOperands, dependentOnFieldInfo.FieldType);
                }
            }
        }

        private void ValidateStringInComparisonOperation(LogicalExpression firstStringOperand, ExpressionToken op, LogicalExpression secondOperand, bool reverseOperands)
        {
            if (secondOperand is BinaryArithmeticOperation || secondOperand is UnaryOperation)
            {
                ThrowInvalidOperationException(firstStringOperand, op, secondOperand, reverseOperands);
            }
            else if (secondOperand is VariableOperand variableOperand)
            {
                DependentOnFieldInfoDataIn dependentOnFieldInfo = this._dependentOnInfo.GetDependentOnFieldInfoByVariable(variableOperand.ExpressionToken.Value);
                if (dependentOnFieldInfo != null && dependentOnFieldInfo.IsNumeric())
                {
                    ThrowInvalidOperationException(firstStringOperand, op, secondOperand, reverseOperands, dependentOnFieldInfo.FieldType);
                }
            }
            else if (secondOperand is NumericOperand)
            {
                ThrowInvalidOperationException(firstStringOperand, op, secondOperand, reverseOperands);
            }
        }

        private void ThrowInvalidOperationException(LogicalExpression firstOperand, ExpressionToken op, LogicalExpression secondOperand, bool reverseOperands, string fieldType = null)
        {
            string fieldTypeVariableSuffix = fieldType != null ? $"[{fieldType}]" : "";
            throw new InvalidFormulaException($"Invalid operands at posiiton {this._curPos}. Operation: {GetOperation(firstOperand.PrintExpression(), op.ToString(), secondOperand.PrintExpression(), reverseOperands)}, explanation: type dismatch for {GetOperation(firstOperand.GetName(), op.ToString(), $"{secondOperand.GetName()}{fieldTypeVariableSuffix}", reverseOperands)}");
        }

        private void ThrowInvalidNestingOperation(BinaryComparisonOperation firstOperand, ExpressionToken op, bool reverseOperands)
        {
            throw new InvalidFormulaException($"Invalid operands at posiiton {this._curPos}. Comparison operations cannot be nesting, first {GetOperation(firstOperand.Operator.Value, "then", op.Value, reverseOperands)}");
        }

        private string GetOperation(object firstOperand, string op, object secondOperand, bool reverseOperands)
        {
            return reverseOperands ? $"{secondOperand} {op} {firstOperand}" : $"{firstOperand} {op} {secondOperand}";
        }

        private void AddVariable(string variable)
        {
            if (!_variablesFromFormula.Contains(variable))
            {
                _variablesFromFormula.Add(variable);
            }
        }
        #endregion /Common Methods
    }

    #region Exceptions
    [Serializable]
    public class InvalidFormulaException : Exception
    {
        public InvalidFormulaException()
        {
        }

        public InvalidFormulaException(string message) : base(message)
        {
        }
    }
    #endregion /Exceptions
}
