using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using sReportsV2.Cache.Resources;
using sReportsV2.Domain.Entities.Form;

namespace sReportsV2.BusinessLayer.Helpers.TabularExportGenerator
{
    public class LongTableExportGenerator : TabularExportGenerator
    {
        public LongTableExportGenerator(IFormInstanceDAL formInstanceDAL, IPatientDAL patientDAL, TabularExportGeneratorInputParams inputParams) : base(formInstanceDAL, patientDAL, inputParams) { }

        protected override void DoExportLogic(DataTable table, List<FormInstance> batch, Form currentForm, int offset)
        {
            // Generate header of the DataTable
            if (offset == inputParams.BatchSize)
                inputParams.FileWriter.WriteRow(GenerateHeaderRow());

            foreach (FormInstance formInstance in batch)
            {
                inputParams.FileWriter.WriteRow(new List<string>() {
                    inputParams.Organization.Value,
                    formInstance.Id, 
                    string.Empty, string.Empty, string.Empty,
                    TextLanguage.Date_And_Time, 
                    formInstance.EntryDatetime.ToTimeZoned(inputParams.TimeZoneOffset, inputParams.DateFormat) });

                AddPatientInfoToLongTable(inputParams.FileWriter, formInstance.Id, formInstance.PatientId);

                foreach (FormChapter chapter in currentForm.Chapters)
                {
                    foreach (FormPage page in chapter.Pages)
                    {
                        foreach (FieldSet fieldset in page.ListOfFieldSets.SelectMany(x => x))
                        {
                            var fieldInstancesInFieldsetGrouped = formInstance.FieldInstances.Where(x => x.FieldSetId == fieldset.Id).GroupBy(x => x.FieldSetInstanceRepetitionId);
                            
                            foreach (var fieldInstanceGroup in fieldInstancesInFieldsetGrouped)
                            {
                                foreach (Field field in fieldset.Fields.Where(x => !x.Id.Equals(Domain.Entities.DFD.Constants.StateSmsSystemFieldId) && !x.Id.Equals("1001")))
                                {
                                
                                    List<FieldInstance> fieldInstances = fieldInstanceGroup.Where(x => x.FieldId == field.Id).ToList();

                                    fieldInstances.ForEach(x =>
                                    {
                                        if (x.FieldInstanceValues != null && x.FieldInstanceValues.Count > 0)
                                        {
                                            foreach (FieldInstanceValue fieldInstanceValue in x.FieldInstanceValues)
                                            {
                                                inputParams.FileWriter.WriteRow(new List<string>() {
                                                    inputParams.Organization.Value,
                                                    formInstance.Id,
                                                    chapter.Title,
                                                    page.Title,
                                                    fieldset.Label,
                                                    field.Label,
                                                    field.GetDisplayValue(fieldInstanceValue, inputParams.MissingValues) });
                                            }
                                        }
                                        else
                                        {
                                            inputParams.FileWriter.WriteRow(new List<string>() {
                                                inputParams.Organization.Value,
                                                formInstance.Id,
                                                chapter.Title,
                                                page.Title,
                                                fieldset.Label,
                                                field.Label, 
                                                string.Empty });
                                        }
                                    });
                                }

                            }
                        }
                    }
                }
            }
        }

        private List<string> GenerateHeaderRow()
        {
            return new List<string>() {
                    TextLanguage.Organization,
                    TextLanguage.Document_Id,
                    TextLanguage.Chapter_Name,
                    TextLanguage.Page_Name,
                    TextLanguage.Fieldset_Name,
                    TextLanguage.Field_Label,
                    TextLanguage.Field_Value,

                };
        }


    }
}
