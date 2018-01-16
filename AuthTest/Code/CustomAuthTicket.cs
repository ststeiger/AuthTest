
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;

using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;


namespace NiHaoCookie
{


    public class JwtCookieDataFormat : ISecureDataFormat<AuthenticationTicket>
    {

        private readonly string algorithm;
        private readonly TokenValidationParameters validationParameters;

        public JwtCookieDataFormat(string algorithm, TokenValidationParameters validationParameters)
        {
            this.algorithm = algorithm;
            this.validationParameters = validationParameters;
        }

        
        // This ISecureDataFormat implementation is decode-only
        string ISecureDataFormat<AuthenticationTicket>.Protect(AuthenticationTicket data)
        {
            return EncryptCookie(data, null);
        }

        string ISecureDataFormat<AuthenticationTicket>.Protect(AuthenticationTicket data, string purpose)
        {
            return EncryptCookie(data, purpose);
        }

        AuthenticationTicket ISecureDataFormat<AuthenticationTicket>.Unprotect(string protectedText)
        {
            return DecryptCookie(protectedText, null);
        }

        AuthenticationTicket ISecureDataFormat<AuthenticationTicket>.Unprotect(string protectedText, string purpose)
        {
            return DecryptCookie(protectedText, purpose);
        }


        private string EncryptCookie(AuthenticationTicket data, string purpose)
        {
            // data.AuthenticationScheme
            // data.Principal
            System.Console.WriteLine(data.Principal);

            string jwtTicket = AuthHelper.IssueToken(data);
            System.Console.WriteLine(jwtTicket);

            return jwtTicket;
        } // End Function EncryptCookie 


        // http://blogs.microsoft.co.il/sasha/2012/01/20/aggressive-inlining-in-the-clr-45-jit/
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private AuthenticationTicket DecryptCookie(string protectedText, string purpose)
        {
            // http://stackoverflow.com/questions/36140550/cant-get-signature-from-jwtsecuritytoken
            SecurityToken token;
            ClaimsPrincipal inPrinciple = AuthHelper.ValidateJwtToken(protectedText, out token);
            System.Console.WriteLine(inPrinciple);
            System.Console.WriteLine(token);

            // JwtSecurityToken validJwt = token as JwtSecurityToken;


            // AuthenticationProperties ap = new AuthenticationProperties();
            return new AuthenticationTicket(inPrinciple, new Microsoft.AspNetCore.Authentication.AuthenticationProperties()
                , "MyCookieMiddlewareInstance");


            /*
            System.Collections.Generic.List<System.Security.Claims.Claim> ls = 
                new System.Collections.Generic.List<System.Security.Claims.Claim>();

            ls.Add(
                new System.Security.Claims.Claim(
                    System.Security.Claims.ClaimTypes.Name, "IcanHazUsr_éèêëïàáâäåãæóòôöõõúùûüñçø_ÉÈÊËÏÀÁÂÄÅÃÆÓÒÔÖÕÕÚÙÛÜÑÇØ 你好，世界 Привет\tмир"
                , System.Security.Claims.ClaimValueTypes.String
                )
            );

            // 

            System.Security.Claims.ClaimsIdentity id = new System.Security.Claims.ClaimsIdentity("authenticationType");
            id.AddClaims(ls);

            principal = new System.Security.Claims.ClaimsPrincipal(id);
            
            return new AuthenticationTicket(principal, new AuthenticationProperties(), "MyCookieMiddlewareInstance");
            */
        } // End Function DecryptCookie 


    } // End Class JwtCookieDataFormat : ISecureDataFormat<AuthenticationTicket>


} // End Namespace NiHaoCookie
