// See https://aka.ms/new-console-template for more information

using AngleSharp.Common;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using notissimus.toyparser.core.Enums;
using notissimus.toyparser.core.Export;
using notissimus.toyparser.core.Models;
using notissimus.toyparser.core.Parser;

ILogger<CategoryParser> logger = LoggerFactory.Create(builder => builder.AddNLog()).CreateLogger<CategoryParser>();

var parserMoscow = new CategoryParser(CityEnum.Moscow, logger);
IEnumerable<Product> productsMoscow = await parserMoscow.GetAllGoodsAsync("https://www.toy.ru/catalog/boy_transport/");

var parserRostov = new CategoryParser(CityEnum.RostovOnDon, logger); 
IEnumerable<Product> productsRostov  = await parserRostov.GetAllGoodsAsync("https://www.toy.ru/catalog/boy_transport/");

var exporter = new CsvExporter();

exporter.Export(productsMoscow.Concat(productsRostov), "result.csv");