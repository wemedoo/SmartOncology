namespace sReportsV2.DTOs.DTOs.Field.DataOut
{
    public interface IBinaryFieldDataOut
    {
        bool ExcludeGUIDPartFromName { get; }
        string RemoveClass { get; }
        string BinaryType { get; }
    }
}
