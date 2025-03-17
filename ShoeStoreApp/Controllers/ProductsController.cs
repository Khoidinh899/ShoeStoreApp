using Microsoft.AspNetCore.Mvc;
using ShoeStoreApp.Models;
using System.Diagnostics;
using ShoeStoreApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Logging;

using ShoeStoreApp.Decorator;
using ShoeStoreApp.Interface;

using ShoeStoreApp.Services;

namespace ShoeStoreApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ShoeStoreAppContext _context;
        private readonly ILogger<ProductsController> _logger;
        private readonly IProductService _productService;

        public ProductsController(ShoeStoreAppContext context, 
            ILogger<ProductsController> logger, 
            IProductService productService)
        {
            _logger = logger;
            _context = context;
            _productService = productService;
        }

        public IActionResult Index(string search, string category, string sort, string[] gender, string[] brand, string[] color, string[] price) 
        {
            ViewData["filtered"] = false;
            ISearchDecorator searchService = new ProductSearchService(_productService);
			Dictionary<string, string[]> searchConditions = new Dictionary<string, string[]>();


			if (gender.Count() != 0)
			{
	
				searchService = new GenderFilterDecorator(_context, searchService, ViewData);
				searchConditions["gender"] = gender;
			}


			if (brand.Count() != 0)
			{

				searchService = new BrandFilterDecorator(_context, searchService, ViewData);
				searchConditions["brand"] = brand;

			}

			if (color.Count() != 0)
			{

				searchService = new ColorFilterDecorator(_context, searchService, ViewData);
				searchConditions["color"] = color;
			}

			if (price.Count() != 0)
			{
				searchService = new PriceFilterDecorator(_context, searchService, ViewData);
				searchConditions["price"] = price;

			}
			var products = searchService.SearchProducts(searchConditions);
			if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.ToLower().Contains(search.ToLower()));
                ViewData["search"] = search;
                ViewData["filtered"] = true;
            }

            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category.Equals(category));
                ViewData["category"] = category;
                ViewData["filtered"] = true;
            }

            if (!string.IsNullOrEmpty(sort))
            {
                if (sort.Equals("price_asc"))
                    products = products.OrderBy(o => o.Price);
                else if (sort.Equals("price_desc"))
                    products = products.OrderByDescending(o => o.Price);
                ViewData["sort"] = sort;
                ViewData["filtered"] = true;
            }

		


			return View(products.ToList());
        }

        public IActionResult Details(int? id, string success)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            var products = _context.Products.Include(p => p.Variants).ThenInclude(v => v.Items).Where(p => p.Id == id);

            if (products.Count() == 0)
            {
                return RedirectToAction("Index");
            }

            var product = products.First();

            if (product == null)
            {
                return RedirectToAction("Index");
            }

            var model = new DetailModel
            {
                Product = product
            };
            if (success != null)
            {
                if (success.Equals("true"))
                    model.StatusMessage = "Thành công! Sản phẩm đã được thêm vào giỏ hàng";
                else if (success.Equals("false"))
                    model.StatusMessage = "Vui lòng chọn kích cỡ";

            }
            return View(model);
        }

        public List<Product> Search(string q)
        {

            if (!string.IsNullOrEmpty(q))
            {
                var products = _context.Products.Where(p => p.Name.ToLower().Contains(q.ToLower()));
                return products.ToList();
            }
            return new List<Product>();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public class DetailModel
        {
            [TempData]
            public string StatusMessage {  get; set; }
            public Product Product { get; set; }
            public CartItem CartItem { get; set; }
        }

        private bool FilterPrice(long productPrice, string priceFilter)
        {
            _logger.LogInformation($"Filter price: {productPrice}");
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