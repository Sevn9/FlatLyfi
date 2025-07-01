using System.ComponentModel;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;
using eShop.WebAppComponents.Catalog;
using eShop.WebAppComponents.Services;
using Microsoft.Extensions.AI;

namespace eShop.WebApp.Chatbot;

public class ChatState
{
    private readonly ICatalogService _catalogService;
    private readonly IBasketState _basketState;
    private readonly ClaimsPrincipal _user;
    private readonly ILogger _logger;
    private readonly IProductImageUrlProvider _productImages;
    private readonly IChatClient _chatClient;
    private readonly ChatOptions _chatOptions;

    public ChatState(
        ICatalogService catalogService,
        IBasketState basketState,
        ClaimsPrincipal user,
        IProductImageUrlProvider productImages,
        ILoggerFactory loggerFactory,
        IChatClient chatClient)
    {
        _catalogService = catalogService;
        _basketState = basketState;
        _user = user;
        _productImages = productImages;
        _logger = loggerFactory.CreateLogger(typeof(ChatState));

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("ChatModel: {model}", chatClient.GetService<ChatClientMetadata>()?.DefaultModelId);
        }

        _chatClient = chatClient;
        _chatOptions = new()
        {
            Tools =
            [
                AIFunctionFactory.Create(GetUserInfo),
                AIFunctionFactory.Create(SearchCatalog),
                AIFunctionFactory.Create(AddToCart),
                AIFunctionFactory.Create(GetCartContents),
            ],
        };

