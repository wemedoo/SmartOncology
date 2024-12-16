using MongoDB.Driver;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Helpers;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Mongo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.Domain.DatabaseMigrationScripts
{
    public class M_202308231355_FieldInstanceValue_Property : MongoMigration
    {
        private readonly IMongoCollection<FormInstance> Collection;

        public override int Version => 5;

        public M_202308231355_FieldInstanceValue_Property()
        {
            Collection = MongoDBInstance.Instance.GetDatabase().GetCollection<FormInstance>(MongoCollectionNames.FormInstance);
        }

        protected override void Up()
        {
            MigrateFormInstancesToNewModel().Wait();
        }

        protected override void Down()
        {

        }

        private async Task MigrateFormInstancesToNewModel()
        {
            LogHelper.Info("MigrateFormInstancesToNewModel is started");
            
            try
            {
                var findOptions = new FindOptions<FormInstance, FormInstance>() { };
                var allFormInstancesFilter = Builders<FormInstance>.Filter.Empty;

                var instancesToWrite = new List<WriteModel<FormInstance>>();

                using (var cursor = await Collection.FindAsync(allFormInstancesFilter, findOptions).ConfigureAwait(false))
                {
                    while (await cursor.MoveNextAsync().ConfigureAwait(false))
                    {
                        var batch = cursor.Current;

                        foreach (FormInstance formInstance in batch)
                        {
                            formInstance.FieldInstances = GetFieldInstanceValuesAfterProcessing(formInstance);

                            var replaceFilter = Builders<FormInstance>.Filter.Eq(x => x.Id, formInstance.Id);
                            instancesToWrite.Add(new ReplaceOneModel<FormInstance>(replaceFilter, formInstance));
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
                LogHelper.Error("Eror while MigrateFormInstancesToNewModel, error: " + ex.Message);
            }
            LogHelper.Info("MigrateFormInstancesToNewModel is finished");
        }

        #region Helpers

        private List<FieldInstance> GetFieldInstanceValuesAfterProcessing(FormInstance formInstance)
        {
            Dictionary<string, Tuple<string, Dictionary<string, FieldInstance>>> groupedFields = new Dictionary<string, Tuple<string, Dictionary<string, FieldInstance>>>();
            foreach (FieldInstance currentFieldFromDB in formInstance.Fields)
            {
                FieldInstance updatedFieldForDB = new FieldInstance()
                {
                    Type = currentFieldFromDB.Type,
                    ThesaurusId = currentFieldFromDB.ThesaurusId
                };
                string[] instanceIdSplited = currentFieldFromDB.InstanceId.Split('-');
                if (int.TryParse(instanceIdSplited[1], out int fieldSetCounter))
                {
                    updatedFieldForDB.FieldSetId = instanceIdSplited[0];
                    updatedFieldForDB.FieldId = instanceIdSplited[2];

                    string fieldSetKey = $"{updatedFieldForDB.FieldSetId}-{fieldSetCounter}";

                    SetFieldInstanceValues(updatedFieldForDB, currentFieldFromDB);
                    if (groupedFields.TryGetValue(fieldSetKey, out Tuple<string, Dictionary<string, FieldInstance>> fieldsInFieldSet))
                    {
                        updatedFieldForDB.FieldSetInstanceRepetitionId = fieldsInFieldSet.Item1;

                        if (fieldsInFieldSet.Item2.TryGetValue(updatedFieldForDB.FieldId, out FieldInstance alreadyAddedFieldInDict))
                        {
                            alreadyAddedFieldInDict.FieldInstanceValues.AddRange(updatedFieldForDB.FieldInstanceValues);
                        }
                        else
                        {
                            fieldsInFieldSet.Item2.Add(updatedFieldForDB.FieldId, updatedFieldForDB);
                        }
                    }
                    else
                    {
                        string fieldSetInstanceRepetitionId = GuidExtension.NewGuidStringWithoutDashes();
                        updatedFieldForDB.FieldSetInstanceRepetitionId = fieldSetInstanceRepetitionId;

                        groupedFields.Add(
                            fieldSetKey,
                            new Tuple<string, Dictionary<string, FieldInstance>>
                            (
                                fieldSetInstanceRepetitionId,
                                new Dictionary<string, FieldInstance> {
                                                {
                                                    updatedFieldForDB.FieldId,
                                                    updatedFieldForDB
                                                }
                                }
                            )
                        );
                    }
                }


            }

            return groupedFields.SelectMany(x => x.Value.Item2.Values).ToList();
        }

        private void SetFieldInstanceValues(FieldInstance updatedFieldForDB, FieldInstance currentFieldFromDB)
        {
            updatedFieldForDB.FieldInstanceValues = currentFieldFromDB.FieldInstanceValues.GetFieldInstanceValuesOrInitial();
            currentFieldFromDB.Value = currentFieldFromDB.Value ?? new List<string>();
            currentFieldFromDB.ValueLabel = currentFieldFromDB.ValueLabel ?? new List<string>();
            for (int i = 0; i < currentFieldFromDB.Value.Count; i++)
            {
                string value = GetValue(currentFieldFromDB.Type, currentFieldFromDB.Value[i]);
                string valueLabel = string.Empty;
                if (i <= currentFieldFromDB.ValueLabel.Count - 1)
                {
                    valueLabel = GetValue(currentFieldFromDB.Type, currentFieldFromDB.ValueLabel[i]);
                }
                updatedFieldForDB.FieldInstanceValues.Add(new FieldInstanceValue(new List<string> { value }, valueLabel));
            }
        }

        private string GetValue(string fieldType, string value)
        {
            if (IsSpecialValue(value) || string.IsNullOrEmpty(value))
            {
                return value;
            }

            if (fieldType == FieldTypes.Date)
            {
                return RenderDate(value);
            }
            else if (fieldType == FieldTypes.Datetime)
            {
                return RenderDateTime(value);
            }
            else
            {
                return value;
            }
        }

        private bool IsSpecialValue(string value)
        {
            return value == "-2147483646";
        }

        #region Date(time) helpers
        public string RenderDate(string dateTimeValue)
        {
            string[] dateTimeParts = dateTimeValue.Split('T');
            string datePart = dateTimeParts[0];
            datePart = HandleValueDuplication(datePart);

            if (CouldDateBeParsed(datePart, out DateTime parsedDate))
            {
                return parsedDate.ToString(DateConstants.UTCDatePartFormat);
            }
            else
            {
                return string.Empty;
            }
        }

        public string RenderDateTime(string dateTimeValue)
        {
            string datePart = RenderDate(dateTimeValue);
            if (!string.IsNullOrEmpty(datePart))
            {
                datePart += $"T{dateTimeValue.RenderTime()}";
            }
            return datePart;
        }

        private string HandleValueDuplication(string dateTimeValue)
        {
            return dateTimeValue.Contains(',') ? dateTimeValue.Split(',')[0] : dateTimeValue;
        }

        private bool CouldDateBeParsed(string datePart, out DateTime parsedDate)
        {
            string[] currentDateFormatsInDatabase = new string[] 
            { 
                DateConstants.DateFormat, 
                DateConstants.UTCDatePartFormat, 
                "dd-MM-yyyy", 
                "d/M/yyyy", 
                "d/MM/yyyy", 
                "dd/M/yyyy" 
            };
            return DateTime.TryParseExact(datePart, currentDateFormatsInDatabase, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate);
        }
        #endregion

        #endregion /Helpers
    }
}
