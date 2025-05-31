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
        Task<List<Service>> GetAll();
        Task<Service?> GetById(int id);
        Task Add(Service service);
        Task Update(Service service);
    }

    public class ServiceRepository : IServiceRepository
    {
        private readonly AppDbContext _context;

        public ServiceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Service>> GetAll()
        {
            return await _context.Services
                .Include(s => s.Applications)
                .Include(s => s.Rules)
                .ToListAsync();
        }

        public async Task<Service?> GetById(int id)
        {
            return await _context.Services
                .Include(s => s.Applications)
                .Include(s => s.Rules)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task Add(Service service)
        {
            await _context.Services.AddAsync(service);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Service service)
        {
            _context.Services.Update(service);
            await _context.SaveChangesAsync();
        }
    }

}
