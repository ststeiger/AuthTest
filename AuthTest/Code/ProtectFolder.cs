
using System.Threading.Tasks;


using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authorization;


// http://odetocode.com/blogs/scott/archive/2015/10/06/authorization-policies-and-middleware-in-asp-net-5.aspx
namespace NiHaoCookie
{


    public class ProtectFolderOptions
    {
        public PathString Path { get; set; }
        public string PolicyName { get; set; }
    }


    public static class ProtectFolderExtensions
    {
        public static IApplicationBuilder UseProtectFolder(
            this IApplicationBuilder builder,
            ProtectFolderOptions options)
        {
            return builder.UseMiddleware<ProtectFolder>(options);
        }
    }


    public class ProtectFolder
    {
        private readonly Microsoft.AspNetCore.Http.RequestDelegate _next;
        private readonly Microsoft.AspNetCore.Http.PathString _path;
        private readonly string _policyName;


        public ProtectFolder(RequestDelegate next, ProtectFolderOptions options)
        {
            _next = next;
            _path = options.Path;
            _policyName = options.PolicyName;
        }


        public async Task Invoke(HttpContext httpContext,
                                 IAuthorizationService authorizationService)
        {
            if (httpContext.Request.Path.StartsWithSegments(_path))
            {
                var authorized = await authorizationService.AuthorizeAsync(
                                    httpContext.User, null, _policyName);
                //if (authorized == Microsoft.AspNetCore.Authorization.AuthorizationResult.Failed)
                if(!authorized.Succeeded)
                {
                    /*
                     Severity	Code	Description	Project	File	Line	Suppression State
Warning	CS0618	'HttpContext.Authentication' is obsolete: 'This is obsolete and will be removed in a future version. 
The recommended alternative is to use Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions. See 
https://go.microsoft.com/fwlink/?linkid=845470

                     */
                    
                    await Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.ChallengeAsync(httpContext);
                    return;
                }
            }

            await _next(httpContext);
        }


    }


}
