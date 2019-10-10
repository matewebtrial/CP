using CP.Data;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ninject.Activation;
using System;
using System.Linq;
using System.Web.Providers.Entities;
using Umbraco.Core;
using Umbraco.Core.Models.Membership;

namespace CP
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            new UserLoginInfo("Google", "Google", "Google");
            /*using (var dataContext = new SampleDBContext())
            {
                dataContext.Categories.Add(new Category() { CategoryName = "AspNetUserLogins" });
                dataContext.Categories.Add(new Category() { CategoryName = "Footwear" });
                dataContext.Categories.Add(new Category() { CategoryName = "Accessories" });
                dataContext.SaveChanges();

                foreach (var cat in dataContext.Categories.ToList())
                {
                    Console.WriteLine($"CategoryId= {cat.CategoryID}, CategoryName = {cat.CategoryName}");
                }
            }   */

            Configuration = configuration;
        }

        public string ProviderDisplayName { get; set; }

        public IConfiguration Configuration { get; }
        public class Category
        {
            public int CategoryID { get; set; }
            public string CategoryName { get; set; }

        }
        public class SampleDBContext : DbContext
        {
            private static bool _created = false;
            /*public SampleDBContext()
            {
                if (!_created)
                {
                    _created = true;
                    Database.EnsureDeleted();
                    Database.EnsureCreated();
                }
            }*/
            protected override void OnConfiguring(DbContextOptionsBuilder optionbuilder)
            {
                optionbuilder.UseSqlite(@"Data Source=CP.db");
            }
            public DbSet<Category> Categories { get; set; }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite("Filename=cp.db"));
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite($"Data Source=CP.db"));
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = "170575440203-0u4mhp7itepe493sqm9okmqsp8qpjfib.apps.googleusercontent.com";
                googleOptions.ClientSecret = "5gz2ct6L7btDGiY19Pxf4YTm";
            })/*.AddCookie(options => { options.LoginPath = "/Login"; })*/;
            services.AddMvc();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCookiePolicy();
            app.Use((context, next) =>
            {
                context.Request.Scheme = "https";
                context.Request.PathBase = new PathString("");
                return next();
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }

}
