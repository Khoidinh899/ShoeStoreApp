// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoeStoreApp.Controllers;
using ShoeStoreApp.Data;
using ShoeStoreApp.Models;

namespace ShoeStoreApp.Areas.Identity.Pages.Account.Manage
{
    public class OrderModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ShoeStoreAppContext _context;
        private readonly ILogger<AddressModel> _logger;


        public OrderModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, ShoeStoreAppContext context, ILogger<AddressModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }


        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// 

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// 
        public List<Order> Orders { get; set; }


        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);

            Username = userName;
        }

        public async Task<IActionResult> OnGetAsync(int statusCode = -1)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            await LoadAsync(user);

            ViewData["statusCode"] = statusCode;

            if (statusCode >= 0 && statusCode <= 3) {
                Orders = _context.Orders.Include(o => o.DeliveryAddress).Include(o => o.Items).ThenInclude(i => i.ProductVariantItem).ThenInclude(i => i.ProductVariant).ThenInclude(v => v.Product).Where(a => a.DeliveryAddress.CustomerId.Equals(user.Id) && a.Status == statusCode).OrderByDescending(o => o.TimeCreated).ToList();
            }
            else
            {
                Orders = _context.Orders.Include(o => o.DeliveryAddress).Include(o => o.Items).ThenInclude(i => i.ProductVariantItem).ThenInclude(i => i.ProductVariant).ThenInclude(v => v.Product).Where(a => a.DeliveryAddress.CustomerId.Equals(user.Id)).OrderByDescending(o => o.TimeCreated).ToList();
            }

            return Page();
        }

    }
}
