using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreEstoreMvc.Components
{
         
    
    public class BrandsBarViewComponent : ViewComponent
    {
        private readonly AppDbContext context;

        public BrandsBarViewComponent(AppDbContext context)
        {
            this.context = context;
        }
        public IViewComponentResult Invoke()
        {
            var model = context.Brands.Where(p => p.Enabled).ToList();
            return View(model);
        }
    }
}
