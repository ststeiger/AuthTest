
using Microsoft.Extensions.Logging;
using AuthTest.Services;


namespace AuthTest.Logger
{


    public class EmailLoggerProvider : ILoggerProvider
    {
        private readonly System.Func<string, LogLevel, bool> m_filter;
        private readonly IMailService m_mailService;
        private Microsoft.AspNetCore.Http.IHttpContextAccessor m_httpContext;

        public EmailLoggerProvider(System.Func<string, LogLevel, bool> filter, IMailService mailService
            , Microsoft.AspNetCore.Http.IHttpContextAccessor context)
        {
            this.m_mailService = mailService;
            this.m_filter = filter;
            this.m_httpContext = context;
        }


        public ILogger CreateLogger(string categoryName)
        {
            return new EmailLogger(categoryName, this.m_filter, this.m_mailService, this.m_httpContext);
        }


        public void Dispose()
        { }


    }


}
