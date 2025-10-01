using AutoMapper;
using Azure.AI.OpenAI;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.DAL.MongoDb.Interfaces;
using sReportsV2.Domain.MongoDb.Entities.Promp;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.UploadPatientData;
using sReportsV2.DTOs.DTOs.UploadPatientData.DataIn;
using sReportsV2.DTOs.DTOs.UploadPatientData.DataOut;
using sReportsV2.DTOs.Field.DataOut;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using sReportsV2.Common.Helpers;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class UploadPatientDataBLL : IUploadPatientDataBLL
    {
        private readonly IBlobStorageBLL blobStorageBLL;
        private readonly IUploadPatientDataDAL uploadPatientDataDAL;
        private readonly IPromptConfigurationDAL promptConfigurationDAL;
        private readonly IFormDAL formDAL;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public UploadPatientDataBLL(IBlobStorageBLL blobStorageBLL, IUploadPatientDataDAL uploadPatientDataDAL, IMapper mapper, IPromptConfigurationDAL promptConfigurationDAL, IConfiguration configuration, IFormDAL formDAL)
        {
            this.blobStorageBLL = blobStorageBLL;
            this.uploadPatientDataDAL = uploadPatientDataDAL;
            this.mapper = mapper;
            this.promptConfigurationDAL = promptConfigurationDAL;
            this.configuration = configuration;
            this.formDAL = formDAL;
        }

        public async Task<PaginationDataOut<UploadPatientDataOut, UploadPatientFilterDataIn>> ReloadData(UploadPatientFilterDataIn dataIn)
        {
            PaginationData<UploadPatientData> patientPagination = await uploadPatientDataDAL.GetAllAndCount(mapper.Map<UploadPatientDataFilter>(dataIn)).ConfigureAwait(false);
            PaginationDataOut<UploadPatientDataOut, UploadPatientFilterDataIn> result = new PaginationDataOut<UploadPatientDataOut, UploadPatientFilterDataIn>()
            {
                Count = patientPagination.Count,
                Data = mapper.Map<List<UploadPatientDataOut>>(patientPagination.Data),
                DataIn = dataIn
            };
            return result;
        }

        public async Task UploadPatientData(IFormFileCollection files, string domain)
        {
            files = await MergeToZipIfNecessary(files);
            List<UploadPatientData> uploadPatientData = new List<UploadPatientData>();
            foreach (var file in files)
            {
                string path = await blobStorageBLL.CreateAsync(file, domain).ConfigureAwait(false);
                uploadPatientData.Add(mapper.Map<UploadPatientData>(new UploadPatientDataIn { UploadPath = path }));
            }
            uploadPatientDataDAL.InsertOrUpdate(uploadPatientData);
        }

        private async Task<IFormFileCollection> MergeToZipIfNecessary(IFormFileCollection files)
        {
            if (files.Count > 1 && files.All(file => !IsZip(file.Name)))
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(files[0].FileName);
                string fileZipName = $"{fileNameWithoutExtension}.zip";
                string zipFilePath = Path.Combine(Path.GetTempPath(), fileZipName);

                try
                {
                    using (var zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
                    {
                        foreach (var file in files)
                        {
                            // Add each file to the ZIP archive
                            var entry = zipArchive.CreateEntry(file.FileName);

                            using (var entryStream = entry.Open())
                            using (var fileStream = file.OpenReadStream())
                            {
                                await fileStream.CopyToAsync(entryStream);
                            }
                        }
                    }

                    // Return the ZIP file as a download
                    byte[] zipBytes = await System.IO.File.ReadAllBytesAsync(zipFilePath);
                    return ConvertToFormFileCollection(zipBytes, fileZipName);
                }
                finally
                {
                    // Delete the temporary ZIP file
                    if (System.IO.File.Exists(zipFilePath))
                    {
                        System.IO.File.Delete(zipFilePath);
                    }
                }
            }
            else
            {
                return files;
            }
        }

        private IFormFileCollection ConvertToFormFileCollection(byte[] zipBytes, string fileName)
        {
            var formFileCollection = new FormFileCollection();
            var memoryStream = new MemoryStream(zipBytes);
            memoryStream.Position = 0;
            var formFile = new FormFile(memoryStream, 0, zipBytes.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/zip"
            };

            formFileCollection.Add(formFile);

            return formFileCollection;
        }

        private bool IsZip(string fileName)
        {
            return Path.GetExtension(fileName) == ".zip";
        }

        private bool IsTxt(string fileName)
        {
            return Path.GetExtension(fileName) == ".txt";
        }



        #region LLM
        public async Task<List<PromptResultDataOut>> ProceedLLM(int uploadPatientDataId)
        {
            UploadPatientData uploadPatientData = await uploadPatientDataDAL.GetById(uploadPatientDataId);
            Stream stream = await blobStorageBLL.DownloadAsync(new DTOs.Common.BinaryMetadataDataIn { Domain = StorageDirectoryNames.UploadPatientData, ResourceId = uploadPatientData.UploadPath });
            if (IsZip(uploadPatientData.UploadPath))
            {
                stream = await MergeTxtFilesFromZip(stream);
            }
            else if (!IsTxt(uploadPatientData.UploadPath))
            {
                throw new NotImplementedException("Other file formats are not supported");
            }
            string stringStream = StreamToString(stream);
            List<PromptForm> promptForms = await promptConfigurationDAL.GetAll();
            PromptForm promptForm = promptForms.FirstOrDefault();
            List<PromptResultDataOut> promptResults = new List<PromptResultDataOut>();
            if (promptForm != null)
            {
                promptResults = await PrepareResults(promptForm);
                await CallAzureOpenAI(stringStream, promptResults);

            }
            return promptResults;
        }

        private async Task<Stream> MergeTxtFilesFromZip(Stream zipStream)
        {
            if (zipStream == null || !zipStream.CanRead)
                throw new InvalidOperationException("The stream is not readable.");

            // Reset stream position if possible
            if (zipStream.CanSeek)
            {
                zipStream.Seek(0, SeekOrigin.Begin);
            }

            List<FileByDateDataOut> files = new List<FileByDateDataOut>();
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Read, leaveOpen: true))
            {
                foreach (var entry in archive.Entries)
                {
                    if (IsTxt(entry.FullName) && !string.IsNullOrEmpty(entry.Name))
                    {
                        DateOnly? date = ExtractDateFromFileName(entry);
                        if (date.HasValue)
                        {
                            using var entryStream = entry.Open();
                            using var reader = new StreamReader(entryStream);
                            files.Add(new FileByDateDataOut
                            {
                                Content = await reader.ReadToEndAsync().ConfigureAwait(false),
                                Date = date.Value
                            });
                        }
                    }
                }
            }
            var mergedStream = new MemoryStream();
            using (var writer = new StreamWriter(mergedStream, Encoding.UTF8, leaveOpen: true))
            {
                foreach (FileByDateDataOut fileByDateData in files.OrderBy(f => f.Date))
                {
                    writer.WriteLine(fileByDateData.Content);
                    writer.WriteLine();
                }

            }

            mergedStream.Seek(0, SeekOrigin.Begin);
            return mergedStream;
        }

        private DateOnly? ExtractDateFromFileName(ZipArchiveEntry entry)
        {
            string pattern = @"\d{4}-\d{2}-\d{2}";
            Match match = Regex.Match(entry.FullName, pattern);
            string dateString = string.Empty;
            DateOnly? date = null;
            if (match.Success)
            {
                dateString = match.Value;
                if (DateOnly.TryParseExact(dateString, DateTimeConstants.UTCDatePartFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly dateOnly))
                {
                    date = dateOnly;
                }
            }
            return date;
        }

        private string StreamToString(Stream stream)
        {
            using var reader = new StreamReader(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return reader.ReadToEnd();
        }

        private async Task CallAzureOpenAI(string input, List<PromptResultDataOut> prompts)
        {
            string uri = configuration["AzureOpenAIUrl"];
            string keyCredential = configuration["AzureOpenAIKeyCredential"];

            AzureOpenAIClient azureClient = new(
                new Uri(uri),
                new AzureKeyCredential(keyCredential)
            );

            List<ChatMessage> chatMessages = new List<ChatMessage>
            {
                new SystemChatMessage(input)
            };
            chatMessages.AddRange(prompts.Select(p => new UserChatMessage(p.Prompt)));
            ChatClient chatClient = azureClient.GetChatClient(configuration["AzureOpenAIVersion"]);
            ChatCompletionOptions options = new()
            {
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                    jsonSchemaFormatName: "patient_llm_extraction",
                    jsonSchema: BinaryData.FromString($@"
                    {{
                        ""type"": ""object"",
                        ""properties"": {{
                            {GetProperties(prompts)}
                        }},
                        ""required"": [{GetRequiredValues(prompts)}],
                        ""additionalProperties"": false
                    }}
                    "),
                    jsonSchemaIsStrict: true)
            };
            ChatCompletion chatCompletion = await chatClient.CompleteChatAsync(chatMessages, options);
            JsonDocument structuredJson = JsonDocument.Parse(chatCompletion.Content[0].Text);
            foreach (PromptResultDataOut prompt in prompts)
            {
                if (structuredJson.RootElement.TryGetProperty(prompt.Field.Id, out JsonElement jsonElement))
                {
                    prompt.Answer = jsonElement.ToString();
                }
                else
                {
                    LogHelper.Warning($"Answer for {prompt.Field.Label} is not found");
                }
            }
        }

        private string GetProperties(List<PromptResultDataOut> prompts)
        {
            return string.Join(',', prompts.Select(p => $@"""{p.Field.Id}"": {{""type"": ""string""}}"));
        }

        private string GetRequiredValues(List<PromptResultDataOut> prompts)
        {
            return string.Join(',', prompts.Select(p => "\"" + p.Field.Id + "\""));
        }

        private async Task<List<PromptResultDataOut>> PrepareResults(PromptForm promptForm)
        {
            FormDataOut form = mapper.Map<FormDataOut>(await formDAL.GetFormAsync(promptForm.FormId));
            List<PromptResultDataOut> promptResults = new List<PromptResultDataOut>();
            //foreach (FormPageDataOut page in form.Chapters.SelectMany(f => f.Pages))
            //{
            //    foreach (FormFieldSetDataOut fieldSet in page.ListOfFieldSets.SelectMany(fs => fs))
            //    {
            //        foreach (FieldDataOut field in fieldSet.Fields)
            //        {
            //            promptResults.Add(new PromptResultDataOut
            //            {
            //                Field = field,
            //                Prompt = $"{page.Title} -> {fieldSet.Label} -> {field.Label} and map answer to {{{field.Id}}}"
            //            });
            //        }
            //    }
            //}
            List<PromptField> promptFields = promptForm.GetPromptFormVersion(promptForm.CurrentVersionId).GetPromptStructure();
            List<FieldDataOut> fields = form.GetAllFields();
            foreach (PromptField promptField in promptFields)
            {
                PromptResultDataOut promptResult = new PromptResultDataOut
                {
                    Prompt = promptField.Prompt
                };
                if (string.IsNullOrEmpty(promptField.FieldId))
                {
                    promptResult.FormName = form.Title;
                }
                else
                {
                    promptResult.Field = fields.FirstOrDefault(f => f.Id == promptField.FieldId);
                    promptResult.Prompt += $" and map answer to {{{promptResult.Field.Id}}}";
                    promptResults.Add(promptResult);
                }
            }
            return promptResults;
        }
        #endregion /LLM
    }
}
