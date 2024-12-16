using Azure.Storage.Blobs;
using sReportsV2.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Common.Helpers
{
    public static class CloudStorageHelper
    {
        public static async Task<BlobClient> GetOrCreateBlob(string resourceId, string domain, string connectionString)
        {
            BlobContainerClient container = await GetCloudBlobContainer(connectionString).ConfigureAwait(false);
            return container.GetBlobClient(FormatFullFilePath(resourceId, domain));
        }

        public static async Task<BlobContainerClient> GetCloudBlobContainer(string connectionString)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient container = blobServiceClient.GetBlobContainerClient(StorageConstants.ContainerName);
            await container.CreateIfNotExistsAsync().ConfigureAwait(false);

            return container;
        }

        public static string FormatFullFilePath(string resourceId, string domain)
        {
            return $"{domain}/{resourceId}";
        }

        
    }
}
