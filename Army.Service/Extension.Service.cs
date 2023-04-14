using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Army.Infrastructure.Extensions;
using System.Runtime.CompilerServices;
using System;
using Army.Service.ApiClients;
using Army.Domain.Consts;

namespace Army.Service
{
    public static class Extension
    {
        /// <summary>
        /// 注册Service
        /// </summary>
        /// <param name="services">服务集合</param>
        public static IServiceCollection AddService(this IServiceCollection services)
        {
            services.InjectAssembly(Assembly.GetExecutingAssembly().GetName().Name);


            services.AddHttpApi<IDilidiliSourceApi>().ConfigureHttpApi((o, p) =>
            {
                o.HttpHost = new Uri(AppConfigHelper.DiliDiliSourceHost);
                o.JsonSerializeOptions.DictionaryKeyPolicy = null;
            }).ConfigureHttpClient(c =>
            {
                c.Timeout = TimeSpan.FromSeconds(10);
            });



            return services;
        }
    }
}
