using System.Collections.Generic;

namespace sReportsV2.DTOs.DTOs.Form.DataIn.QuestionnaireResponse
{
    public class QuestionnaireResponseItemDataIn
    {
        public string LinkId { get; set; }
        public string Text { get; set; }
        public List<QuestionnaireResponseAnswerDataIn> Answer { get; set; }
        public List<QuestionnaireResponseItemDataIn> Item { get; set; }
    }
}
