using JwtWebApiSelfHost.Injections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Swashbuckle.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Mvc;

namespace JwtWebApiSelfHost
{
    /// <summary>
    /// The Startup class is specified as a type parameter in the WebApp.Start method.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// This code configures Web API. The Startup class is specified as a type parameter in the WebApp.Start method.
        /// </summary>
        /// <param name="app">Application Builder</param>
        public void Configuration(IAppBuilder app)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();

            // Configure Swagger help page
            config.EnableSwagger(c =>
            {
                c.SingleApiVersion("v1", "My API").License(lc => lc.Name("My Company").Url("https://github.com/richardne97/"));
                c.IncludeXmlComments($"{AppContext.BaseDirectory}{Assembly.GetExecutingAssembly().GetName().Name}.xml");
                c.DescribeAllEnumsAsStrings();
                c.ApiKey("Authorization").Description("OAuth2 JWT for accessing secure APIs").Name("Authorization").In("header");
                c.RootUrl(r => 
                {
                    if (!string.IsNullOrEmpty(Properties.Settings.Default.ExternalUriRoot))
                        return Properties.Settings.Default.ExternalUriRoot;
                    else
                        return r.RequestUri.ToString().Replace(r.RequestUri.LocalPath,"");
                });
            })
            .EnableSwaggerUi(u =>
            {
                u.DocumentTitle("My API");
                u.EnableApiKeySupport("Authorization", "header");
            });

            //Enable JWT Authentication
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            
            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Properties.Settings.Default.JwtIssuer,   
                    ValidAudience = Properties.Settings.Default.JwtAudidence,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Properties.Settings.Default.JwtSecurityKey))
                }
            });

            //Enable Attribute Route
            config.MapHttpAttributeRoutes();

            //Validating model format while receving request.
            config.Filters.Add(new Filter.ModelValidatorFilter());

            //Injection settings
            var services = new ServiceCollection();

            //Injection setting for controllers. 
            services.AddControllersAsServices(typeof(Startup).Assembly.GetExportedTypes()
                    .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
                    .Where(t => typeof(IController).IsAssignableFrom(t)
                    || typeof(IHttpController).IsAssignableFrom(t)));

            object injectObject = Guid.NewGuid();
            services.AddSingleton(typeof(object), injectObject);

            var resolver = new DefaultDependencyResolver(services.BuildServiceProvider());

            //For MVC
            DependencyResolver.SetResolver(resolver);

            //For Web API
            config.DependencyResolver = resolver; //For Web API

            app.UseWebApi(config);
        }
    }
}
