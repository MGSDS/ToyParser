namespace notissimus.toyparser.core.Models;

public record Product(string Name, decimal Price, decimal OldPrice, bool InStock, string Images, string Link, string BreadCrumbs, string Region)
{ }