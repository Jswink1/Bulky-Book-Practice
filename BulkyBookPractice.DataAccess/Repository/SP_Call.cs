using BulkyBookPractice.DataAccess.Data;
using BulkyBookPractice.DataAccess.Repository.IRepository;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBookPractice.DataAccess.Repository
{
    // "SP_Call" means "StoredProcedure_Call
    public class SP_Call : ISP_Call
    {
        private readonly ApplicationDbContext _db;
        private static string ConnectionString = "";

        public SP_Call(ApplicationDbContext db)
        {
            _db = db;
            ConnectionString = db.Database.GetDbConnection().ConnectionString;
        }

        public void Dispose()
        {
            _db.Dispose();
        }                      

        // Retrieve a single record
        public T OneRecord<T>(string procedureName, DynamicParameters parameters = null)
        {
            using (SqlConnection sqlConn = new(ConnectionString))
            {
                sqlConn.Open();

                // Retrieve the value
                var value = sqlConn.Query<T>(procedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);

                // Convert the value
                return (T)Convert.ChangeType(value.FirstOrDefault(), typeof(T));
            }
        }

        // Retrieve a single value, such as a count, or Id, or bool, by using Execute Scalar
        public T Single<T>(string procedureName, DynamicParameters parameters = null)
        {
            using (SqlConnection sqlConn = new(ConnectionString))
            {
                sqlConn.Open();
                return (T)Convert.ChangeType(sqlConn.ExecuteScalar<T>(procedureName, parameters, commandType: System.Data.CommandType.StoredProcedure), typeof(T));
            }
        }

        // Retrieve a list of records
        public IEnumerable<T> List<T>(string procedureName, DynamicParameters parameters = null)
        {
            using (SqlConnection sqlConn = new(ConnectionString))
            {
                sqlConn.Open();
                return sqlConn.Query<T>(procedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        // Execute a procedure on the database that does not require a return value
        public void Execute(string procedureName, DynamicParameters parameters = null)
        {
            using (SqlConnection sqlConn = new(ConnectionString))
            {
                sqlConn.Open();
                sqlConn.Execute(procedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        // Retrieve two lists of records
        public Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string procedureName, DynamicParameters parameters = null)
        {
            using (SqlConnection sqlConn = new(ConnectionString))
            {
                sqlConn.Open();

                // Retrieve the results
                var result = SqlMapper.QueryMultiple(sqlConn, procedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);

                // Form them into a tuple and return
                var item1 = result.Read<T1>().ToList();
                var item2 = result.Read<T2>().ToList();

                if (item1 != null && item2 != null)
                {
                    return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(item1, item2);
                }
            }

            return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(new List<T1>(), new List<T2>());
        }
    }
}
