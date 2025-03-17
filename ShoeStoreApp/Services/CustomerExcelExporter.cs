using Microsoft.AspNetCore.Identity;
using ShoeStoreApp.Data;
using ShoeStoreApp.Models;

namespace ShoeStoreApp.Services
{
    public class CustomerExcelExporter : ExcelExporter
    {
        public CustomerExcelExporter(ShoeStoreAppContext context) : base(context)
        {
        }
        public override object getData()
        {
            return _context.Users.ToList();
        }
        public override byte[] ExportFromDataToExcel(object data)
        {
            List<ApplicationUser> customers = (List<ApplicationUser>)data;
            byte[] fileContents = _excelService.GenerateCustomersToExcel(customers);
            return fileContents;
        }
    }
}
