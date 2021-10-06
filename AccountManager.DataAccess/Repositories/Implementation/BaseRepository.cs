using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountManager.DataAccess.Repositories.Interfaces;
using AccountManager.Domain.Interfaces;

namespace AccountManager.DataAccess.Repositories.Implementation
{
    public class BaseRepository<T, TKey, TContext> : IRepository<T, TKey> 
        where T : class, Domain.Interfaces.IEntity<TKey>
        where TContext : DbContext, IUnitOfWork
    {
        protected readonly DbSet<T> _dbSet;
        protected readonly TContext _context;

        public BaseRepository(TContext context)
        {
            _dbSet = context.Set<T>();
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task<T> GetByIdAsync(TKey id)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id.Equals(id));
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public Task<T> InsertAsync(T item)
        {
            var entity = _dbSet.Add(item);
            return Task.FromResult(entity.Entity);
        }

        public Task UpdateAsync(T item)
        {
            _context.Entry(item).State = EntityState.Modified;
            return Task.CompletedTask;
        }
        public Task DeleteAsync(T item)
        {
            _dbSet.Remove(item);
            return Task.CompletedTask;
        }

        public async Task<T> GetSingleAsync(ISpecification<T> specification)
        {
            return await ApplySpecification(specification).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetManyAsync(ISpecification<T> specification)
        {
            return await ApplySpecification(specification).ToListAsync();
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> specification)
        {
            var evaluator = new SpecificationEvaluator();
            return evaluator.GetQuery(_dbSet.AsNoTracking(), specification);
        }
    }
}
