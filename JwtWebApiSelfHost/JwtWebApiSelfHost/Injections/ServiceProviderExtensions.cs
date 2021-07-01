using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace JwtWebApiSelfHost.Injections
{
    /// <summary>
    /// 
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="controllerTypes"></param>
        /// <returns></returns>
        public static IServiceCollection AddControllersAsServices(this IServiceCollection services, IEnumerable<Type> controllerTypes)
        {
            if (controllerTypes != null)
            {
                foreach (var type in controllerTypes)
                    services.AddTransient(type);
            }

            return services;
        }
    }
}