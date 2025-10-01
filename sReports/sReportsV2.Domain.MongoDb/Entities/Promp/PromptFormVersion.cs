using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace sReportsV2.Domain.MongoDb.Entities.Promp
{
    public class PromptFormVersion
    {
        [BsonDateTimeOptions(Representation = BsonType.Document)]
        public DateTime CreatedOn { get; set; }
        public int CreatedById { get; set; }
        public string Prompt {  get; set; }
        public sReportsV2.Domain.Entities.Form.Version Version { get; set; }
        public List<PromptField> PromptFields { get; set; }

        public void AddPromptField(PromptField promptField)
        {
            if (PromptFields == null)
            {
                PromptFields = new List<PromptField>();
            }
            else
            {
                PromptFields.Add(promptField);
            }
        }

        public List<PromptField> GetPromptStructure()
        {
            List<PromptField> prompts = new List<PromptField>();
            if (!string.IsNullOrEmpty(Prompt))
            {
                prompts.Add(new PromptField { Prompt = Prompt });
            }
            prompts.AddRange(PromptFields.Where(pf => !string.IsNullOrEmpty(pf.Prompt)).ToList());

            return prompts;
        }
    }
}
