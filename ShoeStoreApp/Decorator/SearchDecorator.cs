using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ShoeStoreApp.Data;
using ShoeStoreApp.Interface;
using ShoeStoreApp.Models;

namespace ShoeStoreApp.Decorator
{
	public abstract class SearchDecorator : ISearchDecorator
    {
		protected readonly ShoeStoreAppContext _context;
		
		protected ISearchDecorator _searchService;
		protected ViewDataDictionary _viewData;

		public SearchDecorator(ShoeStoreAppContext context, ISearchDecorator searchService, ViewDataDictionary viewData)
		{
			_context = context;
			_searchService = searchService;
            _viewData = viewData;
		}

	
		public abstract IEnumerable<Product> SearchProducts(Dictionary<string, string[]> searchConditions);
		
	}
}
