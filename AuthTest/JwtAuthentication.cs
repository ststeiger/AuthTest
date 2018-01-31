
namespace AuthTest
{


    public class JwtAuthentication
    {


        public static Microsoft.IdentityModel.Tokens.TokenValidationParameters GetTokenValidationParameters()
        {
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

            return tokenValidationParameters;
        }


        public static void SetCookieSchemes(Microsoft.AspNetCore.Authentication.AuthenticationOptions options)
        {

            options.DefaultScheme = Microsoft.AspNetCore.Authentication.Cookies
                .CookieAuthenticationDefaults.AuthenticationScheme;

            options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.Cookies
                .CookieAuthenticationDefaults.AuthenticationScheme;

            options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.Cookies
                .CookieAuthenticationDefaults.AuthenticationScheme;

            options.DefaultForbidScheme = Microsoft.AspNetCore.Authentication.Cookies
                .CookieAuthenticationDefaults.AuthenticationScheme;

            options.DefaultSignInScheme = Microsoft.AspNetCore.Authentication.Cookies
                .CookieAuthenticationDefaults.AuthenticationScheme;

            options.DefaultSignOutScheme = Microsoft.AspNetCore.Authentication.Cookies
                .CookieAuthenticationDefaults.AuthenticationScheme;
        }



        public static void SetupCookie(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationOptions opts)
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
                OnValidatePrincipal = async delegate (Microsoft.AspNetCore.Authentication.Cookies
                .CookieValidatePrincipalContext context)
                    {
                        System.Console.WriteLine(context);
                        // context.coo

                        //string userSessionToken  = context.Principal.Claims
                        //    .FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

                        string userSessionToken = null;

                        if (string.IsNullOrEmpty(userSessionToken))
                        {
                            /*
                            await context.HttpContext.Authentication.SignOutAsync(
                                Microsoft.AspNetCore.Authentication.Cookies
                                    .CookieAuthenticationDefaults.AuthenticationScheme);
                            */
                            context.RejectPrincipal();
                        }

                        await System.Threading.Tasks.Task.CompletedTask;
                    }
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

            var tokenValidationParameters = GetTokenValidationParameters();
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


        } // End Sub xxx


        public static void SetupBearer(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerOptions options)
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


    } // End Class 


}