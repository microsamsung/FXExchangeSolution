namespace FXExchange.Application.Interfaces;

public interface IRepository<T>
{
    Task<T?> Get(string id);

    Task Add(T entity);

    Task Update(T entity);

    Task Delete(T entity);
}