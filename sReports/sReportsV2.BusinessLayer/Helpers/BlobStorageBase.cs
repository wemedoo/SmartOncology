using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.DTOs.Common;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Helpers
{
    public abstract class BlobStorageBase : IBlobStorageBLL
    {
        public readonly IConfiguration configuration;

        protected BlobStorageBase(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public abstract Task<string> CreateAsync(IFormFile file, string domain);
        public abstract Task<Stream> DownloadAsync(BinaryMetadataDataIn data);
        public abstract void Delete(BinaryMetadataDataIn data);
        public abstract Task<string> CreateAudio(byte[] fileData, string fileName);

        public void SaveChunk(byte[] fileChunk, string tempDirectory)
        {
            if (!Directory.Exists(tempDirectory))
            {
                Directory.CreateDirectory(tempDirectory);
            }
            string chunkFileName = Guid.NewGuid().ToString();

            string chunkFilePath = Path.Combine(tempDirectory, chunkFileName);

            File.WriteAllBytes(chunkFilePath, fileChunk);
        }

        public async Task<string> MergeAudioFiles(string tempDirectory)
        {
            string finalFilePath = "";

            try
            {
                string[] chunkFiles = Directory.GetFiles(tempDirectory);
                var sortedFiles = chunkFiles.Select(f => new FileInfo(f))
                                       .OrderBy(f => f.CreationTime)
                                       .Select(f => f.FullName)
                                       .ToArray();

                using (MemoryStream assembledFileStream = new MemoryStream())
                {
                    foreach (string chunkFile in sortedFiles)
                    {
                        using (FileStream chunkFileStream = new FileStream(chunkFile, FileMode.Open))
                        {
                            chunkFileStream.CopyTo(assembledFileStream);
                        }
                    }

                    byte[] assembledFileData = assembledFileStream.ToArray();
                    finalFilePath = await CreateAudio(assembledFileData, "audio.mp3");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error assembling MP3 file: " + ex.Message);
            }

            Directory.Delete(tempDirectory, true);

            return finalFilePath;
        }

        protected string GetUniqueResourceName(string simpleResourceName)
        {
            return $"{Guid.NewGuid()}_{simpleResourceName}";
        }
    }
}