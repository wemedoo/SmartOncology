using sReportsV2.Common.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Common.Helpers
{
    public static class FileZip
    {
        public static void CreateFileZip(Stream file, string fileName, string outputPath, bool isCsv)
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

        public static void DeleteFile(Dictionary<string, Stream> files, string outputDirectory) 
        {
            if (files != null)
            {
                foreach (var file in files)
                {
                    string sanitizedFileName = file.Key.SanitizeFileName();
                    string outputPath = outputDirectory.CombineFilePath(sanitizedFileName);
                    System.IO.File.Delete(outputPath);
                }
            }
        }
    }
}
