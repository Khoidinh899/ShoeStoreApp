using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoeStoreApp.Data;
using ShoeStoreApp.Interface;
using ShoeStoreApp.Models;
using ShoeStoreApp.Services;

namespace ShoeStoreApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "Admin")]
    public class OrderController : Controller
    {
        private readonly ILogger<OrderController> _logger;
        private ShoeStoreAppContext _context;

        public OrderController(ILogger<OrderController> logger, ShoeStoreAppContext context)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<IActionResult> Index(string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["StatusSortParm"] = sortOrder == "status_asc" ? "status_desc" : "status_asc";
            ViewData["NameCustomerSortParm"] = sortOrder == "customerName_asc" ? "customerName_desc" : "customerName_asc";
            ViewData["TotalSortParm"] = sortOrder == "money_asc" ? "money_desc" : "money_asc";
            ViewData["DateSortParm"] = sortOrder == "date_asc" ? "date_desc" : "date_asc";
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;
            var orderList = from s in _context.Orders select s;
            orderList = orderList.Include(c => c.DeliveryAddress)
                .Include(e => e.Items);

            if (!String.IsNullOrEmpty(searchString))
            {
                orderList = orderList.Include(c => c.DeliveryAddress).Where(
                    s => s.TimeCreated.ToString().Contains(searchString) ||
                    s.DeliveryAddress.Name.Contains(searchString) ||
                    s.DeliveryAddress.Address.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "status_asc":
                    orderList = orderList.Include(c => c.DeliveryAddress).OrderBy(e => e.Status);
                    break;
                case "status_desc":
                    orderList = orderList.Include(c => c.DeliveryAddress).OrderByDescending(e => e.Status);
                    break;
                case "customerName_asc":
                    orderList = orderList.Include(c => c.DeliveryAddress).OrderBy(e => e.DeliveryAddress.Name);
                    break;
                case "customerName_desc":
                    orderList = orderList.Include(c => c.DeliveryAddress).OrderByDescending(e => e.DeliveryAddress.Name);
                    break;
                case "money_asc":
                    orderList = orderList.Include(c => c.DeliveryAddress).OrderBy(e => e.Total);
                    break;
                case "money_desc":
                    orderList = orderList.Include(c => c.DeliveryAddress).OrderByDescending(e => e.Total);
                    break;
                case "date_asc":
                    orderList = orderList.Include(c => c.DeliveryAddress).OrderBy(e => e.TimeCreated);
                    break;
                case "date_desc":
                    orderList = orderList.Include(c => c.DeliveryAddress).OrderByDescending(e => e.TimeCreated);
                    break;

            }
            int pageSize = 10;
            OrderCombine combine = new OrderCombine();
            var paginatedOrder = await PaginatedList<Order>.CreateAsync(orderList, pageNumber ?? 1, pageSize);
            combine.PaginatedListOrder = paginatedOrder;
            //get all order item 
            List<OrderItem> orderItemList = _context.OrderItems.ToList();
            Dictionary<int, List<OrderItem>> dictionaryOrderItem = new Dictionary<int, List<OrderItem>>();
            List<Order> orders = _context.Orders.ToList();

            foreach (Order order in orders)
            {
                dictionaryOrderItem.Add(order.Id,
                    _context.OrderItems.Where(c => c.OrderId == order.Id)
                    .Include(e => e.ProductVariantItem.ProductVariant.Product)
                    .ToList());
            }
            combine.DictionaryOrderItem = dictionaryOrderItem;
            combine.TotalOrder = _context.Orders.ToList().Count;
            return PartialView(combine);
        }
        [HttpPost]
        [Route("/OrderSuccess", Name = "OrderSuccess")]
        public async Task<IActionResult> OrderSuccess(string orderSelectedIdSuccess)
        {
            string[] orderIds = orderSelectedIdSuccess.Split(',');
            int[] intOrderIds = orderIds.Select(int.Parse).ToArray();

            foreach (int orderId in intOrderIds)
            {
                Order order = _context.Orders.Find(orderId);
                if (order.Status == OrderStatusCode.DELIVERING)
                {
                    order.Status = OrderStatusCode.SUCCESS;
                    _context.SaveChanges();
                }
            }
         
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Route("/OrderPending", Name = "OrderPending")]
        public async Task<IActionResult> OrderPending(string orderSelectedIdPending)
        {
            string[] orderIds = orderSelectedIdPending.Split(',');
            int[] intOrderIds = orderIds.Select(int.Parse).ToArray();
            foreach (int orderId in intOrderIds)
            {
                Order order = _context.Orders.Find(orderId);
                if (order.Status == OrderStatusCode.PENDING)
                {
                    order.Status = OrderStatusCode.DELIVERING;
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("/OrderCancelled", Name = "OrderCancelled")]
        public async Task<IActionResult> OrderCancelled(string orderSelectedIdCancelled)
        {
            string[] orderIds = orderSelectedIdCancelled.Split(',');
            int[] intOrderIds = orderIds.Select(int.Parse).ToArray();
            foreach (int orderId in intOrderIds)
            {
                Order order = _context.Orders.Find(orderId);
                if (order.Status == OrderStatusCode.PENDING)
                {
                    order.Status = OrderStatusCode.CANCELLED;
                    _context.SaveChanges();
                }
            }
        
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Route("/DeleteOrder/{id}", Name = "DeleteOrder")]
        public async Task<IActionResult> DeleteOrder([FromRoute] int id)
        {
            Order order = _context.Orders.Find(id);
            _context.Orders.Remove(order);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> ExportToExcel()
        {
            OrderExcelExporter orderExcelExporter = new OrderExcelExporter(_context);
            return await orderExcelExporter.ExportDataToExcel(this, "Orders.xlsx");
        }
    }
    public class OrderCombine
    {
        public PaginatedList<Order> PaginatedListOrder { get; set; }
        public Dictionary<int, List<OrderItem>> DictionaryOrderItem { get; set; }
        public int TotalOrder { get; set; }
    }
}
