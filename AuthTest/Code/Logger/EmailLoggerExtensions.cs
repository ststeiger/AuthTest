
using Microsoft.Extensions.Logging;
using AuthTest.Services;


namespace AuthTest.Logger
{


    public static class EmailLoggerExtensions
    {


        public static ILoggerFactory AddEmail(
            this ILoggerFactory factory,
            IMailService mailService,
            System.Func<string, LogLevel, bool> filter,
            Microsoft.AspNetCore.Http.IHttpContextAccessor context
        )
        {
            factory.AddProvider(new EmailLoggerProvider(filter, mailService, context));
            return factory;
        }


        public static ILoggerFactory AddEmail(this ILoggerFactory factory, IMailService mailService, LogLevel minLevel
            , Microsoft.AspNetCore.Http.IHttpContextAccessor context)
        {
            return AddEmail(
                factory,
                mailService,
                //(categoryName, logLevel) => logLevel >= minLevel
                delegate (string categoryName, LogLevel logLevel)
                {
                    return logLevel >= minLevel;
                }
                ,context
            );
        }


    }


}
