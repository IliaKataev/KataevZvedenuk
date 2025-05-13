using gos.models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gos.repositories
{
    public interface IServiceRepository
    {
        Task<List<Service>> GetAllAsync();
        Task<Service?> GetByIdAsync(int id);
        Task AddAsync(Service service);
        Task UpdateAsync(Service service);
    }

    public class ServiceRepository : IServiceRepository
    {
        private readonly AppDbContext _context;

        public ServiceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Service>> GetAllAsync()
        {
            return await _context.Services
                .Include(s => s.Applications)
                .Include(s => s.Rules)
                .ToListAsync();
        }

        public async Task<Service?> GetByIdAsync(int id)
        {
            return await _context.Services
                .Include(s => s.Applications)
                .Include(s => s.Rules)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task AddAsync(Service service)
        {
            await _context.Services.AddAsync(service);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Service service)
        {
            _context.Services.Update(service);
            await _context.SaveChangesAsync();
        }
    }

}
