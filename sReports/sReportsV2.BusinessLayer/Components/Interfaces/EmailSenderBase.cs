using System;
using System.IO.Compression;
using System.IO;
using sReportsV2.DTOs.Common.DTO;
using System.Collections.Generic;
using sReportsV2.Common.Extensions;
using Microsoft.Extensions.Configuration;
using sReportsV2.SqlDomain.Interfaces;
using sReportsV2.Common.Helpers;

namespace sReportsV2.BusinessLayer.Components.Interfaces
{
    public abstract class EmailSenderBase : IEmailSender
    {
        public abstract void SendAsync(EmailDTO messageDto);
        protected readonly IConfiguration configuration;
        protected readonly string outputDirectory;

        protected EmailSenderBase(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.outputDirectory = DirectoryHelper.AppDataFolder;
        }

        protected void CreateFileZip(Stream file, string fileName, string outputPath, bool isCsv)
        {
            try
            {
                using (MemoryStream zipStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
                    {
                        using (var fileStream = file)
                        {
                            ZipArchiveEntry zipArchiveEntry;
                            if (isCsv)
                                zipArchiveEntry = archive.CreateEntry(fileName + ".csv");
                            else
                                zipArchiveEntry = archive.CreateEntry(fileName + ".xlsx");

                            using (var entryStream = zipArchiveEntry.Open())
                            {
                                fileStream.CopyTo(entryStream);
                            }
                        }
                    }
                    System.IO.File.WriteAllBytes(outputPath, zipStream.ToArray());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        protected void DeleteFile(Dictionary<string, Stream> files, string outputDirectory)
        {
            if (files != null)
            {
                foreach (var file in files)
                {
                    string sanitizedFileName = file.Key.SanitizeFileName();
                    string outputPath = outputDirectory.CombineFilePath(sanitizedFileName);
                    File.Delete(outputPath);
                }
            }
        }
    }
}
