namespace Readora.DataBase.Abstractions;

public interface IDbWriter
{
    void Add<TEntity>(TEntity entity) where TEntity : class;

    void Update<TEntity>(TEntity entity) where TEntity : class;

    void Delete<TEntity>(TEntity entity) where TEntity : class;
}