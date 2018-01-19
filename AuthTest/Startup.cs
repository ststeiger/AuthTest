
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration; // Not yet used
using Microsoft.Extensions.DependencyInjection; // Various extensions

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder; // Various extensions
using Microsoft.AspNetCore.Hosting; // env.IsDevelopment

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;


namespace AuthTest
{


    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
    public class Startup : Microsoft.AspNetCore.Hosting.IStartup
    {
        private IHostingEnvironment HostingEnvironment { get; set; }
        public Microsoft.Extensions.Configuration.IConfiguration Configuration { get; }
        private Microsoft.Extensions.Logging.ILoggerFactory m_LoggerFactory;
        private IApplicationBuilder m_Application;


        public Startup(IHostingEnvironment env, Microsoft.Extensions.Configuration.IConfiguration config
            ,Microsoft.Extensions.Logging.ILoggerFactory logFactory
            )
        {
            this.HostingEnvironment = env;
            this.Configuration = config;
            this.m_LoggerFactory = logFactory;
        } // End Constructor 


        System.IServiceProvider Microsoft.AspNetCore.Hosting.IStartup.ConfigureServices(
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

            // Microsoft.Extensions.DependencyInjection.OptionsConfigurationServiceCollectionExtensions
            //    .Configure<Mail.SmtpConfig>(services, Configuration.GetSection("Smtp"));

            services.Configure<Mail.SmtpConfig>(Configuration.GetSection("Smtp"));



            services.AddSingleton<Services.IPathProvider, Services.PathProvider>();

            services.AddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor
                , Microsoft.AspNetCore.Http.HttpContextAccessor>();

            services.AddTransient<AuthTest.Services.IMailService, AuthTest.Services.MailService>();
            
            
            // https://docs.microsoft.com/en-us/aspnet/core/migration/1x-to-2x/identity-2x
            // services.ConfigureExternalCookie()
            // services.ConfigureApplicationCookie(delegate (Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationOptions opts) {});
            
            Microsoft.IdentityModel.Tokens.SecurityKey signingKey = AuthTest.Cryptography.SecurityKeyRepo.GetRsaKey();

            signingKey = AuthTest.Cryptography.SecurityKeyRepo.GetBouncyRsaKey();

            // signingKey = AuthTest.Cryptography.SecurityKeyRepo.GetECDsaKey();
            // signingKey = AuthTest.Cryptography.SecurityKeyRepo.GetBouncyEcdsaKey();
            

            Microsoft.IdentityModel.Tokens.TokenValidationParameters tokenValidationParameters =
                    new Microsoft.IdentityModel.Tokens.TokenValidationParameters
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
                        // ClockSkew = System.TimeSpan.Zero,
                        ClockSkew = new System.TimeSpan(0, 5, 0) // 5 minutes
                    };

            // tokenValidationParameters.valid


