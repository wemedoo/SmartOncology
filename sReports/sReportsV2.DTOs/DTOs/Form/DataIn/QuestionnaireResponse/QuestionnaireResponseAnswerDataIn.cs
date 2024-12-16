using sReportsV2.DTOs.DTOs.Form.DTO;

namespace sReportsV2.DTOs.DTOs.Form.DataIn.QuestionnaireResponse
{
    public class QuestionnaireResponseAnswerDataIn
    {
        public string ValueString { get; set; }

        public int ValueInteger { get; set; }

        public double ValueDecimal { get; set; }

        public QuestionnaireCodeDTO ValueCoding { get; set; }
    }
}
