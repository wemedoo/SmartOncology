using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DigitalGuideline.DataIn;
using sReportsV2.DTOs.DigitalGuideline.DataOut;
using sReportsV2.DTOs.DigitalGuidelineInstance.DataIn;
using sReportsV2.DTOs.DigitalGuidelineInstance.DataOut;
using sReportsV2.DTOs.DTOs.Autocomplete.DataOut;
using sReportsV2.DTOs.Patient;
using sReportsV2.DTOs.User.DTO;
using System.Collections.Generic;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IDigitalGuidelineInstanceBLL
    {
        PatientDataOut GetGuidelineInstance(int episodeOfCareId);
        GuidelineDataOut GetGraph(string guidelineInstanceId, string guidelineId);
        ResourceCreatedDTO InsertOrUpdate(GuidelineInstanceDataIn guidelineInstance);
        List<GuidelineInstanceDataOut> GetGuidelineInstancesByEOC(int episodeOfCareId);
        bool Delete(string guidelineInstanceId);
        GuidelineInstanceViewDataOut ListDigitalGuidelines(int? episodeOfCareId, UserCookieData userCookieData);
        GuidelineInstanceViewDataOut ListDigitalGuidelineDocuments(int episodeOfCareId, UserCookieData userCookieData);
        GuidelineElementDataDataOut PreviewInstanceNode(GuidelineElementDataDataIn dataIn);
        string GetValueFromDocument(string formInstanceId, int thesaurusId);
        void MarksAsCompleted(string value, string nodeId, string guidelineInstanceId);
        List<AutocompleteOptionDataOut> GetConditions(string nodeId, string digitalGuidelineId);
        void SaveCondition(string condition, string nodeId, string guidelineInstanceId, string digitalGuidelineId);
    }
}
