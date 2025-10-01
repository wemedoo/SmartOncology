using AutoMapper;
using Chapters;
using iText.Kernel.Pdf;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Domain.Sql.Entities.Encounter;
using sReportsV2.Domain.Sql.Entities.EpisodeOfCare;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.Encounter;
using sReportsV2.DTOs.EpisodeOfCare;
using sReportsV2.DTOs.DTOs.PDF.DataIn;
using sReportsV2.DTOs.DTOs.PDF.DataOut;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using sReportsV2.Cache.Singleton;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Exceptions;
using System.Net;
using sReportsV2.Cache.Resources;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class PdfBLL : IPdfBLL
    {
        private readonly IOrganizationDAL organizationDAL;
        private readonly IFormDAL formDAL;
        private readonly IFormBLL formBLL;
        private readonly IFormInstanceDAL formInstanceDAL;
        private readonly IEpisodeOfCareDAL episodeOfCareDAL;
        private readonly IPatientDAL patientDAL;
        private readonly IEncounterDAL encounterDAL;
        private readonly IPersonnelDAL personnelDAL;
        private readonly IMapper mapper;

        public PdfBLL(IOrganizationDAL organizationDAL, IFormDAL formDAL, IFormInstanceDAL formInstanceDAL, IEpisodeOfCareDAL episodeOfCareDAL, IPatientDAL patientDAL, IEncounterDAL encounterDAL, IPersonnelDAL personnelDAL, IFormBLL formBLL, IMapper mapper)
        {
            this.organizationDAL = organizationDAL;
            this.formDAL = formDAL;
            this.formInstanceDAL = formInstanceDAL;
            this.episodeOfCareDAL = episodeOfCareDAL;
            this.patientDAL = patientDAL;
            this.encounterDAL = encounterDAL;
            this.personnelDAL = personnelDAL;
            this.formBLL = formBLL;
            this.mapper = mapper;
        }

        #region Generate PDF

        public PdfDocumentDataOut Generate(PdfDocumentDataIn documentDataIn)
        {
            Form form = formDAL.GetForm(documentDataIn.ResourceId);
            Ensure.IsNotNull(form, nameof(form));

            UserCookieData userCookieData = documentDataIn.UserCookieData;
            FormPdfGenerator pdfGenerator = new FormPdfGenerator(form, organizationDAL.GetById(userCookieData.ActiveOrganization), userCookieData.GetFirstAndLastName());

            return new PdfDocumentDataOut
            {
                Content = pdfGenerator.Generate(),
                ContentType = "application/pdf",
                DocumentName = $"{form.Title}.pdf"
            };
        }

        public PdfDocumentDataOut GenerateSynoptic(PdfDocumentDataIn documentDataIn)
        {
            FormInstance formInstance = formInstanceDAL.GetById(documentDataIn.ResourceId);
            Ensure.IsNotNull(formInstance, nameof(formInstance));

            UserCookieData userCookieData = documentDataIn.UserCookieData;
            FormDataOut data = GetForm(formInstance);
            string signingUserCompleteName = GetUserWhoLock(formInstance);

            SynopticPdfGenerator pdfGenerator = new SynopticPdfGenerator(
                data, 
                signingUserCompleteName, 
                organizationDAL.GetById(userCookieData.ActiveOrganization),
                userCookieData.GetFirstAndLastName(),
                patientDAL.GetIdentifierByPatientId(
                    formInstance.PatientId, 
                    SingletonDataContainer.Instance.GetCodeId((int)CodeSetList.PatientIdentifierType, ResourceTypes.OomniaScreeningNumber))?.IdentifierValue
                );

            return new PdfDocumentDataOut
            {
                Content = pdfGenerator.Generate(),
                ContentType = "application/pdf",
                DocumentName = $"{pdfGenerator.GetDownloadedSynopticPdfName()}.pdf"
            };
        }

        private string GetUserWhoLock(FormInstance formInstance)
        {
            string signingUserCompleteName = string.Empty;
            if (formInstance.IsFormInstanceLocked())
            {
                FormInstanceStatus latestChange = formInstance.GetLastChange();
                Personnel signingUser = personnelDAL.GetById(latestChange.CreatedById);
                signingUserCompleteName = signingUser.GetFirstAndLastName();
            }
            return signingUserCompleteName;
        }

        private FormDataOut GetForm(FormInstance formInstance)
        {
            Form form = new Form(formInstance, formDAL.GetForm(formInstance.FormDefinitionId))
            {
                LastUpdate = formInstance.LastUpdate,
                EntryDatetime = formInstance.EntryDatetime
            };
            FormDataOut data = formBLL.SetFormDependablesAndReferrals(form);

            return data;
        }

        #endregion /Generate PDF

        #region Upload PDF

        public void UploadFile(IFormFile file, UserCookieData userCookieData)
        {
            Ensure.IsNotNull(file, nameof(file));
            UserData userData = mapper.Map<UserData>(userCookieData);

            using (PdfReader reader = new PdfReader(file.OpenReadStream()))
            {
                using (PdfDocument pdfDocument = new PdfDocument(reader))
                {
                    string formId = pdfDocument.GetDocumentInfo().GetMoreInfo("formId") ?? throw new UserAdministrationException((int)HttpStatusCode.BadRequest, TextLanguage.InvalidUploadedPdfFormat);
                    Form form = formDAL.GetForm(formId) ?? throw new UserAdministrationException((int)HttpStatusCode.NotFound, string.Format(TextLanguage.FormNotExists, string.Empty));

                    PdfFormParser parser = new PdfFormParser(form, pdfDocument);
                    Form parsedForm = parser.ReadFieldsFromPdf();
                    FormInstance parsedFormInstance = new FormInstance(parsedForm)
                    {
                        FieldInstances = parser.FieldInstances,
                        Date = parsedForm.Date,
                        Notes = parsedForm.Notes,
                        FormState = parsedForm.FormState,
                        Referrals = new List<string>()
                    };
                    SetPatientRelatedData(form, parsedFormInstance, parser.Patient, userData);

                    InsertFormInstance(parsedFormInstance, userCookieData);
                }
            }
        }

        private void SetPatientRelatedData(Form form, FormInstance parsedFormInstance, Patient patient, UserData user)
        {
            if (!form.DisablePatientData)
            {
                patient.OrganizationId = form.GetActiveOrganizationId(user.ActiveOrganization.GetValueOrDefault());
                int patientId = InsertPatient(patient);

                int episodeOfCareId = InsertEpisodeOfCare(patientId, form.EpisodeOfCare, "Pdf", parsedFormInstance.Date.Value, user);
                int encounterId = InsertEncounter(episodeOfCareId);
                parsedFormInstance.EncounterRef = encounterId;
                parsedFormInstance.EpisodeOfCareRef = episodeOfCareId;
                parsedFormInstance.PatientId = patientId;
            }
        }

        private void InsertFormInstance(FormInstance formInstance, UserCookieData userCookieData)
        {
            formInstance = Ensure.IsNotNull(formInstance, nameof(formInstance));

            formInstance.UserId = userCookieData.Id;
            formInstance.Language = userCookieData.ActiveLanguage;
            formInstance.OrganizationId = userCookieData.ActiveOrganization;

            formInstanceDAL.InsertOrUpdate(formInstance, formInstance.GetCurrentFormInstanceStatus(userCookieData?.Id));
        }

        private int InsertPatient(Patient patient)
        {
            int patientId = 0;
            if (patient != null)
            {
                patientId = patient != null && patient.PatientIdentifiers != null && patient.PatientIdentifiers.Count > 0 ?
                    patientDAL.GetByIdentifier(patient.PatientIdentifiers[0]).PatientId
                    :
                    0;

                if (patientId != 0)
                {
                    patientDAL.InsertOrUpdate(patient, null);
                }
            }

            return patientId;
        }

        private int InsertEpisodeOfCare(int patientId, FormEpisodeOfCare episodeOfCare, string source, DateTime startDate, UserData user)
        {
            startDate = startDate.Date;
            EpisodeOfCare eoc;
            if (episodeOfCare != null)
            {
                eoc = mapper.Map<EpisodeOfCare>(episodeOfCare);
                eoc.Period = new Domain.Sql.Entities.Common.PeriodDatetime() { Start = startDate };
                eoc.Description = $"Generated from {source}";
                eoc.PatientId = patientId;
                eoc.OrganizationId = 1;
            }
            else
            {
                eoc = mapper.Map<EpisodeOfCare>(new EpisodeOfCareDataIn()
                    {
                        Description = $"Generated from {source}",
                        PatientId = patientId,
                        StatusCD = (int)EocStatus.Active,
                        Period = new PeriodDTO() { StartDate = startDate }
                    }
                );
                eoc.OrganizationId = 1;
            }

            return episodeOfCareDAL.InsertOrUpdate(eoc, user);
        }

        private int InsertEncounter(int episodeOfCareId)
        {
            Encounter encounterEntity = mapper.Map<Encounter>(new EncounterDataIn()
            {
                ClassCD = 12246,
                Period = new PeriodOffsetDTO
                {
                    StartDate = DateTimeOffset.Now,
                    EndDate = DateTimeOffset.Now
                },
                StatusCD = 12218,
                TypeCD = 12208,
                ServiceTypeCD = 11087
            }
            );
            encounterEntity.EpisodeOfCareId = episodeOfCareId;

            return encounterDAL.InsertOrUpdate(encounterEntity);
        }

        #endregion /Upload PDF
    }
}
