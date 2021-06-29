using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace ShoppingCart.Data.Sql
{
    public class DbConnectionFactory: IDbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection Create()
        {
            return new SqlConnection(_connectionString);
        }
    }
}