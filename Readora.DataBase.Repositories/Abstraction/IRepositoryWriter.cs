namespace Readora.DataBase.Repositories.Abstraction;

public interface IRepositoryWriter<in TEntity> where TEntity : class
{
    void Add(TEntity entity);

    void Update(TEntity entity);

    void Delete(TEntity entity);
}