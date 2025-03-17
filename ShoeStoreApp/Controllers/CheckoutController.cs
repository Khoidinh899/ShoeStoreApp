using Microsoft.AspNetCore.Mvc;
using ShoeStoreApp.Models;
using System.Diagnostics;
using ShoeStoreApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using ShoeStoreApp.Utils;
using ShoeStoreApp.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using ShoeStoreApp.Services;

namespace ShoeStoreApp.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ShoeStoreAppContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IOrderService _orderService;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(ShoeStoreAppContext context, UserManager<ApplicationUser> userManager, ILogger<CheckoutController> logger, IOrderService orderService, IEmailSender emailSender)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _orderService = orderService;
            _emailSender = emailSender;


            _orderService.SubscribeOutOfStockListener(new OutOfStockLoggingListener());
            _orderService.SubscribeOutOfStockListener(new OutOfStockEmailListener(emailSender, userManager));
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Redirect("/Identity/Account/Login");
            }

            var cartItems = _context.CartItems.Include(i => i.ProductVariantItem).ThenInclude(i => i.ProductVariant).ThenInclude(v => v.Product).Where(i => i.Customer.Id.Equals(user.Id)).ToList();

            if (cartItems.Count == 0)
                return Redirect("/Products");
            
            var addresses = _context.DeliveryAddresses.Where(a => a.CustomerId.Equals(user.Id)).ToList();

            var model = new CheckoutModel
            {
                CartItems = cartItems,
                DeliveryAddresses = addresses
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(int addressId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Redirect("/Identity/Account/Login");
            }
            var address = _context.DeliveryAddresses.Find(addressId);

            if (address == null)
            {
                return Redirect("/Products");
            }

            var cartItems = _context.CartItems.Include(i => i.ProductVariantItem).ThenInclude(i => i.ProductVariant).ThenInclude(v => v.Product).Where(i => i.Customer.Id.Equals(user.Id)).ToList();

            if (cartItems.Count == 0)
                return Redirect("/Products");

            var failedItem = _orderService.CheckAvailability(cartItems);
            if (failedItem != null)
            {

                return RedirectToAction("Index");
            }

            var orderItems = new List<OrderItem>();


            var order = new Order
            {
                TimeCreated = DateTime.Now,
                DeliveryAddress = address,
                DeliveryFee = 50000,
                ItemsTotal = CalculateSumOfCartTimes(cartItems),
                Status = OrderStatusCode.PENDING
            };
            bool checkoutFailed = false;

            foreach (var cartItem in cartItems)
            {

                if (cartItem.Quantity > cartItem.ProductVariantItem.StockQuantity)
                {
                    checkoutFailed = true;
                    TempData["FailedCartItem"] = cartItem.ProductVariantItem.ProductVariant.Product.Name + " - " + PropertyDisplayHelper.GetTextForColor(cartItem.ProductVariantItem.ProductVariant.Color);
                    break;
                }
                var orderItem = new OrderItem
                {
                    Order = order,
                    ProductVariantItem = cartItem.ProductVariantItem,
                    Quantity = cartItem.Quantity,
                };
                _context.CartItems.Remove(cartItem);
                orderItem.ProductVariantItem.StockQuantity -= orderItem.Quantity;
                _context.ProductVariantItems.Update(orderItem.ProductVariantItem);
                orderItems.Add(orderItem);
            }

            if (checkoutFailed) { 

                TempData["FailedCartItem"] = failedItem.ProductVariantItem.ProductVariant.Product.Name + " - " + PropertyDisplayHelper.GetTextForColor(failedItem.ProductVariantItem.ProductVariant.Color);

                return RedirectToAction("Failure");
            }
            _orderService.Checkout(address, cartItems);

            return RedirectToAction("Success");
        }

        [HttpPost]
        public async Task<IActionResult> CheckoutWithNewAddress(InputModel input)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Redirect("/Identity/Account/Login");
            }

            var cartItems = _context.CartItems.Include(i => i.ProductVariantItem).ThenInclude(i => i.ProductVariant).ThenInclude(v => v.Product).Where(i => i.Customer.Id.Equals(user.Id)).ToList();

            if (cartItems.Count == 0)
                return Redirect("/Products");

            var failedItem = _orderService.CheckAvailability(cartItems);
            if (failedItem != null)
            {
                TempData["FailedCartItem"] = failedItem.ProductVariantItem.ProductVariant.Product.Name + " - " + PropertyDisplayHelper.GetTextForColor(failedItem.ProductVariantItem.ProductVariant.Color);
                return RedirectToAction("Failure");
            }

            var address = new DeliveryAddress
            {
                Name = input.Name,
                PhoneNumber = input.PhoneNumber,
                Address = input.HouseAddress + ", " + input.WardAddress + ", " + input.DistrictAddress + ", " + input.CityAddress,
                IsDefault = input.IsDefault,
                Customer = user
            };

            user.DeliveryAddresses.Add(address);

            // set default address
            if (input.IsDefault)
            {
                var addresses = _context.DeliveryAddresses.ToList();
                foreach (var a in addresses)
                {
                    a.IsDefault = false;
                }
            }

            _context.DeliveryAddresses.Add(address);

            _orderService.Checkout(address, cartItems);
            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View();
        }

        public IActionResult Failure()
        {
            if (string.IsNullOrEmpty((string)TempData["FailedCartItem"]))
                return Redirect("/Products");
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public static long CalculateSumOfCartTimes(List<CartItem> cartItems)
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

        public class CheckoutModel
        {
            public List<DeliveryAddress> DeliveryAddresses { get; set; }
            public List<CartItem> CartItems { get; set; }
            public InputModel Input { get; set; }
        }
        public class InputModel
        {
            [Required]
            [Display(Name = "Họ và tên")]
            public string Name { get; set; }

            [Required]
            [Phone]
            [Display(Name = "Số điện thoại")]
            public string PhoneNumber { get; set; }

            [Required]
            [Display(Name = "Địa chỉ nhà")]
            public string HouseAddress { get; set; }

            [Required]
            [Display(Name = "Tỉnh / Thành phố")]
            public string CityAddress { get; set; }


            [Required]
            [Display(Name = "Quận / Huyện")]
            public string DistrictAddress { get; set; }

            [Required]
            [Display(Name = "Phường / Xã")]
            public string WardAddress { get; set; }

            [Required]
            public bool IsDefault { get; set; }
        }
    }
}