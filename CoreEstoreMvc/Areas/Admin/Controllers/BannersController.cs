using CoreEstoreMvc;
using CoreEstoreMvc.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using X.PagedList;

namespace MVCCoreEStore.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "Administrators, ProductAdministrators")]
    public class BannersController : Controller
    {
        private readonly AppDbContext context;
        private readonly UserManager<User> userManager;
        private readonly ILogger<BannersController> logger;
        private readonly string name = "Tanıtım Görseli";

        public BannersController(AppDbContext context, UserManager<User> userManager, ILogger<BannersController> logger)
        {
            this.context = context;
            this.userManager = userManager;
            this.logger = logger;
        }

        public async Task<IActionResult> Index(int? page)
        {
            var model = context.Banners.OrderBy(p => p.SortOrder);
            var pageNumber = page ?? 1;

            return View(await model.ToPagedListAsync(pageNumber, 10));
        }

        public IActionResult Create()
        {
            return View(new Banner { Enabled = true });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Banner model)
        {
            try
            {
                using (var logo = await Image.LoadAsync(model.ImageFile.OpenReadStream()))
                {
                    logo.Mutate(p => p.Resize(960, 340));
                    model.Image = logo.ToBase64String(JpegFormat.Instance);
                }
            }
            catch (UnknownImageFormatException ex)
            {
                logger.LogError("Resim çevirme hatası!! " + ex.Message);
                return View(model);
            }
            catch (ImageProcessingException ex)
            {
                logger.LogError("Resim çevirme hatası!! " + ex.Message);
                return View(model);
            }


            model.Date = DateTime.Now;
            model.UserId = (await userManager.FindByNameAsync(User.Identity.Name)).Id;
            model.SortOrder = ((await context.Banners.OrderByDescending(p => p.SortOrder).FirstOrDefaultAsync())?.SortOrder ?? 0) + 1;

            context.Entry(model).State = EntityState.Added;
            try
            {
                await context.SaveChangesAsync();
                TempData["success"] = $"{name} ekleme işlemi başarıyla tamamlanmıştır.";
                return RedirectToAction("Index");
            }
            catch (DbUpdateException ex)
            {
                logger.Log(LogLevel.Error, ex.Message);
                return View(model);
            }
        }

        public IActionResult Edit(int id)
        {
            return View(context.Banners.Find(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Banner model)
        {
            if (model.ImageFile != null)
            {
                using (var logo = await Image.LoadAsync(model.ImageFile.OpenReadStream()))
                {
                    logo.Mutate(p => p.Resize(160, 160));
                    model.Image = logo.ToBase64String(JpegFormat.Instance);
                }
            }

            model.UserId = (await userManager.FindByNameAsync(HttpContext.User.Identity.Name)).Id;

            context.Entry(model).State = EntityState.Modified;
            try
            {
                await context.SaveChangesAsync();
                TempData["success"] = $"{name} güncelleme işlemi başarıyla tamamlanmıştır.";
                return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                return View(model);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var model = await context.Banners.FindAsync(id);
            context.Entry(model).State = EntityState.Deleted;
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> SortOrders()
        {
            var form = HttpContext.Request.Form;
            foreach (var item in form)
            {
                if (item.Key != "__RequestVerificationToken")
                {
                    var id = int.Parse(Regex.Split(item.Key, "_")[1]);
                    var model = await context.Banners.FindAsync(id);
                    model.SortOrder = int.Parse(item.Value);
                    context.Entry(model).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index");
        }

    }
}