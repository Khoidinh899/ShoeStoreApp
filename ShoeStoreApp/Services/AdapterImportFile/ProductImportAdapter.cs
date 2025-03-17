using ShoeStoreApp.Models;

namespace ShoeStoreApp.Services.AdapterImportFile
{
    public class ProductImportAdapter : IProductDataReader
    {
        private readonly ExcelProductDataReader _excelReader;
        public ProductImportAdapter(ExcelProductDataReader excelReader)
        {
            _excelReader = excelReader;
        }
        public IEnumerable<Product> GetProducts()
        {
            return _excelReader.GetProducts();
        }
    }
}
