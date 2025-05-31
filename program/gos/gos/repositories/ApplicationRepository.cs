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
        Task<List<models.Application>> GetByUserId(int userId);
        Task<models.Application?> GetById(int applicationId);
        Task<List<models.Application>> GetAll();
        Task<List<models.Application>> GetByStatus(ApplicationStatus status);
        Task Add(models.Application app);
        Task Update(models.Application app);
        Task Delete(int appId);
    }

    public class ApplicationRepository : IApplicationRepository
    {
        private readonly AppDbContext _context;

        public ApplicationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<models.Application>> GetByUserId(int userId)
        {
            return await _context.Applications
                .Include(a => a.Service)
                .Include(a => a.User)
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task<models.Application?> GetById(int applicationId)
        {
            return await _context.Applications
                .Include(a => a.Service)
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == applicationId);
        }

        public async Task<List<models.Application>> GetAll()
        {
            return await _context.Applications
                .Include(a => a.Service)
                .Include(a => a.User)
                .ToListAsync();
        }

        public async Task<List<models.Application>> GetByStatus(ApplicationStatus status)
        {
            return await _context.Applications
                .Include(a => a.Service)
                .Include(a => a.User)
                .Where(a => a.Status == status)
                .ToListAsync();
        }

        public async Task Add(models.Application app)
        {
            await _context.Applications.AddAsync(app);
            await _context.SaveChangesAsync();
        }

        public async Task Update(models.Application app)
        {
            _context.Applications.Update(app);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int appId)
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
