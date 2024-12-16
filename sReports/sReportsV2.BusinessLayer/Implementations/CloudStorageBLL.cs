using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using sReportsV2.BusinessLayer.Helpers;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Helpers;
using sReportsV2.DTOs.Common;
using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
namespace sReportsV2.BusinessLayer.Implementations
{
    public class CloudStorageBLL : BlobStorageBase
    {
        public CloudStorageBLL(IConfiguration configuration) : base(configuration)
        {
        }

        public async override Task<string> CreateAsync(IFormFile file, string domain)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File cannot be null or empty.", nameof(file));

            string generatedResourceName = GetUniqueResourceName(file.FileName);
            BlobClient cloudBlockBlob = await CloudStorageHelper.GetOrCreateBlob(generatedResourceName, domain, configuration["AccountStorage"]).ConfigureAwait(false);

            using (MemoryStream stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;

                await cloudBlockBlob.UploadAsync(stream);
            }

            return generatedResourceName;
        }

        public override async Task<string> CreateAudio(byte[] fileData, string fileName)
        {
            using (MemoryStream stream = new MemoryStream(fileData))
            {
                string generatedResourceName = GetUniqueResourceName(fileName);
                BlobClient cloudBlockBlob = await CloudStorageHelper.GetOrCreateBlob(generatedResourceName, StorageDirectoryNames.Audio, configuration["AccountStorage"]).ConfigureAwait(false);
                await cloudBlockBlob.UploadAsync(stream).ConfigureAwait(false);

                return generatedResourceName;
            }
        }

        public override async void Delete(BinaryMetadataDataIn data)
        {
            BlobClient cloudBlockBlob = await CloudStorageHelper.GetOrCreateBlob(data.ResourceId, data.Domain, configuration["AccountStorage"]).ConfigureAwait(false);
            bool isDeleted = await cloudBlockBlob.DeleteIfExistsAsync().ConfigureAwait(false);
            if (!isDeleted) 
            {
                LogHelper.Warning($"Delete binary operation was not executed because binary with the resource name {data.ResourceId} does not exist");
            }
        }

        public override async Task<Stream> DownloadAsync(BinaryMetadataDataIn data)
        {
            if (!string.IsNullOrWhiteSpace(data.ResourceId))
            {
                BlobClient cloudBlockBlob = await CloudStorageHelper.GetOrCreateBlob(data.ResourceId, data.Domain, configuration["AccountStorage"]).ConfigureAwait(false);
                Stream blobStream = await cloudBlockBlob.OpenReadAsync().ConfigureAwait(false);
                return blobStream;
            }
            else
            {
                return null;
            }
        }
    }
}