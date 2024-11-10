using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageLibrary.Services
{
    public class TableStorageService<TEntity> : INoSqlStorage<TEntity> where TEntity : class, ITableEntity, new()
    {
        private readonly TableClient _tableClient;
        public TableStorageService()
        {
            var connectionString = ConnectionStrings.AzureStorageConnectionString;
            var tableName = typeof(TEntity).Name;
            var serviceClient = new TableServiceClient(connectionString);
            _tableClient = serviceClient.GetTableClient(tableName);
            _tableClient.CreateIfNotExists();
        }
        public async Task<TEntity> Add(TEntity entity)
        {
            await _tableClient.AddEntityAsync(entity);
            return await Get(entity.RowKey, entity.PartitionKey);
        }

        public async Task<IQueryable<TEntity>> All()
        {
            var entities = new List<TEntity>();
            var result = _tableClient.QueryAsync<TEntity>();
            await foreach (var entity in result)
            {
                entities.Add(entity);
            }
            return entities.AsQueryable();
        }

        public async Task Delete(string rowKey, string partitionKey)
        {
            await _tableClient.DeleteEntityAsync(rowKey, partitionKey);
        }

        public async Task<TEntity?> Get(string rowKey, string partitionKey)
        {
            try
            {
                var response = await _tableClient.GetEntityAsync<TEntity>(partitionKey, rowKey);
                return response.Value;
            }
            catch (RequestFailedException e) when (e.Status == 404)
            {
                return default(TEntity);
            }
        }

        public async Task<IQueryable<TEntity>> Query(Expression<Func<TEntity, bool>> query)
        {
            var entities = new List<TEntity>();
            var result = _tableClient.QueryAsync<TEntity>(query);
            await foreach (var entity in result)
            {
                entities.Add(entity);
            }
            return entities.AsQueryable();
        }

        public async Task<TEntity> Update(TEntity entity)
        {
            await _tableClient.UpdateEntityAsync(entity, entity.ETag, TableUpdateMode.Merge);
            return await Get(entity.RowKey, entity.PartitionKey);
        }
    }
}
