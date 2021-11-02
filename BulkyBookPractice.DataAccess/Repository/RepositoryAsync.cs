﻿using BulkyBookPractice.DataAccess.Data;
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
    public class RepositoryAsync<T> : IRepositoryAsync<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public RepositoryAsync(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        // Add an entity
        public async Task AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
        }

        // Retrieve an entity from the DB based on the Id
        public async Task<T> GetAsync(int id)
        {
            return await dbSet.FindAsync(id);
        }

        // Retrieve a list of entities from the DB based on a filter expression, order properties, and include properties
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null)
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
                return await orderBy(query).ToListAsync();
            }

            return await query.ToListAsync();
        }

        // Retrieve an entity from the DB based on a filter expression and include properties
        public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter = null, string includeProperties = null)
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

            return await query.FirstOrDefaultAsync();
        }

        // Remove an entity based on its Id
        public async Task RemoveAsync(int id)
        {
            // Retrieve the entity and remove
            T entity = await dbSet.FindAsync(id);
            await RemoveAsync(entity);
        }

        // Remove an entity
        public async Task RemoveAsync(T entity)
        {
            dbSet.Remove(entity);
        }

        // Remove a list of entities
        public async Task RemoveRangeAsync(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
    }
}
