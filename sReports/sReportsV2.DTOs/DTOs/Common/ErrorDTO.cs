using Newtonsoft.Json;

namespace sReportsV2.DTOs.Common
{
    public class ErrorDTO
    {
        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }

        public ErrorDTO(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}
