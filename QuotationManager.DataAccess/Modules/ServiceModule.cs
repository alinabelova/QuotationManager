using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuotationManager.DataAccess.Repository;
using QuotationManager.Models;
using QuotationManager.Models.Config;

namespace QuotationManager.DataAccess.Modules
{
    public static class ServiceModule
    {
        public static IServiceCollection AddServiceModule(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var pr = services.BuildServiceProvider();
            var mc = pr.GetService<MainConfig>();
            var connectionString = mc.ConnectionString;

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
                {
                    config.Password.RequireNonAlphanumeric = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<IRepository<Quota>, EfRepository<Quota>>();
            services.AddTransient<IRepository<City>, EfRepository<City>>();
            services.AddTransient<IRepository<Contribution>, EfRepository<Contribution>>();
            return services;
        }
    }
}
