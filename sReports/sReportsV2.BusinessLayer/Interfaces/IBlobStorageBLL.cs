using sReportsV2.DTOs.Common;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IBlobStorageBLL
    {
        Task<string> CreateAsync(IFormFile file, string domain);
        Task<Stream> DownloadAsync(BinaryMetadataDataIn data);
        void Delete(BinaryMetadataDataIn data);
        Task<string> CreateAudio(byte[] fileData, string fileName);
        void SaveChunk(byte[] fileChunk, string tempDirectory);
        Task<string> MergeAudioFiles(string tempDirectory);
    }
}
