using ShoeStoreApp.Data;
using ShoeStoreApp.Models;

namespace ShoeStoreApp.Services
{
    public class OrderExcelExporter : ExcelExporter
    {
        public OrderExcelExporter(ShoeStoreAppContext context) : base(context)
        {
        }
        public override object getData()
        {
            return _context.Orders.ToList();
        }
        public override byte[] ExportFromDataToExcel(object data)
        {
            List<Order> orders = (List<Order>)data;
            byte[] fileContents = _excelService.GenerateOrdersToExcel(orders);
            return fileContents;
        }
    }
}
