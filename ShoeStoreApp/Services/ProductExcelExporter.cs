using ShoeStoreApp.Data;
using ShoeStoreApp.Models;

namespace ShoeStoreApp.Services
{
    public class ProductExcelExporter : ExcelExporter
    {
        public ProductExcelExporter(ShoeStoreAppContext context) : base(context)
        {
        }
        public override object getData()
        {
            return _context.Products.ToList();
        }
        public override byte[] ExportFromDataToExcel(object data)
        {
            List<Product> products = (List<Product>)data;
            byte[] fileContents = _excelService.GenerateProductsToExcel(products);
            return fileContents;
        }
    }
}
