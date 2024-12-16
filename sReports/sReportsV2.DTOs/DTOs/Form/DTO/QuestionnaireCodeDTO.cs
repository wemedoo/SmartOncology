namespace sReportsV2.DTOs.DTOs.Form.DTO
{
    public class QuestionnaireCodeDTO
    {
        public string System { get; set; }
        public string Code { get; set; }
        public string Display { get; set; }

        public QuestionnaireCodeDTO()
        {
        }

        public QuestionnaireCodeDTO(string urlTemplate, string code, string display)
        {
            System = urlTemplate + code;
            Code = code;
            Display = display;
        }
    }
}
