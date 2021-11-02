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
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product product)
        {
            // Retreive the product record from the DB
            var productFromDB = _db.Products.FirstOrDefault(x => x.Id == product.Id);

            if (productFromDB != null)
            {
                // Check if we are updating the image
                if (product.ImageUrl != null)
                {
                    productFromDB.ImageUrl = product.ImageUrl;
                }

                // Map the updated fields to the database
                productFromDB.ISBN = product.ISBN;
                productFromDB.Price = product.Price;
                productFromDB.Price50 = product.Price50;
                productFromDB.ListPrice = product.ListPrice;
                productFromDB.Price100 = product.Price100;
                productFromDB.Title = product.Title;
                productFromDB.Description = product.Description;
                productFromDB.CategoryId = product.CategoryId;
                productFromDB.Author = product.Author;
                productFromDB.CoverTypeId = product.CoverTypeId;
            }
        }
    }
}
