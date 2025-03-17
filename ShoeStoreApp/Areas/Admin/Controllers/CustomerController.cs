using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NuGet.Versioning;
using ShoeStoreApp.Data;
using ShoeStoreApp.Models;
using ShoeStoreApp.Services;
using System.Drawing.Printing;

namespace ShoeStoreApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "Admin")]
    public class CustomerController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CustomerController> _logger;
        private ShoeStoreAppContext _context;
        public CustomerController(ILogger<CustomerController> logger, UserManager<ApplicationUser> userManager,
                ShoeStoreAppContext context)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context;

        }
        public async Task<IActionResult> Index(string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["EmailSortParm"] = sortOrder == "email_asc" ? "email_desc" : "email_asc";
            ViewData["StatusSortParm"] = sortOrder == "status_asc" ? "status_desc" : "status_asc";
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;
            var listUser = _userManager.Users;
            if (!String.IsNullOrEmpty(searchString))
            {
                listUser = listUser.Where(s => s.Email.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "email_desc":
                    listUser = listUser.OrderByDescending(s => s.Email); break;
                case "email_asc":
                    listUser = listUser.OrderBy(s => s.Email); break;
                case "status_asc":
                    listUser = listUser.OrderBy(s => s.EmailConfirmed); break;
                case "status_desc":
                    listUser = listUser.OrderByDescending(s => s.EmailConfirmed); break;
            }
            int pageSize = 10;
            CombineCustomer combine = new CombineCustomer();
            var paginatedCustomer = await PaginatedList<ApplicationUser>.CreateAsync(listUser, pageNumber ?? 1, pageSize);
            combine.PaginatedListCustomer = paginatedCustomer;
            combine.TotalOrder = _userManager.Users.Count();
            return PartialView(combine);
        }
        [HttpPost]
        [Route("/DeleteCustomer/{id}", Name = "DeleteCustomer")]
        public async Task<IActionResult> DeleteCustomer([FromRoute] string id)
        {

            var listUser = _userManager.Users;
            ApplicationUser userFind = new ApplicationUser();
            foreach (var user in listUser)
            {
                if (user.Id.Equals(id))
                {
                    userFind = user; break;
                }
            }
            await _userManager.DeleteAsync(userFind);
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Route("/DeleteCustomers", Name = "DeleteCustomers")]
        public async Task<IActionResult> DeleteCustomers(string productSelectedIds)
        {
            string[] productIds = productSelectedIds.Split(',');
            foreach (string id in productIds)
            {
                var listUser = _userManager.Users;
                ApplicationUser userFind = new ApplicationUser();
                foreach (var user in listUser)
                {
                    if (user.Id.Equals(id))
                    {
                        userFind = user;
                        break;
                    }
                }
                await _userManager.DeleteAsync(userFind);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> ExportToExcel()
        {
            CustomerExcelExporter customerExcelExporter = new CustomerExcelExporter(_context);
            return await customerExcelExporter.ExportDataToExcel(this, "Customers.xlsx");
        }
    }
    public class CombineCustomer
    {
        public PaginatedList<ApplicationUser> PaginatedListCustomer { get; set; }
        public int TotalOrder { get; set; }
    }
}
