using ShoeStoreApp.Interface;
using ShoeStoreApp.Models;

namespace ShoeStoreApp.SortStrategies
{
	public class SortByGenderAsc : ISortStrategy
	{
		public IQueryable<Product> Sort(IQueryable<Product> items)
		{
			return items.OrderBy(s => s.Gender);
		}
	}
}
