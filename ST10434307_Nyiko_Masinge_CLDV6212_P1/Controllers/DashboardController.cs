using Microsoft.AspNetCore.Mvc;

namespace ST10434307_Nyiko_Masinge_CLDV6212_P1.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            
            /*
        Code attribution
        https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/actions?view=aspnetcore-8.0
        accessed 7 October 2025
            */



            //  backend data
            var dashboardData = new
            {
                TotalCustomers = 152,
                TotalSales = 48450.75,
                PendingOrders = 8,
                TotalProducts = 27,
                RevenueGrowth = 12.5
            };

            return View(dashboardData);
        }
    }
}
