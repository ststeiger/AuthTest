
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AuthTest.Services;


namespace AuthTest.Logger
{
    

    public class EmailLogger : ILogger
    {
        private string m_categoryName;
        private System.Func<string, LogLevel, bool> m_filter;
        private IMailService m_mailService;


        private Microsoft.AspNetCore.Http.IHttpContextAccessor m_httpContext;

        public EmailLogger(string categoryName, System.Func<string, LogLevel, bool> filter, IMailService mailService
            , Microsoft.AspNetCore.Http.IHttpContextAccessor context)
        {
            this.m_categoryName = categoryName;
            this.m_filter = filter;
            this.m_mailService = mailService;
            this.m_httpContext = context;
        } // End Constructor 


        // public 
        System.IDisposable ILogger.BeginScope<TState>(TState state)
        {
            // Not necessary
            return null;
        } // End Function BeginScope 


        // public 
        bool ILogger.IsEnabled(LogLevel logLevel)
        {
            return (this.m_filter == null || this.m_filter(this.m_categoryName, logLevel));
        } // End Function IsEnabled 


        // public
        void ILogger.Log<TState>(
              LogLevel logLevel, EventId eventId, TState state, System.Exception exception
            , System.Func<TState, System.Exception, string> formatter
        )
        {
            ILogger thisLogger = (this as ILogger);

            if (!thisLogger.IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new System.ArgumentNullException(nameof(formatter));
            }

            if(this.m_httpContext != null && this.m_httpContext.HttpContext != null)
                System.Console.WriteLine(this.m_httpContext.HttpContext);

            string message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            message = $@"Level: {logLevel}

{message}";

            if (exception != null)
            {
                message += System.Environment.NewLine + System.Environment.NewLine + exception.ToString();
            } // End if (exception != null)

            this.m_mailService.SendMail("elmah@daniel-steiger.ch"
                , "Stefan Steiger", "steiger@cor-management.ch"
                , "[Blog Log Message]"
                , message
            );

        } // End Sub Log 


    } // End Class EmailLogger : ILogger 


} // End Namespace AuthTest.Logger 
