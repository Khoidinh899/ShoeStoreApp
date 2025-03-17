using ShoeStoreApp.Models;

namespace ShoeStoreApp.Interface
{
	public interface ISortStrategy
	{
		IQueryable<Product> Sort(IQueryable<Product> items);
	}
}
