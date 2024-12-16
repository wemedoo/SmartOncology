using AutoMapper;
using sReportsV2.BusinessLayer.Components.Implementations;
using sReportsV2.BusinessLayer.Helpers;
using sReportsV2.Cache.Singleton;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.DTOs.DTOs.FieldInstance.DataIn;
using sReportsV2.DTOs.DTOs.FieldInstance.DataOut;
using sReportsV2.DTOs.DTOs.FormInstance.DataIn;
using sReportsV2.DTOs.Field.DataIn;
using sReportsV2.DTOs.Field.DataOut;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.User.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace sReportsV2.BusinessLayer.Implementations
{
    public partial class FormBLL
    {
        public FieldInstanceDependenciesDataOut ExecuteDependenciesFormulas(FieldInstanceDependenciesDataIn dataIn)
        {
            FieldInstanceDependenciesDataOut result = new FieldInstanceDependenciesDataOut
            {
                DependenciesResult = new Dictionary<string, FieldInstanceDependencyDataOut>()
            };

            foreach (FieldInstanceDependencyDataIn dependency in dataIn.Dependencies)
            {
                if (!result.DependenciesResult.ContainsKey(dependency.ChildFieldInstanceRepetitionId))
                {
                    LogicalExpressionParser interpreter = new LogicalExpressionParser(dependency);
                    LogicalExpression node = interpreter.Parse();

                    //Debug.WriteLine("Formula: " + formula);
                    //Debug.WriteLine(string.Format("Tree graph:{0}{1}", Environment.NewLine, node.Accept(new LogicalExpressionVisualizer())));
                    //Debug.WriteLine("**********************************");

                    OperandBooleanValue dependencyFormulaResult = (OperandBooleanValue)node.Accept(new LogicalExpressionEvaluator(dependency));
                    result.DependenciesResult.Add(
                        dependency.ChildFieldInstanceRepetitionId,
                        new FieldInstanceDependencyDataOut(dependencyFormulaResult.Value, dependency.FieldActions)
                    );
                }
            }

            return result;
        }

        public FormDataOut SetFormDependablesAndReferrals(Form form, List<Form> referrals, UserCookieData userCookieData)
        {
            FormDataOut data = mapper.Map<FormDataOut>(form);
            SetDependables(data);
            if (referrals != null && referrals.Count > 0)
            {
                data.ReferrableFields = GetReferrableFields(form, referrals, userCookieData);
            }

            return data;
        }

        public bool ValidateFormula(DependentOnInfoDataIn dataIn)
        {
            ValidateBeforeParsing(dataIn);

            if (!string.IsNullOrEmpty(dataIn.Formula))
            {
                LogicalExpressionParser logicalExpressionParser = new LogicalExpressionParser(dataIn);
                logicalExpressionParser.Parse();
                logicalExpressionParser.CheckIfAllVariablesAreIncluded();
            }

            return true;
        }

        private void SetDependables(FormDataOut form)
        {
            int? missingCodeValueId = SingletonDataContainer.Instance.GetCodeId((int)CodeSetList.NullFlavor, ResourceTypes.NotApplicable);
            List<FieldDataOut> populatedFieldInstances = form.GetAllFields();
            FieldInstanceDependenciesDataIn fieldInstanceDependenciesDataIn = PrepareFieldInstanceDependenciesObject(form, populatedFieldInstances);

            FieldInstanceDependenciesDataOut fieldInstanceDependenciesData = ExecuteDependenciesFormulas(fieldInstanceDependenciesDataIn);
            foreach (var dependencyResult in fieldInstanceDependenciesData.DependenciesResult)
            {
                FieldDataOut childField = populatedFieldInstances.Find(fI => fI.FieldInstanceValues.Exists(fIV => fIV.FieldInstanceRepetitionId == dependencyResult.Key));
                childField?.HandleDependency(dependencyResult.Value.ExpressionResult, missingCodeValueId);
            }
        }

        private FieldInstanceDependenciesDataIn PrepareFieldInstanceDependenciesObject(FormDataOut form, List<FieldDataOut> populatedFieldInstances)
        {
            List<FieldInstanceDTO> populatedParentFieldInstances = form.CreateParentDependableStructure(populatedFieldInstances);

            FieldInstanceDependenciesDataIn fieldInstanceDependenciesDataIn = new FieldInstanceDependenciesDataIn
            {
                Dependencies = form
                    .ParentFieldInstanceDependencies.Values
                    .SelectMany(v => v)
                    .Select(x => new FieldInstanceDependencyDataIn
                    {
                        ChildFieldInstanceRepetitionId = x.ChildFieldInstanceRepetitionId,
                        ChildFieldSetInstanceRepetitionId = x.ChildFieldSetInstanceRepetitionId,
                        IsChildDependentFieldSetRepetitive = x.IsChildDependentFieldSetRepetitive,
                        Formula = x.Formula,
                        DependentOnFieldInfos = mapper.Map<List<DependentOnFieldInfoDataIn>>(x.DependentOnFieldInfos),
                        FieldInstancesInFormula = populatedParentFieldInstances
                    })
                    .ToList()
            };

            return fieldInstanceDependenciesDataIn;
        }

        private void ValidateBeforeParsing(DependentOnInfoDataIn dataIn)
        {
            List<string> formulaVariables = dataIn.GetFormulaVariables();
            if ((formulaVariables.Count > 0 && string.IsNullOrEmpty(dataIn.Formula)) || formulaVariables.Exists(v => string.IsNullOrEmpty(v)))
            {
                throw new InvalidFormulaException("Some of variables are not defined");
            }
            if (formulaVariables.Exists(v => !Regex.IsMatch(v, @"^[a-zA-Z]+$")))
            {
                throw new InvalidFormulaException("Some of variables are not valid. Only characters should be used");
            }
            if (formulaVariables.Count != formulaVariables.Distinct().Count())
            {
                throw new InvalidFormulaException("Some of variables are assigned to more than one field");
            }
        }
    }
}
