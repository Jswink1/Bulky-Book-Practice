using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBookPractice.DataAccess.Repository.IRepository
{    
    public interface IRepository<T> where T : class
    {
        void Add(T entity);

        T Get(int id);
        
        IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null,
                              Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                              string includeProperties = null);
        
        T GetFirstOrDefault(Expression<Func<T, bool>> filter = null,
                            string includeProperties = null);        
        
        void Remove(int id);
        
        void Remove(T entity);
        
        void RemoveRange(IEnumerable<T> entity);
    }
}
