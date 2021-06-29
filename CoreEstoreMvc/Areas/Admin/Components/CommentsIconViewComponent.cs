using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreEstoreMvc.Areas.Admin.Components
{
    public class CommentsIconViewComponent : ViewComponent
    {
        private readonly AppDbContext context;

        public CommentsIconViewComponent(AppDbContext context)
        {
            this.context = context;
        }
        public IViewComponentResult Invoke()
        {
            var model = context.ProductComments.Where(p => !p.Enabled).ToList();
            return View(model);
        }

    }
}
