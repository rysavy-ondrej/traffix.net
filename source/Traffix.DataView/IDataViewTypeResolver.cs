namespace Traffix.DataView
{
    /// <summary>
    /// An IDataViewTypeResolver is storage of typed records. 
    /// </summary>
    public interface IDataViewTypeResolver
    {
        IDataViewType<T> GetDataViewType<T>();
    }
}
