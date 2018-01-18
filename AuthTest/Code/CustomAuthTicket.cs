
namespace NiHaoCookie
{


    public class JwtCookieDataFormat : Microsoft.AspNetCore.Authentication.ISecureDataFormat<
        Microsoft.AspNetCore.Authentication.AuthenticationTicket>
    {

        private readonly Microsoft.IdentityModel.Tokens.TokenValidationParameters m_validationParameters;
        private readonly int m_tokenLifetimeMinutes;
        private readonly Microsoft.IdentityModel.Tokens.SigningCredentials m_signingCredentials;

        
        public JwtCookieDataFormat(
              string algorithm
            , Microsoft.IdentityModel.Tokens.TokenValidationParameters validationParameters
            , int tokenLifetimeMinutes)
        {
            this.m_validationParameters = validationParameters;
            this.m_tokenLifetimeMinutes = tokenLifetimeMinutes;

            this.m_signingCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                        this.m_validationParameters.IssuerSigningKey
                      , algorithm
            );
        } // End Constructor 

        
        // This ISecureDataFormat implementation is decode-only
        string Microsoft.AspNetCore.Authentication.ISecureDataFormat<
            Microsoft.AspNetCore.Authentication.AuthenticationTicket
        >.Protect(Microsoft.AspNetCore.Authentication.AuthenticationTicket data)
        {
            return EncryptCookie(data, null);
        }

        string Microsoft.AspNetCore.Authentication.ISecureDataFormat<
            Microsoft.AspNetCore.Authentication.AuthenticationTicket
        >.Protect(Microsoft.AspNetCore.Authentication.AuthenticationTicket data, string purpose)
        {
            return EncryptCookie(data, purpose);
        }

        Microsoft.AspNetCore.Authentication.AuthenticationTicket 
        Microsoft.AspNetCore.Authentication.ISecureDataFormat<
            Microsoft.AspNetCore.Authentication.AuthenticationTicket
        >.Unprotect(string protectedText)
        {
            return DecryptCookie(protectedText, null);
        }


        Microsoft.AspNetCore.Authentication.AuthenticationTicket 
            Microsoft.AspNetCore.Authentication.ISecureDataFormat<
            Microsoft.AspNetCore.Authentication.AuthenticationTicket
        >.Unprotect(string protectedText, string purpose)
        {
            return DecryptCookie(protectedText, purpose);
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public string IssueToken(Microsoft.AspNetCore.Authentication.AuthenticationTicket data)
        {
            System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler tokenHandler =
                new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();

            System.DateTime now = System.DateTime.UtcNow;

            Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor desc =
                new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
                {
                    //Subject = new System.Security.Claims.ClaimsIdentity(claimList),
                    Subject = (System.Security.Claims.ClaimsIdentity)data.Principal.Identity,
                    Issuer = this.m_validationParameters.ValidIssuer, //SecurityConstants.TokenIssuer,
                    Audience = this.m_validationParameters.ValidAudience, // SecurityConstants.TokenAudience,
                    IssuedAt = now,
                    Expires = now.AddMinutes(this.m_tokenLifetimeMinutes),
                    NotBefore = now.AddTicks(-1),
                    SigningCredentials = this.m_signingCredentials
                };

            // System.IdentityModel.Tokens.Jwt.JwtSecurityToken tok =
            //        tokenHandler.CreateJwtSecurityToken(desc);
            // // tok.Header.Add("jti", "foo");
            // // tok.Payload.Add("jti", "foobar");

            // System.Console.WriteLine(tok.Id);

            // string tokk = tok.ToString();
            // System.Console.WriteLine(tokk);

            return tokenHandler.CreateEncodedJwt(desc);
        } // End Function IssueToken 

        
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private string EncryptCookie(
              Microsoft.AspNetCore.Authentication.AuthenticationTicket data
            , string purpose)
        {
            // data.AuthenticationScheme, data.Principal
            // System.Console.WriteLine(data.Principal);

            string jwtTicket = this.IssueToken(data);
            // System.Console.WriteLine(jwtTicket);

            return jwtTicket;
        } // End Function EncryptCookie 


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        protected System.Security.Claims.ClaimsPrincipal ValidateJwtToken(
              string jwtToken
            , out Microsoft.IdentityModel.Tokens.SecurityToken token)
        {
            System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler tokenHandler =
                new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();

            // Parse JWT from the Base64UrlEncoded wire form 
            // (<Base64UrlEncoded header>.<Base64UrlEncoded body>.<signature>)
            System.IdentityModel.Tokens.Jwt.JwtSecurityToken parsedJwt = tokenHandler.ReadToken(jwtToken)
                as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;
            
            return tokenHandler.ValidateToken(jwtToken, this.m_validationParameters, out token);
        } // End Function ValidateJwtToken 


        // http://blogs.microsoft.co.il/sasha/2012/01/20/aggressive-inlining-in-the-clr-45-jit/
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private Microsoft.AspNetCore.Authentication.AuthenticationTicket 
            DecryptCookie(string protectedText, string purpose)
        {
            // http://stackoverflow.com/questions/36140550/cant-get-signature-from-jwtsecuritytoken
            Microsoft.IdentityModel.Tokens.SecurityToken token;

            System.Security.Claims.ClaimsPrincipal inPrinciple = 
                this.ValidateJwtToken(protectedText, out token);

            // System.Console.WriteLine(inPrinciple);
            // System.Console.WriteLine(token);

            return new Microsoft.AspNetCore.Authentication.AuthenticationTicket(
                  inPrinciple
                , new Microsoft.AspNetCore.Authentication.AuthenticationProperties()
                // , "MyCookieMiddlewareInstance"
                , Microsoft.AspNetCore.Authentication.Cookies
                        .CookieAuthenticationDefaults.AuthenticationScheme 
            );
        } // End Function DecryptCookie 


    } // End Class JwtCookieDataFormat : ISecureDataFormat<AuthenticationTicket>


} // End Namespace NiHaoCookie
