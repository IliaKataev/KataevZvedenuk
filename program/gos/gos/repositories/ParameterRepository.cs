using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gos.models;
using Microsoft.EntityFrameworkCore;

namespace gos.repositories
{

    public interface IParameterRepository
    {
        Task<Parameter?> GetByIdAsync(int parameterId);
        Task AddAsync(Parameter parameter);
        Task UpdateAsync(Parameter parameter);
        Task DeleteAsync(int parameterId);
    }


    public class ParameterRepository : IParameterRepository
    {
        private readonly AppDbContext _context;

        public ParameterRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Parameter?> GetByIdAsync(int parameterId)
        {
            return await _context.Parameters
                .Include(p => p.Type)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == parameterId);
        }

        public async Task AddAsync(Parameter parameter)
        {
            await _context.Parameters.AddAsync(parameter);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Parameter parameter)
        {
            _context.Parameters.Update(parameter);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int parameterId)
        {
            var parameter = await _context.Parameters.FindAsync(parameterId);
            if (parameter != null)
            {
                _context.Parameters.Remove(parameter);
                await _context.SaveChangesAsync();
            }
        }
    }

}
