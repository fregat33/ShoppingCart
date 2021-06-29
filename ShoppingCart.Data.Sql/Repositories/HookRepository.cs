using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using ShoppingCart.Common;
using ShoppingCart.Common.Entities;

namespace ShoppingCart.Data.Sql.Repositories
{
    public sealed class HooksRepository : IHooksRepository
    {
        #region SqlRoutine

        private const string TableName = "Hooks";

        private static readonly string CreateSql = $@"INSERT INTO {TableName} 
([{nameof(HookEntity.CartId)}], [{nameof(HookEntity.Url)}], [{nameof(HookEntity.Payload)}], [{nameof(HookEntity.CreationDate)}]) 
OUTPUT INSERTED.Id 
VALUES (@{nameof(HookEntity.CartId)}, @{nameof(HookEntity.Url)}, @{nameof(HookEntity.Payload)}, @{nameof(HookEntity.CreationDate)});";

        private static readonly string UpdateStateByIdSql = $@"UPDATE {TableName} 
SET [{nameof(HookEntity.State)}] = @{nameof(HookEntity.State)}, 
[{nameof(HookEntity.RetriesCount)}] = @{nameof(HookEntity.RetriesCount)} 
WHERE {nameof(HookEntity.Id)} = @{nameof(HookEntity.Id)};";

        private static readonly string UpdateStateByCartIdSql = $@"UPDATE {TableName} SET [{nameof(HookEntity.State)}] = @{nameof(HookEntity.State)} 
OUTPUT INSERTED.Id 
WHERE {nameof(HookEntity.CartId)} IN (@{nameof(HookEntity.CartId)}s);";

        private static readonly string ReadByStateSql = $@"SELECT * FROM {TableName} WHERE [{nameof(HookEntity.State)}] = @{nameof(HookEntity.State)};";
        private static readonly string ReadByCartIdsSql = $@"SELECT * FROM {TableName} WHERE [{nameof(HookEntity.CartId)}] IN @{nameof(HookEntity.CartId)}s;";
        private static readonly string ReadByIdSql = $@"SELECT * FROM {TableName} WHERE [{nameof(HookEntity.Id)}] = @{nameof(HookEntity.Id)};";

        private static readonly string DeleteSql = $@"DELETE FROM [dbo].[{TableName}] 
OUTPUT DELETED.Id
WHERE {nameof(HookEntity.Id)} = @{nameof(HookEntity.Id)};";

        #endregion

        private readonly IDbConnectionFactory _dbConnectionFactory;

        public HooksRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = _dbConnectionFactory.Create();
            var result = await connection.ExecuteAsync(DeleteSql, new {Id = id});
            return result > 0;
        }

        public async Task<IEnumerable<int>> SetActivateAsync(IReadOnlyCollection<Guid> cartIds)
        {
            using var connection = _dbConnectionFactory.Create();
            return await connection.QueryAsync<int>(UpdateStateByCartIdSql, new {CartIds = cartIds, State = (int) HookState.Activated});
        }

        public async Task SetFailedAsync(int id, int retries)
        {
            using var connection = _dbConnectionFactory.Create();
            await connection.ExecuteAsync(UpdateStateByIdSql, new {Id = id, State = (int) HookState.Failed, RetriesCount = retries});
        }

        public async Task<IEnumerable<HookEntity>> ReadByStateAsync(HookState state)
        {
            using var connection = _dbConnectionFactory.Create();
            return await connection.QueryAsync<HookEntity>(ReadByStateSql, new {State = state});
        }

        public Task<IEnumerable<HookEntity>> ReadByCartIdAsync(Guid cartId)
        {
            return ReadByCartIdsAsync(new[] {cartId});
        }

        public async Task<IEnumerable<HookEntity>> ReadByCartIdsAsync(IReadOnlyCollection<Guid> cartIds)
        {
            using var connection = _dbConnectionFactory.Create();
            return await connection.QueryAsync<HookEntity>(ReadByCartIdsSql, new {CartIds = cartIds});
        }

        public async Task<HookEntity> CreateAsync(Guid requestCartId, string requestUrl, string payload, DateTimeOffset creationDate)
        {
            using var connection = _dbConnectionFactory.Create();
            var insertedEntity = new HookEntity
                                 {
                                     Url = requestUrl,
                                     CartId = requestCartId,
                                     Payload = payload,
                                     CreationDate = creationDate
                                 };
            var insertedId = await connection.QuerySingleAsync<int>(CreateSql, insertedEntity);
            insertedEntity.Id = insertedId;
            return insertedEntity;
        }
    }
}