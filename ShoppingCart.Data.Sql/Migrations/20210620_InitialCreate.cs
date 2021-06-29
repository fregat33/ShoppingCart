using System.Data;
using FluentMigrator;
using System.Data.SqlClient;
using FluentMigrator.Runner.Initialization;
using Microsoft.Extensions.Configuration;

namespace ShoppingCart.Data.Sql.Migrations
{
    [Migration(202106201800)]
    public class InitialCreate: Migration
    {
        public override void Up()
        {
            Create.Table("Carts")
              .WithColumn("Id").AsGuid().PrimaryKey().NotNullable().WithDefault(SystemMethods.NewGuid)
              .WithColumn("Cost").AsDecimal()
              .WithColumn("Items").AsString()
              .WithColumn("HasBonusPoints").AsBoolean()
              .WithColumn("CreationDate").AsDateTimeOffset().Indexed();
        
            Create.Table("Hooks")
              .WithColumn("Id").AsInt32().PrimaryKey().NotNullable().Identity()
              .WithColumn("CartId").AsGuid().ForeignKey("Carts", "Id").OnDelete(Rule.Cascade)
              .WithColumn("Url").AsString()
              .WithColumn("Payload").AsString()
              .WithColumn("State").AsInt32().WithDefaultValue(0).Indexed()
              .WithColumn("RetriesCount").AsInt32().WithDefaultValue(0)
              .WithColumn("CreationDate").AsDateTimeOffset();

            Create.Table("Stats")
              .WithColumn("Id").AsInt32().PrimaryKey().NotNullable().Identity()
              .WithColumn("Data").AsString()
              .WithColumn("CreationDate").AsDateTimeOffset().Indexed();
        }

        public override void Down()
        { 
            Delete.Table("Carts");
            Delete.Table("Hooks");
            Delete.Table("Stats");
        }
    }
}