
namespace System.Web
{

    public static class HttpContext
    {
        private static Microsoft.AspNetCore.Http.IHttpContextAccessor m_httpContextAccessor;
        private static Microsoft.AspNetCore.Mvc.Infrastructure.IActionContextAccessor m_actionContextAccessor;


        public static void Configure(Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor)
        {
            m_httpContextAccessor = httpContextAccessor;
        }


        public static void ConfigureActionContext(Microsoft.AspNetCore.Mvc.Infrastructure.IActionContextAccessor actionContextAccessor)
        {
            m_actionContextAccessor = actionContextAccessor;
        }


        public static Microsoft.AspNetCore.Http.HttpContext Current
        {
            get
            {
                return m_httpContextAccessor.HttpContext;
            }
        }


        public static Microsoft.AspNetCore.Mvc.ActionContext ActionContext
        {
            get
            {
                return m_actionContextAccessor.ActionContext;;
            }
        }


    }


}
