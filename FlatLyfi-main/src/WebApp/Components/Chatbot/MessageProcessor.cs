using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Markdig;

namespace eShop.WebApp.Chatbot;

public static partial class MessageProcessor
{
    //AllowImages -> ProcessMessageContent
    public static MarkupString ProcessMessageContent(string message)
    {
        // Having to process markdown and deal with HTML encoding isn't ideal. If the language model could return
        // search results in some defined format like JSON we could simply loop over it in .razor code. This is
        // fine for now though.

        var result = new StringBuilder();
        var prevEnd = 0;
        message = message.Replace("&lt;", "<").Replace("&gt;", ">");

        foreach (Match match in FindMarkdownLinksAndImages().Matches(message))
        {
            // Добавляем текст перед текущим совпадением, HTML-экранированный
            if (match.Index > prevEnd)
            {
                result.Append(HtmlEncoder.Default.Encode(message.Substring(prevEnd, match.Index - prevEnd)));
            }

            string leadingChar = match.Groups[1].Value; // Захватывает '!' или пустую строку
            string textOrAlt = match.Groups[2].Value;   // Текст ссылки или alt текст изображения
            string url = match.Groups[3].Value;         // URL

            if (leadingChar == "!") // Это изображение: ![alt](url)
            {
                result.Append($"<img title=\"{HtmlEncoder.Default.Encode(textOrAlt)}\" src=\"{HtmlEncoder.Default.Encode(url)}\" style=\"max-width: 100%; height: auto;\" />");
            }
            else // Это ссылка: [text](url)
            {
                result.Append($"<a href=\"{HtmlEncoder.Default.Encode(url)}\" target=\"_blank\">{HtmlEncoder.Default.Encode(textOrAlt)}</a>");
            }

            /*
            var contentToHere = message.Substring(prevEnd, match.Index - prevEnd);
            result.Append(HtmlEncoder.Default.Encode(contentToHere));
            result.Append($"<img title=\"{(HtmlEncoder.Default.Encode(match.Groups[1].Value))}\" src=\"{(HtmlEncoder.Default.Encode(match.Groups[2].Value))}\" />");
            */
            prevEnd = match.Index + match.Length;
            
        }
        //result.Append(HtmlEncoder.Default.Encode(message.Substring(prevEnd)));

        // Добавляем оставшийся текст после последнего совпадения, HTML-экранированный
        if (prevEnd < message.Length)
        {
            result.Append(HtmlEncoder.Default.Encode(message.Substring(prevEnd)));
        }

        return new MarkupString(result.ToString());
    }

    [GeneratedRegex(@"\!?\[([^\]]+)\]\s*\(([^\)]+)\)")]
    private static partial Regex FindMarkdownImages();

    // Обновленное регулярное выражение:
    // Группа 1 (!?): Захватывает необязательный '!' в начале.
    // Группа 2 ([^\]]*): Захватывает текст внутри квадратных скобок (alt для img, текст для a).
    // Группа 3 ([^\)]+): Захватывает URL внутри круглых скобок.
    [GeneratedRegex(@"(!?)\[([^\]]*)\]\s*\(([^\)]+)\)")]
    private static partial Regex FindMarkdownLinksAndImages();

    public static MarkupString ProcessMessageMDContent(string message)
    {
        if (string.IsNullOrEmpty(message))
            return new MarkupString("");

        // Преобразуем Markdown в HTML
        var html = Markdown.ToHtml(message);

        // Возвращаем как MarkupString для Blazor
        return new MarkupString(html);
    }
}
