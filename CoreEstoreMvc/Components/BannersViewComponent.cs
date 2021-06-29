using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreEstoreMvc.Components
{
    public class BannersViewComponent : ViewComponent
    {
        private readonly AppDbContext context;

        public BannersViewComponent(AppDbContext context)
        {
            this.context = context;
        }
        public IViewComponentResult Invoke()
        {
            var model = context.Banners.Where(p => p.Enabled && (p.DateStart <= DateTime.Today || p.DateStart == null) && (p.DateEnd >= DateTime.Today || p.DateEnd == null)).ToList();
            return View(model);
        }
    }
}
