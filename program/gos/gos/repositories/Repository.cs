using gos.repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gos.repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<List<T>> GetAll() => await _dbSet.ToListAsync();

        public async Task<T?> GetById(int id) => await _dbSet.FindAsync(id);

        public async Task Add(T entity) => await _dbSet.AddAsync(entity);

        public async Task Update(T entity) => _dbSet.Update(entity);

        public async Task Delete(T entity) => _dbSet.Remove(entity);

        public async Task SaveChanges() => await _context.SaveChangesAsync();
    }

}
