using Dashboard.Core.Data;
using Dashboard.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

public abstract class CrudService<TEntity, TId>
    : ICrudService<TEntity, TId>
    where TEntity : class, IEntity<TId>
{
    protected readonly AppDbContext _db;
    protected readonly DbSet<TEntity> _set;

    protected CrudService(AppDbContext db)
    {
        _db = db;
        _set = db.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(TId id)
        => await _set.FindAsync(id);

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        => await _set.ToListAsync();

    public virtual async Task<TEntity> CreateAsync(TEntity entity)
    {
        _set.Add(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity)
    {
        _set.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<bool> DeleteAsync(TId id)
    {
        var entity = await _set.FindAsync(id);
        if (entity == null)
            return false;

        _set.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }
}
