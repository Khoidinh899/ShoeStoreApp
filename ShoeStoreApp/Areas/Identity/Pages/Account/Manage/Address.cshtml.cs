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
using ShoeStoreApp.Controllers;
using ShoeStoreApp.Data;
using ShoeStoreApp.Models;

namespace ShoeStoreApp.Areas.Identity.Pages.Account.Manage
{
    public class AddressModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ShoeStoreAppContext _context;
        private readonly ILogger<AddressModel> _logger;


        public AddressModel(
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
        public List<DeliveryAddress> Addresses { get; set; }
        
        [BindProperty]
        public InputModel Input {  get; set; }

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

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);

            Username = userName;

            Addresses = _context.DeliveryAddresses.Where(a => a.CustomerId.Equals(user.Id)).ToList();


        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            DeliveryAddress address = new DeliveryAddress
            {
                Name = Input.Name,
                PhoneNumber = Input.PhoneNumber,
                Address = Input.HouseAddress + ", " + Input.WardAddress + ", " + Input.DistrictAddress + ", " + Input.CityAddress,
                IsDefault = Input.IsDefault
            };

            user.DeliveryAddresses.Add(address);
            if (Input.IsDefault)
            {
                var addresses = _context.DeliveryAddresses.ToList();
                foreach (var a in addresses)
                {
                    a.IsDefault = false;
                }
            }

            _context.DeliveryAddresses.Add(address);
            _context.SaveChanges();


            StatusMessage = "Thêm địa chỉ thành công.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAddressAsync(int addressId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }


            var address = _context.DeliveryAddresses.Single(a => a.Id == addressId);
            if (address == null || !address.CustomerId.Equals(user.Id))
            {
                StatusMessage = "Xóa địa chỉ thất bại.";
                return RedirectToPage();
            }


            _context.DeliveryAddresses.Remove(address);
            _context.SaveChanges();
            _logger.LogInformation(addressId.ToString());

            StatusMessage = "Xóa địa chỉ thành công.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSetDefaultAddressAsync(int addressId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }


            var address = _context.DeliveryAddresses.Single(a => a.Id == addressId);
            address.IsDefault = true;
            var addresses = _context.DeliveryAddresses.ToList();
            foreach (var a in addresses)
            {
                if (a.Id != addressId)
                    a.IsDefault = false;
            }
            
            _context.SaveChanges();
            return RedirectToPage();
        }
    }
}
