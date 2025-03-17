using Microsoft.AspNetCore.Identity;
using OfficeOpenXml;
using ShoeStoreApp.Models;

namespace ShoeStoreApp.Services
{
    public class ExcelService
    {
        public static ExcelService _instance;
        private ExcelService()
        {
        }
        public static ExcelService GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ExcelService();
            }
            return _instance;
        }
        public byte[] GenerateProductsToExcel(List<Product> products)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Products");
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Tên sản phẩm";
                worksheet.Cells[1, 3].Value = "Hãng";
                worksheet.Cells[1, 4].Value = "Giá";
                worksheet.Cells[1, 5].Value = "Mô tả";
                worksheet.Cells[1, 6].Value = "Giới tính";
                worksheet.Cells[1, 7].Value = "Loại";
                int row = 2;
                foreach (var product in products)
                {
                    worksheet.Cells[row, 1].Value = product.Id;
                    worksheet.Cells[row, 2].Value = product.Name;
                    worksheet.Cells[row, 3].Value = product.Brand;
                    worksheet.Cells[row, 4].Value = product.Price;
                    worksheet.Cells[row, 5].Value = product.Description;
                    worksheet.Cells[row, 6].Value = product.Gender;
                    worksheet.Cells[row, 7].Value = product.Category;
                    row++;
                }
                return package.GetAsByteArray();
            }
        }
        public byte[] GenerateCustomersToExcel(List<ApplicationUser> customers)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Customers");
                worksheet.Cells[1, 1].Value = "STT";
                worksheet.Cells[1, 2].Value = "Email";
                worksheet.Cells[1, 3].Value = "Số điện thoại";
                worksheet.Cells[1, 4].Value = "Trạng thái";
                int row = 2;
                foreach (var customer in customers)
                {
                    worksheet.Cells[row, 1].Value = row-1;
                    worksheet.Cells[row, 2].Value = customer.Email;
                    worksheet.Cells[row, 3].Value = customer.PhoneNumber;
                    if (customer.EmailConfirmed)
                    {
                        worksheet.Cells[row, 4].Value = "Đã xác nhận";
                    } else
                    {
                        worksheet.Cells[row, 4].Value = "Chưa xác nhận";

                    }
                    row++;
                }
                return package.GetAsByteArray();
            }
        }
        public byte[] GenerateOrdersToExcel(List<Order> orders)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Orders");
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Trạng thái";
                worksheet.Cells[1, 3].Value = "Địa chỉ giao";
                worksheet.Cells[1, 4].Value = "Tổng tiền";
                worksheet.Cells[1, 5].Value = "Ngày tạo";
                int row = 2;
                foreach (var order in orders)
                {
                    worksheet.Cells[row, 1].Value = order.Id;
                    worksheet.Cells[row, 2].Value = order.Status;
                    worksheet.Cells[row, 3].Value = order.DeliveryAddress;
                    worksheet.Cells[row, 4].Value = order.ItemsTotal;
                    worksheet.Cells[row, 5].Value = order.TimeCreated;
                    row++;
                }
                return package.GetAsByteArray();
            }
        }
    }
}
