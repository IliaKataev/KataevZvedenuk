using gos.repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gos.models;

namespace gos.repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByLoginAsync(string login);
        Task<List<Parameter>> GetParametersAsync(int userId);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
    }



    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Applications)
                .Include(u => u.Parameters)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByLoginAsync(string login)
        {
            return await _context.Users
                .Include(u => u.Applications)
                .Include(u => u.Parameters)
                .FirstOrDefaultAsync(u => u.Login == login);
        }

        public async Task<List<Parameter>> GetParametersAsync(int userId)
        {
            return await _context.Parameters
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);

            //var sql = _context.Users.ToQueryString();
            //MessageBox.Show(sql, "SQL-запрос");
            MessageBox.Show(user.ToString());

            await _context.SaveChangesAsync();
        }
    }



}
