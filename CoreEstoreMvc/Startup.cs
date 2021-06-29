using CoreEstoreMvc.Data;
using CoreEstoreMvc.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CoreEstoreMvc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;

            });



            services.AddControllersWithViews();

            services.AddDbContext<AppDbContext>(options =>
            {
                
                
                    options.UseSqlServer(Configuration.GetConnectionString("Default"));

                
                //if (Configuration.GetValue<string>("Application:DbType") == " SqlServer")
                //{
                //    options.UseSqlServer(Configuration.GetConnectionString("Default"));

                //}

                options.UseLazyLoadingProxies();
            });

            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = Configuration.GetValue<bool>("security:PasswordPolicy:RequireDigit");
                options.Password.RequiredLength = Configuration.GetValue<int>("security:PasswordPolicy:RequiredLength");
                options.Password.RequireLowercase = Configuration.GetValue<bool>("security:PasswordPolicy:RequireLowercase");
                options.Password.RequireNonAlphanumeric = Configuration.GetValue<bool>("security:PasswordPolicy:RequireNonAlphanumeric");
                options.Password.RequireUppercase = Configuration.GetValue<bool>("security:PasswordPolicy:RequireUppercase");

                options.SignIn.RequireConfirmedEmail = Configuration.GetValue<bool>("security:PasswordPolicy:RequireConfirmedEmail");
                //options.Lockout.AllowedForNewUsers = false;
                //options.Lockout.MaxFailedAccessAttempts = 5;
                //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);



            })
                .AddEntityFrameworkStores<AppDbContext>();

            services.AddScoped<IShoppingCartService, ShoppingCartService>();
            services.AddTransient<IMailMessageService, MailMessageService>();
            services.AddTransient<IPaymentSevice, PaymentService>();

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppDbContext context, RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            context.Database.Migrate();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseStatusCodePagesWithReExecute("/error/{0}");
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            //var cultures = new List<CultureInfo>
            //{
            //    new CultureInfo ("tr-TR")
            //};
            //app.UseRequestLocalization(options =>
            //{
            //    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture('tr-TR');
            //    options.SupportedCultures = cultures;
            //    options.SupportedUICultures = cultures;
            //});

            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                     name: "areas",
                     pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "product",
                    pattern: "p/{id}/ {name}.html",
                    defaults: new {controller = "Home", action ="Product"}
                    );

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

            });

            

            new[]
            {
                new Role{ Name ="Administrators", FriendlyName="Yöneticiler"},
                new Role{ Name ="ProductAdministrators", FriendlyName="Ürün Yöneticileri"},
                new Role{ Name ="OrderAdministrators", FriendlyName="Sipariþ Yöneticileri"},
                new Role{ Name ="Members", FriendlyName="Üyeler"},

            }
            .ToList()
            .ForEach(role =>
            {
                if (!roleManager.RoleExistsAsync(role.Name).Result)
                    roleManager.CreateAsync(role).Wait();
            });
            {
                var user = new User
                {
                    UserName = Configuration.GetValue<string>("Security:DefaultUser:UserName"),
                    Name = Configuration.GetValue<string>("Security:DefaultUser:Name")
                };
                if (userManager.FindByNameAsync(user.Name).Result == null)
                {
                    userManager.CreateAsync(user, Configuration.GetValue<string>("Security:DefaultUser:Password")).Wait();
                    userManager.AddToRoleAsync(user, "Administrators").Wait();
                }
            }
            if (env.IsDevelopment())
            {
                //{
                //    var user = new User
                //    {
                //        UserName = "productadmin",
                //        Name = "Product Admin"
                //    };
                //    userManager.CreateAsync(user,"1234aA!").Wait();
                //    userManager.AddToRoleAsync(user, "ProductAdministrators").Wait();

                //}
                //{
                //    var user = new User
                //    {
                //        UserName = "orderadmin",
                //        Name = "Order Admin"
                //    };
                //    userManager.CreateAsync(user, "1234aA!").Wait();
                //    userManager.AddToRoleAsync(user, "OrderAdministrators").Wait();

                //}
                //{
                //    var user = new User
                //    {
                //        UserName = "member",
                //        Name = "Mehmet Sarýçizmeli"
                //    };
                //    userManager.CreateAsync(user, "1234aA!").Wait();
                //    userManager.AddToRoleAsync(user, "Members").Wait();

                //}

            }

        }
    }
}
