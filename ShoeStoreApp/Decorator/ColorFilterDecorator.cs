using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ShoeStoreApp.Data;
using ShoeStoreApp.Interface;
using ShoeStoreApp.Models;
using System.Drawing;

namespace ShoeStoreApp.Decorator
{
	public class ColorFilterDecorator : SearchDecorator
	{
		public ColorFilterDecorator(ShoeStoreAppContext context, ISearchDecorator searchService, ViewDataDictionary viewData) 
			: base(context, searchService, viewData)
		{
		}

		public override IEnumerable<Product> SearchProducts(Dictionary<string, string[]> searchConditions)
		{
			IEnumerable<Product> products = _searchService.SearchProducts(searchConditions);

			if (searchConditions.ContainsKey("color"))
			{

				string[] colors = searchConditions["color"];
				products = products.Where(product => colors.Any(c => product.Variants.Any(v => v.Color.Contains(c))));
				base._viewData["color"] = colors;
				base._viewData["filtered"] = true;
			}

			return products;
		}
	}
}
