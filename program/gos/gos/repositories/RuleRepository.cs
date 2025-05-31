using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gos.models;
using Microsoft.EntityFrameworkCore;

namespace gos.repositories
{
    public interface IRuleRepository
    {
        Task Add(Rule rule);
        Task Update(Rule rule);
        Task Delete(int ruleId);
        Task<Rule?> GetById(int ruleId);
        Task<List<Rule>> GetByServiceId(int serviceId);
    }

    public class RuleRepository : IRuleRepository
    {
        private readonly AppDbContext _context;

        public RuleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(Rule rule)
        {
            await _context.Rules.AddAsync(rule);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Rule rule)
        {
            _context.Rules.Update(rule);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int ruleId)
        {
            var rule = await _context.Rules.FindAsync(ruleId);
            if (rule != null)
            {
                _context.Rules.Remove(rule);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Rule?> GetById(int ruleId)
        {
            return await _context.Rules
                .Include(r => r.Service)
                .Include(r => r.NeededType)
                .FirstOrDefaultAsync(r => r.Id == ruleId);
        }
        public async Task<List<Rule>> GetByServiceId(int serviceId)
        {
            return await _context.Rules
                .Include(r => r.Service)
                .Include(r => r.NeededType)
                .Where(r => r.ServiceId == serviceId)
                .ToListAsync();
        }
    }

}
