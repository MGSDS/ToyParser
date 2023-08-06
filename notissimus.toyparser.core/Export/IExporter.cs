using notissimus.toyparser.core.Models;

namespace notissimus.toyparser.core.Export;

public interface IExporter
{
    public void Export(IEnumerable<Product> products, string path);
}