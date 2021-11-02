using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBookPractice.DataAccess.Repository.IRepository
{
    // <T> makes this repository generic, "where" specifies that the object will be a class, such as "Category" 
    public interface IRepositoryAsync<T> where T : class
    {
        Task AddAsync(T entity);
        
        Task<T> GetAsync(int id);
        
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null,
                              Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                              string includeProperties = null);
        
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter = null,
                            string includeProperties = null);
                
        
        Task RemoveAsync(int id);
        
        Task RemoveAsync(T entity);
        
        Task RemoveRangeAsync(IEnumerable<T> entity);
    }
}
