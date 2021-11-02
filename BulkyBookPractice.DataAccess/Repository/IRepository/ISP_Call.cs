using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBookPractice.DataAccess.Repository.IRepository
{
    public interface ISP_Call : IDisposable
    {
        T OneRecord<T>(string procedureName, DynamicParameters parameters = null);

        T Single<T>(string procedureName, DynamicParameters parameters = null);              

        IEnumerable<T> List<T>(string procedureName, DynamicParameters parameters = null);        

        void Execute(string procedureName, DynamicParameters parameters = null);

        Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string procedureName, DynamicParameters parameters = null);
    }
}
