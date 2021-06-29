using CoreEstoreMvc.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace CoreEstoreMvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrators")]
    public class UsersController : Controller
    {
        private readonly AppDbContext context;
        private readonly UserManager<User> userManager;

        public UsersController(
            AppDbContext context,
            UserManager<User> userManager

            )
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index( int? page)
        {
            ViewData["Roles"] = new SelectList(await context.Roles.ToListAsync(), "Name", "FriendlyName");

            var model = context.Users.OrderBy(p => p.Name);
            var pageNumber = page ?? 1;

            return View(await model.ToPagedListAsync(pageNumber, 10));                     
        }

        public async Task<IActionResult> ChangeRole(string id, string newRole)
        {
            var user = await userManager.FindByIdAsync(id);
            var roles = await userManager.GetRolesAsync(user);
            var admins = context.UserRoles.Count(p => p.RoleId == 1);
            if (admins < 2 && roles[0] == "Administrators")
            {
                TempData["error"] = "Sistemde en az bir yönetici olmalıdır";
            }
            else
            {
                await userManager.RemoveFromRoleAsync(user, roles[0]);
                await userManager.AddToRoleAsync(user, newRole);
                TempData["success"] = "Kullanıcı rolu başarıyla değiştirilmiştir.";
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ChangeRole(int id, string newRole )
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            var roles = await userManager.GetRolesAsync(user);

            var admins = context.UserRoles.Count(p => p.RoleId == 1);
            if (admins<2 && roles[0]=="Administrators")
            {
                TempData["error"] = "Sistemde en az bir yönetici olmalıdır!";
            }
            else
            {
                var result = await userManager.RemoveFromRoleAsync(user, roles[0]);
                await userManager.AddToRoleAsync(user, newRole);
                TempData["success"] = "Kullanıcı rolü başarıyla değiştirildi.";
            }

            

            return RedirectToAction("Index");
        }
    }
}
