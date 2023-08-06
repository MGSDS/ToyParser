using System.Globalization;
using CsvHelper;
using notissimus.toyparser.core.Models;

namespace notissimus.toyparser.core.Export;
public class CsvExporter : IExporter
{
    public void Export(IEnumerable<Product> products, string path)
    {
        using var writer = new StreamWriter(path);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(products);
    }
}