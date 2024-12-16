using MathNet.Numerics.Distributions;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.Distribution;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using sReportsV2.Common.Extensions;

namespace DocumentGenerator
{
    public static class FormInstanceGenerator
    {
        public static List<FormInstance> Generate(Form form, FormDistribution formDistribution, int numOfDocuments)
        {
            Dictionary<string, List<GeneratedFieldInstanceValue>> generatedFieldInstanceValues = SetNonDependableFields(formDistribution, numOfDocuments);
            List<FormInstance> generatedFormInstances = GenerateFormInstances(form, generatedFieldInstanceValues, numOfDocuments);

            SetDependableFields(form, formDistribution, generatedFormInstances);
            SetValuesForCalculativeField(form, generatedFormInstances);
            return generatedFormInstances;
        }

        public static List<FormInstance> GenerateDailyForms(List<FormInstance> whoDocuments, Form form, FormDistribution formDistribution)
        {
            List<int> daysDistribution = GenerateDailyFormNumExamples(whoDocuments.Count);
            int dailyNumOfDocuments = daysDistribution.Sum();
            int skip = 0;
            List<FormInstance> dailyDocumentsGenerated = Generate(form, formDistribution, dailyNumOfDocuments);
            for (int i = 0; i < daysDistribution.Count; i++)
            {
                List<FormInstance> dailyDocuments = dailyDocumentsGenerated.Skip(skip).Take(daysDistribution[i]).ToList();
                skip += daysDistribution[i];

                FormInstance whoDocument = whoDocuments[i];
                FieldInstance patientIdField = whoDocument.FieldInstances.Find(x => x.ThesaurusId == 15114);
                SetDailyFormsPatienId(dailyDocuments, patientIdField?.FieldInstanceValues?.GetFirstValue());
            }
            return dailyDocumentsGenerated;
        }

        #region Non-Dependent Fields

        private static Dictionary<string, List<GeneratedFieldInstanceValue>> SetNonDependableFields(FormDistribution formDistribution, int numOfDocuments)
        {
            Dictionary<string, List<GeneratedFieldInstanceValue>> generatedFieldInstanceValues = new Dictionary<string, List<GeneratedFieldInstanceValue>>();
            if (formDistribution.Fields != null)
            {
                foreach (FormFieldDistribution field in GetNonDependableFields(formDistribution))
                {
                    switch (field.Type)
                    {
                        case FieldTypes.Radio:
                        case FieldTypes.Select:
                            generatedFieldInstanceValues.Add(field.Id, GenerateRadioOrSelectExamples(field.ValuesAll[0].Values, numOfDocuments));
                            break;
                        case FieldTypes.Number:
                            generatedFieldInstanceValues.Add(field.Id, GenerateNumberExamples(field.ValuesAll[0].NormalDistributionParameters, numOfDocuments));
                            break;

                        case FieldTypes.Checkbox:
                            generatedFieldInstanceValues.Add(field.Id, GenerateCheckboxExamples(field.ValuesAll[0].Values, numOfDocuments));
                            break;
                    }
                }

            }

            return generatedFieldInstanceValues;
        }

        private static IEnumerable<FormFieldDistribution> GetNonDependableFields(FormDistribution formDistribution)
        {
            return formDistribution.Fields.Where(x => x.ValuesAll.Exists(y => y.DependOn == null || !y.DependOn.Any()));
        }

        #endregion /Non-Dependent Fields

        #region Generate Form Instance Distribution Values
        private static List<GeneratedFieldInstanceValue> GenerateRadioOrSelectExamples(List<FormFieldValueDistribution> fieldValues, int numberOfDocuments)
        {
            List<GeneratedFieldInstanceValue> result = new List<GeneratedFieldInstanceValue>();
            if (fieldValues.Sum(x => x.SuccessProbability) == 1)
            {
                Multinomial multinomial = new Multinomial(GetMultinominalWeights(fieldValues), numberOfDocuments);
                var samples = multinomial.Samples().Take(1).ToList();
                for (int i = 0; i < samples[0].Count(); i++)
                {
                    string value = fieldValues[i].ThesaurusId.ToString();
                    if (samples[0][i] > 0)
                    {
                        for (int j = 0; j < samples[0][i]; j++)
                        {
                            result.Add(new GeneratedFieldInstanceValue(new List<string> { value }));
                        }
                    }
                }
            }
            else
            {
                result = GenerateEmptyValues(numberOfDocuments);
            }

            return result;
        }

