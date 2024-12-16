using sReportsV2.Cache.Resources;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.File.Interfaces;
using sReportsV2.Common.Helpers;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace sReportsV2.BusinessLayer.Helpers.TabularExportGenerator
{
    public abstract class TabularExportGenerator
    {
        protected TabularExportGeneratorInputParams inputParams { get; set; }
        private readonly IFormInstanceDAL formInstanceDAL;
        private readonly IPatientDAL patientDAL;

        protected TabularExportGenerator(IFormInstanceDAL formInstanceDAL, IPatientDAL patientDAL, TabularExportGeneratorInputParams inputParams)
        {
            this.formInstanceDAL = formInstanceDAL;
            this.patientDAL = patientDAL;
            this.inputParams = inputParams;
        }

        public void CreateTabularExport()
        {
            int offset = 0;
            List<FormInstance> batch;
            DataTable table = new DataTable();

            do
            {
                batch = formInstanceDAL.GetByAllByDefinitionId(inputParams.CurrentForm.Id, inputParams.Organization.Key, inputParams.BatchSize, offset);
                offset += inputParams.BatchSize;

                DoExportLogic(table, batch, inputParams.CurrentForm, offset);
            } while (batch.Count == inputParams.BatchSize);

        }

        protected abstract void DoExportLogic(DataTable table, List<FormInstance> batch, Form currentForm, int offset);

        #region COMMON HELPERS

        protected List<CustomDataColumn> CreateWideTableHeader(List<FormInstance> formInstances, List<Field> allFieldCurrentForm, bool isPureWideFormat)
        {
            List<CustomDataColumn> header = new List<CustomDataColumn>() { 
                new CustomDataColumn() { ColumnName = TextLanguage.Organization },
                new CustomDataColumn() { ColumnName = TextLanguage.Document_Id },
                new CustomDataColumn() { ColumnName = TextLanguage.Date_And_Time },
                new CustomDataColumn() { ColumnName = TextLanguage.PatientName },
                new CustomDataColumn() { ColumnName = TextLanguage.Patient_lastname },
                new CustomDataColumn() { ColumnName = TextLanguage.Patient_date_of_birth },            
            };

            if (isPureWideFormat)
                header = header.Concat(GetRepetitiveFieldLabels(formInstances, allFieldCurrentForm)).ToList();
            else
                header = header.Concat(GetNonRepetitiveFieldLabels(allFieldCurrentForm)).ToList();

            return header;
        }

        protected List<CustomDataColumn> GetRepetitiveFieldLabels(List<FormInstance> formInstances, List<Field> allFieldCurrentForm)
        {
            Dictionary<string, int> fieldSetCounts;
            List<CustomDataColumn> columns = new List<CustomDataColumn>();

            foreach (FormInstance formInstance in formInstances.AsParallel().AsOrdered())
            {
                fieldSetCounts = new Dictionary<string, int>();

                foreach (var fieldInstancesGroup in formInstance.FieldInstances.GroupBy(x => x.FieldSetInstanceRepetitionId))
                {
                    string fieldSetId = fieldInstancesGroup.FirstOrDefault()?.FieldSetId ?? string.Empty;

                    if (fieldSetCounts.ContainsKey(fieldSetId))
                    {
                        fieldSetCounts[fieldSetId]++;
                    }
                    else
                    {
                        fieldSetCounts[fieldSetId] = 1;
                    }

                    int fieldSetRepetition = fieldSetCounts[fieldSetId];

                    foreach (FieldInstance fieldInstance in fieldInstancesGroup)
                    {
                        int repetitiveFieldCount = fieldInstance.FieldInstanceValues.GetRepetitiveFieldCount();
                        string label = allFieldCurrentForm.Find(y => y.Id == fieldInstance.FieldId)?.Label ?? fieldInstance.FieldId;  // If the label isn't found, the field was deleted from Designer.

                        if(!columns.Exists(c => c.FieldSetId == fieldInstance.FieldSetId && c.RepetitiveFieldSetCounter == fieldSetRepetition && c.FieldId == fieldInstance.FieldId && c.RepetitiveFieldCounter == repetitiveFieldCount))
                        {
                            for (int i = 0; i < repetitiveFieldCount; i++)
                            {
                                columns.Add(new CustomDataColumn()
                                {
                                    ColumnName = CustomDataColumn.CreateRepetitiveFieldName(fieldInstance.FieldSetId, fieldSetRepetition, fieldInstance.FieldId, i),
                                    RepetitiveFieldCounter = i + 1,
                                    RepetitiveFieldSetCounter = fieldSetRepetition,
                                    FieldId = fieldInstance.FieldId,
                                    FieldSetId = fieldInstance.FieldSetId,
                                    ColumnLabel = label
                                });
                            }
                        }
                    }
                }
            }
            return columns; 
        }

        protected List<CustomDataColumn> GetNonRepetitiveFieldLabels(List<Field> allFieldCurrentForm)
        {
            return allFieldCurrentForm.Select(x => new CustomDataColumn() { ColumnName = CustomDataColumn.CreateNonRepetitiveFieldName(x.Id, x.Label), ColumnLabel = x.Label }).ToList();
        }

        protected DataRow AddPatientAndMetaDataToWideTableRow(DataRow dataRow, FormInstance formInstance, string organization, string TimeZoneOffset, string DateFormat)
        {
            dataRow[TextLanguage.Organization] = organization;
            dataRow[TextLanguage.Document_Id] = formInstance.Id;
            dataRow[TextLanguage.Date_And_Time] = formInstance.EntryDatetime.ToTimeZoned(TimeZoneOffset, DateFormat);

            dataRow = AddPatientInfoToWideTableRow(dataRow, formInstance.PatientId);
            return dataRow;
        }

        protected DataRow AddPatientInfoToWideTableRow(DataRow dataRow, int patientId)
        {
            if (patientId != 0)
            {
                Patient patient = patientDAL.GetById(patientId);
                if (patient != null)
                {
                    dataRow[TextLanguage.PatientName] = patient.NameGiven ?? String.Empty;
                    dataRow[TextLanguage.Patient_lastname] = patient.NameFamily ?? String.Empty;
                    dataRow[TextLanguage.Patient_date_of_birth] = patient.BirthDate != null ? patient.BirthDate.Value.ToString(DateConstants.DateFormat, CultureInfo.InvariantCulture) : String.Empty;
                }
            }
            return dataRow;
        }

        protected void AddPatientInfoToLongTable(FileWriter fileWriter, string formInstanceId, int patientId)
        {
            if (patientId != 0)
            {
                Patient patient = patientDAL.GetById(patientId);
                if (patient != null)
                {
                    fileWriter.WriteRow(new List<string>() { formInstanceId, TextLanguage.Patient_name, patient.NameGiven ?? String.Empty });
                    fileWriter.WriteRow(new List<string>() { formInstanceId, TextLanguage.Patient_lastname, patient.NameFamily ?? String.Empty });
                    fileWriter.WriteRow(new List<string>() { formInstanceId, TextLanguage.Patient_date_of_birth, patient.BirthDate != null ? patient.BirthDate.Value.ToString(DateConstants.DateFormat, CultureInfo.InvariantCulture) : String.Empty });
                }
            }
        }

        #endregion
    }
}
