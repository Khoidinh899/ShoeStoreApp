using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoeStoreApp.Data;
using ShoeStoreApp.Models;
using ShoeStoreApp.Controllers;
using System;
using System.Collections;
using System.Drawing;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Drawing.Printing;
using Microsoft.IdentityModel.Logging;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;
using Newtonsoft.Json;
using Microsoft.Extensions.Hosting.Internal;
using ShoeStoreApp.Services.AdapterImportFile;
using ShoeStoreApp.Interface;
using ShoeStoreApp.Factory;
using ShoeStoreApp.Services;

namespace ShoeStoreApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "Admin")]
    public class ProductController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ShoeStoreAppContext _context;
        private readonly IProductService _productService;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ProductController(ILogger<HomeController> logger, ShoeStoreAppContext context, IWebHostEnvironment hostingEnvironment, IProductService productService)
        {
            _logger = logger;
            _context = context;
            _productService = productService;
            _hostingEnvironment = hostingEnvironment;
        }
       
        public async Task<IActionResult> Index(string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";
            ViewData["BrandSortParm"] = sortOrder == "brand_asc" ? "brand_desc" : "brand_asc";
            ViewData["GenderSortParm"] = sortOrder == "gender_asc" ? "gender_desc" : "gender_asc";
            ViewData["CategorySortParm"] = sortOrder == "category_asc" ? "category_desc" : "category_asc";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;
            var productList = from s in _context.Products select s;
            List<Product> products = _context.Products.ToList();
            Combine combine = new Combine();
            combine.Products = products;
            if (!String.IsNullOrEmpty(searchString))
            {
                productList = _context.Products.Where(s => s.Name.Contains(searchString));
            }

            combine.Products = productList.ToList();

            ISortStrategy sortStrategy = SortStrategyFactory.CreateSortStrategy(sortOrder);
            if(sortStrategy != null)
            {
				productList = sortStrategy.Sort(productList);

			}
			int pageSize = 10;

            var paginatedProducts = await PaginatedList<Product>.CreateAsync(productList, pageNumber ?? 1, pageSize);
            combine.PaginatedListProduct = paginatedProducts;
            return PartialView(combine);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct([Bind("Name", "Price", "Description", "Brand", "Gender", "Category")] Product product, string[] color, IFormFile[] file)
        {
            if (ModelState.IsValid)
            {
                product.Variants = new List<ProductVariant>();
                for (int i = 0; i < color.Length; i++)
                {
                    var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    var fileNameStore = Guid.NewGuid().ToString();
                    var fileName = Path.Combine(uploadFolder, fileNameStore + Path.GetExtension(file[i].FileName));
                    using (var fileStream = new FileStream(fileName, FileMode.Create))
                    {
                        await file[i].CopyToAsync(fileStream);
                    }
                    ProductVariant newVarient = new ProductVariant
                    {
                        Color = String.Join("/", color[i]),
                        ImgPath = "/images/" + fileNameStore + Path.GetExtension(file[i].FileName),
                        Product = product,
                    };
                    product.Variants.Add(newVarient);
                }

                _productService.AddProduct(product);
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            _productService.DeleteProductById(id);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteProducts(string productSelectedIds)
        {
            string[] productIds = productSelectedIds.Split(',');
            int[] intProductIds = productIds.Select(int.Parse).ToArray();
            foreach (int id in intProductIds)
            {
                _productService.DeleteProductById(id);
            }

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            Product product = _context.Products.Find(id);
            List<ProductVariant> productVariants = _context.ProductVariants.Where(
                    c => c.ProductId == id).Include(c => c.Items).ToList();
            CombineEditProduct combine = new CombineEditProduct
            {
                Product = product,
                ProductVariants = productVariants,
            };
            return PartialView(combine);
        }
        [HttpPost]
        public async Task<IActionResult> PostEditProduct([FromRoute] int id,
            [Bind("Name", "Price", "Description", "Brand", "Gender", "Category")] Product product,
            string[] color,
            IFormFile[] file,
            string[] size,
            string saveProductEditImage)
        {
            //Change product
            _productService.UpdateProduct(id, product, color, file, size, saveProductEditImage);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> ExportToExcel()
        {
            ProductExcelExporter productExcelExporter = new ProductExcelExporter(_context);
            return await productExcelExporter.ExportDataToExcel(this, "Products.xlsx");
        }
        [HttpPost]
        public async Task<IActionResult> ImportToExcel()
        {
            var webRootPath = _hostingEnvironment.WebRootPath;
            var filePath = Path.Combine(webRootPath, "Uploads", "Products.xlsx");
            try
            {
                // call adaptee
                var excelProductData = new ExcelProductDataReader().FromFile(filePath);
                // call adapter
                var productDataReader = new ProductImportAdapter(excelProductData);
                var products = productDataReader.GetProducts();
                
                foreach (Product product in products)
                {
                    _context.Products.Add(product);
                    _context.SaveChanges();
                    List<Product> products1 = _context.Products.ToList();
                    foreach (Product p in products1)
                    {
                        if (product.Name.Equals(p.Name))
                        {
                            
                            List<ProductVariant> variants = p.Variants.ToList();
                            foreach (ProductVariant variant in variants)
                            {
                                for (int j = 36; j <= 45; j++)
                                {
                                    ProductVariantItem newVariantItem = new ProductVariantItem
                                    {
                                        Size = j.ToString(),
                                        StockQuantity = 10,
                                        ProductVariantId = variant.Id
                                    };
                                    _context.ProductVariantItems.Add(newVariantItem);
                                    _context.SaveChanges();
                                }
                            }
                        }
                    }
                }   
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }
    }
    public class Combine
    {
        public List<Product> Products { get; set; }
        public Product Product { get; set; }
        public PaginatedList<Product> PaginatedListProduct { get; set; }
    }
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
    public class CombineEditProduct
    {
        public Product Product { get; set; }
        public List<ProductVariant> ProductVariants { get; set; }

    }
}