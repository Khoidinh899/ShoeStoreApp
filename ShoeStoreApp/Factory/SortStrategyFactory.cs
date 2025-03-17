using ShoeStoreApp.Interface;
using ShoeStoreApp.Models;
using ShoeStoreApp.SortStrategies;

namespace ShoeStoreApp.Factory
{
	public static class SortStrategyFactory
	{
		public static ISortStrategy CreateSortStrategy(string sortOrder)
		{
			switch (sortOrder)
			{
				case "price_desc":
					return new SortByPriceDesc();
				case "Price":
					return new SortByPriceAsc();
				case "brand_asc":
					return new SortByBrandAsc();
				case "brand_desc":
					return new SortByBrandDesc();
				case "gender_asc":
					return new SortByGenderAsc();
				case "gender_desc":
					return new SortByGenderDesc();
				case "category_asc":
					return new SortByCategoryAsc();
				case "category_desc":
					return new SortByCategoryDesc();
				default:
					return null;
			}
		}
	}

}
