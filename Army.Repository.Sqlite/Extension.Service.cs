using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Army.Infrastructure.Extensions;

namespace Army.Repository.Sqlite
{
    public static class Extension
    {
        /// <summary>
        /// 注册Service
        /// </summary>
        /// <param name="services">服务集合</param>
        public static IServiceCollection AddArmyRepository(this IServiceCollection services)
        {
            services.InjectAssembly(Assembly.GetExecutingAssembly().GetName().Name);
            return services;
        }
    }
}
