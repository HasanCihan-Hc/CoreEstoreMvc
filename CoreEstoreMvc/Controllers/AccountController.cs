using CoreEstoreMvc.Data;
using CoreEstoreMvc.Models;
using CoreEstoreMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PaymentBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreEstoreMvc.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class AccountController : Controller
    {
        private readonly SignInManager<User> signInManager;
        private readonly Microsoft.AspNetCore.Identity.UserManager<User> userManager;
        private readonly IMailMessageService mailMessageService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly AppDbContext context;
        private readonly IShoppingCartService shoppingCartService;
        private readonly IPaymentSevice paymentSevice;
        private readonly RoleManager<Role> roleManager;

        public AccountController(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IMailMessageService mailMessageService,
            IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccesor,
            AppDbContext context,
            IShoppingCartService shoppingCartService,
            IPaymentSevice paymentSevice,
            RoleManager<Role> roleManager
            )
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.mailMessageService = mailMessageService;
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccesor;
            this.context = context;
            this.shoppingCartService = shoppingCartService;
            this.paymentSevice = paymentSevice;
            this.roleManager = roleManager;
            ;
            ;
        }
        public IActionResult Login()
        {
            return View(new LoginViewModel { RememberMe = true, ReturnUrl = HttpContext.Request.Query["ReturnUrl"] });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
            if (result.Succeeded)
                return Redirect(model.ReturnUrl ?? "/");
            else
            {
                if (result.IsNotAllowed)
                    ModelState.AddModelError("", "E-postanız henüz doğrulanmadı.");
                else if (result.IsLockedOut)
                    ModelState.AddModelError("", "Çok fazla yanlış parola denemesi");
                else
                    ModelState.AddModelError("", "Geçersiz kullanıcı girişi");
                return View(model);
            }

        }


        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login","Home");
        }

        public IActionResult Register()
        {
            return View(new RegisterViewModel { });
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var newUser = new User
            {
                UserName = model.UserName,
                Name = model.Name,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender
            };
            var result = await userManager.CreateAsync(newUser, model.Password);
            await userManager.AddToRoleAsync(newUser, "Members");

            if (result.Succeeded)
            {
                var emailConfirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(newUser);
                var messageBody =
                    string.Format(
                    System.IO
                    .File
                    .ReadAllText(System.IO.Path.Combine(webHostEnvironment.WebRootPath, "Content", "EMailConfirmationTemplate.html"))
                    , model.Name
                    , Url.Action("ConfirmEmail", "Account", new { id = newUser.Id, token = emailConfirmationToken }, httpContextAccessor.HttpContext.Request.Scheme)
                    );

                await mailMessageService.Send(model.UserName, "E-Posta Doğrulama Mesajı", messageBody);
                return View("RegisterSuccess");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    switch (error.Code)
                    {
                        case "DuplicateUserName":
                            ModelState.AddModelError("", "E-Posta zaten kayıtlı");
                            break;
                        case "PasswordTooShort":
                            ModelState.AddModelError("", "Parolanız en az 6(altı) karakter uzunluğunda olmalıdır.");
                            break;
                        case "PasswordRequiresDigit":
                            ModelState.AddModelError("", "Parolanız en az bir adet rakam içermelidir.");
                            break;
                        case "PasswordRequiresNonAlphanumeric":
                            ModelState.AddModelError("", "Parolanız en az bir adet sembol içermelidir.");
                            break;
                        case "PasswordRequiresLower":
                            ModelState.AddModelError("", "Parolanız en az bir adet küçük harf içermelidir.");
                            break;
                        case "PasswordRequiresUpper":
                            ModelState.AddModelError("", "Parolanız en az bir adet büyük harf içermelidir.");
                            break;
                    }
                }
                return View(model);
            }
        }

        public IActionResult ResetPasswordForm()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPasswordForm(ResetPasswordViewModel model)
        {
            var user = await userManager.FindByNameAsync(model.UserName);
            if (user != null)
            {
                var passwordResetToken = await userManager.GeneratePasswordResetTokenAsync(user);
                var messageBody =
                    string.Format(
                    System.IO
                    .File.ReadAllText(System.IO.Path.Combine(webHostEnvironment.WebRootPath, "Content", "ResetPasswordTemplate.html"))
                    , user.Name
                    , Url.Action("ResetPassword", "Account", new { id = user.Id, token = passwordResetToken }, httpContextAccessor.HttpContext.Request.Scheme)
                    );
                await mailMessageService.Send(user.UserName, "Parola Yenileme Mesajı", messageBody);
                return View("ResetPasswordFormSuccess");
            }
            else
            {
                ModelState.AddModelError("", "Belirttiğiniz e-posta adresi ile kayıtlı bir üyemiz bulunmamaktadır");
                return View(model);
            }
        }

        public IActionResult ResetPassword(string id,string token)
        {
            return View(new NewPasswordViewModel { Id = id,Token= token});
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(NewPasswordViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);
            await userManager.ResetPasswordAsync(user,model.Token, model.Password);
            return View("ResetPasswordSuccess");

        }

        [Authorize()]
        public async Task<IActionResult> CheckOut()
        {
            await shoppingCartService.TransferCookieToDatabaseAsync();
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            if (user.ShoppingCartsItems.Any(p => p.Product == null))
            {
                return RedirectToAction("CheckOut");
            }
            return View(user);

        }

        public async Task<IActionResult> Payment()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            ViewData["grandTotal"] = user.ShoppingCartGrandTotal.ToString("c2");
            ViewData["months"] = new SelectList(Enumerable.Range(1,12).Select(p=>p.ToString("00")));
            ViewData["years"] = new SelectList(Enumerable.Range(DateTime.Today.Year,10).Select(p => p.ToString()));
            return View();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Payment (PaymentParameters model)
        {
           var result = await  paymentSevice.Pay(model);
            if (!result.Succeded)
            {
                ModelState.AddModelError("", result.Error);
                return View(model);
            }
            else
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                var order = new Order
                {
                    Date = DateTime.Now,
                    Enabled = true,
                    OrderState= OrderStates.New,
                    UserId = user.Id,
                };
                user
                    .ShoppingCartsItems
                    .ToList()
                    .ForEach(p =>{
                        var orderItem = new OrderItem
                        { Discount = p.Product.Discount, 
                          Price = p.Product.Price,
                          ProductId = p.Product.Id,
                          Quantity = p.Quantity,
                        };
                        context.Entry(orderItem).State = EntityState.Added;
                        order.OrderItems.Add(orderItem);
                        context.Entry(p).State = EntityState.Deleted;
                    });

                context.Entry(order).State = EntityState.Added;

                await context.SaveChangesAsync();
                return View("PaymentSuccess");
            }
        }


        [Authorize, HttpGet]
        public async Task<IActionResult> BinCheck(string binNumber)
        {
            var response = await PaymentBase.BinCheck.Request(binNumber);
            return Json(response);
        }

        [Authorize]
        [HttpGet("home/orders/{state}/{startDate?}/{endDate?}")]
        public async Task<IActionResult> Orders(DateTime? startDate, DateTime? endDate, OrderStates state)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);

            //ViewData["newOrders"] = user.Orders.Where(p => p.OrderState == OrderStates.New).OrderBy(p => p.Date).ToList();
            

            var model = user.Orders
                .Where(p =>
                (p.OrderState == state)
                &&  (p.Date >= startDate || startDate == null) 
                &&  (p.Date <= endDate || endDate == null))
                .ToList();


            return View(model);
        }

        
        


    }
}
