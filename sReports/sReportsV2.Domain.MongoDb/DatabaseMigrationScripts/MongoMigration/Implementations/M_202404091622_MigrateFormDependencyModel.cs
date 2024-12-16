using MongoDB.Driver;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Helpers;
using sReportsV2.Domain.Entities.Dependency;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.Domain.DatabaseMigrationScripts
{
    public class M_202404091622_MigrateFormDependencyModel : MongoMigration
    {
        private readonly IMongoCollection<Form> Collection;

        public override int Version => 17;

        public M_202404091622_MigrateFormDependencyModel()
        {
            Collection = MongoDBInstance.Instance.GetDatabase().GetCollection<Form>(MongoCollectionNames.Form);
        }

        protected override void Up()
        {
            MigrateFieldDependecyNewModel().Wait();
        }

        protected override void Down()
        {

        }

        private async Task MigrateFieldDependecyNewModel()
        {
            try
            {
                var allFormsFilter = Builders<Form>.Filter.Empty;
                var findOptions = new FindOptions<Form, Form>() { };
                var instancesToWrite = new List<WriteModel<Form>>();

                using (var cursor = await Collection.FindAsync(allFormsFilter, findOptions).ConfigureAwait(false))
                {
                    while (await cursor.MoveNextAsync().ConfigureAwait(false))
                    {
                        var batch = cursor.Current;

                        foreach (Form form in batch)
                        {
                            if (MigrateFieldDependecyForForm(form))
                            {
                                var replaceFilter = Builders<Form>.Filter.Eq(x => x.Id, form.Id);
                                instancesToWrite.Add(new ReplaceOneModel<Form>(replaceFilter, form));
                            }
                        }

                        if (instancesToWrite.Any())
                        {
                            var result = await Collection.BulkWriteAsync(instancesToWrite).ConfigureAwait(false);
                            if (!result.IsAcknowledged)
                                throw new InvalidOperationException($"BulkWriteAsync wrote {result.InsertedCount} items instead of {instancesToWrite.Count}");

                            instancesToWrite.Clear();
                        }
                    }
                } 
            }
            catch (Exception ex)
            {
                LogHelper.Error("Eror while MigrateFieldDependecyNewModel, error: " + ex.Message);
            }
        }

        private bool MigrateFieldDependecyForForm(Form form)
        {
            bool formModified = false;

            Dictionary<string, Field> fields = form.GetAllFields().ToDictionary(x => x.Id, x => x);
            foreach (FieldSelectable fieldSelectable in form.GetAllSelectableFields())
            {
                foreach (FormFieldDependable dependable in fieldSelectable.Dependables)
                {
                    if (fields.TryGetValue(dependable.ActionParams, out Field field))
                    {
                        field.DependentOn = field.DependentOn ?? new DependentOnInfo
                        {
                            DependentOnFieldInfos = new List<DependentOnFieldInfo>(),
                            FieldActions = new List<Common.Enums.FieldAction> { Common.Enums.FieldAction.DataCleaning }
                        };

                        string selectedOptionId = fieldSelectable.Values.Find(fV => fV.Value.Trim() == dependable.Condition)?.Id;

                        if (selectedOptionId != null)
                        {
                            string selectableFieldVariable = GetVariable(field.DependentOn.DependentOnFieldInfos.Count);
                            DependentOnFieldInfo selectableField = new DependentOnFieldInfo
                            {
                                FieldId = fieldSelectable.Id,
                                Variable = selectableFieldVariable
                            };

                            string selectableOptionVariable = GetVariable(field.DependentOn.DependentOnFieldInfos.Count + 1);
                            DependentOnFieldInfo selectableOptionField = new DependentOnFieldInfo
                            {
                                FieldId = fieldSelectable.Id,
                                FieldValueId = selectedOptionId,
                                Variable = selectableOptionVariable
                            };

                            UpdateFormula(field.DependentOn, selectableFieldVariable, selectableOptionVariable);
                            field.DependentOn.DependentOnFieldInfos.Add(selectableField);
                            field.DependentOn.DependentOnFieldInfos.Add(selectableOptionField);
                        }

                        formModified = true;
                    }
                }
            }

            return formModified;
        }

        private string GetVariable(int numberOfDependentFields)
        {
            return ((char)(97 + numberOfDependentFields)).ToString();
        }

        private void UpdateFormula(DependentOnInfo dependentOnInfo, string leftOperand, string rightOperand)
        {
            int numberOfFields = dependentOnInfo.DependentOnFieldInfos.Count;
            string equalComparison = $"[{leftOperand}] == [{rightOperand}]";

            if (numberOfFields == 0)
            {
                dependentOnInfo.Formula = equalComparison;
            }
            else
            {
                dependentOnInfo.Formula += $" || {equalComparison}";
            }
        }
    }
}