            services.AddAuthentication(
                delegate (Microsoft.AspNetCore.Authentication.AuthenticationOptions options)
                {
                    options.DefaultScheme = Microsoft.AspNetCore.Authentication.Cookies
                        .CookieAuthenticationDefaults.AuthenticationScheme;
                    
                    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.Cookies
                        .CookieAuthenticationDefaults.AuthenticationScheme;
                    
                    options.DefaultForbidScheme = Microsoft.AspNetCore.Authentication.Cookies
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
                
                
                opts.Cookie = new Microsoft.AspNetCore.Http.CookieBuilder()
                {
                    // Domain = "localhost:64972",
                    Domain = "localhost",
                    // Path = null,
                    Name = "IANA-JOSE-JWT",
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
                
                
                opts.TicketDataFormat = new NiHaoCookie.JwtCookieDataFormat(
                     // Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256, 
                     // Microsoft.IdentityModel.Tokens.SecurityAlgorithms.RsaSha256, 
                     // Microsoft.IdentityModel.Tokens.SecurityAlgorithms.RsaSha384,
                     Microsoft.IdentityModel.Tokens.SecurityAlgorithms.RsaSha512,
                     // Microsoft.IdentityModel.Tokens.SecurityAlgorithms.EcdsaSha512, // lower not allowed
                     tokenValidationParameters,
                    (int)opts.ExpireTimeSpan.TotalMinutes // 900 = 60*15 = tokenLifetime - in minutes
                ); 
                
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
        
        
        void Microsoft.AspNetCore.Hosting.IStartup.Configure(IApplicationBuilder app)
        {
            this.m_Application = app;
            
            Microsoft.Extensions.Logging.ILoggerFactory loggerFactory = app.ApplicationServices.
                GetRequiredService<Microsoft.Extensions.Logging.ILoggerFactory>();
            
            Services.IMailService mailService = app.ApplicationServices.
                GetRequiredService<Services.IMailService>();
            
            Microsoft.AspNetCore.Hosting.IHostingEnvironment env = app.ApplicationServices.
                GetRequiredService<Microsoft.AspNetCore.Hosting.IHostingEnvironment>();
            
            Microsoft.AspNetCore.Http.IHttpContextAccessor httpContext = app.ApplicationServices.
                GetRequiredService<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            
            Services.IPathProvider pathProvider = app.ApplicationServices.
                GetRequiredService<Services.IPathProvider>();
            
            this.ConfigureVirtualDirectory(app, loggerFactory, mailService, env, httpContext, pathProvider);
        } // End Function Configure 
        


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void ConfigureVirtualDirectory(
              Microsoft.AspNetCore.Builder.IApplicationBuilder app
            , Microsoft.Extensions.Logging.ILoggerFactory loggerFactory
            , Services.IMailService mailService
            , Microsoft.AspNetCore.Hosting.IHostingEnvironment env
            , Microsoft.AspNetCore.Http.IHttpContextAccessor httpContext
            , Services.IPathProvider pathProvider)
        {
            string virtual_directory = "/Virt_DIR";
            virtual_directory = "/";
            // virtual_directory = "/Virt_DIR";

            if (virtual_directory.EndsWith("/"))
                virtual_directory = virtual_directory.Substring(0, virtual_directory.Length - 1);

            // Don't map if you don't have to 
            if (string.IsNullOrWhiteSpace(virtual_directory))
                ConfigureApplication(
                              app
                            , loggerFactory
                            , mailService
                            , env
                            , httpContext
                            , pathProvider
                );
            else
                // app.Map("/Virt_DIR", (mappedApp) => Configure1(mappedApp, env, loggerFactory));
                app.Map(virtual_directory, 
                    delegate (IApplicationBuilder mappedApp)
                    {
                        ConfigureApplication(
                              mappedApp
                            , loggerFactory
                            , mailService
                            , env
                            , httpContext
                            , pathProvider
                        );
                    }
                );

        } // End Sub ConfigureMapping 



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void ConfigureApplication(
              Microsoft.AspNetCore.Builder.IApplicationBuilder app
            , Microsoft.Extensions.Logging.ILoggerFactory loggerFactory
            , Services.IMailService mailService
            , Microsoft.AspNetCore.Hosting.IHostingEnvironment env
            , Microsoft.AspNetCore.Http.IHttpContextAccessor httpContext
            , Services.IPathProvider pathProvider)
        {

            // Microsoft.Extensions.Logging.ConsoleLoggerExtensions.AddConsole(loggerFactory
            //     , Microsoft.Extensions.Logging.LogLevel.Error
            // );

            
            // TODO: Uncomment when error found
            
            AuthTest.Logger.EmailLoggerExtensions.AddEmail(
                  loggerFactory
                , mailService
                , Microsoft.Extensions.Logging.LogLevel.Error  // , Microsoft.Extensions.Logging.LogLevel.Critical
                , httpContext
            );
            

            //app.UseExceptionHandler(
            //    delegate(IApplicationBuilder builder)
            //    {
            //        builder.Run(
            //        context =>
            //            {
            //                var lf = builder.ApplicationServices.GetService<ILoggerFactory>();
            //                var logger = lf.CreateLogger("myExceptionHandlerLogger");
            //                logger.LogDebug("I am a debug message");
            //                return null;
            //            }
            //        );
            //    }
            //);



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


            // https://docs.microsoft.com/en-us/aspnet/core/performance/caching/middleware?tabs=aspnetcore2x



            // https://docs.microsoft.com/en-us/aspnet/core/performance/caching/response
            // https://stackoverflow.com/questions/6601553/execute-rewiter-http-module-before-outputcache
            // https://madskristensen.net/post/use-if-modified-since-header-in-aspnet
            // https://andrewlock.net/adding-cache-control-headers-to-static-files-in-asp-net-core/
            // https://stackoverflow.com/questions/35458737/implement-http-cache-etag-in-asp-net-core-web-api
            // https://www.codeproject.com/Tips/1197510/Cache-headers-for-MVC-File-Action-Result-ASP-NET-C
            // http://asp.net-hacker.rocks/2016/08/04/add-http-header-to-static-files-in-aspnetcore.html
            // http://www.ryadel.com/en/asp-net-core-static-files-cache-control-using-http-headers/
            // https://www.strathweb.com/2012/06/extending-your-asp-net-web-api-responses-with-useful-metadata/
            // https://www.owasp.org/index.php/.NET_Security_Cheat_Sheet
            app.UseStaticFiles(new StaticFileOptions
            {

                OnPrepareResponse = delegate(Microsoft.AspNetCore.StaticFiles.StaticFileResponseContext ctx)
                {
                    // const int durationInSeconds = 60 * 60 * 24;
                    // ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.CacheControl] = "public,max-age=" + durationInSeconds.ToString();

                    ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.CacheControl] = "no-cache, no-store, must-revalidate, private, max-age=0";
                    // https://stackoverflow.com/questions/3096888/standard-for-adding-multiple-values-of-a-single-http-header-to-a-request-or-resp
                    //ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Expires] = "Tue, 01 Jan 1980 1:00:00 GMT, 0";
                    ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Expires] = "0";
                    ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Pragma] = "no-cache";

                    ctx.Context.Response.Headers["X-Content-Type-Options"] = "nosniff";

                    if (ctx.Context.Request.IsHttps)
                    {
                        ctx.Context.Response.Headers["Strict-Transport-Security"] = "max-age=86400; includeSubDomains; preload";
                    }

                    if (ctx.Context.Request.Path.HasValue)
                    {
                        System.IO.FileInfo fi = new System.IO.FileInfo(pathProvider.MapPath(ctx.Context.Request.Path));
                        if (fi.Exists)
                        {
                            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Last-Modified
                            string lm = fi.LastWriteTimeUtc.ToString("ddd', 'dd' 'MMM' 'yyyy' 'HH':'mm':'ss' GMT'", System.Globalization.CultureInfo.InvariantCulture);
                            ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.LastModified] = lm;

                            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/ETag
                            string etag = ETagGenerator.GetETag(ctx.Context.Request.Path.Value, System.Text.Encoding.UTF8.GetBytes(lm));
                            ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.ETag] = etag;
                        } // End if (fi.Exists) 

                    } // End if (ctx.Context.Request.Path.HasValue) 


                    // Microsoft.Net.Http.Headers.HeaderNames.ContentDisposition
                    // Microsoft.Net.Http.Headers.HeaderNames.Age
                    // Microsoft.Net.Http.Headers.HeaderNames.

                } // End OnPrepareResponse 
            });
            
            
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
