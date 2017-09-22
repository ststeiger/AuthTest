
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Builder; // Various extensions
// using Microsoft.AspNetCore.Hosting; // env.IsDevelopment

using Microsoft.Extensions.Configuration; // Not yet used
using Microsoft.Extensions.DependencyInjection; // Various extensions


using Microsoft.Extensions.Logging;


using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;



using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;



namespace AuthTest
{


    public class Startup : Microsoft.AspNetCore.Hosting.IStartup
    {

        public Microsoft.Extensions.Configuration.IConfiguration Configuration { get; }

        public Startup(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            Configuration = configuration;
        } // End Constructor 


        IServiceProvider Microsoft.AspNetCore.Hosting.IStartup.ConfigureServices(
            Microsoft.Extensions.DependencyInjection.IServiceCollection services)
        {
            
            // // https://stackoverflow.com/questions/45781873/is-net-core-2-0-logging-broken
            //services.AddLogging( 
            //    delegate(ILoggingBuilder builder)
            //    { 
            //        builder.AddConfiguration(Configuration.GetSection("Logging"))
            //            .AddConsole()
            //            .AddDebug()
            //        ;

            //    }
            //);
            

            services.AddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor
                , Microsoft.AspNetCore.Http.HttpContextAccessor>();


            Microsoft.Extensions.DependencyInjection.OptionsConfigurationServiceCollectionExtensions
                .Configure<Mail.SmtpConfig>(services, Configuration.GetSection("Smtp"));

            services.AddTransient<AuthTest.Services.IMailService, AuthTest.Services.MailService>();


            // https://docs.microsoft.com/en-us/aspnet/core/migration/1x-to-2x/identity-2x

            // services.ConfigureExternalCookie()
            // services.ConfigureApplicationCookie(delegate (Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationOptions opts) {});


            services.AddAuthentication(
                delegate (Microsoft.AspNetCore.Authentication.AuthenticationOptions options) 
                {
                    options.DefaultScheme = Microsoft.AspNetCore.Authentication.Cookies
                        .CookieAuthenticationDefaults.AuthenticationScheme;

                    options.DefaultAuthenticateScheme= Microsoft.AspNetCore.Authentication.Cookies
                        .CookieAuthenticationDefaults.AuthenticationScheme;

                    options.DefaultForbidScheme= Microsoft.AspNetCore.Authentication.Cookies
                        .CookieAuthenticationDefaults.AuthenticationScheme;

                    options.DefaultSignInScheme = Microsoft.AspNetCore.Authentication.Cookies
                        .CookieAuthenticationDefaults.AuthenticationScheme;

                    options.DefaultSignOutScheme = Microsoft.AspNetCore.Authentication.Cookies
                        .CookieAuthenticationDefaults.AuthenticationScheme;

                    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.Cookies
                        .CookieAuthenticationDefaults.AuthenticationScheme;
                }
            )
            // 
            .AddCookie(delegate (Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationOptions opts) 
            {
                opts.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login/");
                opts.LogoutPath = new Microsoft.AspNetCore.Http.PathString("/Account/Logout/");
                opts.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Account/Forbidden/");


                Microsoft.IdentityModel.Tokens.SecurityKey signingKey = null;

                // var x = new System.Security.Cryptography.RSACryptoServiceProvider();
                // Microsoft.IdentityModel.Tokens.RsaSecurityKey rsakew = 
                // new Microsoft.IdentityModel.Tokens.RsaSecurityKey(x);

                // var securityKey = new InMemorySymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("sec"));


                Microsoft.IdentityModel.Tokens.SymmetricSecurityKey symkey =
                    new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("Test"));


                var tokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    // The signing key must match!
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,

                    // Validate the JWT Issuer (iss) claim
                    ValidateIssuer = true,
                    ValidIssuer = "ExampleIssuer",

                    // Validate the JWT Audience (aud) claim
                    ValidateAudience = true,
                    ValidAudience = "ExampleAudience",

                    // Validate the token expiry
                    ValidateLifetime = true,

                    // If you want to allow a certain amount of clock drift, set that here:
                    ClockSkew = System.TimeSpan.Zero,
                };



                opts.Cookie = new Microsoft.AspNetCore.Http.CookieBuilder()
                {
                    // Domain = "localhost:64972",
                    Domain = "localhost",
                    // Path = null,
                    Name = "SecurityByObscurityDoesntWork",
                    Expiration = new System.TimeSpan(15, 0, 0),
                    HttpOnly = true,
                    SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest,
                };

                opts.SlidingExpiration = true;
                opts.ExpireTimeSpan = new System.TimeSpan(15, 0, 0);

                // https://long2know.com/2017/05/migrating-from-net-core-1-1-to-2-0/
                opts.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents()
                {
                    OnValidatePrincipal = async context => { await ValidateAsync(context); }
                };


                /*
                //AuthenticationScheme = "MyCookieMiddlewareInstance",
                // opts.Cookie = null;
                // http://localhost:64972/
                opts.Cookie.Domain = "localhost:64972";
                opts.Cookie.Name = "SecurityByObscurityDoesntWork";
                opts.Cookie.Expiration = new System.TimeSpan(15, 0, 0);
                opts.Cookie.HttpOnly = true;
                opts.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
                */
                opts.TicketDataFormat = new NiHaoCookie.JwtCookieDataFormat("foo", tokenValidationParameters);

                // opts.DataProtectionProvider = null;

                // AutomaticAuthenticate = true,
                // AutomaticChallenge = true,

            })
            /*
            .AddJwtBearer(
                delegate (Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerOptions options) 
                {
                    options.Authority = "";
                    options.Audience = "";
                    options.ClaimsIssuer = "";
                    options.Challenge = "";
                    options.Challenge = "";
                    //options.BackchannelTimeout
                    //options.BackchannelHttpHandler

                    // options.Configuration.SigningKeys = null;
                    // options.Configuration.TokenEndpoint

                    // options.Events

                    //options.SecurityTokenValidators

                    options.TokenValidationParameters = null;
                    options.RequireHttpsMetadata = true;
                    // options.Events.
                    // options.SaveToken
                    // options.TokenValidationParameters
                    // options.SecurityTokenValidators
                    // options.MetadataAddress
                }
            )*/
            ;


