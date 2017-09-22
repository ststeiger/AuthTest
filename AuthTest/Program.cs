
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;


namespace AuthTest
{


    // https://developers.redhat.com/topics/dotnet/
    public class Program
    {


        // Entity-Framework strikes again ... 
        // First it will look for an implementation of this new IDesignTimeDbContextFactory interface 
        // (which just allows it to create a new context). 

        // If you don’t have this interface, it will attempt to startup your project, 
        // but it doesn’t want to start listening for HTTP calls, 
        // so it attempts to find an IWebHost but it never runs it. 
        // How does it look for it? 
        // It tries to call a public static method of the Program class called BuildWebHost.

        // Yeah, so that BuildWebHost is a convention. 
        // If you want to get rid of it, just implement the IDesignTimeDbContextFactory, it’s really simple.
        public static IWebHost BuildWebHost(string[] args)
        {
            //System.Console.WriteLine(WebHostDefaults.ApplicationKey); // applicationName
            //System.Console.WriteLine(WebHostDefaults.StartupAssemblyKey); // startupAssembly 
            //System.Console.WriteLine(WebHostDefaults.HostingStartupAssembliesKey); // hostingStartupAssemblies 
            //System.Console.WriteLine(WebHostDefaults.DetailedErrorsKey); // detailedErrors
            //System.Console.WriteLine(WebHostDefaults.EnvironmentKey); // environment 
            //System.Console.WriteLine(WebHostDefaults.WebRootKey); // webroot
            //System.Console.WriteLine(WebHostDefaults.CaptureStartupErrorsKey); // captureStartupErrors
            //System.Console.WriteLine(WebHostDefaults.ServerUrlsKey); // urls
            //System.Console.WriteLine(WebHostDefaults.ContentRootKey); // contentRoot
            //System.Console.WriteLine(WebHostDefaults.PreferHostingUrlsKey); // preferHostingUrls
            //System.Console.WriteLine(WebHostDefaults.PreventHostingStartupKey); // preventHostingStartup
            //System.Console.WriteLine(WebHostDefaults.ShutdownTimeoutKey);// shutdownTimeoutSeconds


            // https://wildermuth.com/2017/07/06/Program-cs-in-ASP-NET-Core-2-0

            IWebHostBuilder builder = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(System.IO.Directory.GetCurrentDirectory())
                // https://github.com/aspnet/Hosting/issues/1137
                .ConfigureServices(
                    delegate (IServiceCollection services)
                    {
                        services.AddSingleton<IStartup, Startup>();
                    }
                )
                .UseSetting(
                    WebHostDefaults.ApplicationKey, 
                    System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(Startup)).Assembly.FullName
                )
                .ConfigureAppConfiguration(
                    delegate (WebHostBuilderContext hostingContext, IConfigurationBuilder config)
                    {
                        IHostingEnvironment env = hostingContext.HostingEnvironment;

                        config
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                        if (env.IsDevelopment())
                        {
                            System.Reflection.Assembly appAssembly = System.Reflection.Assembly.Load(
                                new System.Reflection.AssemblyName(env.ApplicationName)
                            );

                            if (appAssembly != null)
                            {
                                // config.AddUserSecrets("");
                                config.AddUserSecrets(appAssembly, optional: true);
                            }
                        }

                        config.AddEnvironmentVariables();

                        if (args != null)
                        {
                            config.AddCommandLine(args);
                        }
                    } // End delegate 
                )
                .ConfigureLogging( 
                    delegate(WebHostBuilderContext hostingContext, ILoggingBuilder logging)
                    {
                        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                        logging.AddConsole();
                        logging.AddDebug();

                        

                    } // End delegate 
                )
                .UseIISIntegration()
                .UseDefaultServiceProvider(
                    delegate(WebHostBuilderContext context, ServiceProviderOptions options)
                    {
                        options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                    } // End delegate 
                );


            return builder.Build();


            /*
            // Not the default-builder 
            // https://codingblast.com/asp-net-core-configuration/
            return new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    IHostingEnvironment env = builderContext.HostingEnvironment;

                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                })
                .UseIISIntegration()
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                })
                .UseStartup<Startup>()
                .Build();



            // With Default-Builder 
            return WebHost.CreateDefaultBuilder(args)
               .UseStartup<Startup>()
               .Build();
            */
        } // End Function BuildWebHost 


        // http://www.urbanterror.info/support/174-linux-server-install/
        // #!/bin/bash
        // while true
        // do
        // /home/username/bin/test.exe +set fs_game q3ut4  +exec server.cfg
        // echo "server crashed on `date`" > last_crash.txt
        // done
        public static void Main(string[] args)
        {
            // https://stackoverflow.com/questions/38291567/killing-gracefully-a-net-core-daemon-running-on-linux
            System.Runtime.Loader.AssemblyLoadContext.Default.Unloading += Default_Unloading;
            
            System.AppDomain.CurrentDomain.UnhandledException += 
                new System.UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            System.AppDomain.CurrentDomain.FirstChanceException += new System.EventHandler<System.Runtime.ExceptionServices
                .FirstChanceExceptionEventArgs>(CurrentDomain_FirstChanceException);

            BuildWebHost(args).Run();
        } // End Sub Main 


        private static void Default_Unloading(System.Runtime.Loader.AssemblyLoadContext obj)
        {
            System.Console.WriteLine("unloading");
        }


        private static void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            // System.Console.WriteLine("first chance");
        }

        // Yes from CLR 2.0 stack overflow is considered a non-recoverable situation. So the runtime still shut down the process.
        // https://stackoverflow.com/questions/1599219/c-sharp-catch-a-stack-overflow-exception/1599238#1599238
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
        private static void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            System.Console.WriteLine("appdomain - unhandled");
        }


    } // End Class Program


} // End Namespace AuthTest
