using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using ShoeStoreApp.Data;
using ShoeStoreApp.Models;
using ShoeStoreApp.Utils;

namespace ShoeStoreApp.Services
{
    public interface IOrderService
    {
        CartItem? CheckAvailability(List<CartItem> cartItems);
        bool Checkout(DeliveryAddress address, List<CartItem> cartItems);
        void SubscribeOutOfStockListener(IOutOfStockListener listener);
        void UnsubscribeOutOfStockListener(IOutOfStockListener listener);
        void NotifyOutOfStockListeners(ProductVariantItem item);
    }
    public class OrderService : IOrderService
    {
        private readonly ShoeStoreAppContext _context;
        private List<IOutOfStockListener> _listeners;
        public OrderService(ShoeStoreAppContext context)
        {
            _context = context;
            _listeners = new List<IOutOfStockListener>();
        }

        public CartItem? CheckAvailability(List<CartItem> cartItems)
        {
            foreach (var cartItem in cartItems)
            {

                if (cartItem.Quantity > cartItem.ProductVariantItem.StockQuantity)
                {
                    return cartItem;
                }
            }
            return null;
        }
        public bool Checkout(DeliveryAddress address, List<CartItem> cartItems)
        {
            var orderItems = new List<OrderItem>();

            var order = new Order
            {
                TimeCreated = DateTime.Now,
                DeliveryAddress = address,
                DeliveryFee = 50000,
                ItemsTotal = CalculateSumOfCartTimes(cartItems),
                Status = OrderStatusCode.PENDING
            };

            foreach (var cartItem in cartItems)
            {
                var orderItem = new OrderItem
                {
                    Order = order,
                    ProductVariantItem = cartItem.ProductVariantItem,
                    Quantity = cartItem.Quantity,
                };
                _context.CartItems.Remove(cartItem);
                orderItem.ProductVariantItem.StockQuantity -= orderItem.Quantity;

                // Notify out of stock
                if (orderItem.ProductVariantItem.StockQuantity <= 0)
                    NotifyOutOfStockListeners(orderItem.ProductVariantItem);

                _context.ProductVariantItems.Update(orderItem.ProductVariantItem);
                orderItems.Add(orderItem);
            }

            order.Items = orderItems;
            order.Total = order.ItemsTotal + order.DeliveryFee;
            _context.Orders.Add(order);
            _context.SaveChanges();
            return true;
        }

        private static long CalculateSumOfCartTimes(List<CartItem> cartItems)
        {
            if (cartItems == null)
                return 0;
            long sum = 0;
            foreach (var cartItem in cartItems)
            {
                sum += cartItem.Quantity * cartItem.ProductVariantItem.ProductVariant.Product.Price;
            }
            return sum;
        }

        public void SubscribeOutOfStockListener(IOutOfStockListener listener)
        {
            if (_listeners.Contains(listener)) return;
            _listeners.Add(listener);
        }

        public void UnsubscribeOutOfStockListener(IOutOfStockListener listener)
        {
            _listeners.Remove(listener);
        }

        public void NotifyOutOfStockListeners(ProductVariantItem item)
        {
            foreach (var listener in _listeners)
            {
                listener.Update(item);
            }
        }

    }

    public interface IOutOfStockListener
    {
        void Update(ProductVariantItem item);
    }

    public class OutOfStockEmailListener : IOutOfStockListener
    {
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private List<ApplicationUser> adminList;

        public OutOfStockEmailListener(IEmailSender emailSender, UserManager<ApplicationUser> userManager)
        {
            _emailSender = emailSender;
            _userManager = userManager;

        }

        public void Update(ProductVariantItem item)
        {
            UpdateAsync(item);
        }

        private async void UpdateAsync(ProductVariantItem item)
        {
            if (adminList == null)
                adminList = (List<ApplicationUser>) _userManager.GetUsersInRoleAsync("Admin").Result;

            string productName = item.ProductVariant.Product.Name;
            string color = PropertyDisplayHelper.GetTextForColor(item.ProductVariant.Color);
            string message = $"Sản phẩm <b>{productName} - {color}</b> đã hết hàng với kích cỡ <b>{item.Size}</b>.";
            foreach (var admin in adminList)
            {
                await _emailSender.SendEmailAsync(admin.Email, "Spring - Thông báo hết hàng", message);
            
            }
        }
    }

    public class OutOfStockLoggingListener : IOutOfStockListener
    {
        public void Update(ProductVariantItem item)
        {
            string filename = "out_of_stock_log.csv";
            string time = DateTime.Now.ToString();
            string productName = item.ProductVariant.Product.Name;
            string color = item.ProductVariant.Color;
            string size = item.Size;
            if (!File.Exists(filename)) 
            {
                string columns = "time,id,name,color,size";
                var file = File.Create(filename);
                file.Close();
                File.WriteAllLines(filename, new string[] { columns });
            }
            string[] lines = { $"{time},{item.Id},{productName},{color},{size}" };
            File.AppendAllLines(filename, lines);
        }
    }
}
