using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gos.repositories.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAll();
        Task<T?> GetById(int id);
        Task Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);
        Task SaveChanges();
    }
}
