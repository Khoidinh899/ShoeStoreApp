using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ShoeStoreApp.Data;
using ShoeStoreApp.Interface;
using ShoeStoreApp.Models;

namespace ShoeStoreApp.Decorator
{
	public class PriceFilterDecorator : SearchDecorator
	{
		public PriceFilterDecorator(ShoeStoreAppContext context, ISearchDecorator searchService, ViewDataDictionary viewData) 
			: base(context,searchService, viewData)
		{
		}

		public override IEnumerable<Product> SearchProducts(Dictionary<string, string[]> searchConditions )
		{
			
			IEnumerable<Product> products = base._searchService.SearchProducts(searchConditions);

			if (searchConditions.ContainsKey("price"))
			{
				string[] price = searchConditions["price"];

				products = products.Where(product => price.Any(pr => FilterPrice(product.Price, pr)));
				base._viewData["price"] = price;
				base._viewData["filtered"] = true;
			}

			return products;

		}
		private bool FilterPrice(long productPrice, string priceFilter)
		{
			if (priceFilter.Contains("-"))
			{
				long lower;
				long upper;
				bool check = long.TryParse(priceFilter.Split("-")[0], out lower);
				if (!check) return false;
				check = long.TryParse(priceFilter.Split("-")[1], out upper);
				if (!check) return false;

				if (productPrice >= lower && productPrice <= upper)
					return true;
				return false;
			}
			else
			{
				long lower;
				bool check = long.TryParse(priceFilter, out lower);
				if (!check) return false;
				if (productPrice >= lower)
					return true;
			}
			return false;
		}
	}
}