            // services.ConfigureApplicationCookie(options => options.LoginPath = "/Account/LogIn");

            /*
            services.AddAuthentication()
                .AddJwtBearer(options => { })
                .AddCookie(options => { })
                
                .AddGoogle
                .AddFacebook(options => {
                    options.AppId = Configuration["auth:facebook:appid"];
                    options.AppSecret = Configuration["auth:facebook:appsecret"];
                })
                
            ;
            */


            services.AddAntiforgery(
                 delegate (Microsoft.AspNetCore.Antiforgery.AntiforgeryOptions options)
                 {
                        // https://damienbod.com/2017/05/09/anti-forgery-validation-with-asp-net-core-mvc-and-angular/
                        options.HeaderName = "X-XSRF-TOKEN";
                        //options.CookieDomain = "localhost";
                        options.Cookie.Name = "foobr";
                 }
            );



            // https://stackoverflow.com/questions/40097229/when-i-develop-asp-net-core-mvc-which-service-should-i-use-addmvc-or-addmvccor
            // AddMvcCore(), as the name implies, only adds core components, 
            // requiring you to add any other middleware(needed for your project) by yourself.
            // AddMvc() internally calls AddMvcCore() and adds other middleware 
            // such as the Razor view engine, JSON formatters, CORS, etc.

