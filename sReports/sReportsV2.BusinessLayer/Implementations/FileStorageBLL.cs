using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using sReportsV2.BusinessLayer.Helpers;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Exceptions;
using sReportsV2.Common.Helpers;
using sReportsV2.DTOs.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class FileStorageBLL : BlobStorageBase
    {
        private readonly string uploadFolderName;

        public FileStorageBLL(IConfiguration configuration) : base(configuration)
        {
            this.uploadFolderName = Path.Combine(GetUploadedBaseDirectory(), "UploadedFiles");
        }

        public async override Task<string> CreateAsync(IFormFile file, string domain)
        {
            string resourceUrl = string.Empty;

            if (file != null && file.Length > 0)
            {
                string generatedResourceName = GetUniqueResourceName(file.FileName);
                string filePath = GetFilePath(domain, generatedResourceName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                resourceUrl = generatedResourceName;
            }

            return resourceUrl;
        }

        public override async Task<string> CreateAudio(byte[] fileData, string fileName)
        {
            string resourceUrl = string.Empty;

            if (fileData != null && fileData.Length > 0)
            {
                string generatedResourceName = GetUniqueResourceName(fileName);
                string filePath = GetFilePath(StorageDirectoryNames.Audio, generatedResourceName);
                File.WriteAllBytes(filePath, fileData);

                resourceUrl = generatedResourceName;
            }

            return resourceUrl;
        }

        public override void Delete(BinaryMetadataDataIn data)
        {
            try
            {
                string path = GetFilePath(data.Domain, data.ResourceId);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                else
                {
                    LogHelper.Warning($"Delete binary operation was not executed because file with the path {path} does not exist");
                }
            }
            catch (Exception ex)
            {
                throw new UserAdministrationException((int)HttpStatusCode.Conflict, "An error occurred while attempting to delete the binary file.");
            }
        }

        public override async Task<Stream> DownloadAsync(BinaryMetadataDataIn data)
        {
            string filePath = GetFilePath(data.Domain, data.ResourceId);
            byte[] readedFile = File.ReadAllBytes(filePath);
            Stream stream = new MemoryStream(readedFile);
            return stream;
        }

        private string GetUploadedBaseDirectory()
        {
            string uploadedFilesBaseDirectory = DirectoryHelper.ProjectBaseDirectory;
            if (!string.IsNullOrWhiteSpace(configuration["UploadedFilesBaseDirectory"]))
                uploadedFilesBaseDirectory = configuration["UploadedFilesBaseDirectory"];
            return uploadedFilesBaseDirectory;
        }

        private void CreateDirectoryIfNotExist(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private string FormatFullDirectoryName(string subDirectoryDomain)
        {
            return Path.Combine(uploadFolderName, subDirectoryDomain);
        }

        private string FormatFullFilePath(string fullDirectoryPath, string generatedResourceName)
        {
            return Path.Combine(fullDirectoryPath, generatedResourceName);
        }

        private string GetFilePath(string domain, string resourceId)
        {
            string directory = FormatFullDirectoryName(domain);
            CreateDirectoryIfNotExist(directory);
            return FormatFullFilePath(directory, resourceId);
        }
    }
}