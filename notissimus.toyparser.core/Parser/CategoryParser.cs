using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Logging;
using notissimus.toyparser.core.Enums;
using notissimus.toyparser.core.Extensions;
using notissimus.toyparser.core.Models;

namespace notissimus.toyparser.core.Parser;

public class CategoryParser
{
    private readonly IConfiguration _config = Configuration.Default.WithDefaultLoader();
    private readonly IBrowsingContext _context;
    private readonly string _cookies;
    private readonly ProductParser _productParser;
    private readonly ILogger<CategoryParser>? _logger;
    public CategoryParser(CityEnum city = CityEnum.NoCity, ILogger<CategoryParser>? logger = null)
    {
        _logger = logger;
        _context = BrowsingContext.New(_config);
        _productParser = new ProductParser(city);
        _cookies = Region.EnumToCookie(city);
    }
    
    public async Task<IEnumerable<Product>> GetAllGoodsAsync(string link)
    {
        IDocument document = await GetPageAsync(link, 0);
        uint pageCount = GetPagesCount(document);
        _logger?.Log(LogLevel.Information, "Total pages to parse: {}", pageCount);

        List<Product> products = new();
        for (uint i = 1; i <= pageCount; ++i)
        {
            _logger?.Log(LogLevel.Information, "Parsing {} page from {}", i, pageCount);
            IEnumerable<Product> parsedProducts = await GetPageGoodsAsync(document);
            products.AddRange(parsedProducts);
            
            if (i < pageCount)
            {
                document = await GetPageAsync(link, i);
            }
        }
        
        return products;
    }

    private async Task<IDocument> GetPageAsync(string link, uint pageNum)
    {
        var url = new Url($"?count=45&&PAGEN_5={pageNum + 1}", link);
        return await _context.OpenWithCookiesAsync(url.ToString(), _cookies, _logger);
    }

    private uint GetPagesCount(IDocument document)
    {
        uint pages = 1;
        string? lastPageNum = document.QuerySelector(".pagination li:nth-last-of-type(2) > a")?.Text();
        if (!string.IsNullOrEmpty(lastPageNum) && uint.TryParse(lastPageNum, out uint converted))
        {
            pages = converted;
        }
        
        return pages;
    }

    private async Task<IEnumerable<Product>> GetPageGoodsAsync(IDocument document)
    {
        IHtmlCollection<IElement> productElements = document.QuerySelectorAll(".product-card");
        
        IEnumerable<string> links = productElements
            .Select(el => el.QuerySelector("meta[itemprop=\"url\"]")?.GetAttribute("content"))
            .Where(content => content is not null)!;
        var products = new List<Product>();
        
        foreach (string link in links)
        {
            Product product = await _productParser.ParseAsync(link);
            _logger?.Log(LogLevel.Information, "Parsed product: {}", product.Link);
            products.Add(product);
            
        }

        return products;
    }
}