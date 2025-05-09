using Readora.Models.Abstractions;

namespace Readora.DataBase.Repositories;

public static class Specifications
{

    public static IQueryable<TEntity> ById<TEntity>(this IQueryable<TEntity> query, int id)
        where TEntity : class, IEntityWithIntId
    {
        return query.Where(x => x.Id == id);
    }
    
    public static IQueryable<TEntity> ById<TEntity>(this IQueryable<TEntity> query, Guid id)
        where TEntity : class, IEntityWithGuidId
    {
        return query.Where(x => x.Id == id);
    }
}