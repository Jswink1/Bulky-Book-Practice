using BulkyBookPractice.DataAccess.Data;
using BulkyBookPractice.DataAccess.Repository;
using BulkyBookPractice.DataAccess.Repository.IRepository;
using BulkyBookPractice.Utility;
using BulkyBookPractice.Utility.BrainTree;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBookPractice
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
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;

            }).AddDefaultTokenProviders()
              .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Configure Email Sender
            services.AddSingleton<IEmailSender, EmailSender>();
            services.Configure<EmailOptions>(Configuration);

            // Redirect users to "Login" or "Access Denied" page when trying to access a resource that requires authorization
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";

            });

            // Configure Session Options
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Configure Stripe Settings
            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));

            // Configure Twilio Settings
            services.Configure<TwilioSettings>(Configuration.GetSection("Twilio"));

            // Configure BrainTree Settings
            services.Configure<BrainTreeSettings>(Configuration.GetSection("BrainTree"));
            services.AddSingleton<IBrainTreeGate, BrainTreeGate>();

            // Configure Temporary Data
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
            
            services.AddAuthentication().AddFacebook(options =>
            {
                options.AppId = "4448981588514639";
                options.AppSecret = "1c41b7e6d439b86ef4e82f9234579d44";
            });

            // Steps to configure Google Login:
            // 1. Go to google cloud console, create your app
            // 2. Go to "APIs and Services," click the "Enable APIs and Services" button, search for "Google+ API"
            // 3. Go to "OAuth Consent Screen." Select External. Enter your app name and email
            // 4. Go to "Credentials", click "Create Credential" for "OAuth Client Id",
            // enter app name, app type, LocalHost URI, and "https://localhost:#####/signin-google" for Authorized Redirect URI
            // 5. Insert Client Id and Secret
            services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = "778879023434-g7e8drc5n2q13ro8rdcfogd5s14pd1ho.apps.googleusercontent.com";
                options.ClientSecret = "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";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            StripeConfiguration.ApiKey = Configuration.GetSection("Stripe")["SecretKey"];

            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
