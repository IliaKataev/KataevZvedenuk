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
        Task<List<ParameterType>> GetAll();
        Task<ParameterType?> GetById(int id);
        Task Add(ParameterType type);
        Task Update(ParameterType type);
        Task Delete(int id);
        Task<List<ParameterType>> GetByServiceId(int serviceId);
    }

    public class ParameterTypeRepository : IParameterTypeRepository
    {
        private readonly AppDbContext _context;

        public ParameterTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ParameterType>> GetAll()
        {
            return await _context.ParameterTypes
                .Include(pt => pt.Parameters)
                .Include(pt => pt.Rules)
                .ToListAsync();
        }

        public async Task<ParameterType?> GetById(int id)
        {
            return await _context.ParameterTypes
                .Include(pt => pt.Parameters)
                .Include(pt => pt.Rules)
                .FirstOrDefaultAsync(pt => pt.Id == id);
        }

        public async Task Add(ParameterType type)
        {
            await _context.ParameterTypes.AddAsync(type);
            await _context.SaveChangesAsync();
        }

        public async Task Update(ParameterType type)
        {
            _context.ParameterTypes.Update(type);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var parameterType = await _context.ParameterTypes.FindAsync(id);
            if (parameterType != null)
            {
                _context.ParameterTypes.Remove(parameterType);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<ParameterType>> GetByServiceId(int serviceId)
        {
            return await _context.ParameterTypes
                .Where(pt => pt.Rules.Any(r => r.ServiceId == serviceId)) 
                .ToListAsync();
        }

    }
}
