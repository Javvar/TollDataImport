namespace Intertoll.Toll.DataImport.Interfaces
{
    public interface ITollEntityBuilder<T>
    {
        T Build(T entity);
    }
}
