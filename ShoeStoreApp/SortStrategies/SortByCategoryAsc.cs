using ShoeStoreApp.Interface;
using ShoeStoreApp.Models;

namespace ShoeStoreApp.SortStrategies
{
	public class SortByCategoryAsc : ISortStrategy
	{
		public IQueryable<Product> Sort(IQueryable<Product> items)
		{
			return items.OrderBy(s => s.Category);
		}
	}
}
