using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Helpers;
using sReportsV2.DAL.Sql.Implementations;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.FieldInstanceHistory;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Mongo;

namespace sReportsV2.Domain.DatabaseMigrationScripts
{
    public class M_202402141255_FieldInstanceValue_Modification : MongoMigration
    {
        private readonly IMongoCollection<FormInstance> Collection;
        private readonly IMongoCollection<Form> CollectionForm;
        private readonly IMongoCollection<FieldInstanceHistory> CollectionFieldInstance;
        private readonly IConfiguration configuration;
        private readonly SReportsContext dbContext;

        private readonly Dictionary<string, string> CheckboxIdsWhereOptionsHaveComma;
        private readonly Dictionary<string, string> FormInstanceDefinitions;

        public override int Version => 14;

        public M_202402141255_FieldInstanceValue_Modification(IConfiguration configuration, SReportsContext dbContext)
        {
            this.configuration = configuration;
            this.dbContext = dbContext;
            Collection = MongoDBInstance.Instance.GetDatabase().GetCollection<FormInstance>(MongoCollectionNames.FormInstance);
            CollectionForm = MongoDBInstance.Instance.GetDatabase().GetCollection<Form>(MongoCollectionNames.Form);
            CollectionFieldInstance = MongoDBInstance.Instance.GetDatabase().GetCollection<FieldInstanceHistory>(MongoCollectionNames.FieldInstanceHistory);

            FormInstanceDefinitions = new Dictionary<string, string>();
            CheckboxIdsWhereOptionsHaveComma = new Dictionary<string, string>
            {
                {"dea5d90e36c0478b9dc76c0032c212a2", string.Empty},
                {"9d4e596ee1af45709cd00d681a27c7c4", string.Empty},
                {"7cdfc801f86442c0a54cd624db1e055d", string.Empty},
                {"db7ad81407fc4f72b9e171bec778d44d", string.Empty}
            };
        }

        protected override void Up()
        {
            CodeDAL codeDAL = new CodeDAL(dbContext, configuration);
            int? notApplicableCodeId = codeDAL.GetByPreferredTerm("Not Applicable", (int)CodeSetList.NullFlavor)?.CodeId;
            Dictionary<string, Dictionary<string, FieldSelectable>> selectableFieldsInForms =
                CollectionForm
                    .AsQueryable()
                    .AsEnumerable()
                    .ToDictionary(
                        form => form.Id,
                        form => form.GetAllSelectableFields()
                            .ToDictionary(
                                formField => formField.Id,
                                formField => formField
                                )
                        )
                    ;

            AddMissingValuePropertyToFormsWhereMissing(notApplicableCodeId).Wait();
            MigrateFieldInstanceValuesToNewModel(selectableFieldsInForms, notApplicableCodeId).Wait();
            MigrateFieldInstanceHistoryValuesToNewModel(selectableFieldsInForms, notApplicableCodeId).Wait();
        }

        protected override void Down()
        {

        }

        private async Task AddMissingValuePropertyToFormsWhereMissing(int? notApplicableCodeId)
        {
            if (notApplicableCodeId.HasValue)
            {
                var filterDefinition = Builders<Form>
                    .Filter
                    .Exists(x => x.NullFlavors.First(), false)
                    ;

                var update = Builders<Form>
                    .Update
                    .Set(x => x.NullFlavors, new List<int> { notApplicableCodeId.Value });

                _ = await CollectionForm.UpdateManyAsync(filterDefinition, update).ConfigureAwait(false);
            }
        }