        Messages =
        [
            new ChatMessage(ChatRole.System, """
                Вы — агент службы поддержки клиентов на базе искусственного интеллекта для интернет-площадки для подбора жилья FlatLyfi.
                Вы НИКОГДА не отвечаете на темы, не связанные с FlatLyfi.
                Ваша работа — отвечать на вопросы клиентов о квартирах в каталоге FlatLyfi.
                FlatLyfi в основном специализируется на арендном жилье и и всем что связано с арендой квартир в том числе информацию о жилье.
                Вы стараетесь быть краткими и давать более развернутые ответы только при необходимости.
                Отвечай только тем, что реально есть в каталоге. 
                Если нет точного совпадения, сначала уточни запрос пользователя.
                Если кто-то задает вопрос о чем-либо, кроме FlatLyfi, его каталога или его учетной записи,
                вы отказываетесь отвечать, а вместо этого спрашиваете, есть ли тема, связанная с FlatLyfi, с которой вы можете помочь.
                """),
            new ChatMessage(ChatRole.Assistant, """
                Привет! Я ИИ риелтор. Хотите подобрать себе жилье? Опишите что бы вы хотели видеть в вашей квартире, а я порекомендую вам варианты.
                """),
        ];
    }

    public IList<ChatMessage> Messages { get; }

    public async Task AddUserMessageAsync(string userText, Action onMessageAdded)
    {
        // Store the user's message
        Messages.Add(new ChatMessage(ChatRole.User, userText));
        onMessageAdded();

        // Get and store the AI's response message
        try
        {
            var response = await _chatClient.GetResponseAsync(Messages, _chatOptions);
            if (!string.IsNullOrWhiteSpace(response.Text))
            {
                Messages.AddMessages(response);
            }
        }
        catch (Exception e)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(e, "Error getting chat completions.");
            }
            Messages.Add(new ChatMessage(ChatRole.Assistant, $"My apologies, but I encountered an unexpected error."));
        }
        onMessageAdded();
    }


    [Description("Gets information about the chat user")]
    private string GetUserInfo()
    {
        var claims = _user.Claims;
        return JsonSerializer.Serialize(new
        {
            Name = GetValue(claims, "name"),
            LastName = GetValue(claims, "last_name"),
            Street = GetValue(claims, "address_street"),
            City = GetValue(claims, "address_city"),
            State = GetValue(claims, "address_state"),
            ZipCode = GetValue(claims, "address_zip_code"),
            Country = GetValue(claims, "address_country"),
            Email = GetValue(claims, "email"),
            PhoneNumber = GetValue(claims, "phone_number"),
        });

        static string GetValue(IEnumerable<Claim> claims, string claimType) =>
            claims.FirstOrDefault(x => x.Type == claimType)?.Value ?? "";
    }

    [Description("Searches the FlatLyfi catalog for a provided apartment description")]
    private async Task<string> SearchCatalog([Description("The apartment description for which to search")] string productDescription)
    {
        try
        {
            Debug.WriteLine("SearchCatalog method: \n");
            Debug.WriteLine("productDescription:" + productDescription);
            var criteria = ParseCriteria(productDescription);

            Debug.WriteLine("criteria:" + "NumberOfRooms:" + criteria.NumberOfRooms +"Floor:" + criteria.Floor);

            // берём 40 «кандидатов» уже после жёстких фильтров
            var resultsFiltered = await _catalogService.GetCatalogItemsFilteredAsync(
                criteria, 40, productDescription);

            Debug.WriteLine("resultsFiltered.Count:" + resultsFiltered.Count);

            //var results = await _catalogService.GetCatalogItemsWithSemanticRelevance(0, 40, productDescription!);
            for (int i = 0; i < resultsFiltered.Count; i++)
            {
                resultsFiltered[i] = resultsFiltered[i] with { PictureUrl = _productImages.GetProductImageUrl(resultsFiltered[i].Id) };
            }

            var selectedItems = resultsFiltered.Select(item => new
            {
                item.Name,
                item.Price,
                item.PictureUrl,
                item.LinkToProductCard
            }).ToList();

            return JsonSerializer.Serialize(selectedItems);
        }
        catch (HttpRequestException e)
        {
            return Error(e, "Error accessing catalog.");
        }
    }

    [Description("Adds a product to the user's shopping cart.")]
    private async Task<string> AddToCart([Description("The id of the product to add to the shopping cart (basket)")] int itemId)
    {
        try
        {
            var item = await _catalogService.GetCatalogItem(itemId);
            await _basketState.AddAsync(item!);
            return "Item added to shopping cart.";
        }
        catch (Grpc.Core.RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.Unauthenticated)
        {
            return "Unable to add an item to the cart. You must be logged in.";
        }
        catch (Exception e)
        {
            return Error(e, "Unable to add the item to the cart.");
        }
    }

    [Description("Gets information about the contents of the user's favorites (basket)")]
    private async Task<string> GetCartContents()
    {
        try
        {
            var basketItems = await _basketState.GetBasketItemsAsync();
            return JsonSerializer.Serialize(basketItems);
        }
        catch (Exception e)
        {
            return Error(e, "Unable to get the cart's contents.");
        }
    }

    private string Error(Exception e, string message)
    {
        if (_logger.IsEnabled(LogLevel.Error))
        {
            _logger.LogError(e, message);
        }

        return message;
    }

    private static QueryCriteria ParseCriteria(string text)
    {
        // 1 комната / 2-комнатная / однокомнатная
        int? rooms = Regex.Match(text, @"(\d)[-\s]*комнат")
                          is { Success: true } m1 ? int.Parse(m1.Groups[1].Value) : null;

        // 10 этаж / на 10-м этаже
        int? floor = Regex.Match(text, @"(\d+)[-\s]*(?:й|ой)?\s*этаж")
                          is { Success: true } m2 ? int.Parse(m2.Groups[1].Value) : null;

        // до 60 000 ₽ / 60к / 60000
        int? maxPrice = Regex.Match(text, @"до\s*(\d[\d\s]*)\s*([₽р]|k|к)")
                              is { Success: true } m3
                         ? int.Parse(m3.Groups[1].Value.Replace(" ", "")) * (m3.Groups[2].Value.ToLower() switch { "k" or "к" => 1000, _ => 1 })
                         : null;

        return new QueryCriteria(rooms.ToString(), floor.ToString());
    }
}
