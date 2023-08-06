using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Microsoft.Extensions.Logging;
using notissimus.toyparser.core.Enums;
using notissimus.toyparser.core.Extensions;
using notissimus.toyparser.core.Models;

namespace notissimus.toyparser.core.Parser;

public class ProductParser
{
    private readonly IConfiguration _config = Configuration.Default.WithDefaultLoader();
    private readonly IBrowsingContext _context;
    private readonly string _cookies;
    
    public ProductParser(CityEnum city = CityEnum.NoCity)
    {
        _context = BrowsingContext.New(_config);
        _cookies = Region.EnumToCookie(city);
    }

    public async Task<Product> ParseAsync(string link, ILogger? logger = null)
    {
        IDocument document = await _context.OpenWithCookiesAsync(link, _cookies, logger);
        
        string name = GetName(document);
        decimal price = GetPrice(document);
        decimal oldPrice = GetOldPrice(document);
        string images = string.Join(';', GetImages(document));
        string breadcrumbs = GetBreadCrumbs(document);
        bool stock = IsInStock(document);
        string region = GetRegion(document);

        var product = new Product(name, price, oldPrice, stock, images, link, breadcrumbs, region);
        
        return product;
    }

    private string GetName(IDocument document)
    {
        return document.QuerySelector(".detail-name")?.Text() ?? String.Empty;
    }

    private decimal GetPrice(IDocument document)
    {
        decimal price = 0;
        string? priceText = document.QuerySelector<IHtmlSpanElement>("span.price")?.GetAttribute("content");
        if (!string.IsNullOrEmpty(priceText))
            price = decimal.Parse(priceText);

        return price;
    }

    private decimal GetOldPrice(IDocument document)
    {
        decimal price = 0;
        string? oldPriceText = document.QuerySelector(".old-price")?.Text().RemoveNonNumChars();
        if (! string.IsNullOrEmpty(oldPriceText))
            price = decimal.Parse(oldPriceText);

        return price;
    }

    private IEnumerable<string> GetImages(IDocument document)
    {
        return document
            .QuerySelectorAll<IHtmlImageElement>(".card-slider-for a > img")
            .Select(img => img.Source)
            .Where(url => url is not null)!;
    }

    private string GetBreadCrumbs(IDocument document)
    {
        IEnumerable<string> breadcrumbs = document.QuerySelectorAll(".breadcrumb-item > span > span")
            .Select(breadcrumb => breadcrumb.Text().Trim());

        return string.Join(" > ", breadcrumbs);
    }

    private bool IsInStock(IDocument document)
    {
        return document.QuerySelector(".ok") is not null;
    }

    private string GetRegion(IDocument document)
    {
        return document.QuerySelector(".select-city-link > a")?.Text().Trim() ?? String.Empty;
    }
}