using CashFlow.Consolidated.Infra.Data.Contracts;
using CashFlow.Consolidated.Infra.Data.Helpers;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace CashFlow.Consolidated.Infra.Data.Gateways
{
    public class GenericRepository<T>(IDatabaseContext context) : IGenericRepository<T> where T : class
    {
        private readonly IMongoCollection<T> _collection = context.Database.GetCollection<T>(StringHelper.CamelCase<T>());
        private readonly IMongoQueryable<T> _queryable = context.Database.GetCollection<T>(StringHelper.CamelCase<T>()).AsQueryable();

        #region Write operations

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
        }

        public async Task AddManyAsync(ICollection<T> entities, CancellationToken cancellationToken = default)
        {
            await _collection.InsertManyAsync(entities, new InsertManyOptions { IsOrdered = true }, cancellationToken: cancellationToken);
        }

        public Dictionary<string, object> PropertiesDictionary<TProp>(TProp val)
        {
            var properties = typeof(TProp).GetProperties();
            var dictionary = new Dictionary<string, object>();

            foreach (var property in properties)
            {
                var value = property.GetValue(val);
                dictionary[property.Name] = value;
            }

            return dictionary;
        }

        public async Task<bool> UpdateAsync<TValue>(Expression<Func<T, bool>> query, TValue value, CancellationToken cancellationToken = default)
        {

            var properties = PropertiesDictionary(value);

            var updates = new List<UpdateDefinition<T>>
            {
                Builders<T>.Update.Set("atualizadoEm", DateTime.UtcNow)
            };

            foreach (var property in properties)
            {
                var updateDefinitionSet = Builders<T>.Update.Set(property.Key, property.Value);
                updates.Add(updateDefinitionSet);
            }

            var updateDefinition = Builders<T>.Update.Combine(updates);

            var result = await _collection.UpdateManyAsync(query, updateDefinition, cancellationToken: cancellationToken);

            return result.IsAcknowledged;
        }

        public async Task<bool> DeleteAsync(Expression<Func<T, bool>> query, CancellationToken cancellationToken = default)
        {
            var result = await _collection.DeleteManyAsync<T>(query, cancellationToken: cancellationToken);

            return result.IsAcknowledged;
        }

        #endregion Write operations

        #region Read operations

        public async Task<ICollection<T>> WhereAsync(Expression<Func<T, bool>> query, CancellationToken cancellationToken = default)
        {
            return await _queryable
                .Where(query)
                .ToListAsync(cancellationToken);
        }

        public async Task<ICollection<TOutput>> WhereAsync<TOutput>(Expression<Func<T, bool>> query, CancellationToken cancellationToken = default)
        {
            var collection = await _queryable
                .Where(query)
                .ToListAsync(cancellationToken);

            var json = JsonConvert.SerializeObject(collection);
            return JsonConvert.DeserializeObject<ICollection<TOutput>>(json);
        }

        public async Task<ICollection<TOutput>> WhereAsync<TOutput>(Expression<Func<T, bool>> query, Expression<Func<T, dynamic>> selector, CancellationToken cancellationToken = default)
        {
            var collection = await _queryable
                .Where(query)
                .Select(selector)
                .ToListAsync(cancellationToken);
            var json = JsonConvert.SerializeObject(collection);
            return JsonConvert.DeserializeObject<ICollection<TOutput>>(json);

        }

        public async Task<ICollection<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _queryable.ToListAsync(cancellationToken);
        }

        public async Task<T> FirstAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
        {
            return await _queryable.FirstOrDefaultAsync(expression, cancellationToken: cancellationToken);
        }

        public async Task<TOutput> FirstAsync<TOutput>(Expression<Func<T, bool>> expression, Expression<Func<T, TOutput>> selector, CancellationToken cancellationToken = default)
        {
            return await _queryable
                .Where(expression)
                .Select(selector)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<TOutput>> GroupByAsync<TOutput, TGroup>(Expression<Func<T, bool>> query, Expression<Func<T, TGroup>> keySelector, Expression<Func<IGrouping<TGroup, T>, TOutput>> selector, CancellationToken cancellationToken = default)
        {
            return await _queryable
                 .Where(query)
                 .GroupBy(keySelector)
                 .Select(selector)
                 .ToListAsync(cancellationToken);
        }

        public async Task<long> CountAsync(Expression<Func<T, bool>> query, CancellationToken cancellationToken = default)
        {
            return await _queryable
                 .LongCountAsync(query, cancellationToken);
        }

        public async Task<TOutput> MaxAsync<TOutput>(Expression<Func<T, bool>> query, Expression<Func<T, TOutput>> selector, CancellationToken cancellationToken = default)
        {
            var any = await _queryable
                 .Where(query)
                 .AnyAsync();
            if (!any) return default;
            return await _queryable
                 .Where(query)
                 .MaxAsync(selector, cancellationToken);
        }


        #endregion Read operations

    }

}
