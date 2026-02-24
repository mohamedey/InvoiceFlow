using Application.Data;
using Application.Interfaces.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _db;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _db = context.Set<T>();
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        return await _db.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _db.ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _db.Where(predicate).ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _db.AddAsync(entity);
    }

    public Task UpdateAsync(T entity)
    {
        _db.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity)
    {
        _db.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}