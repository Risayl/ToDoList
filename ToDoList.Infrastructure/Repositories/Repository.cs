using Microsoft.EntityFrameworkCore;
using ToDoList.Models;
using ToDoList.Data;

namespace ToDoList.Repositories;

public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(int? id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int? id);
    IQueryable<T> Query();
}

public interface ITagRepository : IRepository<Tag>
{
    Task<Tag> GetByNameAsync(string name);
}

public class Repository<T> : IRepository<T>, IDisposable, IAsyncDisposable where T : class
{
    protected ApplicationDbContext DbContext { get; }
    protected DbSet<T> DbSet { get; }
    public Repository(ApplicationDbContext dbContext)
    {
        DbContext = dbContext;
        DbSet = dbContext.Set<T>();
    }

    public async Task<T> GetByIdAsync(int? id)
    {
        var query = DbSet.AsQueryable();

        if (typeof(T) == typeof(Note) || typeof(T) == typeof(Reminder))
        {
            query = query.Include("Tags");
        }
        return await query.FirstOrDefaultAsync(entity => EF.Property<int>(entity, "Id") == id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var query = DbSet.AsQueryable();

        if (typeof(T) == typeof(Note) || typeof(T) == typeof(Reminder))
        {
            query = query.Include("Tags");
        }
        return await query.ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        await DbSet.AddAsync(entity);
        await DbContext.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        DbSet.Update(entity);
        await DbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int? id)
    {
        var entity = await DbSet.FindAsync(id);
        if (entity != null)
        {
            DbSet.Remove(entity);
            await DbContext.SaveChangesAsync();
        }
    }
    public IQueryable<T> Query()
    {
        return DbSet;
    }

    public void Dispose()
    {
        DbContext.Dispose();
    }
    public async ValueTask DisposeAsync()
    {
        await DbContext.DisposeAsync();
    }
}

public class TagRepository : Repository<Tag>, ITagRepository
{
    public TagRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Tag> GetByNameAsync(string name)
    {
        return await DbContext.Tags.FirstOrDefaultAsync(t => t.Name == name);
    }
}