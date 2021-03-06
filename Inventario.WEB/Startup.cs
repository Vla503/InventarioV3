using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Inventario.Core.Model;
using Inventario.Core.Repository;
using Inventario.Core.Service;
using Inventario.Service;
using Inventario.Service.AutoMapper;
using Inventario.SQL;
using Inventario.SQL.Repository;
using Inventario.WEB.Automapper;
using Inventario.WEB.Helpers;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Inventario.WEB
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
            string connection = Configuration["ConnectionStrings:DefaultConnection"];

            services.AddScoped<DbContext, InventarioContext>();
            services.AddDbContext<InventarioContext>(options => options.UseSqlServer(connection));
            services.AddScoped<DbContext, InventarioContext>();
            //Asp Identity
            services.AddIdentity<Usuario, Microsoft.AspNetCore.Identity.IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
            }).AddErrorDescriber<SpanishIdentityErrorDescriber>()
              .AddEntityFrameworkStores<InventarioContext>()
              .AddDefaultTokenProviders(); ;

            //repositorio
            services.AddScoped<IMarcaRepository, MarcaRepository>();
            services.AddScoped<IProductoRepository, ProductoRepository>();
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<IRegistroEntradaProductoRepository, RegistroEntradaProductoRepository>();
            services.AddScoped<IRegistroVentaRepository, RegistroVentaRepository>();
            services.AddScoped<IRolRepository, RolRepository>();
            services.AddScoped<ITipoUsuarioRepository, TipoUsuarioRepository>();
            services.AddScoped<IUserRolRepository, UserRolRepository>();
            services.AddScoped<IVentaProductoRepository, VentaProductoRepository>();

            //provider
            services.AddAutoMapper(typeof(MapperProfile), typeof(AutoMapperProfile));
            //service
            services.AddScoped<IMarcaService, MarcaService>();
            services.AddScoped<IProductoService, ProductoService>();
            services.AddScoped<ICategoriaService, CategoriaService>();
            services.AddScoped<IRegistroEntradaProductoService, RegistroEntradaProductoService>();
            services.AddScoped<IRegistroVentaService, RegistroVentaService>();
            services.AddScoped<IRolService, RolService>();
            services.AddScoped<ITipoUsuarioService, TipoUsuarioService>();
            services.AddScoped<IUserRolService, UserRolService>();
            services.AddScoped<IVentaProductoService, VentaProductoService>();




            services.AddHttpClient();
            services.AddControllersWithViews();

            services.AddMemoryCache();
            services.AddResponseCaching();

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Cuenta/Login";
                options.AccessDeniedPath = "/Cuenta/Login";
                options.SlidingExpiration = true;
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddSessionStateTempDataProvider();
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