        private static double[] GetMultinominalWeights(List<FormFieldValueDistribution> values)
        {
            if (values == null) 
                throw new ArgumentNullException(nameof(values), "Values are not defined");
            if (values.Exists(x => x.SuccessProbability == null))
                throw new ArgumentException("The SuccessProbability property in one or more items cannot be null.", nameof(values));

            return values.Select(x => Convert.ToDouble(x.SuccessProbability)).ToArray();
        }

        private static List<GeneratedFieldInstanceValue> GenerateEmptyValues(int numOfRepeats)
        {
            List<GeneratedFieldInstanceValue> result = new List<GeneratedFieldInstanceValue>();
            for (int i = 0; i < numOfRepeats; i++)
            {
                result.Add(new GeneratedFieldInstanceValue(new List<string> { string.Empty }));
            }
            return result;
        }

        private static List<GeneratedFieldInstanceValue> GenerateNumberExamples(FormFieldNormalDistributionParameters parameters, int numberOfDocuments)
        {
            List<GeneratedFieldInstanceValue> result;
            double mean = parameters.Mean;
            double standardDeviation = parameters.Deviation;

            var samples = new double[numberOfDocuments];
            Normal.Samples(samples, mean, standardDeviation);
            result = samples.Select(x =>
                new GeneratedFieldInstanceValue(
                    new List<string> {
                        x < 0 ? "0": ((float)x).ToString()
                    }
                )
            ).ToList();
            return result;
        }

        private static List<GeneratedFieldInstanceValue> GenerateCheckboxExamples(List<FormFieldValueDistribution> fieldValues, int numberOfDocuments)
        {
            Dictionary<string, List<string>> values = new Dictionary<string, List<string>>();
            foreach (FormFieldValueDistribution value in fieldValues)
            {
                if (value.SuccessProbability == null)
                    throw new ArgumentException("The SuccessProbability property in one or more items cannot be null.", nameof(fieldValues));

                int[] samples = new int[1];
                Binomial.Samples(samples, value.SuccessProbability ?? 0, numberOfDocuments);
                string thesaurusId = value.ThesaurusId.ToString();
                for (int i = 0; i < numberOfDocuments; i++)
                {
                    if (!values.ContainsKey(thesaurusId))
                    {
                        values[thesaurusId] = new List<string>();
                    }
                    if (values[thesaurusId].Count < samples[0])
                    {
                        values[thesaurusId].Add(thesaurusId);
                    }
                    else
                    {
                        values[thesaurusId].Add(string.Empty);
                    }
                }
            }
            List<GeneratedFieldInstanceValue> result = GetCheckboxValuesJoined(values, numberOfDocuments);
            return result;
        }

        private static List<GeneratedFieldInstanceValue> GetCheckboxValuesJoined(Dictionary<string, List<string>> values, int numberOfDocuments)
        {
            List<GeneratedFieldInstanceValue> result = new List<GeneratedFieldInstanceValue>();
            for (int i = 0; i < numberOfDocuments; i++)
            {
                List<string> valueList = new List<string>();
                foreach (string key in values.Keys)
                {
                    valueList.Add(values[key][i]);
                }
                result.Add(new GeneratedFieldInstanceValue(valueList));
            }

            return result;
        }

        #endregion /Generate Form Instance Distribution Values

        #region Generate Form Instances

