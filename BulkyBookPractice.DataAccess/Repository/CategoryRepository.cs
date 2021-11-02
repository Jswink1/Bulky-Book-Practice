using BulkyBookPractice.DataAccess.Data;
using BulkyBookPractice.DataAccess.Repository.IRepository;
using BulkyBookPractice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBookPractice.DataAccess.Repository
{
    public class CategoryRepository : RepositoryAsync<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Category category)
        {
            // Retrieve the category record from the DB
            var categoryFromDB = _db.Categories.FirstOrDefault(x => x.Id == category.Id);

            if (categoryFromDB != null)
            {
                // Map the updated name to the database
                categoryFromDB.Name = category.Name;
            }
        }
    }
}
