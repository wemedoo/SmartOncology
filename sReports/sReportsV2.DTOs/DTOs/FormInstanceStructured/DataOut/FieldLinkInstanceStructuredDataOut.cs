namespace sReportsV2.DTOs.Field.DataOut
{
    public partial class FieldLinkDataOut
    {
        public override string GetChildFieldInstanceCssSelector(string fieldInstanceRepetitionId)
        {
            return $"#{fieldInstanceRepetitionId}";
        }
    }
}
