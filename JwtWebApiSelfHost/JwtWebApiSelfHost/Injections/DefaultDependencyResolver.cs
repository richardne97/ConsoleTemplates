using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Web.Mvc;

namespace JwtWebApiSelfHost.Injections
{
    /// <summary>
    /// Dependency Resolver
    /// </summary>
    public sealed class DefaultDependencyResolver : System.Web.Http.Dependencies.IDependencyResolver, System.Web.Mvc.IDependencyResolver
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceProvider"></param>
        public DefaultDependencyResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Get Service
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public object GetService(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }

        /// <summary>
        /// Get Services
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _serviceProvider.GetServices(serviceType);
        }

        /// <summary>
        /// Implement BeginScope
        /// </summary>
        /// <returns></returns>
        public System.Web.Http.Dependencies.IDependencyScope BeginScope()
        {
            using (IServiceScope serviceScope = _serviceProvider.CreateScope())
            {
                return new DefaultDependencyResolver(serviceScope.ServiceProvider);
            }
        }

        /// <summary>
        /// Implement Dispose
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}