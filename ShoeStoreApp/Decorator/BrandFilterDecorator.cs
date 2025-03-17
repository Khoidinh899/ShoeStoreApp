using Json.Net;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ShoeStoreApp.Data;
using ShoeStoreApp.Interface;
using ShoeStoreApp.Models;

namespace ShoeStoreApp.Decorator
{
	public class BrandFilterDecorator : SearchDecorator
	{
		public BrandFilterDecorator(ShoeStoreAppContext context, ISearchDecorator searchService, ViewDataDictionary viewData) 
			: base(context, searchService, viewData)
		{
		}

		public override IEnumerable<Product> SearchProducts(Dictionary<string, string[]> searchConditions )
		{
			IEnumerable<Product> products = _searchService.SearchProducts(searchConditions);

			if (searchConditions.ContainsKey("brand"))
			{
				string[] colors = searchConditions["brand"];

				products = products.Where(p => colors.Contains(p.Brand));
				base._viewData["brand"] = colors;
				base._viewData["filtered"] = true;
			}
		
			return products;
		}
	}
}
