using Readora.DataBase.Abstractions;

namespace Readora.DataBase.Repositories.Abstraction;

public abstract class BaseWriteDbRepository<T> : IRepositoryWriter<T> where T : class
{
    private readonly IDbWriter _writer;

    protected BaseWriteDbRepository(IDbWriter writer)
    {
        _writer = writer;
    }

    void IRepositoryWriter<T>.Add(T entity)
    {
        _writer.Add(entity);
    }

    void IRepositoryWriter<T>.Update(T entity)
    {
        _writer.Update(entity);
    }

    void IRepositoryWriter<T>.Delete(T entity)
    {
        _writer.Delete(entity);
    }
}