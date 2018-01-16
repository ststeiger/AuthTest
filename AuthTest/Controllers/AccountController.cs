
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
        //public async Task<ContentResult> SignIn(string ReturnUrl, string uname, string pwd)
        public async Task<IActionResult> SignIn(string ReturnUrl, string uname, string pwd)
        {
            System.Console.WriteLine(ReturnUrl);

            //string htmlTemplate = blankHtmlTemplate;
            //htmlTemplate = htmlTemplate.Replace("@title", "Logged out");
            //htmlTemplate = htmlTemplate.Replace("@body", "<h1>Signed In</h1>");

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

            // return Content(htmlTemplate, "text/html");
            return this.LocalRedirect(ReturnUrl ?? "/");
        }


        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public ContentResult Login(string ReturnUrl)
        {
            System.Console.WriteLine(ReturnUrl);
            // path = Context.Request.Path;
            // https://codepen.io/ehermanson/pen/KwKWEv
            // https://bootsnipp.com/tags/login
            // https://www.w3schools.com/howto/howto_css_login_form.asp

            string retUrl = System.Web.HttpUtility.HtmlAttributeEncode(System.Web.HttpUtility.UrlEncode(ReturnUrl));

            return Content(@"<!DOCTYPE html>
<html>
<head>
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge,chrome=1"" />
    <meta http-equiv=""Content-Type"" content=""text/html;charset=utf-8"" />
    <meta charset=""utf-8"" />

    <meta http-equiv=""cache-control"" content=""max-age=0"" />
    <meta http-equiv=""cache-control"" content=""no-cache"" />
    <meta http-equiv=""expires"" content=""0"" />
    <meta http-equiv=""expires"" content=""Tue, 01 Jan 1980 1:00:00 GMT"" />
    <meta http-equiv=""pragma"" content=""no-cache"" />

    <title>Legende</title>

    <meta name=""viewport"" content=""width=device-width, initial-scale=1.00, minimum-scale=0.00, maximum-scale=10.00, user-scalable=yes"" />

    <style type=""text/css"">
        
        *
        {
            margin: 0px;
            padding: 0px;
            box-sizing: border-box;
            #white-space-collapse: discard;
        }

        body 
        {
            width: 84.1cm;
            height: 118.9cm;
            background-color: lightgrey; 
            background-color: white; 
            z-index: 999999; 
            position:relative;

            font-family: Arial;
            font-size: 11px;
            height: 12.4px;
        }


/* Bordered form */
form {
    border: 3px solid #f1f1f1;
}

/* Full-width inputs */
input[type=text], input[type=password] {
    width: 100%;
    padding: 12px 20px;
    margin: 8px 0;
    display: inline-block;
    border: 1px solid #ccc;
    box-sizing: border-box;
}

/* Set a style for all buttons */
button {
    background-color: #4CAF50;
    color: white;
    padding: 14px 20px;
    margin: 8px 0;
    border: none;
    cursor: pointer;
    width: 100%;
}

/* Add a hover effect for buttons */
button:hover {
    opacity: 0.8;
}

/* Extra style for the cancel button (red) */
.cancelbtn {
    width: auto;
    padding: 10px 18px;
    background-color: #f44336;
}

/* Center the avatar image inside this container */
.imgcontainer {
    text-align: center;
    margin: 24px 0 12px 0;
}

/* Avatar image */
img.avatar {
    width: 40%;
    border-radius: 50%;
}

/* Add padding to containers */
.container {
    padding: 16px;
}

/* The ""Forgot password"" text */
span.psw {
                float: right;
                padding - top: 16px;
            }

            /* Change styles for span and cancel button on extra small screens */
            @media screen and(max - width: 300px) {
                span.psw {
                    display: block;
                    float: none;
                }
    .cancelbtn {
                    width: 100 %;
                }
            }

    </style>

    <script>

    </script>

</head>
<body>
    <h1>Login</h1>
    <form action=""/Account/SignIn?ReturnUrl=" + retUrl + @""" method=""POST"">
      <div class=""imgcontainer"">
        <img src=""img_avatar2.png"" alt=""Avatar"" class=""avatar"">
      </div>

      <div class=""container"">
        <label><b>Username</b></label>
        <input type=""text"" placeholder=""Enter Username"" name=""uname"" required>

        <label><b>Password</b></label>
        <input type=""password"" placeholder=""Enter Password"" name=""pwd"" required>

        <button type=""submit"">Login</button>
        <label>
          <input type=""checkbox"" checked=""checked""> Remember me
        </label>
      </div>

      <div class=""container"" style=""background-color:#f1f1f1"">
        <button type=""button"" class=""cancelbtn"">Cancel</button>
        <span class=""psw"">Forgot <a href=""#"">password?</a></span>
      </div>
    </form>


</body>
</html>
", "text/html");

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
