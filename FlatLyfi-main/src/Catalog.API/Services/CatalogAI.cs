﻿using System.Diagnostics;
using Microsoft.Extensions.AI;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace eShop.Catalog.API.Services;

public sealed class CatalogAI : ICatalogAI
{
    private const int EmbeddingDimensions = 384;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;

    /// <summary>The web host environment.</summary>
    private readonly IWebHostEnvironment _environment;
    /// <summary>Logger for use in AI operations.</summary>
    private readonly ILogger _logger;

    public CatalogAI(IWebHostEnvironment environment, ILogger<CatalogAI> logger, IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator = null)
    {
        _embeddingGenerator = embeddingGenerator;
        _environment = environment;
        _logger = logger;
    }

    /// <inheritdoc/>
    public bool IsEnabled => _embeddingGenerator is not null;

    /// <inheritdoc/>
    public ValueTask<Vector> GetEmbeddingAsync(CatalogItem item) =>
        IsEnabled ?
            GetEmbeddingAsync(CatalogItemToString(item)) :
            ValueTask.FromResult<Vector>(null);

    /// <inheritdoc/>
    public async ValueTask<IReadOnlyList<Vector>> GetEmbeddingsAsync(IEnumerable<CatalogItem> items)
    {
        if (IsEnabled)
        {
            long timestamp = Stopwatch.GetTimestamp();

            GeneratedEmbeddings<Embedding<float>> embeddings = await _embeddingGenerator.GenerateAsync(items.Select(CatalogItemToString));
            var results = embeddings.Select(m => new Vector(m.Vector[0..EmbeddingDimensions])).ToList();

            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace("Generated {EmbeddingsCount} embeddings in {ElapsedMilliseconds}s", results.Count, Stopwatch.GetElapsedTime(timestamp).TotalSeconds);
            }

            return results;
        }

        return null;
    }

    /// <inheritdoc/>
    public async ValueTask<Vector> GetEmbeddingAsync(string text)
    {
        if (IsEnabled)
        {
            long timestamp = Stopwatch.GetTimestamp();

            var embedding = await _embeddingGenerator.GenerateEmbeddingVectorAsync(text);
            embedding = embedding[0..EmbeddingDimensions];

            var array = embedding.ToArray();
            var normalized = array.NormalizeL2();

            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace("Generated embedding in {ElapsedMilliseconds}s: '{Text}'", Stopwatch.GetElapsedTime(timestamp).TotalSeconds, text);
            }

            return new Vector(normalized);
        }

        return null;
    }

    private static string CatalogItemToString(CatalogItem item) => $"{item.Name} " +
        $"{item.Description} Цена {item.Price}Руб. Метро {item.Metro} Время до метро {item.TimeToTheMetro} " +
        $"Адрес {item.Address} количество комнат {item.NumberOfRooms} Этаж {item.Floor} Санузел {item.Bathroom} " +
        $"Ремонт {item.Repair} Мебель {item.Furniture} из приборов и техники в доме есть {item.Technique} " +
        $"дом {item.HouseType} есть {item.InternetAndTV} Грузовой лифт {item.FreightElevator} " +
        $"Парковка {item.Parking}";
}

public static class VectorExtensions
{
    public static float[] NormalizeL2(this float[] vector)
    {
        // вычисляем L2-норму
        double sumSquares = 0;
        for (int i = 0; i < vector.Length; i++)
            sumSquares += vector[i] * vector[i];

        var norm = Math.Sqrt(sumSquares);
        if (norm == 0) return vector; // на всякий случай

        // создаём новый массив-копию, чтобы не мутировать исходный
        var normalized = new float[vector.Length];
        for (int i = 0; i < vector.Length; i++)
            normalized[i] = (float)(vector[i] / norm);

        return normalized;
    }
}
