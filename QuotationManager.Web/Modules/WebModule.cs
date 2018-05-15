using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using QuotationManager.DataAccess.Modules;
using QuotationManager.Models.Config;

namespace QuotationManager.Web.Modules
{
    public static class WebModule
    {
        public static IServiceCollection AddWebModule(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            services.AddSingleton(c => c.GetService<IOptions<MainConfig>>().Value);
            services.AddServiceModule();

            return services;
        }
    }
}
