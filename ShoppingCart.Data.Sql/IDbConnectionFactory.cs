using System.Data;

namespace ShoppingCart.Data.Sql
{
    public interface IDbConnectionFactory
    {
        IDbConnection Create();
    }
}