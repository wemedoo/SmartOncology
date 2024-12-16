using System.Collections.Generic;

namespace sReportsV2.DTOs.DTOs.Form.DataIn.QuestionnaireResponse
{
    public class QuestionnaireResponseDataIn
    {
        public string ResourceType { get; set; }
        public string Status { get; set; }
        public string Questionnaire { get; set; }
        public string Id { get; set; }
        public QuestionnaireResponseReferenceDataIn Subject { get; set; }
        public string Authored { get; set; }
        public List<QuestionnaireResponseItemDataIn> Item { get; set; }
    }
}
