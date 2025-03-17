using ShoeStoreApp.Interface;
using ShoeStoreApp.Models;

namespace ShoeStoreApp.SortStrategies
{
	public class SortByPriceDesc: ISortStrategy
	{
		public IQueryable<Product> Sort(IQueryable<Product> items)
		{
			return items.OrderByDescending(s => s.Price);
		}
	}

}
