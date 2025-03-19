namespace ChatApp.Services.Ingestion;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;

public class DataIngestor(
    ILogger<DataIngestor> logger,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    IVectorStore vectorStore,
    IngestionCacheDbContext ingestionCacheDb)
{
    public static async Task IngestDataAsync(IServiceProvider services, IIngestionSource source)
    {
        using var scope = services.CreateScope();
        var ingestor = scope.ServiceProvider.GetRequiredService<DataIngestor>();
        await ingestor.IngestDataAsync(source);
    }

    public async Task IngestDataAsync(IIngestionSource source)
    {
        var vectorCollection = vectorStore.GetCollection<string, SemanticSearchRecord>("data-chatapp-ingested");
        await vectorCollection.CreateCollectionIfNotExistsAsync();

        var documentsForSource = ingestionCacheDb.Documents
            .Where(d => d.SourceId == source.SourceId)
            .Include(d => d.Records);

        var deletedFiles = await source.GetDeletedDocumentsAsync(documentsForSource);
        foreach (var deletedFile in deletedFiles)
        {
            logger.LogInformation("Removing ingested data for {file}", deletedFile.Id);
            await vectorCollection.DeleteBatchAsync(deletedFile.Records.Select(r => r.Id));
            _ = ingestionCacheDb.Documents.Remove(deletedFile);
        }

        _ = await ingestionCacheDb.SaveChangesAsync();

        var modifiedDocs = await source.GetNewOrModifiedDocumentsAsync(documentsForSource);
        foreach (var modifiedDoc in modifiedDocs)
        {
            logger.LogInformation("Processing {file}", modifiedDoc.Id);

            if (modifiedDoc.Records.Count > 0)
            {
                await vectorCollection.DeleteBatchAsync(modifiedDoc.Records.Select(r => r.Id));
            }

            var newRecords = await source.CreateRecordsForDocumentAsync(embeddingGenerator, modifiedDoc.Id);
            await foreach (var id in vectorCollection.UpsertBatchAsync(newRecords))
            {
            }

            modifiedDoc.Records.Clear();
            modifiedDoc.Records.AddRange(newRecords.Select(r => new IngestedRecord { Id = r.Key, DocumentId = modifiedDoc.Id }));

            if (ingestionCacheDb.Entry(modifiedDoc).State == EntityState.Detached)
            {
                _ = ingestionCacheDb.Documents.Add(modifiedDoc);
            }
        }

        _ = await ingestionCacheDb.SaveChangesAsync();
        logger.LogInformation("Ingestion is up-to-date");
    }
}