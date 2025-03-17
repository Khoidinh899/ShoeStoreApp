using ShoeStoreApp.Data;
using ShoeStoreApp.Models;

namespace ShoeStoreApp.Interface
{
	public interface  ISearchDecorator
	{

		IEnumerable<Product> SearchProducts(Dictionary<string, string[]> searchConditions);


	}
}
