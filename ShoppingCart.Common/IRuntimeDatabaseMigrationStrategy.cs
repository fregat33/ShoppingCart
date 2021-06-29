namespace ShoppingCart.Common
{
    public interface IRuntimeDatabaseMigrationStrategy
    {
        void ApplyMigrations();
    }
}