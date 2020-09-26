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

            app.UseWebApi(config);
        }
    }
}
