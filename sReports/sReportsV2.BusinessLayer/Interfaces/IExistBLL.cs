namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IExistBLL<in T>
    {
        bool ExistEntity(T dataIn);
    }
}
