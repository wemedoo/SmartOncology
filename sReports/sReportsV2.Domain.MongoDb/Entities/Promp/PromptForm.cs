using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using sReportsV2.Domain.Entities;

namespace sReportsV2.Domain.MongoDb.Entities.Promp
{
    public class PromptForm : Entity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string FormId { get; set; }
        public int ProjectId { get; set; }
        public string CurrentVersionId { get; set; }
        public List<PromptFormVersion> PromptFormVersions { get; set; }

        public PromptForm()
        {
        }

        public PromptForm(string formId, int projectId, string fieldId, string versionId, string prompt, int userId)
        {
            List<PromptField> promptFields = new List<PromptField>();
            if (!string.IsNullOrEmpty(fieldId))
            {
                promptFields.Add(new PromptField
                {
                    FieldId = fieldId,
                    Prompt = prompt
                });
            }

            this.FormId = formId;
            this.ProjectId = projectId;
            this.EntryDatetime = DateTime.Now;
            this.CurrentVersionId = versionId;
            this.PromptFormVersions = new List<PromptFormVersion>
            {
                new PromptFormVersion
                {
                    CreatedById = userId,
                    CreatedOn = DateTime.Now,
                    Prompt = string.IsNullOrEmpty(fieldId) ? prompt : string.Empty,
                    Version = new Domain.Entities.Form.Version
                    {
                        Id = versionId,
                        Major = 1
                    },
                    PromptFields = promptFields
                }
            };
        }

        public PromptFormVersion GetPromptFormVersion(string versionId)
        {
            PromptFormVersion promptFormVersion = null;
            if (string.IsNullOrEmpty(versionId))
            {
                promptFormVersion = PromptFormVersions.OrderByDescending(p => p.CreatedOn).FirstOrDefault();
            }
            else
            {
                promptFormVersion = PromptFormVersions.Find(p => p.Version.Id == versionId);
            }

            return promptFormVersion;
        }

        public string UpdatePrompt(string versionId, string fieldId, string prompt)
        {
            PromptFormVersion promptFormVersion = this.PromptFormVersions.Find(pf => pf.Version.Id == versionId);
            if (promptFormVersion != null)
            {
                if (string.IsNullOrEmpty(fieldId))
                {
                    promptFormVersion.Prompt = prompt;
                }
                else
                {
                    PromptField promptFieldDb = promptFormVersion.PromptFields?.Find(pf => pf.FieldId == fieldId);
                    if (promptFieldDb != null)
                    {
                        promptFieldDb.Prompt = prompt;
                    }
                    else
                    {
                        promptFormVersion.AddPromptField(new PromptField
                        {
                            FieldId = fieldId,
                            Prompt = prompt
                        });
                    }
                }
            }

            return promptFormVersion?.Version?.Id;
        }

        public Domain.Entities.Form.Version AddNewVersion(int userId)
        {
            PromptFormVersion promptFormVersion = GetPromptFormVersion(string.Empty);
            string versionId = Guid.NewGuid().ToString();
            Domain.Entities.Form.Version newVersion = new Domain.Entities.Form.Version
            {
                Id = versionId,
                Major = promptFormVersion.Version.Major,
                Minor = promptFormVersion.Version.Minor + 1
            };
            this.PromptFormVersions.Add(new PromptFormVersion()
            {
                CreatedById = userId,
                CreatedOn = DateTime.Now,
                Version = newVersion,
                PromptFields = new List<PromptField>()
            });
            this.CurrentVersionId = versionId;
            return newVersion;
        }
    }
}