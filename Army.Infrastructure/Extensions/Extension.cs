using Army.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace Army.Infrastructure.Extensions
{
    public static class Extension
    {
        public static void InjectAssembly(this IServiceCollection services, string assemblyName)
        {
            foreach (Type type1 in AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(assemblyName)).GetTypes())
            {
                Type type = type1;
                if (!type.IsAbstract)
                {
                    InjectAttribute customAttribute = type.GetCustomAttribute<InjectAttribute>();
                    if (customAttribute != null)
                    {
                        Type serviceType = customAttribute.ServiceType;
                        if (serviceType == (Type)null && customAttribute.InterfaceServiceType)
                            serviceType = ((IEnumerable<Type>)type.GetInterfaces()).FirstOrDefault<Type>((Func<Type, bool>)(x => x.Name.Contains(type.Name)));
                        if (serviceType == (Type)null)
                            serviceType = type;
                        ServiceDescriptor descriptor = new ServiceDescriptor(serviceType, type, customAttribute.OptionsLifetime);
                        if (customAttribute.ReplaceServices)
                            services.Replace(descriptor);
                        else
                            services.TryAdd(descriptor);
                    }
                }
            }
        }
    }
}
