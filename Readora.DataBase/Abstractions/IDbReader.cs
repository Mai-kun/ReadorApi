namespace Readora.DataBase.Abstractions;

public interface IDbReader
{
    IQueryable<TEntity> Read<TEntity>() where TEntity : class;
}