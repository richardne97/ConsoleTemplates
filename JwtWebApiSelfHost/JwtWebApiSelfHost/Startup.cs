﻿using JwtWebApiSelfHost.Injections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Swashbuckle.Application;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
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

            #region Swagger 

            // Configure Swagger help page
            // 設定 Swagger Help 頁面
            config.EnableSwagger(c =>
            {
                c.SingleApiVersion("v1", "My API").License(lc => lc.Name("My Company").Url("https://github.com/richardne97/"));
                c.IncludeXmlComments($"{AppContext.BaseDirectory}{Assembly.GetExecutingAssembly().GetName().Name}.xml");
                c.DescribeAllEnumsAsStrings();
                c.ApiKey("Authorization").Description("OAuth2 JWT for accessing secure APIs").Name("Authorization").In("header");
                c.RootUrl(r => 
                {
                    if (!string.IsNullOrEmpty(Properties.Settings.Default.ExternalUriRoot) && Properties.Settings.Default.EnableExternalUriRoot)
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

            #endregion

            #region JWT Authentication

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
                    ValidAudience = Properties.Settings.Default.JwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Properties.Settings.Default.JwtSecurityKey))
                }
            });

            #endregion

            #region Injection
            var services = new ServiceCollection();

            //Injection setting for controllers. 
            services.AddControllersAsServices(typeof(Startup).Assembly.GetExportedTypes()
                    .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
                    .Where(t => typeof(IController).IsAssignableFrom(t) || typeof(IHttpController).IsAssignableFrom(t)));

            //Inject customized objects (Add your code here)
            object injectObject = Guid.NewGuid();
            services.AddSingleton(typeof(object), injectObject);
            #endregion

            //Enable Attribute Route
            config.MapHttpAttributeRoutes();

            //Validating model format while receving request.
            //驗證接收參數並且回應適當錯誤訊息
            config.Filters.Add(new Filter.ModelValidatorFilterAttribute());

            DefaultDependencyResolver resolver = new DefaultDependencyResolver(services.BuildServiceProvider());

            //For MVC
            DependencyResolver.SetResolver(resolver);

            //For Web API
            config.DependencyResolver = resolver; //For Web API

            //Start Web API
            app.UseWebApi(config);

            config.Dispose();
        }
    }
}
