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
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private readonly ApplicationDbContext _db;

        public CoverTypeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(CoverType coverType)
        {
            // Retrieve the coverType record from the DB
            var coverTypeFromDB = _db.CoverTypes.FirstOrDefault(x => x.Id == coverType.Id);

            if (coverTypeFromDB != null)
            {
                // Map the updated name to the database
                coverTypeFromDB.Name = coverType.Name;
            }
        }
    }
}
