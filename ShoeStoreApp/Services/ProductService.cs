using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoeStoreApp.Data;
using ShoeStoreApp.Models;

namespace ShoeStoreApp.Services
{
    public interface IProductService
    {
        IEnumerable<Product> GetAllProducts();
        void AddProduct(Product product);
        void DeleteProductById(int id);
        void UpdateProduct(int id,
            Product product,
            string[] color,
            IFormFile[] file,
            string[] size,
            string saveProductEditImage);
    }

    public class ProductService : IProductService
    {
        private readonly ShoeStoreAppContext _context;
        public ProductService(ShoeStoreAppContext context)
        {
            _context = context;
        }

        public void AddProduct(Product product)
        {
            if (product == null) return;
            _context.Products.Add(product);
            _context.SaveChanges();
            List<ProductVariant> variants = product.Variants.ToList();
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

        public void DeleteProductById(int id)
        {
            Product product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Product> GetAllProducts()
        {
            IEnumerable<Product> products = _context.Products.Include(p => p.Variants).ToList();
            return products;
        }

        public void UpdateProduct(int id, Product product, string[] color, IFormFile[] file, string[] size, string saveProductEditImage)
        {
            //Change product
            Product productToEdit = _context.Products.Include(c => c.Variants).Where(c => c.Id == id).ToList()[0];
            productToEdit.Name = product.Name;
            productToEdit.Description = product.Description;
            productToEdit.Price = product.Price;
            productToEdit.Brand = product.Brand;
            productToEdit.Gender = product.Gender;
            productToEdit.Category = product.Category;
            _context.SaveChanges();
            //Change productVariant
            List<ProductVariant> productVariants = productToEdit.Variants.ToList();
            int count = 0;
            int imgCount = 0;
            foreach (var variant in productVariants)
            {
                variant.Color = color[count];
                if (file.Length != 0)
                {
                    string[] numberOfImgPathEdits = { };
                    if (saveProductEditImage != "")
                    {
                        numberOfImgPathEdits = saveProductEditImage.Substring(0, saveProductEditImage.Length - 3).Split(" / ");
                    }
                    int[] intVariantChangeImage = numberOfImgPathEdits.Select(int.Parse).ToArray();
                    Array.Sort(intVariantChangeImage);
                    if (intVariantChangeImage.Contains(count + 1))
                    {
                        var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                        var fileNameStore = Guid.NewGuid().ToString();
                        var fileName = Path.Combine(uploadFolder, fileNameStore + Path.GetExtension(file[imgCount].FileName));
                        using (var fileStream = new FileStream(fileName, FileMode.Create))
                        {
                            file[imgCount].CopyTo(fileStream);
                        }
                        variant.ImgPath = "/images/" + fileNameStore + Path.GetExtension(file[imgCount].FileName);
                        imgCount++;
                    }
                }
                _context.SaveChanges();
                // Change product variant size
                ProductVariant variantSeleted = _context.ProductVariants.Include(c => c.Items)
                    .Where(c => c.Id == variant.Id).ToList()[0];
                List<ProductVariantItem> items = variantSeleted.Items.ToList();
                foreach (ProductVariantItem item in items)
                {
                    string stockQuantityBySize = size[count];
                    Dictionary<string, int> productVariantItems = ParseInputString(stockQuantityBySize);
                    item.StockQuantity = productVariantItems[item.Size];
                    _context.SaveChanges();
                }
                count++;

            }
        }
        private static Dictionary<string, int> ParseInputString(string input)
        {
            string[] pairs = input.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, int> keyValuePairs = new Dictionary<string, int>();
            foreach (string pair in pairs)
            {
                string[] parts = pair.Trim().Split('-');
                if (parts.Length == 2 && int.TryParse(parts[1], out int value))
                {
                    keyValuePairs.Add(parts[0], value);
                }
            }
            return keyValuePairs;
        }
    }

    public class CachedProductData
    {
        private IEnumerable<Product> products;

        public IEnumerable<Product> GetProducts()
        {
            return products;
        }

        public void SetProducts(IEnumerable<Product> products)
        {
            this.products = products;
        }

        public void ClearCache()
        {
            products = null;
        }
    }

    public class CachedProductService : IProductService
    {
        /// <summary>
        /// Proxy class for Product Service for caching
        /// </summary>
        private bool needReset; // Trạng thái cập nhật của dữ liệu cache
        private CachedProductData cachedProductData; // Dữ liệu cache
        private ProductService _realService; // Dịch vụ gốc

        public CachedProductService(ProductService service, CachedProductData cachedProductData)
        {
            _realService = service;
            this.cachedProductData = cachedProductData;
            needReset = true;
        }

        public void AddProduct(Product product)
        {
            _realService.AddProduct(product);
            Invalidate();
        }

        public void DeleteProductById(int id)
        {
            _realService.DeleteProductById(id); 
            Invalidate();
        }

        public IEnumerable<Product> GetAllProducts()
        {
            if (needReset) 
            {
                Console.WriteLine("Caching products...");
                cachedProductData.SetProducts(_realService.GetAllProducts());
                needReset = false;
            }
            return cachedProductData.GetProducts();
        }

        public void Invalidate()
        {
            needReset = true;
            cachedProductData.ClearCache();
        }

        public void UpdateProduct(int id, Product product, string[] color, IFormFile[] file, string[] size, string saveProductEditImage)
        {
            _realService.UpdateProduct(id, product, color, file, size, saveProductEditImage);
            Invalidate();
        }
    }


}
