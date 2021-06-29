using CoreEstoreMvc.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CoreEstoreMvc.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "Administrators, ProductAdministrators, OrderAdministrators")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext context;

        public DashboardController(AppDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            ViewBag.Comments = context.ProductComments.Where(p => !p.Enabled).Count();
            ViewBag.UserCount = context.Users.Count();
            ViewBag.OrdersCount = context.Orders.Where(p => !p.Enabled && p.OrderState == Data.OrderStates.New).Count();
            ViewBag.ReviewsCount = context.Products.Sum(p => p.Reviews);
            ViewBag.MonthlySalesMonths = JsonConvert.SerializeObject(CultureInfo.CurrentCulture.DateTimeFormat.MonthNames.Where(p=> !string.IsNullOrEmpty(p)));
            
            var salesByMonth = context
            .Orders
            .Where(p => p.OrderState != Data.OrderStates.Cancelled && p.Date.Year == DateTime.Today.Year)
            .AsEnumerable()
            .GroupBy(p => p.Date.Month)
            .Select(p => new { p.Key, Amount = p.Sum(q => q.GrandTotal) })
            .ToList();

            ViewBag.MonthlySalesAmounts = JsonConvert.SerializeObject(
                Enumerable.Range(1, 12)
                .Select(p=> salesByMonth.SingleOrDefault(q=>q.Key==p)?.Amount ?? 0m));

            ViewBag.TopSellerProducts =
                context
                .OrderItems
                .Where(p => p.Order.OrderState != Data.OrderStates.Cancelled && p.Order.Date.Year == DateTime.Today.Year)
                .ToList()
                .GroupBy(p => p.Product)
                .Select(p => new TopSellerProductViewModel { Product = p.Key,TotalSales = p.Sum(q=>q.Quantity) })
                .OrderByDescending(p=>p.TotalSales)
                .Take(5)
                .ToList();

            return View();
        }
    }
}
