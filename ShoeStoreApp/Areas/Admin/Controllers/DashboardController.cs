using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Cms;
using ShoeStoreApp.Data;
using ShoeStoreApp.Models;
using System.Drawing;
using System.Globalization;

namespace ShoeStoreApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy ="Admin")]
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private ShoeStoreAppContext _context;

        public DashboardController(ILogger<DashboardController> logger, ShoeStoreAppContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index(string time)
        {
            if (time == null) {
                List<long> listTotalInWeek = GetListTotalByWeek(_context);
                return PartialView(listTotalInWeek);
            }
            if (time.Equals("week"))
            {
                List<long> listTotalInWeek = GetListTotalByWeek(_context);
                return PartialView(listTotalInWeek);
            }
            if (time.Equals("month"))
            {
                List<long> listTotalInMonth = GetListTotalByMonth(_context);
                return PartialView(listTotalInMonth);
            } 
            if (time.Equals("year"))
            {
                List<long> listTotalInYear = GetListTotalByYear(_context);
                return PartialView(listTotalInYear);
            }
            return PartialView();
        }
        public static int GetIso8601WeekOfYear(DateTime time)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
        static DateTime GetDateFromWeekNumber(int year, int weekNumber)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            DateTime startOfWeek = jan1.AddDays((weekNumber - 1) * 7 - (int)jan1.DayOfWeek + (int)DayOfWeek.Monday);
            return startOfWeek;
        }
        public static List<long> GetListTotalByWeek(ShoeStoreAppContext context)
        {
            DateTime currentDate = DateTime.Now;
            int numberWeek = GetIso8601WeekOfYear(currentDate);
            DateTime dateStartWeek = GetDateFromWeekNumber(currentDate.Year, numberWeek).Date;
            DateTime dateEndWeek = dateStartWeek.AddDays(6).Date;
            List<long> listTotalInWeek = new List<long>();
            for (DateTime date = dateStartWeek; date <= dateEndWeek; date = date.AddDays(1))
            {
                List<Order> orders = context.Orders.Where(c => c.TimeCreated.Date.Equals(date)).ToList();
                long totalPayInDay = 0;
                foreach (Order order in orders)
                {
                    totalPayInDay += order.Total;
                }
                listTotalInWeek.Add(totalPayInDay);
            }
            return listTotalInWeek;
        }
        public static List<long> GetListTotalByMonth(ShoeStoreAppContext context)
        {
            DateTime currentDate = DateTime.Now;
            int month = currentDate.Month;
            int year = currentDate.Year;
            List<DateTime> daysOfMonth = GetDaysOfMonth(year, month);
            List<long> listTotalInMonth = new List<long>();
            foreach (DateTime date in daysOfMonth){
                List<Order> orders = context.Orders.Where(c => c.TimeCreated.Date.Equals(date)).ToList();
                long totalPayInDay = 0;
                foreach (Order order in orders)
                {
                    totalPayInDay += order.Total;
                }
                listTotalInMonth.Add(totalPayInDay);
            }
            return listTotalInMonth;
        }
        static List<DateTime> GetDaysOfMonth(int year, int month)
        {
            List<DateTime> days = new List<DateTime>();
            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);
            for (int day = 0; day < daysInMonth; day++)
            {
                days.Add(firstDayOfMonth.AddDays(day));
            }
            return days;
        }
        public static List<long> GetListTotalByYear(ShoeStoreAppContext context)
        {
            List<long> listTotalYear = new List<long>();
            for (int i = 1; i <= 12; i++)
            {
                List<Order> orders = context.Orders.Where(c => c.TimeCreated.Month==i).ToList();
                long totalPayInMonth = 0;
                foreach (Order order in orders)
                {
                    totalPayInMonth += order.Total;
                }
                listTotalYear.Add(totalPayInMonth);
            }
            return listTotalYear;
        }
    }
}