        private async Task MigrateFieldInstanceValuesToNewModel(Dictionary<string, Dictionary<string, FieldSelectable>> selectableFieldsInForms, int? notApplicableCodeId)
        {
            LogHelper.Info("MigrateFieldInstanceValuesToNewModel is started");
            
            try
            {
                var findOptions = new FindOptions<FormInstance, FormInstance>() {
                   BatchSize = 200
                };
                var allFormInstancesFilter = Builders<FormInstance>.Filter.Empty;
                
                var instancesToWrite = new List<WriteModel<FormInstance>>();
                
                using (var cursor = await Collection.FindAsync(allFormInstancesFilter, findOptions).ConfigureAwait(false))
                    {
                        while (await cursor.MoveNextAsync().ConfigureAwait(false))
                        {
                            var batch = cursor.Current;
                            foreach (FormInstance formInstance in batch)
                            {
                                if (selectableFieldsInForms.TryGetValue(
                                    formInstance.FormDefinitionId,
                                    out Dictionary<string, FieldSelectable> selectableFieldsInForm
                                    ))
                                {
                                    FormInstanceDefinitions.Add(formInstance.Id, formInstance.FormDefinitionId);
                                    ProcessFieldInstances(formInstance, selectableFieldsInForm, notApplicableCodeId);
                                    AddUpdateFieldInstanceToBatch(instancesToWrite, formInstance);
                                }
                            }

                            if (instancesToWrite.Any())
                            {
                                var result = await Collection.BulkWriteAsync(instancesToWrite).ConfigureAwait(false);
                                if (!result.IsAcknowledged)
                                    throw new InvalidOperationException($"BulkWriteAsync wrote {result.InsertedCount} items instead of {batch.Count()}");

                            }
                            instancesToWrite.Clear();
                        }
                    }

            }
            catch (Exception ex)
            {
                LogHelper.Error("Eror while MigrateFieldInstanceValuesToNewModel, error: " + ex.Message);
                throw;
            }
            LogHelper.Info("MigrateFieldInstanceValuesToNewModel is finished");
        }