        private static List<FormInstance> GenerateFormInstances(Form form, Dictionary<string, List<GeneratedFieldInstanceValue>> generatedFieldInstanceValues, int numOfDocuments)
        {
            List<FormInstance> generatedFormInstances = new List<FormInstance>();
            for (int i = 0; i < numOfDocuments; i++)
            {
                generatedFormInstances.Add(GenerateFormInstance(form, generatedFieldInstanceValues, i));
            }
            return generatedFormInstances;
        }

        private static FormInstance GenerateFormInstance(Form form, Dictionary<string, List<GeneratedFieldInstanceValue>> generatedFieldInstanceValues, int generatedInstanceIndexNumber)
        {
            FormInstance formInstance = new FormInstance(form.Clone())
            {
                FieldInstances = new List<FieldInstance>()
            };

            foreach (FieldSet fieldSet in form.GetAllFieldSets())
            {
                string fieldSetInstanceRepetitionId = GuidExtension.NewGuidStringWithoutDashes();
                foreach (Field field in fieldSet.Fields)
                {
                    formInstance.FieldInstances.Add(new FieldInstance(
                        field, 
                        fieldSet.Id, 
                        fieldSetInstanceRepetitionId, 
                        GetFieldInstanceValue(field, generatedInstanceIndexNumber, generatedFieldInstanceValues)
                        )
                    );
                }
            }

            //the line of code is for covic who form, not will be used in other cases
            //TO DO implement new field type guid
            SetPatientIdentification(formInstance);

            return formInstance;
        }

        private static FieldInstanceValue GetFieldInstanceValue(Field field, int generatedInstanceIndexNumber, Dictionary<string, List<GeneratedFieldInstanceValue>> generatedFieldInstanceValues)
        {
            FieldInstanceValue fieldInstanceValue = null;
            if (generatedFieldInstanceValues.ContainsKey(field.Id))
            {
                GeneratedFieldInstanceValue generatedFieldInstanceValue = generatedFieldInstanceValues[field.Id][generatedInstanceIndexNumber];
                fieldInstanceValue = field.CreateDistributedFieldInstanceValue(generatedFieldInstanceValue.Values);
            }
            return fieldInstanceValue;
        }

        private static void SetPatientIdentification(FormInstance formInstance)
        {
            if (formInstance.ThesaurusId == 14573)
            {
                FieldInstance patientIdField = formInstance.FieldInstances.Find(x => x.ThesaurusId == 15114);
                if (patientIdField != null)
                    patientIdField.FieldInstanceValues = new List<FieldInstanceValue>() { new FieldInstanceValue(value: Guid.NewGuid().ToString()) };
            }
        }
        
        #endregion /Generate Form Instance

        #region Generate Calculative Field Instances

        private static void SetValuesForCalculativeField(Form form, List<FormInstance> generatedFormInstances)
        {
            foreach (FormInstance formInstance in generatedFormInstances)
            {
                foreach (FieldInstance calculativeField in formInstance.FieldInstances.Where(f => f.Type.Equals(FieldTypes.Calculative)))
                {
                    FieldCalculative fieldDefinition = (FieldCalculative)form.GetFieldById(calculativeField.FieldId);
                    calculativeField.AddValue(fieldDefinition.GetCalculation(GetFieldValuesForFormula(fieldDefinition, formInstance)));
                }
            }
        }
        
        private static Dictionary<string, string> GetFieldValuesForFormula(FieldCalculative fieldCalculative, FormInstance formInstance)
        {
            Dictionary<string, string> fieldValuesForFormula = new Dictionary<string, string>();
            foreach (string fieldId in fieldCalculative.IdentifiersAndVariables.Keys)
            {
                string fieldValue = formInstance.GetFieldValueById(fieldId);
                fieldValuesForFormula.Add(fieldId, fieldValue);
            }

            return fieldValuesForFormula;
        }

        #endregion /Generate Calculative Field Instances

        #region Dependent fields
        private static void SetDependableFields(Form form, FormDistribution formDistribution, List<FormInstance> generatedFormInstances)
        {
            if (formDistribution.Fields != null)
            {
                foreach (FormFieldDistribution field in GetDependableFields(formDistribution))
                {
                    GenerateDependentField(
                        form,
                        field,
                        generatedFormInstances
                    );
                }
            }
        }

