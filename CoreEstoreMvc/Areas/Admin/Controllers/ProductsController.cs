using CoreEstoreMvc;
using CoreEstoreMvc.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace MVCCoreEStore.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "Administrators, ProductAdministrators")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext context;
        private readonly UserManager<User> userManager;

        private readonly string name = "Ürün";

        public ProductsController(AppDbContext context, UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index(int ? page)
        {
            var model = context.Products;
            var pageNumber = page ?? 1;
            return View(await model.ToPagedListAsync(pageNumber, 10));
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Brands = new SelectList(await context.Brands.Select(p => new { p.Id, p.Name }).OrderBy(p => p.Name).ToListAsync(), nameof(Brand.Id), nameof(Brand.Name));
            ViewBag.Categories = new MultiSelectList(await context.Categories.Select(p => new { Id = p.Id, Name = p.Name, RayonName = p.Rayon.Name }).ToListAsync(), nameof(Category.Id), nameof(Category.Name), null, dataGroupField: "RayonName");
            return View(new Product { Enabled = true });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product model)
        {
            if (model.PictureFile != null)
            {
                using (var picture = await Image.LoadAsync(model.PictureFile.OpenReadStream()))
                {
                    picture.Mutate(p => p.Resize(800, 800));
                    model.Picture = picture.ToBase64String(JpegFormat.Instance);
                }
            }

            if (model.PictureFiles != null)
            {
                if (model.PictureFiles.Count() > 0)
                {
                    foreach (var item in model.PictureFiles)
                    {
                        using (var picture = await Image.LoadAsync(item.OpenReadStream()))
                        {
                            picture.Mutate(p => p.Resize(800, 800));
                            var pic = new ProductPicture { Picture = picture.ToBase64String(JpegFormat.Instance) };
                            context.Entry(pic).State = EntityState.Added;
                            model.ProductPictures.Add(pic);
                        }
                    }
                }
            }

            foreach (var item in model.SelectedCategories)
            {
                var cat = new CategoryProduct { CategoryId = item };
                context.Entry(cat).State = EntityState.Added;
                model.CategoryProducts.Add(cat);
            }

            model.Date = DateTime.Now;
            model.UserId = (await userManager.FindByNameAsync(User.Identity.Name)).Id;
            model.Price = decimal.Parse(model.PriceText);

            context.Entry(model).State = EntityState.Added;
            try
            {
                await context.SaveChangesAsync();
                TempData["success"] = $"{name} ekleme işlemi başarıyla tamamlanmıştır.";
                return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = context.Products.Find(id);
            ViewBag.Brands = new SelectList(await context.Brands.Select(p => new { p.Id, p.Name }).OrderBy(p => p.Name).ToListAsync(), nameof(Brand.Id), nameof(Brand.Name));
            ViewBag.Categories = new MultiSelectList(await context.Categories.Select(p => new { Id = p.Id, Name = p.Name, RayonName = p.Rayon.Name }).ToListAsync(), nameof(Category.Id), nameof(Category.Name), model.CategoryProducts.Select(p => p.CategoryId));
            model.PriceText = model.Price.ToString("f2");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product model)
        {
            if (model.PictureFile != null)
            {
                using (var logo = await Image.LoadAsync(model.PictureFile.OpenReadStream()))
                {
                    logo.Mutate(p => p.Resize(800, 800));
                    model.Picture = logo.ToBase64String(JpegFormat.Instance);
                }
            }

            if (model.PictureFiles != null)
            {
                if (model.PictureFiles.Count() > 0)
                {
                    foreach (var item in model.PictureFiles)
                    {
                        using (var picture = await Image.LoadAsync(item.OpenReadStream()))
                        {
                            picture.Mutate(p => p.Resize(800, 800));
                            var pic = new ProductPicture { Picture = picture.ToBase64String(JpegFormat.Instance) };
                            context.Entry(pic).State = EntityState.Added;
                            model.ProductPictures.Add(pic);
                        }
                    }
                }
            }

            model.UserId = (await userManager.FindByNameAsync(HttpContext.User.Identity.Name)).Id;
            model.Price = decimal.Parse(model.PriceText);

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
            var model = await context.Products.FindAsync(id);
            context.Entry(model).State = EntityState.Deleted;
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ProductComments()
        {
            var model = await context.ProductComments.Where(p => !p.Enabled).ToListAsync();
            return View(model);
        }

        public async Task<IActionResult> ApproveComment(int id)
        {
            var model = await context.ProductComments.FindAsync(id);
            model.Enabled = true;
            context.Entry(model).State = EntityState.Modified;
            await context.SaveChangesAsync();
            TempData["success"] = $"Yorum onaylama işlemi başarıyla tamamlanmıştır.";
            return RedirectToAction("ProductComments");
        }

        public async Task<IActionResult> DeleteComment(int id)
        {
            var model = await context.ProductComments.FindAsync(id);
            context.Entry(model).State = EntityState.Deleted;
            await context.SaveChangesAsync();
            TempData["success"] = $"Yorum silme işlemi başarıyla tamamlanmıştır.";
            return RedirectToAction("ProductComments");
        }


    }
}