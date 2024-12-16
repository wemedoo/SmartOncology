using Microsoft.AspNetCore.Http;

namespace sReportsV2.DTOs.Common.DTO
{
    public class CustomFormFile
    {
        private readonly IFormFile _originalFile;

        public int ContentLength => (int)(_originalFile?.Length ?? 0);
        public string ContentType => _originalFile?.ContentType;
        public string FileName => _originalFile?.FileName;

        public CustomFormFile(IFormFile originalFile)
        {
            _originalFile = originalFile;
        }
    }
}
