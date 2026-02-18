
namespace Dashboard.Core.Interfaces
{
    public interface ICrudService<TEntity, TId>
    where TEntity : class, IEntity<TId>
    {
        Task<TEntity?> GetByIdAsync(TId id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> CreateAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task<bool> DeleteAsync(TId id);
    }
}
