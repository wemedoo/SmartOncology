namespace sReportsV2.Domain.Entities.FieldEntity
{
    public class FieldString : Field
    {
        public bool IsRepetitive { get; set; }
        public int NumberOfRepetitions { get; set; }

        public override bool IsFieldRepetitive()
        {
            return IsRepetitive;
        }
    }
}
