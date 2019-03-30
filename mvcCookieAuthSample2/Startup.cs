using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using mvcCookieAuthSample.Data;
using Microsoft.EntityFrameworkCore;
using mvcCookieAuthSample.Models;
using Microsoft.AspNetCore.Identity;
using IdentityServer4;
using IdentityServer4.Services;
using mvcCookieAuthSample.Services;
using IdentityServer4.EntityFramework;

namespace mvcCookieAuthSample
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
             
            const string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;database=IdentityServer4.EntityFramework-2.0.0;trusted_connection=yes;";
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddIdentity<ApplicationUser, ApplicationUserRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.AddIdentityServer().AddDeveloperSigningCredential()
                //.AddInMemoryClients(Config.GetClients())
                //.AddInMemoryApiResources(Config.GetResoure())
                //.AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddConfigurationStore(options =>
                    {
                        options.ConfigureDbContext = bulider =>
                            {
                                bulider.UseSqlServer(connectionString,
                                    sql => sql.MigrationsAssembly(migrationsAssembly));
                            };

                    }).AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = bulider =>
                    {
                        bulider.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                    };
                })
                .AddAspNetIdentity<ApplicationUser>()
                .Services.AddScoped<IProfileService, ProfileService>();
            // services.AddDbContext<ApplicationDbContext>(options =>
            // {
            //     options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            // });

            // services.AddIdentity<ApplicationUser, ApplicationUserRole>()
            //     .AddEntityFrameworkStores<ApplicationDbContext>()
            //     .AddDefaultTokenProviders();

            // services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //     .AddCookie(options => {
            //         options.LoginPath = "/Account/Login";
            //     });

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 3;
            });

            services.AddScoped<ConsentService>();

           services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            //app.UseAuthentication();
            app.UseIdentityServer();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
