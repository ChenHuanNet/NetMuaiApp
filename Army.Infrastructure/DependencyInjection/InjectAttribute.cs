using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Army.Infrastructure.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Class)]
    public class InjectAttribute : Attribute
    {
        public ServiceLifetime OptionsLifetime { get; set; } = ServiceLifetime.Scoped;

        public Type ServiceType { get; set; }

        public bool InterfaceServiceType { get; set; } = true;

        public bool ReplaceServices { get; set; }
    }
}
