using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ShoeStoreApp.Data;
using ShoeStoreApp.Interface;
using ShoeStoreApp.Models;

namespace ShoeStoreApp.Decorator
{
	public class GenderFilterDecorator : SearchDecorator
	{
		public GenderFilterDecorator(ShoeStoreAppContext context, ISearchDecorator searchService, ViewDataDictionary viewData) 
			: base(context,searchService, viewData)
		{
		}

		public override IEnumerable<Product> SearchProducts(Dictionary<string, string[]> searchConditions)
		{
			
			IEnumerable<Product> products = _searchService.SearchProducts(searchConditions);

			if (searchConditions.ContainsKey("gender"))
			{
				string[] gender = searchConditions["gender"];

				products = products.Where(p => gender.Contains(p.Gender));
				base._viewData["gender"] = gender;
				base._viewData["filtered"] = true;
			}

			return products;
		}
	}
}
