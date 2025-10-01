using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.Distribution;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.DTOs.Autocomplete.DataOut;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.FormDistribution.DataIn;
using sReportsV2.DTOs.FormDistribution.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.User.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class FormDistributionBLL : IFormDistributionBLL
    {
        private readonly IFormDistributionDAL formDistributionDAL;
        private readonly IFormDAL formDAL;
        private readonly IMapper mapper;

        public FormDistributionBLL(IFormDistributionDAL formDistributionDAL, IFormDAL formDAL, IMapper mapper)
        {
            this.formDistributionDAL = formDistributionDAL;
            this.formDAL = formDAL;
            this.mapper = mapper;
        }

        public FormDistributionDataOut GetById(string id)
        {
            return mapper.Map<FormDistributionDataOut>(formDistributionDAL.GetById(id));
        }

        public PaginationDataOut<FormDistributionTableDataOut, FormDistributionFilterDataIn> GetAll(FormDistributionFilterDataIn dataIn)
        {
            return new PaginationDataOut<FormDistributionTableDataOut, FormDistributionFilterDataIn>()
            {
                Count = formDistributionDAL.GetAllCount(),
                Data = mapper.Map<List<FormDistributionTableDataOut>>(formDistributionDAL.GetAll(new Common.Entities.EntityFilter { Page = dataIn.Page, PageSize = dataIn.PageSize}))
            };
        }

        public FormDistributionParameterizationDataOut GetFormDistributionForParameterization(int thesaurusId, string versionId)
        {
            Form form = formDAL.GetFormByThesaurusAndVersion(thesaurusId, versionId);
            FormDistribution formDistribution = CreateOrUpdate(form);

            FormDistributionDataOut dataOut = mapper.Map<FormDistributionDataOut>(formDistribution);
            SetRelatedFieldsLabels(dataOut, form);
            
            return new FormDistributionParameterizationDataOut() 
            {
                Form = mapper.Map<FormDataOut>(form),
                FormDistribution = dataOut
            };
        }

        public void SetParameters(FormDistributionDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            FormDistribution formDistribution;
            if (!string.IsNullOrEmpty(dataIn.FormDistributionId))
            {
                formDistribution = formDistributionDAL.GetById(dataIn.FormDistributionId);
            }
            else
            {
                Form form = formDAL.GetFormByThesaurusAndVersion(dataIn.ThesaurusId, dataIn.VersionId);
                formDistribution = GetFromForm(form);
            }
            UpdateField(dataIn, formDistribution);
            formDistributionDAL.InsertOrUpdate(formDistribution);            
        }

        public FormFieldDistributionDataOut GetFormFieldDistribution(string formDistributionId, string fieldId)
        {
            return mapper.Map<FormFieldDistributionDataOut>(formDistributionDAL.GetFormFieldDistribution(formDistributionId, fieldId));
        }

        public RelationFieldAutocompleteResultDataOut GetRelationFieldAutocomplete(AutocompleteDataIn dataIn, string formDistributionId)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            FormDistributionDataOut formDistribution = mapper.Map<FormDistributionDataOut>(formDistributionDAL.GetById(formDistributionId));
            FormFieldDistributionDataOut targetField = formDistribution.GetFieldById(dataIn.ExcludeId);
            IEnumerable<FormFieldDistributionDataOut> filteredFields = FilterFieldsByLabel(formDistribution, dataIn.Term, CreateFieldExclusionList(formDistribution, targetField));
            List<RelationFieldAutocompleteDataOut>  formFieldDistributionDataOuts = FormatFieldsForDisplay(filteredFields, dataIn);

            RelationFieldAutocompleteResultDataOut result = new RelationFieldAutocompleteResultDataOut()
            {
                pagination = new AutocompletePaginatioDataOut()
                {
                    more = dataIn.ShouldLoadMore(filteredFields.Count())
                },
                results = formFieldDistributionDataOuts
            };

            return result;
        }

        public FormFieldDistributionDataOut ResetAllRelationsForField(string formDistributionId, string formFieldDistributionId, UserCookieData userCookieData)
        {
            FormDistribution formDistribution = formDistributionDAL.GetById(formDistributionId);
            Form form = formDAL.GetFormByThesaurusAndLanguageAndVersionAndOrganization(formDistribution.ThesaurusId, userCookieData.ActiveOrganization, userCookieData.ActiveLanguage, formDistribution.VersionId);
            Field fieldDefinition = form.GetFieldById(formFieldDistributionId);
            FormFieldDistribution blankFieldDistribution = GetDistributionField(fieldDefinition);

            FormFieldDistribution fieldDb = formDistribution.Fields.Find(x => x.Id.Equals(formFieldDistributionId));
            if (fieldDb != null)
            {
                formDistribution.Fields[formDistribution.Fields.IndexOf(fieldDb)] = blankFieldDistribution;
            }
            formDistributionDAL.InsertOrUpdate(formDistribution);

            return mapper.Map<FormFieldDistributionDataOut>(blankFieldDistribution);
        }

        private IEnumerable<FormFieldDistributionDataOut> FilterFieldsByLabel(FormDistributionDataOut formdistribution, string labelName, List<string> exclusionList)
        {
            return formdistribution.Fields.Where(x => x.CanBeAddedToRelation() && !exclusionList.Contains(x.Id) && x.Label.ToLower().Contains(labelName.ToLower()));
        }

        private List<string> CreateFieldExclusionList(FormDistributionDataOut formDistribution, FormFieldDistributionDataOut targetField)
        {
            List<string> exclusionList = targetField.GetRelatedFieldsIds();
            exclusionList.AddRange(GetFieldIdsWhichDependOnTargetField(formDistribution, targetField.Id));
            exclusionList.Add(targetField.Id);
            return exclusionList;
        }

        private IEnumerable<string> GetFieldIdsWhichDependOnTargetField(FormDistributionDataOut formDistribution, string targetFieldId)
        {
            return formDistribution.Fields.Where(x => x.RelatedVariables.Exists(y => y.Id == targetFieldId) && x.Id != targetFieldId).Select(x => x.Id);
        }

        private List<RelationFieldAutocompleteDataOut> FormatFieldsForDisplay(IEnumerable<FormFieldDistributionDataOut> filteredFields, AutocompleteDataIn dataIn)
        {
            return filteredFields
                .OrderBy(x => x.Label)
                .Skip(dataIn.Page * FilterConstants.DefaultPageSize)
                .Take(FilterConstants.DefaultPageSize)
                .Select(x => new RelationFieldAutocompleteDataOut()
                {
                    id = x.Id.ToString(),
                    text = x.Label,
                    type = x.Type
                })
                .ToList();
        }


        private void UpdateField(FormDistributionDataIn dataIn, FormDistribution formDistribution)
        {
            FormFieldDistributionDataIn fieldDataIn = dataIn.Fields.FirstOrDefault();
            if (fieldDataIn != null)
            {
                FormFieldDistribution fieldDb = formDistribution.Fields.Find(x => x.Id.Equals(fieldDataIn.Id));
                if (fieldDb != null)
                {
                    formDistribution.Fields[formDistribution.Fields.IndexOf(fieldDb)] = mapper.Map<FormFieldDistribution>(fieldDataIn);
                }
            }
        }

        private FormDistribution CreateOrUpdate(Form form)
        {
            FormDistribution formDistribution = GetFromForm(form);
            formDistributionDAL.InsertOrUpdate(formDistribution);

            return formDistribution;
        }

        private void SetRelatedFieldsLabels(FormDistributionDataOut formDistributionDataOut, Form form)
        {
            List<Field> fields = form.GetAllFields();
            foreach (var field in formDistributionDataOut.Fields)
            {
                foreach (var rel in field.RelatedVariables)
                {
                    rel.Label = fields.Find(x => x.Id == rel.Id).Label;
                }
            }
        }

        private FormDistribution GetFromForm(Form form)
        {
            FormDistribution existingDistribution = GetByThesaurusIdAndVersion(form.ThesaurusId, form.Version.Id);
            List<Field> currentFormFields = form.GetAllFields().Where(x => x.IsDistributiveField()).ToList();

            if (existingDistribution == null)
            {
                return new FormDistribution()
                {
                    EntryDatetime = form.EntryDatetime,
                    ThesaurusId = form.ThesaurusId,
                    Title = form.Title,
                    Fields = GetDistributionFields(currentFormFields),
                    VersionId = form.Version.Id
                };
            }

            UpdateDistributionFields(existingDistribution, currentFormFields);

            return existingDistribution;
        }

        private void UpdateDistributionFields(FormDistribution distribution, List<Field> currentFormFields)
        {
            var currentFieldIds = currentFormFields.Select(f => f.Id).ToHashSet();
            var removedFieldIds = distribution.Fields
                .Where(fd => !currentFieldIds.Contains(fd.Id))
                .Select(fd => fd.Id)
                .ToHashSet();

            distribution.Fields.RemoveAll(fd => removedFieldIds.Contains(fd.Id));

            foreach (var field in distribution.Fields)
            {
                int removedCount = field.RelatedVariables.RemoveAll(rv => removedFieldIds.Contains(rv.Id));

                if (removedCount > 0)
                {
                    var matchingFormField = currentFormFields.FirstOrDefault(f => f.Id == field.Id);

                    field.ValuesAll = new List<FormFieldDistributionSingleParameter>()
                     {
                         new FormFieldDistributionSingleParameter()
                         {
                             NormalDistributionParameters = new FormFieldNormalDistributionParameters(),
                             Values = matchingFormField is FieldSelectable fieldSelectable ? fieldSelectable.Values.Select(x => new FormFieldValueDistribution()
                             {
                                 Label = x.Label,
                                 ThesaurusId = x.ThesaurusId,
                                 Value = x.Value
                             }).ToList() : null
                         }
                     };
                }
            }

            foreach (var formField in currentFormFields)
            {
                var existingField = distribution.Fields.FirstOrDefault(f => f.Id == formField.Id);

                if (existingField == null)
                {
                    distribution.Fields.Add(GetDistributionField(formField));
                    continue;
                }

                UpdateFieldIfChanged(distribution, formField, existingField);
            }
        }

        private void UpdateFieldIfChanged(FormDistribution distribution, Field formField, FormFieldDistribution existingField)
        {
            bool isSelectable = formField.Type == "radio" || formField.Type == "checkbox";

            if (isSelectable && formField is FieldSelectable selectableField)
            {
                var newValues = selectableField.Values ?? new List<FormFieldValue>();
                var existingParam = existingField.ValuesAll?.FirstOrDefault();
                var existingValues = existingParam?.Values ?? new List<FormFieldValueDistribution>();

                bool valuesDiffer = newValues.Count != existingValues.Count ||
                    !newValues.All(nv => existingValues.Any(ev =>
                        ev.Label == nv.Label &&
                        ev.Value == nv.Value &&
                        ev.ThesaurusId == nv.ThesaurusId
                    ));

                if (valuesDiffer)
                {
                    distribution.Fields.Remove(existingField);
                    distribution.Fields.Add(GetDistributionField(formField));
                    return;
                }
            }

            existingField.Label = formField.Label;
            existingField.Type = formField.Type;
            existingField.ThesaurusId = formField.ThesaurusId;
        }

        private List<FormFieldDistribution> GetDistributionFields(List<Field> fields)
        {
            List<FormFieldDistribution> result = new List<FormFieldDistribution>();
            foreach (Field field in fields)
            {
                FormFieldDistribution fieldDistribution = GetDistributionField(field);
                result.Add(fieldDistribution);
            }

            return result;
        }

        private FormFieldDistribution GetDistributionField(Field field)
        {
            FormFieldDistribution fieldDistribution = new FormFieldDistribution()
            {
                Id = field.Id,
                Label = field.Label,
                RelatedVariables = new List<Domain.Entities.Distribution.RelatedVariable>(),
                ThesaurusId = field.ThesaurusId,
                Type = field.Type,
                ValuesAll = new List<FormFieldDistributionSingleParameter>()
                 {
                     new FormFieldDistributionSingleParameter()
                     {
                         NormalDistributionParameters = new FormFieldNormalDistributionParameters(),
                         Values = field is FieldSelectable fieldSelectable ? fieldSelectable.Values.Select(x => new FormFieldValueDistribution()
                         {
                             Label = x.Label,
                             ThesaurusId = x.ThesaurusId,
                             Value = x.Value
                         }).ToList() : null
                     }
                 }
            };

            return fieldDistribution;
        }

        public Field GetFormField(int thesaurusId, string versionId, UserCookieData userCookieData, string fieldId)
        {
            Form form = formDAL.GetFormByThesaurusAndLanguageAndVersionAndOrganization(thesaurusId, userCookieData.ActiveOrganization, userCookieData.ActiveLanguage, versionId);
            return form.GetFieldById(fieldId);
        }

        public FormDistribution GetByThesaurusIdAndVersion(int id, string versionId)
        {
            return formDistributionDAL.GetByThesaurusIdAndVersion(id, versionId);
        }

        public FormDistribution GetByThesaurusId(int id)
        {
            return formDistributionDAL.GetByThesaurusId(id);
        }

        public List<FormDistribution> GetAllVersionAndThesaurus()
        {
            return formDistributionDAL.GetAllVersionAndThesaurus();
        }
    }
}
