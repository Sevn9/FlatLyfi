using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Web;
using eShop.WebAppComponents.Catalog;

namespace eShop.WebAppComponents.Services;

public class CatalogService(HttpClient httpClient) : ICatalogService
{
    private readonly string remoteServiceBaseUrl = "api/catalog/";

    public Task<CatalogItem?> GetCatalogItem(int id)
    {
        var uri = $"{remoteServiceBaseUrl}items/{id}";
        return httpClient.GetFromJsonAsync<CatalogItem>(uri);
    }

    public async Task<CatalogResult> GetCatalogItems(int pageIndex, int pageSize, int? brand, int? type)
    {
        var uri = GetAllCatalogItemsUri(remoteServiceBaseUrl, pageIndex, pageSize, brand, type);
        var result = await httpClient.GetFromJsonAsync<CatalogResult>(uri);
        return result!;
    }

    public async Task<List<CatalogItem>> GetCatalogItems(IEnumerable<int> ids)
    {
        var uri = $"{remoteServiceBaseUrl}items/by?ids={string.Join("&ids=", ids)}";
        var result = await httpClient.GetFromJsonAsync<List<CatalogItem>>(uri);
        return result!;
    }

    public Task<CatalogResult> GetCatalogItemsWithSemanticRelevance(int page, int take, string text)
    {
        var url = $"{remoteServiceBaseUrl}items/withsemanticrelevance?text={HttpUtility.UrlEncode(text)}&pageIndex={page}&pageSize={take}";
        var result = httpClient.GetFromJsonAsync<CatalogResult>(url);
        return result!;
    }

    public async Task<IEnumerable<CatalogBrand>> GetBrands()
    {
        var uri = $"{remoteServiceBaseUrl}catalogBrands";
        var result = await httpClient.GetFromJsonAsync<CatalogBrand[]>(uri);
        return result!;
    }

    public async Task<IEnumerable<CatalogItemType>> GetTypes()
    {
        var uri = $"{remoteServiceBaseUrl}catalogTypes";
        var result = await httpClient.GetFromJsonAsync<CatalogItemType[]>(uri);
        return result!;
    }

    private static string GetAllCatalogItemsUri(string baseUri, int pageIndex, int pageSize, int? brand, int? type)
    {
        string filterQs = string.Empty;

        if (type.HasValue)
        {
            filterQs += $"type={type.Value}&";
        }
        if (brand.HasValue)
        {
            filterQs += $"brand={brand.Value}&";
        }

        return $"{baseUri}items?{filterQs}pageIndex={pageIndex}&pageSize={pageSize}";
    }

    public async Task<List<CatalogItem>> GetCatalogItemsFilteredAsync(
        QueryCriteria criteria, int take, string? text)
    {
        // Полный путь к эндпоинту на сервере Catalog.API
        const string endpointPath = "items/filtered";

        var queryStringParts = new List<string>();

        queryStringParts.Add($"take={take}");

        if (criteria.Floor != null)
        {
            queryStringParts.Add($"Floor={criteria.Floor}");
        }

        if (criteria.NumberOfRooms != null)
        {
            queryStringParts.Add($"NumberOfRooms={criteria.NumberOfRooms}");
        }

        // Добавляем текстовый параметр, если он задан и не пустой
        // Uri.EscapeDataString важен для корректной передачи специальных символов в URL
        if (!string.IsNullOrWhiteSpace(text))
        {
            queryStringParts.Add($"text={Uri.EscapeDataString(text)}");
        }

        // Формируем полный URI
        var requestUri = $"{remoteServiceBaseUrl}{endpointPath}";

        if (queryStringParts.Any())
        {
            requestUri += $"?{string.Join("&", queryStringParts)}";
        }

        // _logger?.LogInformation("Вызов Catalog API: {RequestUri}", requestUri);

        try
        {
            var result = await httpClient.GetFromJsonAsync<List<CatalogItem>>(requestUri);
            return result ?? new List<CatalogItem>();
        }
        catch (HttpRequestException ex)
        {
            Debug.WriteLine(ex);
            throw;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            throw;
        }
    }
}
