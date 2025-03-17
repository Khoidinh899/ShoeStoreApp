using ShoeStoreApp.Interface;
using ShoeStoreApp.Models;

namespace ShoeStoreApp.SortStrategies
{
	public class SortByBrandDesc : ISortStrategy
	{
		public IQueryable<Product> Sort(IQueryable<Product> items)
		{
			return items.OrderByDescending(s => s.Brand);
		}
	}
}
