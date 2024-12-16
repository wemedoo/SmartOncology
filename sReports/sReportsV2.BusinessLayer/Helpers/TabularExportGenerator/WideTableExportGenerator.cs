using sReportsV2.Common.Extensions;
using sReportsV2.Common.Helpers;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace sReportsV2.BusinessLayer.Helpers.TabularExportGenerator
{
    public class WideTableExporter : TabularExportGenerator
    {
        public WideTableExporter(IFormInstanceDAL formInstanceDAL, IPatientDAL patientDAL, TabularExportGeneratorInputParams inputParams) : base(formInstanceDAL, patientDAL, inputParams) { }

        protected override void DoExportLogic(DataTable table, List<FormInstance> batch, Form currentForm, int offset)
        {
            // Generate header of the DataTable
            if (offset == inputParams.BatchSize)
            {
                List<CustomDataColumn> header = CreateWideTableHeader(batch, currentForm.GetAllFields(), isPureWideFormat: true);
                header.ForEach(x => table.Columns.Add(x));
                inputParams.FileWriter.WriteRow(header.GetCleanCustomColumnNames(repetitiveHeaders: true));
            }

            // Generate Rows of DataTable
            foreach (FormInstance formInstance in batch.AsParallel().AsOrdered())
            {
                DataRow dataRow = table.NewRow();
                dataRow = AddPatientAndMetaDataToWideTableRow(dataRow, formInstance, inputParams.Organization.Value, inputParams.TimeZoneOffset, inputParams.DateFormat);

                foreach (Field field in currentForm.GetAllFields()) 
                {
                    List<string> fieldSetsToCount = new List<string>();

                    foreach (FieldInstance fieldInstance in formInstance.FieldInstances.Where(x => x.FieldId == field.Id))
                    {
                        if (!fieldSetsToCount.Contains(fieldInstance.FieldSetInstanceRepetitionId)) 
                        {
                            fieldSetsToCount.Add(fieldInstance.FieldSetInstanceRepetitionId);
                        }

                        int repetitiveFieldCount = fieldInstance.FieldInstanceValues.GetRepetitiveFieldCount();

                        for (int i = 0; i < repetitiveFieldCount; i++)
                        {
                            string key = CustomDataColumn.CreateRepetitiveFieldName(fieldInstance.FieldSetId, fieldSetsToCount.Count, fieldInstance.FieldId, i);
                            dataRow[key] = field.GetDisplayValue(fieldInstance.FieldInstanceValues[i], inputParams.MissingValues);
                        }
                    }
                }
                inputParams.FileWriter.WriteRow(dataRow.ItemArray.Select(field => field.ToString()));
            }
        }
    }

    public class WideTableRepetitiveFieldExporter : TabularExportGenerator
    {
        public WideTableRepetitiveFieldExporter(IFormInstanceDAL formInstanceDAL, IPatientDAL patientDAL, TabularExportGeneratorInputParams inputParams) : base(formInstanceDAL, patientDAL, inputParams) { }

        protected override void DoExportLogic(DataTable table, List<FormInstance> batch, Form currentForm, int offset)
        {
            Field repetitiveField = inputParams.RepetitiveElement as Field;

            // Generate header of the DataTable
            if (offset == inputParams.BatchSize)
            {
                List<CustomDataColumn> header = CreateWideTableHeader(null, currentForm.GetAllFields(), isPureWideFormat: false);
                header.ForEach(x => table.Columns.Add(x));
                inputParams.FileWriter.WriteRow(header.GetCleanCustomColumnNames());
            }

            // Generate Rows of DataTable
            foreach (FormInstance formInstance in batch.AsParallel().AsOrdered())
            {
                IEnumerable<FieldInstance> repetitiveFields = formInstance.FieldInstances.Where(x => x.FieldId == repetitiveField.Id);

                for (int i = 0; i < repetitiveFields.Count(); i++)
                {
                    DataRow dataRow = table.NewRow();
                    dataRow = AddPatientAndMetaDataToWideTableRow(dataRow, formInstance, inputParams.Organization.Value, inputParams.TimeZoneOffset, inputParams.DateFormat);

                    foreach (Field field in currentForm.GetAllFields())
                    {
                        FieldInstance fieldInstance = repetitiveFields.ElementAtOrDefault(i) ?? formInstance.FieldInstances.Find(x => x.FieldId == field.Id);
                        dataRow[CustomDataColumn.CreateNonRepetitiveFieldName(field.Id, field.Label)] = field.GetDisplayValue(fieldInstance.FieldInstanceValues[i], inputParams.MissingValues);
                    }
                    inputParams.FileWriter.WriteRow(dataRow.ItemArray.Select(f => f.ToString()));
                }
            }
        }
    }


    public class WideTableRepetitiveFieldSetExporter : TabularExportGenerator
    {
        public WideTableRepetitiveFieldSetExporter(IFormInstanceDAL formInstanceDAL, IPatientDAL patientDAL, TabularExportGeneratorInputParams inputParams) : base(formInstanceDAL, patientDAL, inputParams) { }

        protected override void DoExportLogic(DataTable table, List<FormInstance> batch, Form currentForm, int offset)
        {
            FieldSet repetitiveFieldset = inputParams.RepetitiveElement as FieldSet;

            // Generate header of the DataTable
            if (offset == inputParams.BatchSize)
            {
                List<CustomDataColumn> header = CreateWideTableHeader(null, currentForm.GetAllFields(), isPureWideFormat: false);
                header.ForEach(x => table.Columns.Add(x));
                inputParams.FileWriter.WriteRow(header.GetCleanCustomColumnNames());
            }

            // Generate Rows of DataTable
            foreach (FormInstance formInstance in batch.AsParallel().AsOrdered())
            {
                var repetitiveFieldsetGroups = formInstance.FieldInstances.Where(x => x.FieldSetId == repetitiveFieldset.Id).GroupBy(x => x.FieldSetInstanceRepetitionId);

                foreach (var group in repetitiveFieldsetGroups)
                {
                    DataRow dataRow = table.NewRow();
                    dataRow = AddPatientAndMetaDataToWideTableRow(dataRow, formInstance, inputParams.Organization.Value, inputParams.TimeZoneOffset, inputParams.DateFormat);

                    foreach (Field field in currentForm.GetAllFields())
                    {
                        FieldInstance fieldInstance = group.FirstOrDefault(x => x.FieldId == field.Id) ?? formInstance.FieldInstances.Find(x => x.FieldId == field.Id);
                        dataRow[CustomDataColumn.CreateNonRepetitiveFieldName(field.Id, field.Label)] = field.GetDisplayValue(fieldInstance.FieldInstanceValues?.FirstOrDefault(), inputParams.MissingValues);
                    }
                    inputParams.FileWriter.WriteRow(dataRow.ItemArray.Select(f => f.ToString()));
                }
            }
        }
    }


}