        private static IEnumerable<FormFieldDistribution> GetDependableFields(FormDistribution formDistribution)
        {
            return formDistribution.Fields.Where(x => x.ValuesAll.Exists(y => y.DependOn != null && y.DependOn.Any()));
        }

        private static void GenerateDependentField(Form form, FormFieldDistribution field, List<FormInstance> generatedFormInstances)
        {
            Field fieldDefinition = form.GetFieldById(field.Id);
            List<string> relatedVariableFieldIds = field.RelatedVariables.Select(x => x.Id).ToList();
            List<Field> relatedVariableFields = form.GetAllFields().Where(x => relatedVariableFieldIds.Contains(x.Id)).ToList();
            foreach (FormFieldDistributionSingleParameter fieldSingleValue in field.ValuesAll)
            {
                List<FormInstance> listToUpdateGenerate = GetDocumentsWithDependOnField(relatedVariableFields, field, generatedFormInstances, fieldSingleValue);

                if (listToUpdateGenerate != null)
                {
                    List<GeneratedFieldInstanceValue> examples = GenerateDependentExamples(fieldSingleValue, field, generatedFormInstances);
                    UpdateListWithDependables(fieldDefinition, listToUpdateGenerate, examples);
                }
            }
        }

        private static List<GeneratedFieldInstanceValue> GenerateDependentExamples(FormFieldDistributionSingleParameter fieldSingleValue, FormFieldDistribution field, List<FormInstance> formInstancesToUpdate)
        {
            switch (field.Type)
            {
                case FieldTypes.Radio:
                case FieldTypes.Select:
                    return GenerateRadioOrSelectExamples(fieldSingleValue.Values, formInstancesToUpdate.Count);
                case FieldTypes.Number:
                    return GenerateNumberExamples(fieldSingleValue.NormalDistributionParameters, formInstancesToUpdate.Count);
                case FieldTypes.Checkbox:
                    return GenerateCheckboxExamples(fieldSingleValue.Values, formInstancesToUpdate.Count);
                default:
                    throw new NotImplementedException($"{field.Label} cannot be generated because {field.Type} type is not supported");
            }
        }

        private static List<FormInstance> GetDocumentsWithDependOnField(List<Field> relatedVariableFields, FormFieldDistribution targetField, List<FormInstance> instancesToFilter, FormFieldDistributionSingleParameter singleValue)
        {
            List<FormInstance> result = instancesToFilter;
            if (relatedVariableFields.Count > 0 && instancesToFilter != null && singleValue.DependOn != null)
            {
                foreach (SingleDependOnValue dependOnValue in singleValue.DependOn)
                {
                    result = FilterForDocumentsWithDependOnFields(relatedVariableFields, targetField, result, dependOnValue);
                }
            }

            return result;
        }

        private static List<FormInstance> FilterForDocumentsWithDependOnFields(List<Field> relatedVariableFields, FormFieldDistribution targetField, List<FormInstance> instancesToFilter, SingleDependOnValue dependOnValue)
        {
            List<FormInstance> result = instancesToFilter;
            if (dependOnValue.Type == FieldTypes.Number)
            {
                result = FilterByNumericDependant(targetField, result, dependOnValue);
            }
            else
            {
                result = FilterBySelectableDependant(relatedVariableFields, instancesToFilter, dependOnValue);
            }
            return result;
        }

