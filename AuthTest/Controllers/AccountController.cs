
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;



using Microsoft.AspNetCore.Authentication;


namespace AuthTest.Controllers
{


    public class AccountController : Controller
    {

        private Services.IMailService m_MailService;


        public AccountController(Services.IMailService mailService)
        {
            this.m_MailService = mailService;
        } // End Constructor 


        private const string blankHtmlTemplate = @"<!DOCTYPE html>
<html>
<head>
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge,chrome=1"" />
    <meta charset=""utf-8"" /> 
    <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
    <meta http-equiv=""cache-control"" content=""max-age=0"" />
    <meta http-equiv=""cache-control"" content=""no-cache"" />
    <meta http-equiv=""expires"" content=""0"" />
    <meta http-equiv=""expires"" content=""Tue, 01 Jan 1980 1:00:00 GMT"" />
    <meta http-equiv=""pragma"" content=""no-cache"" />
    <title>@title</title>
    <meta name=""viewport"" content=""width=device-width, initial-scale=1, maximum-scale=1"" />
</head>
<body>
    @body
</body>
</html>";


        public IActionResult Index()
        {
            return View();
        } // End Action Index 


        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public new ContentResult Unauthorized()
        {
            string htmlTemplate = blankHtmlTemplate;
            htmlTemplate = htmlTemplate.Replace("@title", "Access denied (unauthorized)");
            htmlTemplate = htmlTemplate.Replace("@body", "<h1>I am the new Unauthorized</h1>");

            return Content(htmlTemplate, "text/html");
        } // End Action Unauthorized 


        public class StackOverflower
        {
            private string m_MyText;

            public string MyText
            {
                get { return MyText; }
                set { this.m_MyText = value; }
            }
        }



        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public ContentResult NotAuthorized()
        {
            string htmlTemplate = blankHtmlTemplate;
            htmlTemplate = htmlTemplate.Replace("@title", "Access denied (unauthorized)");
            htmlTemplate = htmlTemplate.Replace("@body", "<h1>Unauthorized</h1>");

            var foo = new StackOverflower();
            System.Console.WriteLine(foo.MyText);
            // throw new Exception("foo");

            this.m_MailService.SendMail("stefan.steiger@rsnweb.ch", "Stefan Steiger"
                , "steiger@cor-management.ch", "Unauthorized", htmlTemplate
            );

            return Content(htmlTemplate, "text/html");
        } // End Action NotAuthorized 


        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<ContentResult> Login()
        {
            string htmlTemplate = blankHtmlTemplate;
            htmlTemplate = htmlTemplate.Replace("@title", "Logged out");
            htmlTemplate = htmlTemplate.Replace("@body", "<h1>SignIn</h1>");

            // https://stackoverflow.com/questions/28664686/how-do-i-get-client-ip-address-in-asp-net-core
            System.Net.IPAddress remoteIpAddress = this.Request.HttpContext.Connection.RemoteIpAddress;
            System.Console.WriteLine(remoteIpAddress);


            List<System.Security.Claims.Claim> ls = new List<System.Security.Claims.Claim>();

            ls.Add(
                new System.Security.Claims.Claim(
                    System.Security.Claims.ClaimTypes.Name, "IcanHazUsr_éèêëïàáâäåãæóòôöõõúùûüñçø_ÉÈÊËÏÀÁÂÄÅÃÆÓÒÔÖÕÕÚÙÛÜÑÇØ 你好，世界 Привет\tмир"
                , System.Security.Claims.ClaimValueTypes.String
                )
            );

            // 

            System.Security.Claims.ClaimsIdentity id = new System.Security.Claims.ClaimsIdentity("authenticationType");
            id.AddClaims(ls);

            System.Security.Claims.ClaimsPrincipal principal = new System.Security.Claims.ClaimsPrincipal(id);

            // https://docs.asp.net/en/latest/security/authentication/cookie.html

            
            await HttpContext.SignInAsync(principal);

            return Content(htmlTemplate, "text/html");
        } // End Action Login 



        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public ContentResult Logout()
        {
            string htmlTemplate = blankHtmlTemplate;
            htmlTemplate = htmlTemplate.Replace("@title", "Logged out");
            htmlTemplate = htmlTemplate.Replace("@body", "<h1>Logout</h1>");
            
            return Content(htmlTemplate, "text/html");
        } // End Action Logout 


        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public ContentResult Forbidden()
        {
            string htmlTemplate = blankHtmlTemplate;
            htmlTemplate = htmlTemplate.Replace("@title", "Forbidden");
            htmlTemplate = htmlTemplate.Replace("@body", "<h1>Access Denied</h1>");

            return Content(htmlTemplate, "text/html");
        } // End Action Forbidden 


    } // End class AccountController : Controller 


} // End Namespace AuthTest.Controllers 
