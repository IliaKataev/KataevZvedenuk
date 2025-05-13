using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gos.models;

namespace gos.repositories
{
    public interface IApplicationRepository
    {
        Task<List<models.Application>> GetByUserIdAsync(int userId);
        Task<models.Application?> GetByIdAsync(int applicationId);
        Task<List<models.Application>> GetAllAsync();
        Task<List<models.Application>> GetByStatusAsync(ApplicationStatus status);
        Task AddAsync(models.Application app);
        Task UpdateAsync(models.Application app);
        Task DeleteAsync(int appId);
    }

    public class ApplicationRepository : IApplicationRepository
    {
        private readonly AppDbContext _context;

        public ApplicationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<models.Application>> GetByUserIdAsync(int userId)
        {
            return await _context.Applications
                .Include(a => a.Service)
                .Include(a => a.User)
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task<models.Application?> GetByIdAsync(int applicationId)
        {
            return await _context.Applications
                .Include(a => a.Service)
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == applicationId);
        }

        public async Task<List<models.Application>> GetAllAsync()
        {
            return await _context.Applications
                .Include(a => a.Service)
                .Include(a => a.User)
                .ToListAsync();
        }

        public async Task<List<models.Application>> GetByStatusAsync(ApplicationStatus status)
        {
            return await _context.Applications
                .Include(a => a.Service)
                .Include(a => a.User)
                .Where(a => a.Status == status)
                .ToListAsync();
        }

        public async Task AddAsync(models.Application app)
        {
            await _context.Applications.AddAsync(app);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(models.Application app)
        {
            _context.Applications.Update(app);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int appId)
        {
            var app = await _context.Applications.FindAsync(appId);
            if (app != null)
            {
                _context.Applications.Remove(app);
                await _context.SaveChangesAsync();
            }
        }
    }


}