        private static List<FormInstance> FilterByNumericDependant(FormFieldDistribution targetField, List<FormInstance> instancesToFilter, SingleDependOnValue dependOnValue)
        {
            List<FormInstance> result = instancesToFilter;
            RelatedVariable relatedVariable = targetField.GetRelatedVariableById(dependOnValue.Id);

            switch (dependOnValue.Value)
            {
                case "LTE":
                    result = result.Where(x => 
                        x.FieldInstances.Exists(y => 
                            y.FieldId.Equals(dependOnValue.Id)
                            && y.FieldInstanceValues.HasAnyFieldInstanceValue()
                            && float.Parse(y.FieldInstanceValues.GetFirstValue()) <= relatedVariable.LowerBoundary)
                        ).ToList();
                    break;
                case "BTW":
                    result = result.Where(x => 
                        x.FieldInstances.Exists(y => 
                            y.FieldId.Equals(dependOnValue.Id)
                            && y.FieldInstanceValues.HasAnyFieldInstanceValue()
                            && float.Parse(y.FieldInstanceValues.GetFirstValue()) > relatedVariable.LowerBoundary
                            && float.Parse(y.FieldInstanceValues.GetFirstValue()) <= relatedVariable.UpperBoundary)).ToList();
                    break;
                case "GT":
                    result = result.Where(x => 
                        x.FieldInstances.Exists(y => 
                            y.FieldId.Equals(dependOnValue.Id) 
                            && y.FieldInstanceValues.HasAnyFieldInstanceValue()
                            && float.Parse(y.FieldInstanceValues.GetFirstValue()) > relatedVariable.UpperBoundary)
                        ).ToList();
                    break;
            }

            return result;
        }

        private static List<FormInstance> FilterBySelectableDependant(List<Field> relatedVariableFields, List<FormInstance> instancesToFilter, SingleDependOnValue dependOnValue)
        {
            Field targetFieldDefinition = relatedVariableFields.Find(f => f.Id == dependOnValue.Id);
            string selectedValueId = targetFieldDefinition?.GetDistributiveSelectedOptionId(dependOnValue.Value);
            return instancesToFilter.Where(x =>
                x.FieldInstances.Exists(y =>
                    y.FieldId.Equals(dependOnValue.Id)
                    && y.FieldInstanceValues.HasAnyFieldInstanceValue()
                    && y.FieldInstanceValues.SelectMany(z => z.Values).Contains(selectedValueId)
                    )
                ).ToList();
        }

        private static void UpdateListWithDependables(Field fieldDefinition, List<FormInstance> listToUpdateGenerate, List<GeneratedFieldInstanceValue> values)
        {
            for (int i = 0; i < listToUpdateGenerate.Count; i++)
            {
                FieldInstance fieldInstance = listToUpdateGenerate[i].FieldInstances.Find(x => x.FieldId.Equals(fieldDefinition.Id));
                if (fieldDefinition != null)
                {
                    GeneratedFieldInstanceValue generatedFieldInstanceValue = values[i];
                    if (fieldInstance != null)
                        fieldInstance.FieldInstanceValues = new List<FieldInstanceValue>() { fieldDefinition.CreateDistributedFieldInstanceValue(generatedFieldInstanceValue.Values) };
                }
            }
        }

        #endregion /Dependent fields

        #region Daily Form Instances
        private static List<int> GenerateDailyFormNumExamples(int numOfExamples)
        {
            Multinomial multinomial = new Multinomial(GetDailyFormWeights(), numOfExamples);
            var samples = multinomial.Samples().Take(1).ToList();
            List<int> result = new List<int>();
            for (int i = 0; i < samples[0].Count(); i++)
            {
                if (samples[0][i] > 0)
                {
                    for (int j = 0; j < samples[0][i]; j++)
                    {
                        result.Add(i+1);
                    }
                }
            }

            return result;
        }


        //weight for each day in 14 days period
        private static double[] GetDailyFormWeights()
        {
            return new double[] { 0, 0.01,0.01,0.01, 0.02, 0.03, 0.03, 0.08, 0.11, 0.17, 0.20, 0.18, 0.10,0.05 };
        }

        private static void SetDailyFormsPatienId(List<FormInstance> dailyForms, string patientId)
        {
            if (!string.IsNullOrWhiteSpace(patientId))
            {
                foreach (FormInstance formInstance in dailyForms)
                {
                    formInstance.SetValueByThesaurusId(15114, patientId);
                }
            }

        }

        #endregion /Daily Form Instances
    }
}
