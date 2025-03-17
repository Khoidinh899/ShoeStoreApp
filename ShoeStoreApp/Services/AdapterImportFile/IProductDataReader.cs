using ShoeStoreApp.Models;

namespace ShoeStoreApp.Services.AdapterImportFile
{
    public interface IProductDataReader
    {
        IEnumerable<Product> GetProducts();
    }
}
