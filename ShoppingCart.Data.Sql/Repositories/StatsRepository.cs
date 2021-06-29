using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ShoppingCart.Common;
using ShoppingCart.Common.Entities;

namespace ShoppingCart.Data.Sql.Repositories
{
    public class StatsRepository: IStatsRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        #region SqlRoutin
        
        private const string TableName = "Stats";
        private static readonly string CreateSql = $@"INSERT INTO [dbo].[{TableName}] ([{nameof(StatsEntity.Data)}], [{nameof(StatsEntity.CreationDate)}]) 
INSERTED.Id 
VALUES (@{nameof(StatsEntity.Data)}, @{nameof(StatsEntity.CreationDate)});";
        
        private static readonly string ReadByPeriodSql = $@"SELECT * FROM [dbo].[{TableName}] 
WHERE (@fromDate IS NULL OR {nameof(StatsEntity.CreationDate)} >= @fromDate) and (@toDate IS NULL OR {nameof(StatsEntity.CreationDate)} < @toDate)";
        
        #endregion

        public StatsRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<StatsEntity> CreateAsync(StatData data, DateTimeOffset creationDate)
        {
            using var connection = _dbConnectionFactory.Create();
            var insertedId = await connection.QuerySingleAsync<int>(CreateSql, new {Data = data, CreationDate = creationDate});

            return new StatsEntity(insertedId, data, creationDate);
        }

        public async Task<IReadOnlyCollection<StatsEntity>> ReadByPeriodAsync(DateTimeOffset? fromDate, DateTimeOffset? toDate)
        {
            using var connection = _dbConnectionFactory.Create();
            var result = await connection.QueryAsync<StatsEntity>(CreateSql, new {fromDate, toDate});
            return result.ToArray();
        }
    }
}