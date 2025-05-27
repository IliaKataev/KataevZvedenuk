using gos.models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gos.repositories
{
    public interface IParameterTypeRepository
    {
        Task<List<ParameterType>> GetAllAsync();
        Task<ParameterType?> GetByIdAsync(int id);
        Task AddAsync(ParameterType type);
        Task UpdateAsync(ParameterType type);
        Task DeleteAsync(int id);
        Task<List<ParameterType>> GetByServiceIdAsync(int serviceId);
    }

    public class ParameterTypeRepository : IParameterTypeRepository
    {
        private readonly AppDbContext _context;

        public ParameterTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ParameterType>> GetAllAsync()
        {
            return await _context.ParameterTypes
                .Include(pt => pt.Parameters)
                .Include(pt => pt.Rules)
                .ToListAsync();
        }

        public async Task<ParameterType?> GetByIdAsync(int id)
        {
            return await _context.ParameterTypes
                .Include(pt => pt.Parameters)
                .Include(pt => pt.Rules)
                .FirstOrDefaultAsync(pt => pt.Id == id);
        }

        public async Task AddAsync(ParameterType type)
        {
            await _context.ParameterTypes.AddAsync(type);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ParameterType type)
        {
            _context.ParameterTypes.Update(type);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var parameterType = await _context.ParameterTypes.FindAsync(id);
            if (parameterType != null)
            {
                _context.ParameterTypes.Remove(parameterType);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<ParameterType>> GetByServiceIdAsync(int serviceId)
        {
            return await _context.ParameterTypes
                .Where(pt => pt.Rules.Any(r => r.ServiceId == serviceId))  // Мы ищем те ParameterType, у которых есть связанные правила для данного serviceId
                .ToListAsync();
        }

    }
}
