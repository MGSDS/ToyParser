using System.Net;
using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Logging;
using notissimus.toyparser.core.Exceptions;

namespace notissimus.toyparser.core.Extensions;

public static class BrowsingContextExtensions
{
    public static async Task<IDocument> OpenWithCookiesAsync(this IBrowsingContext context, string link, string cookies, ILogger? logger = null)
    {
        var client = new HttpClient();
        
        while (true)
        {
            HttpRequestMessage request = CreateRequest(link, cookies);
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string content = await response.Content.ReadAsStringAsync();
                return await context.OpenAsync(req=> 
                    req.Content(content)
                        .Header("Content-Type", "text/html; charset=UTF-8"));
            }

            logger?.Log(LogLevel.Error, "{} Can not reach {}. Retrying in 5 seconds", response.StatusCode, link);
            await Task.Delay(5000);
            
        }
    }

    private static HttpRequestMessage CreateRequest(string link, string cookies)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, link);

        if (!string.IsNullOrWhiteSpace(cookies))
            request.Headers.Add("Cookie", cookies);

        return request;
    }
}