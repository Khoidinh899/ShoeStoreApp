using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ShoeStoreApp.Data;
using ShoeStoreApp.Models;

namespace ShoeStoreApp.Services.AdapterImportFile
{
    public class ExcelProductDataReader
    {
        private string _filePath;
        public ExcelProductDataReader FromFile(string filePath)
        {
            _filePath = filePath;
            return this;
        }
        public IEnumerable<Product> GetProducts()
        {
            var productList = new List<Product>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var stream = new FileStream(_filePath, FileMode.Open))
            {
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                    {
                        Product productSelected = new Product();
                        bool flag = false;
                        foreach(Product p in productList) 
                        {
                            if (p.Name.Equals(worksheet.Cells[row, 1].Value.ToString()))
                            {
                                productSelected = p;
                                flag = true;
                                break;
                            }
                        }
                        if (flag)
                        {
                            ProductVariant newVarient = new ProductVariant
                            {
                                Color = worksheet.Cells[row, 7].Value.ToString(),
                                ImgPath = worksheet.Cells[row, 8].Value.ToString(),
                                Product = productSelected,
                            };
                            productSelected.Variants.Add(newVarient);
                        } else
                        {
                            var product = new Product
                            {
                                Name = worksheet.Cells[row, 1].Value.ToString(),
                                Brand = worksheet.Cells[row, 2].Value.ToString(),
                                Price = long.Parse(worksheet.Cells[row, 3].Value.ToString()),
                                Description = worksheet.Cells[row, 4].Value.ToString(),
                                Gender = worksheet.Cells[row, 5].Value.ToString(),
                                Category = worksheet.Cells[row, 6].Value.ToString()
                            };
                            product.Variants = new List<ProductVariant>();
                            ProductVariant newVarient = new ProductVariant
                            {
                                Color = worksheet.Cells[row, 7].Value.ToString(),
                                ImgPath = worksheet.Cells[row, 8].Value.ToString(),
                                Product = product,
                            };
                            product.Variants.Add(newVarient);
                            productList.Add(product);
                            
                        }                 
                    }
                }
            }
            return productList;
        }
    }

}
