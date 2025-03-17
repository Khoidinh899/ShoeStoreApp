using Microsoft.AspNetCore.Mvc;
using ShoeStoreApp.Models;
using System.Diagnostics;
using ShoeStoreApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace ShoeStoreApp.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ShoeStoreAppContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CartController> _logger;

        public CartController(ShoeStoreAppContext context, UserManager<ApplicationUser> userManager, ILogger<CartController> logger)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Redirect("/Identity/Account/Login");
            }

            var cartItems = _context.CartItems.Include(i => i.ProductVariantItem).ThenInclude(i => i.ProductVariant).ThenInclude(v => v.Product).Where(i => i.Customer.Id.Equals(user.Id)).ToList();

            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(CartItem cartItem, string returnUrl)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Redirect("/Identity/Account/Login");
            }
            var itemId = cartItem.ProductVariantItemId;
            var productVariantItem = _context.ProductVariantItems.Find(itemId);
            if (productVariantItem == null)
            {

                return Redirect(returnUrl + "?success=false");
            }


            var cartItems = _context.CartItems.Where(i => i.Customer.Id.Equals(user.Id)).ToList();


            bool exists = cartItems.Any(i => i.ProductVariantItemId == itemId);
            if (exists)
            {
                var cartItemInCart = cartItems.Single(i => i.ProductVariantItemId == itemId);
                cartItemInCart.Quantity += 1;
            }
            else
            {
                cartItem.Quantity = 1;
                cartItem.Customer = user;
                cartItem.ProductVariantItem = productVariantItem;
                _context.CartItems.Add(cartItem);
            }
            _context.SaveChanges();
            return Redirect(returnUrl + "?success=true");
        }

        public async Task<IActionResult> IncrementQuantity(int itemId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Redirect("/Identity/Account/Login");
            }

            var cartItemInCart = _context.CartItems.Find(itemId);
            if (cartItemInCart == null || !cartItemInCart.Customer.Id.Equals(user.Id)) {
                return RedirectToAction("Index");
            }

            cartItemInCart.Quantity += 1;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DecrementQuantity(int itemId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Redirect("/Identity/Account/Login");
            }

            var cartItemInCart = _context.CartItems.Find(itemId);
            if (cartItemInCart == null || !cartItemInCart.Customer.Id.Equals(user.Id))
            {
                return RedirectToAction("Index");
            }

            if (cartItemInCart.Quantity == 1)
            {
                _context.CartItems.Remove(cartItemInCart);
                ViewBag.CartItemCount = GetCartCount(user);
            }
            else if (cartItemInCart.Quantity > 1)
                cartItemInCart.Quantity -= 1;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> RemoveCartItem(int itemId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Redirect("/Identity/Account/Login");
            }

            var cartItemInCart = _context.CartItems.Find(itemId);
            if (cartItemInCart == null || !cartItemInCart.Customer.Id.Equals(user.Id))
            {
                return RedirectToAction("Index");
            }

            _context.CartItems.Remove(cartItemInCart);
            _context.SaveChanges();
            ViewBag.CartItemCount = GetCartCount(user);

            return RedirectToAction("Index");
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private int GetCartCount(ApplicationUser user)
        {
            if (user == null)
            {
                return 0;
            }

            var cartItems = _context.CartItems.Where(i => i.Customer.Id.Equals(user.Id)).ToList();
            _logger.LogInformation("Cart: " +  cartItems.Count);
            return cartItems.Count;
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
    }
}