using CoreEstoreMvc.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using X.PagedList;

namespace CoreEstoreMvc.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "Administrators,ProductAdministrators")]
    public class CategoriesController : Controller
    {
        private readonly AppDbContext context;
        private readonly UserManager<User> userManager;
        private readonly string name = "Kategori";

        public CategoriesController(AppDbContext context, UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }
        public async Task<IActionResult> Index(int? page)
        {
            var model = context.Categories.Include(p => p.Rayon).OrderBy(p => p.RayonId).ThenBy(p => p.SortOrder);
            var pageNumber = page ?? 1;

            return View(await model.ToPagedListAsync(pageNumber, 10));
           
        }
        public IActionResult Create()
        {
            ViewBag.Rayons = new SelectList(context.Rayons.OrderBy(p => p.Name), "Id", "Name");
            return View(new Category { Enabled = true });
        }
        [HttpPost]
        public async Task<IActionResult> Create(Category model)
        {           
            model.Date = DateTime.Now;
            model.Enabled = false;
            model.UserId = (await userManager.FindByNameAsync(HttpContext.User.Identity.Name)).Id;
            model.SortOrder = (((await context.Categories.Where(p=> p.RayonId==model.RayonId).OrderByDescending(p => p.SortOrder).FirstOrDefaultAsync())?.SortOrder ?? 0)) + 1;

            context.Entry(model).State = EntityState.Added;

            try
            {
                await context.SaveChangesAsync();
                TempData["success"] = $"{name} ekleme işlemi başarıyla tamamlanmıştır.";
                return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                TempData["error"] = $"{name} ekleme işlemi tamamlanamıyor. {model.Name} isimli kayıt zaten mevcut!";
                ViewBag.Rayons = new SelectList(context.Rayons.OrderBy(p => p.Name), "Id", "Name");
                return View(model);
            }                                      
        }

        public IActionResult Edit(int id)
        {
            ViewBag.Rayons = new SelectList(context.Rayons.OrderBy(p => p.Name), "Id", "Name");
            return View(context.Categories.Find(id));
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Category model)
        {
            model.UserId = (await userManager.FindByNameAsync(HttpContext.User.Identity.Name)).Id;

            context.Entry(model).State = EntityState.Modified;
            try
            {
                await context.SaveChangesAsync();
                TempData["success"] = $"{name} düzenleme işlemi başarıyla tamamlanmıştır.";
                return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                TempData["error"] = $"{name} düzenleme işlemi tamamlanamıyor. {model.Name} isimli kayıt zaten mevcut!";
                return View(model);
            }
        }
        public async Task<IActionResult> Delete(int id)
        {
            var Category = await context.Categories.FindAsync(id);
            context.Entry(Category).State = EntityState.Deleted;
            try
            {   
                await context.SaveChangesAsync();                
            }
            catch (DbUpdateException)
            {
                TempData["error"] = $"{Category.Name} isimli reyon bir ya da daha fazla kayıt ile ilişkili olduğu için silme işlemi tamamlanamıyor ! ";
            }

            return RedirectToAction("Index");
        }
        
        public  async Task<IActionResult> SortOrders()
        {
            var form = HttpContext.Request.Form;
            foreach (var item in form)
            {
                if (item.Key != "__RequestVerificationToken")
                {               
                var id = int.Parse(Regex.Split(item.Key, "_")[1]);
                var Category = await context.Categories.FindAsync(id);
                Category.SortOrder = int.Parse(item.Value);
                context.Entry(Category).State = EntityState.Modified;
                await context.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index");
        }

    }
}