        private async Task MigrateFieldInstanceHistoryValuesToNewModel(Dictionary<string, Dictionary<string, FieldSelectable>> selectableFieldsInForms, int? notApplicableCodeId)
        {
            LogHelper.Info("MigrateFieldInstanceHistoryValuesToNewModel is started");

            try
            {
                var findOptions = new FindOptions<FieldInstanceHistory, FieldInstanceHistory>()
                {
                    BatchSize = 200
                };
                var allFormInstancesWithValuesFilter = Builders<FieldInstanceHistory>.Filter.Ne(x => x.Value, null);

                var instancesToWrite = new List<WriteModel<FieldInstanceHistory>>();

                using (var cursor = await CollectionFieldInstance.FindAsync(allFormInstancesWithValuesFilter, findOptions).ConfigureAwait(false))
                {
                    while (await cursor.MoveNextAsync().ConfigureAwait(false))
                    {
                        var batch = cursor.Current;
                        foreach (FieldInstanceHistory fieldInstanceHistory in batch)
                        {
                            if (selectableFieldsInForms.TryGetValue(
                                GetFormDefinitionId(fieldInstanceHistory.FormInstanceId),
                                out Dictionary<string, FieldSelectable> selectableFieldsInForm
                                ))
                            {
                                ProcessFieldInstanceHistories(fieldInstanceHistory, selectableFieldsInForm, notApplicableCodeId);
                                AddUpdateFieldInstanceHistoryToBatch(instancesToWrite, fieldInstanceHistory);
                            }
                        }

                        if (instancesToWrite.Any())
                        {
                            var result = await CollectionFieldInstance.BulkWriteAsync(instancesToWrite).ConfigureAwait(false);
                            if (!result.IsAcknowledged)
                                throw new InvalidOperationException($"BulkWriteAsync wrote {result.InsertedCount} items instead of {batch.Count()}");

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error("Eror while MigrateFieldInstanceHistoryValuesToNewModel, error: " + ex.Message);
                throw;
            }
            LogHelper.Info("MigrateFieldInstanceHistoryValuesToNewModel is finished");
        }
        #region Helpers

        private void ProcessFieldInstances(FormInstance formInstance, Dictionary<string, FieldSelectable> selectableFieldsInForm, int? notApplicableCodeId)
        {
            foreach (FieldInstance fieldInstance in formInstance.FieldInstances)
            {
                foreach (FieldInstanceValue fieldInstanceValue in fieldInstance.FieldInstanceValues)
                {
                    fieldInstanceValue.Values = new List<string>();

                    if (!string.IsNullOrEmpty(fieldInstanceValue.Value))
                    {
                        if (HasPreviousSpecialValue(fieldInstanceValue.Value))
                        {
                            MapPreviousSpecialValueToNullFlavors(fieldInstanceValue, notApplicableCodeId);
                        }
                        else if(fieldInstance.Type.IsSelectableField())
                        {
                            if (selectableFieldsInForm.TryGetValue(fieldInstance.FieldId, out FieldSelectable fieldSelectable))
                            {
                                List<string> selectedValues = GetSelectedValues(
                                    fieldInstance.Type,
                                    fieldInstanceValue.Value,
                                    fieldInstanceValue.IsSpecialValue,
                                    fieldSelectable);

                                fieldInstanceValue.Values.AddRange(selectedValues);
                                fieldInstanceValue.ValueLabel = ModifySelectedValueLabels(selectedValues, fieldInstanceValue.IsSpecialValue, fieldSelectable);
                            }
                        }
                        else
                        {
                            fieldInstanceValue.Values.Add(fieldInstanceValue.Value);
                        }
                    }
                }
            }
        }

        private void AddUpdateFieldInstanceToBatch(List<WriteModel<FormInstance>> instancesToWrite, FormInstance formInstance)
        {
            var replaceFilter = Builders<FormInstance>.Filter.Eq(x => x.Id, formInstance.Id);
            var update = Builders<FormInstance>.Update.Set(x => x.FieldInstances, formInstance.FieldInstances);
            instancesToWrite.Add(new UpdateOneModel<FormInstance>(replaceFilter, update));
        }

        private void MapPreviousSpecialValueToNullFlavors(FieldInstanceValue fieldInstanceValue, int? notApplicableCodeId)
        {
            fieldInstanceValue.IsSpecialValue = true;
            if (notApplicableCodeId.HasValue)
            {
                fieldInstanceValue.Values.Add(notApplicableCodeId.ToString());
                fieldInstanceValue.ValueLabel = notApplicableCodeId.ToString();
            }
        }

        private string ModifySelectedValueLabels(List<string> selectedValues, bool isSpecialValue, FieldSelectable fieldSelectable)
        {
            List<string> modifiedSelectedValueLabels;
            if (isSpecialValue)
            {
                modifiedSelectedValueLabels = selectedValues;
            }
            else
            {
                modifiedSelectedValueLabels = selectedValues
                    .Select(selectedOptionId => GetOptionLabel(fieldSelectable, selectedOptionId))
                    .Where(selectedOptionLabel => !string.IsNullOrEmpty(selectedOptionLabel))
                    .ToList();
            }
            return string.Join(",", modifiedSelectedValueLabels);
        }

        private List<string> GetSelectedValues(string fieldInstanceType, string fieldInstanceValue, bool isSpecialValue, FieldSelectable fieldSelectable)
        {
            List<string> selectedValues = new List<string>();

            if (isSpecialValue)
            {
                selectedValues.Add(fieldInstanceValue);
                return selectedValues;
            } 
            else
            {
                switch (fieldInstanceType)
                {
                    case FieldTypes.Radio:
                        SetValueForRadio(selectedValues, fieldSelectable, fieldInstanceValue);
                        break;
                    case FieldTypes.Select:
                        SetValueForSelect(selectedValues, fieldSelectable, fieldInstanceValue);
                        break;
                    case FieldTypes.Checkbox:
                        SetValueForCheckbox(selectedValues, fieldSelectable, fieldInstanceValue);
                        break;
                    default:
                        break;
                }

                return selectedValues;
            }
        }

        private void SetValueForRadio(List<string> selectedValues, FieldSelectable fieldSelectable, string fieldInstanceValue)
        {
            string selectedValueId = fieldSelectable.Values.Find(fV => fV.ThesaurusId.ToString() == fieldInstanceValue)?.Id;
            SetValue(selectedValues, selectedValueId);
        }

        private string GetOptionLabel(FieldSelectable fieldSelectable, string fieldInstanceValue)
        {
            return fieldSelectable.Values.Find(fV => fV.Id == fieldInstanceValue)?.Label;
        }

        private void SetValueForSelect(List<string> selectedValues, FieldSelectable fieldSelectable, string fieldInstanceValue)
        {
            string selectedValueId = fieldSelectable.Values.Find(fV => fV.Value == fieldInstanceValue)?.Id;
            SetValue(selectedValues, selectedValueId);
        }

        private void SetValueForCheckbox(List<string> selectedValues, FieldSelectable fieldSelectable, string fieldInstanceValue)
        {
            foreach (string selectedValueId in 
                SplitChechboxValues(fieldInstanceValue, fieldSelectable)
                .Select(s => s.StartsWith(' ') ? s.Substring(1) : s) // fix checkbox inconsistences
                .Select(singleValue => fieldSelectable.Values.Find(fV => fV.Value == singleValue)?.Id)
                )
            {
                SetValue(selectedValues, selectedValueId);
            }
        }

        private string[] SplitChechboxValues(string value, FieldSelectable fieldSelectable)
        {
            if (CheckboxIdsWhereOptionsHaveComma.ContainsKey(fieldSelectable.Id))
            {
                List<string> checkedOptions = new List<string>();
                foreach (FormFieldValue formFieldValue in fieldSelectable.Values)
                {
                    if (value.Contains(formFieldValue.Value))
                    {
                        checkedOptions.Add(formFieldValue.Value);
                    }
                }
                return checkedOptions.ToArray();
            }
            else
            {
                return value.Split(',');
            }
        }

        private void SetValue(List<string> selectedValues, string selectedValueId)
        {
            if (!string.IsNullOrEmpty(selectedValueId))
            {
                selectedValues.Add(selectedValueId);
            }
        }

        private string GetFormDefinitionId(string formInstanceId)
        {
            if (FormInstanceDefinitions.TryGetValue(formInstanceId, out string formDefinitionId))
            {
                return formDefinitionId;
            }
            else
            {
                return string.Empty;
            }
        }

        private void AddUpdateFieldInstanceHistoryToBatch(List<WriteModel<FieldInstanceHistory>> instancesToWrite, FieldInstanceHistory fieldInstanceHistory)
        {
            var replaceFilter = Builders<FieldInstanceHistory>.Filter.Eq(x => x.Id, fieldInstanceHistory.Id);

            var update = Builders<FieldInstanceHistory>.Update
                .Set(x => x.Values, fieldInstanceHistory.Values)
                .Set(x => x.IsSpecialValue, fieldInstanceHistory.IsSpecialValue)
                ;
            instancesToWrite.Add(new UpdateOneModel<FieldInstanceHistory>(replaceFilter, update));
        }

        private void ProcessFieldInstanceHistories(FieldInstanceHistory fieldInstanceHistory, Dictionary<string, FieldSelectable> selectableFieldsInForm, int? notApplicableCodeId)
        {
            fieldInstanceHistory.Values = new List<string>();

            if (!string.IsNullOrEmpty(fieldInstanceHistory.Value))
            {
                if (HasPreviousSpecialValue(fieldInstanceHistory.Value))
                {
                    MapPreviousSpecialValueToNullFlavors(fieldInstanceHistory, notApplicableCodeId);
                }
                else if (fieldInstanceHistory.Type.IsSelectableField())
                {
                    if (selectableFieldsInForm.TryGetValue(fieldInstanceHistory.FieldId, out FieldSelectable fieldSelectable))
                    {
                        List<string> selectedValues = GetSelectedValues(
                            fieldInstanceHistory.Type,
                            fieldInstanceHistory.Value,
                            false,
                            fieldSelectable);

                        fieldInstanceHistory.Values.AddRange(selectedValues);
                        fieldInstanceHistory.ValueLabel = ModifySelectedValueLabels(selectedValues, fieldInstanceHistory.IsSpecialValue, fieldSelectable);
                    }
                }
                else
                {
                    fieldInstanceHistory.Values.Add(fieldInstanceHistory.Value);
                }
            }
        }

        private void MapPreviousSpecialValueToNullFlavors(FieldInstanceHistory fieldInstanceHistory, int? notApplicableCodeId)
        {
            fieldInstanceHistory.IsSpecialValue = true;
            if (notApplicableCodeId.HasValue)
            {
                fieldInstanceHistory.Values.Add(notApplicableCodeId.ToString());
                fieldInstanceHistory.ValueLabel = notApplicableCodeId.ToString();
            }
        }

        private bool HasPreviousSpecialValue(string value)
        {
            return value == "-2147483646";
        }

        #endregion /Helpers
    }
}
