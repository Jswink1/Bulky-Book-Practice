using BulkyBookPractice.DataAccess.Data;
using BulkyBookPractice.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBookPractice.DataAccess.Repository
{
    // <T> makes this repository generic, "where T" specifies that the object will be a class, such as "Category" 
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        // Add an entity
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        // Retrieve an entity from the DB based on the Id
        public T Get(int id)
        {
            return dbSet.Find(id);
        }

        // Retrieve a list of entities from the DB based on a filter expression, OrderBy properties, and Include properties
        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null)
        {
            // Initialize the query
            IQueryable<T> query = dbSet;

            // Apply the filter to the query
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // "Include" determines if we want to load fields from other tables that the entity has a foreign relationship with
            if (includeProperties != null)
            {
                // The "Include Properties" come as a list of table names in the form of a comma-separated string
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            // Order the query
            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }

            return query.ToList();
        }

        // Retrieve an entity from the DB based on a filter expression and include properties
        public T GetFirstOrDefault(Expression<Func<T, bool>> filter = null, string includeProperties = null)
        {
            // Initialize the query
            IQueryable<T> query = dbSet;

            // Apply the filter to the query
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // "Include" determines if we want to load fields from other tables that the entity has a foreign relationship with
            if (includeProperties != null)
            {
                // The "Include Properties" come as a list of table names in the form of a comma-separated string
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return query.FirstOrDefault();
        }

        // Remove an entity based on its Id
        public void Remove(int id)
        {
            // Retrieve the entity and remove
            T entity = dbSet.Find(id);
            Remove(entity);
        }

        // Remove an entity
        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        // Remove a list of entities
        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
    }
}
