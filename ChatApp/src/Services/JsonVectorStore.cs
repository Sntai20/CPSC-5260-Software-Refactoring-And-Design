﻿namespace ChatApp.Services;

using System.Numerics.Tensors;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Extensions.VectorData;

/// <summary>
/// This IVectorStore implementation is for prototyping only. Do not use this in production.
/// In production, you must replace this with a real vector store. There are many IVectorStore
/// implementations available, including ones for standalone vector databases like Qdrant or Milvus,
/// or for integrating with relational databases such as SQL Server or PostgreSQL.
///
/// This implementation stores the vector records in large JSON files on disk. It is very inefficient
/// and is provided only for convenience when prototyping.
/// </summary>
public class JsonVectorStore(string basePath) : IVectorStore
{
    public IVectorStoreRecordCollection<TKey, TRecord> GetCollection<TKey, TRecord>(string name, VectorStoreRecordDefinition? vectorStoreRecordDefinition = null)
        where TKey : notnull
        => new JsonVectorStoreRecordCollection<TKey, TRecord>(name, Path.Combine(basePath, name + ".json"), vectorStoreRecordDefinition);

    public IAsyncEnumerable<string> ListCollectionNamesAsync(CancellationToken cancellationToken = default)
        => Directory.EnumerateFiles(basePath, "*.json").ToAsyncEnumerable();

    private sealed class JsonVectorStoreRecordCollection<TKey, TRecord> : IVectorStoreRecordCollection<TKey, TRecord>
        where TKey : notnull
    {
        private static readonly Func<TRecord, TKey> GetKey = CreateKeyReader();
        private static readonly Func<TRecord, ReadOnlyMemory<float>> GetVector = CreateVectorReader();
        private readonly string filePath;
        private Dictionary<TKey, TRecord>? records;

#pragma warning disable IDE0060 // Remove unused parameter
        public JsonVectorStoreRecordCollection(string name, string filePath, VectorStoreRecordDefinition? vectorStoreRecordDefinition)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            this.CollectionName = name;
            this.filePath = filePath;

            if (File.Exists(filePath))
            {
                this.records = JsonSerializer.Deserialize<Dictionary<TKey, TRecord>>(File.ReadAllText(filePath));
            }
        }

        public string CollectionName { get; }

        public Task<bool> CollectionExistsAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(this.records is not null);

        public async Task CreateCollectionAsync(CancellationToken cancellationToken = default)
        {
            this.records = [];
            await this.WriteToDiskAsync(cancellationToken);
        }

        public async Task CreateCollectionIfNotExistsAsync(CancellationToken cancellationToken = default)
        {
            if (this.records is null)
            {
                await this.CreateCollectionAsync(cancellationToken);
            }
        }

        public Task DeleteAsync(TKey key, DeleteRecordOptions? options = null, CancellationToken cancellationToken = default)
        {
            _ = this.records!.Remove(key);
            return this.WriteToDiskAsync(cancellationToken);
        }

        public Task DeleteBatchAsync(IEnumerable<TKey> keys, DeleteRecordOptions? options = null, CancellationToken cancellationToken = default)
        {
            foreach (var key in keys)
            {
                _ = this.records!.Remove(key);
            }

            return this.WriteToDiskAsync(cancellationToken);
        }

        public Task DeleteCollectionAsync(CancellationToken cancellationToken = default)
        {
            this.records = null;
            File.Delete(this.filePath);
            return Task.CompletedTask;
        }

        public Task<TRecord?> GetAsync(TKey key, GetRecordOptions? options = null, CancellationToken cancellationToken = default)
            => Task.FromResult(this.records!.GetValueOrDefault(key));

        public IAsyncEnumerable<TRecord> GetBatchAsync(IEnumerable<TKey> keys, GetRecordOptions? options = null, CancellationToken cancellationToken = default)
            => keys.Select(key => this.records!.GetValueOrDefault(key)!).Where(r => r is not null).ToAsyncEnumerable();

        public async Task<TKey> UpsertAsync(TRecord record, UpsertRecordOptions? options = null, CancellationToken cancellationToken = default)
        {
            var key = GetKey(record);
            this.records![key] = record;
            await this.WriteToDiskAsync(cancellationToken);
            return key;
        }

        public async IAsyncEnumerable<TKey> UpsertBatchAsync(IEnumerable<TRecord> records, UpsertRecordOptions? options = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var results = new List<TKey>();
            foreach (var record in records)
            {
                var key = GetKey(record);
                this.records![key] = record;
                results.Add(key);
            }

            await this.WriteToDiskAsync(cancellationToken);

            foreach (var key in results)
            {
                yield return key;
            }
        }

        public Task<VectorSearchResults<TRecord>> VectorizedSearchAsync<TVector>(TVector vector, VectorSearchOptions? options = null, CancellationToken cancellationToken = default)
        {
            if (vector is not ReadOnlyMemory<float> floatVector)
            {
                throw new NotSupportedException($"The provided vector type {vector!.GetType().FullName} is not supported.");
            }

            IEnumerable<TRecord> filteredRecords = this.records!.Values;

            foreach (var clause in options?.Filter?.FilterClauses ?? [])
            {
                if (clause is EqualToFilterClause equalClause)
                {
                    var propertyInfo = typeof(TRecord).GetProperty(equalClause.FieldName);
                    filteredRecords = filteredRecords.Where(record => propertyInfo!.GetValue(record)!.Equals(equalClause.Value));
                }
                else
                {
                    throw new NotSupportedException($"The provided filter clause type {clause.GetType().FullName} is not supported.");
                }
            }

            var ranked = (from record in filteredRecords
                          let candidateVector = GetVector(record)
                          let similarity = TensorPrimitives.CosineSimilarity(candidateVector.Span, floatVector.Span)
                          orderby similarity descending
                          select (Record: record, Similarity: similarity));

            var results = ranked.Skip(options?.Skip ?? 0).Take(options?.Top ?? int.MaxValue);
            return Task.FromResult(new VectorSearchResults<TRecord>(
                results.Select(r => new VectorSearchResult<TRecord>(r.Record, r.Similarity)).ToAsyncEnumerable()));
        }

        private static Func<TRecord, TKey> CreateKeyReader()
        {
            var propertyInfo = typeof(TRecord).GetProperties()
                .Where(p => p.GetCustomAttribute<VectorStoreRecordKeyAttribute>() is not null
                    && p.PropertyType == typeof(TKey))
                .Single();
            return record => (TKey)propertyInfo.GetValue(record)!;
        }

        private static Func<TRecord, ReadOnlyMemory<float>> CreateVectorReader()
        {
            var propertyInfo = typeof(TRecord).GetProperties()
                .Where(p => p.GetCustomAttribute<VectorStoreRecordVectorAttribute>() is not null
                    && p.PropertyType == typeof(ReadOnlyMemory<float>))
                .Single();
            return record => (ReadOnlyMemory<float>)propertyInfo.GetValue(record)!;
        }

        private async Task WriteToDiskAsync(CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(this.records);
            _ = Directory.CreateDirectory(Path.GetDirectoryName(this.filePath)!);
            await File.WriteAllTextAsync(this.filePath, json, cancellationToken);
        }
    }
}