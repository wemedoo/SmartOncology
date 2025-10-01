namespace sReportsV2.Domain.MongoDb.Entities.Promp
{
    public class PromptFormFilter
    {
        public string FormId { get; set; }
        public int ProjectId { get; set; }
        public string FieldId { get; set; }
        public string VersionId { get; set; }
    }
}
