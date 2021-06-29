using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using ShoppingCart.Common;
using ShoppingCart.Common.Entities;

namespace ShoppingCart.Data.Sql.Repositories
{
    public sealed class CartRepository : ICartRepository
    {
        #region SqlRoutine

        private const string TableName = "Carts";

        private static readonly string CountFromDateSql = $@"SELECT COUNT(1) FROM [{TableName}] 
WHERE [{nameof(CartEntity.CreationDate)}] >= @fromDate";
        private static readonly string CountHasBonusFromDateSql = $@"SELECT COUNT(1) FROM [{TableName}] 
WHERE [{nameof(CartEntity.CreationDate)}] >= @fromDate AND [{nameof(CartEntity.HasBonusPoints)}] = [{nameof(CartEntity.HasBonusPoints)}]";
        private static readonly string AvgCostFromDateSql = $@"SELECT AVG([{nameof(CartEntity.Cost)}]) FROM [{TableName}] 
WHERE [{nameof(CartEntity.CreationDate)}] >= @fromDate";

        private static readonly string CreateSql = @$"
DECLARE @generated_keys table([Id] UNIQUEIDENTIFIER);
INSERT INTO [dbo].[{TableName}] 
([{nameof(CartEntity.Cost)}], [{nameof(CartEntity.CreationDate)}], [{nameof(CartEntity.Items)}], [{nameof(CartEntity.HasBonusPoints)}])
OUTPUT INSERTED.Id INTO @generated_keys 
VALUES (@{nameof(CartEntity.Cost)}, @{nameof(CartEntity.CreationDate)}, @{nameof(CartEntity.Items)}, @{nameof(CartEntity.HasBonusPoints)});
SELECT [Id] from @generated_keys;";

        private static readonly string ReadByIdSql = $@"SELECT * FROM [dbo].[{TableName}] WHERE {nameof(CartEntity.Id)} = @{nameof(CartEntity.Id)}";

        private static readonly string ReadByPeriodSql = @$"SELECT * FROM {TableName} 
WHERE (@fromDate IS NULL OR {nameof(CartEntity.CreationDate)} >= @fromDate) and (@toDate IS NULL OR {nameof(CartEntity.CreationDate)} < @toDate)";
        
        private static readonly string ReadIdsByPeriodSql = @$"SELECT Id FROM {TableName} 
WHERE (@fromDate IS NULL OR {nameof(CartEntity.CreationDate)} >= @fromDate) and (@toDate IS NULL OR {nameof(CartEntity.CreationDate)} < @toDate)";

        private static readonly string UpdateSql = $@"UPDATE [dbo].[{TableName}] 
SET [{nameof(CartEntity.Cost)}] = @{nameof(CartEntity.Cost)}, 
[{nameof(CartEntity.CreationDate)}] = @{nameof(CartEntity.CreationDate)}, 
[{nameof(CartEntity.Items)}] = @{nameof(CartEntity.Items)}, 
[{nameof(CartEntity.HasBonusPoints)}] = @{nameof(CartEntity.HasBonusPoints)}) 
WHERE {nameof(CartEntity.Id)} = @{nameof(CartEntity.Id)}";

        private static readonly string DeleteSql = $@"DELETE FROM [dbo].[{TableName}] 
OUTPUT DELETED.Id
WHERE {nameof(CartEntity.Id)} = @{nameof(CartEntity.Id)};";

        private static readonly string DeleteByPeriodSql = $@"DECLARE @Deleted TABLE([Id] UNIQUEIDENTIFIER);
DELETE FROM [dbo].[{TableName}] 
OUTPUT DELETED.Id INTO @Deleted
WHERE (@fromDate IS NULL OR {nameof(CartEntity.CreationDate)} >= @fromDate) and (@toDate IS NULL OR {nameof(CartEntity.CreationDate)} < @toDate);
SELECT [ID] FROM @Deleted;";

        #endregion

        private readonly IDbConnectionFactory _dbConnectionFactory;

        public CartRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<CartEntity>> ReadCartsByPeriodAsync(DateTimeOffset? fromDate, DateTimeOffset? toDate)
        {
            using var connection = _dbConnectionFactory.Create();
            return await connection.QueryAsync<CartEntity>(ReadByPeriodSql, new {fromDate, toDate});
        }

        public async Task<IEnumerable<Guid>> ReadCartIdsByPeriodAsync(DateTimeOffset? fromDate, DateTimeOffset? toDate)
        {
            using var connection = _dbConnectionFactory.Create();
            return await connection.QueryAsync<Guid>(ReadIdsByPeriodSql, new {fromDate, toDate});
        }

        public async Task<CartEntity> ReadByIdAsync(Guid id)
        {
            using var connection = _dbConnectionFactory.Create();
            return await connection.QueryFirstOrDefaultAsync<CartEntity>(ReadByIdSql, new {Id = id});
        }

        public async Task<CartEntity> CreateAsync(
            decimal cost,
            string items,
            bool hasBonusPoints,
            DateTimeOffset creationDate)
        {
            using var connection = _dbConnectionFactory.Create();
            var insertedId = await connection.QuerySingleAsync<Guid>(
                                 CreateSql,
                                 new
                                 {
                                     Cost = cost,
                                     CreationDate = creationDate,
                                     Items = items,
                                     HasBonusPoints = hasBonusPoints
                                 });

            return new CartEntity(insertedId, cost, items, creationDate, hasBonusPoints);
        }

        public async Task<bool> UpdateAsync(CartEntity cartEntity)
        {
            using var connection = _dbConnectionFactory.Create();
            var result = await connection.ExecuteAsync(UpdateSql, cartEntity);
            return result > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            using var connection = _dbConnectionFactory.Create();
            var result = await connection.ExecuteAsync(DeleteSql, new {Id = id});
            return result > 0;
        }

        public async Task<IEnumerable<Guid>> DeleteByPeriodAsync(DateTimeOffset? fromDate, DateTimeOffset? toDate)
        {
            using var connection = _dbConnectionFactory.Create();
            return await connection.QueryAsync<Guid>(DeleteByPeriodSql, new {fromDate, toDate});
        }

        public async Task<int> AvgCostFromDateAsync(DateTimeOffset fromDate)
        {
            using var connection = _dbConnectionFactory.Create();
            return await connection.QuerySingleAsync<int>(AvgCostFromDateSql, new {fromDate});
        }

        public async Task<int> CountFromDateAsync(DateTimeOffset fromDate)
        {
            using var connection = _dbConnectionFactory.Create();
            return await connection.QuerySingleAsync<int>(CountFromDateSql, new {fromDate});
        }

        public async Task<int> CountHasBonusFromDateAsync(DateTimeOffset fromDate)
        {
            using var connection = _dbConnectionFactory.Create();
            return await connection.QuerySingleAsync<int>(CountHasBonusFromDateSql, new {fromDate, HasBonusPoints = true});
        }
    }
}