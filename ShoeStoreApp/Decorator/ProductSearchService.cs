using Microsoft.EntityFrameworkCore;
using ShoeStoreApp.Data;
using ShoeStoreApp.Interface;
using ShoeStoreApp.Models;
using ShoeStoreApp.Services;

namespace ShoeStoreApp.Decorator
{
	public class ProductSearchService : ISearchDecorator
    {
		protected readonly IProductService _productService;

		public ProductSearchService(IProductService productService)
		{
			_productService = productService;
		}

		public IEnumerable<Product> SearchProducts(Dictionary<string, string[]> searchConditions)
		{
            IEnumerable<Product> products = _productService.GetAllProducts();
            return products;
		}


	}

}
