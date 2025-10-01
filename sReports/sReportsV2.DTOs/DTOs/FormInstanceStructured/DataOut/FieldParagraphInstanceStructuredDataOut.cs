namespace sReportsV2.DTOs.Field.DataOut
{
    public partial class FieldParagraphDataOut
    {
        public override string GetChildFieldInstanceCssSelector(string fieldInstanceRepetitionId)
        {
            return $"#{fieldInstanceRepetitionId}";
        }
    }
}
