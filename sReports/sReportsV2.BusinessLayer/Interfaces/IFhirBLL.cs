using Hl7.Fhir.Model;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.DTOs.DTOs.Fhir.DataIn;
using sReportsV2.DTOs.DTOs.FormInstance.DataIn;
using sReportsV2.DTOs.User.DTO;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IFhirBLL
    {
        Questionnaire ExportFormToQuestionnaire(string formId);
        FormInstanceJsonDTO ExportFormToMimacom(string formId);
        System.Threading.Tasks.Task CreateOrUpdateFormInstanceFromQuestionnaireResponse(QuestionnaireResponse questionnaireResponse, UserCookieData userCookieData);
        void InsertFromJson(Form form, FormInstance formInstance, FormInstanceJsonDTO formInstanceJsonInput);
        Task<bool> GenerateDocumentReferenceForDataExtraction(DataExtractionDataIn dataExtractionDataIn);
    }
}
