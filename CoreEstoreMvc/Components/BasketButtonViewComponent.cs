using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CoreEstoreMvc.Services;
using CoreEstoreMvc.Data;
using System.Linq;

namespace CoreEstoreMvc.Components
{
    public class BasketButtonViewComponent : ViewComponent
    {
        private readonly AppDbContext context;
        private readonly UserManager<User> userManager;
        private readonly IShoppingCartService shoppingCartService;

        public BasketButtonViewComponent(
            AppDbContext context,
            UserManager<User> userManager,
            IShoppingCartService shoppingCartService
            )
        {
            this.context = context;
            this.userManager = userManager;
            this.shoppingCartService = shoppingCartService;
        }
        public IViewComponentResult Invoke()
        {
            return View(shoppingCartService.ItemCountAsync().Result);
        }
    }
}