            //services.AddMvcCore(
            services.AddMvc(
                delegate (Microsoft.AspNetCore.Mvc.MvcOptions config)
                {
                    Microsoft.AspNetCore.Authorization.AuthorizationPolicy policy = 
                        new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                                     .RequireAuthenticatedUser()
                                     // .AddRequirements( new NoBannedIPsRequirement(new HashSet<string>() { "127.0.0.1", "0.0.0.1" } ))
                                     .Build();

                    config.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter(policy));
                }
            );

            return services.BuildServiceProvider();
        } // End Function ConfigureServices 

        private Task ValidateAsync(CookieValidatePrincipalContext context)
        {
            return Task.FromResult(true);
            // throw new NotImplementedException();
        }

        IApplicationBuilder Application;

        void Microsoft.AspNetCore.Hosting.IStartup.Configure(IApplicationBuilder app)
        {
            this.Application = app;

            Microsoft.Extensions.Logging.ILoggerFactory loggerFactory = app.ApplicationServices.
                GetRequiredService<Microsoft.Extensions.Logging.ILoggerFactory>();

            Services.IMailService mailService = app.ApplicationServices.
                GetRequiredService<Services.IMailService>();

            Microsoft.AspNetCore.Hosting.IHostingEnvironment env = app.ApplicationServices.
                GetRequiredService<Microsoft.AspNetCore.Hosting.IHostingEnvironment>();

            Microsoft.AspNetCore.Http.IHttpContextAccessor httpContext = app.ApplicationServices.
                GetRequiredService<Microsoft.AspNetCore.Http.IHttpContextAccessor>();

            this.ConfigureMapping(app, loggerFactory, mailService, env, httpContext);
        } // End Function Configure 


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void ConfigureMapping(Microsoft.AspNetCore.Builder.IApplicationBuilder app
            , Microsoft.Extensions.Logging.ILoggerFactory loggerFactory
            , Services.IMailService mailService
            , Microsoft.AspNetCore.Hosting.IHostingEnvironment env
            , Microsoft.AspNetCore.Http.IHttpContextAccessor httpContext)
        {
            string virtual_directory = "/Virt_DIR";
            virtual_directory = "/";
            // virtual_directory = "/Virt_DIR";

            if (virtual_directory.EndsWith("/"))
                virtual_directory = virtual_directory.Substring(0, virtual_directory.Length - 1);

            if (string.IsNullOrWhiteSpace(virtual_directory))
                ConfigureMapped(app, loggerFactory, mailService, env, httpContext); // Don't map if you don't have to 
            else
                // app.Map("/Virt_DIR", (mappedApp) => Configure1(mappedApp, env, loggerFactory));
                app.Map(virtual_directory, 
                    delegate (IApplicationBuilder mappedApp)
                    {
                        ConfigureMapped(mappedApp, loggerFactory, mailService, env, httpContext);
                    }
                );

        } // End Sub ConfigureMapping 


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void ConfigureMapped(Microsoft.AspNetCore.Builder.IApplicationBuilder app
            ,Microsoft.Extensions.Logging.ILoggerFactory loggerFactory
            ,Services.IMailService mailService
            , Microsoft.AspNetCore.Hosting.IHostingEnvironment env
            , Microsoft.AspNetCore.Http.IHttpContextAccessor httpContext)
        {

            // Microsoft.Extensions.Logging.ConsoleLoggerExtensions.AddConsole(loggerFactory
            //     , Microsoft.Extensions.Logging.LogLevel.Error
            // );

            AuthTest.Logger.EmailLoggerExtensions.AddEmail(
                  loggerFactory
                , mailService
                , Microsoft.Extensions.Logging.LogLevel.Error  // , Microsoft.Extensions.Logging.LogLevel.Critical
                , httpContext
            );


            if (Microsoft.AspNetCore.Hosting.HostingEnvironmentExtensions.IsDevelopment(env))
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // loggerFactory.addcon


            app.UseAuthentication();

            // https://github.com/aspnet/Security/issues/1310
            // AppContext.TargetFrameworkName

            app.UseStaticFiles();

            app.UseMvc( 
                delegate(Microsoft.AspNetCore.Routing.IRouteBuilder routes)
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=Home}/{action=Index}/{id?}");
                }
            );

        } // End Sub ConfigureMapped 


    } // End Class Startup : Microsoft.AspNetCore.Hosting.IStartup 


} // End Namespace AuthTest 
