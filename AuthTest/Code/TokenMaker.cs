
// using System.Security.Claims;
// using Microsoft.IdentityModel.Tokens;
// using System.IdentityModel.Tokens.Jwt;


namespace NiHaoCookie
{


    public class AuthHelper
    {


        class SecurityConstants
        {
            public static string TokenIssuer = "nihao.lol";
            public static string TokenAudience = "nobody";
            public static int TokenLifetimeMinutes = 720; // 60*12;

            public static Microsoft.IdentityModel.Tokens.SecurityKey SecurityKey = 
                new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes("i am a top secret password")
            );

        } // End Class SecurityConstants 


        public static string IssueToken(Microsoft.AspNetCore.Authentication.AuthenticationTicket data)
        {
            /*
            List<Claim> claimList = new List<Claim>()
            {
                new Claim(ClaimTypes.Role, "role 1"),     //Not sure what this is for
                new Claim(ClaimTypes.Role, "role 2"),
                new Claim(ClaimTypes.Role, "role 3"),
                new Claim(ClaimTypes.Role, "role 4"),

                new Claim(ClaimTypes.GroupSid, "0"),
                //new Claim(ClaimTypes.PrimaryGroupSid, "0"),
                new Claim(ClaimTypes.PrimarySid, "0"),

                new Claim(ClaimTypes.WindowsDeviceClaim, "machine-name?"),
                new Claim(ClaimTypes.WindowsDeviceGroup, "workgroup-name?"),

                new Claim(ClaimTypes.WindowsUserClaim,"???"),
                new Claim(ClaimTypes.WindowsAccountName, "root"),
                new Claim(ClaimTypes.Sid,"0"), // Security Id
                new Claim(ClaimTypes.Hash, "MD5 of what ?"),
                new Claim(ClaimTypes.Name, "Noob McNoobington"),
                new Claim(ClaimTypes.GivenName, "Noob"), // first name, forename, Christian name, middle name
                new Claim(ClaimTypes.Surname,"McNoobington"), // Family name, last name, second name
                new Claim(ClaimTypes.Gender,"noob"),
                new Claim(ClaimTypes.DateOfBirth, "01.01.1978"),

                new Claim(ClaimTypes.Country,"Afghanistan"),
                new Claim(ClaimTypes.StateOrProvince,"California"),
                new Claim(ClaimTypes.StreetAddress,"1st Ravenscourt Road"),
                new Claim(ClaimTypes.PostalCode, "90210"),
                new Claim(ClaimTypes.Locality,"Tora Bora"),

                new Claim(ClaimTypes.MobilePhone,"666"),
                new Claim(ClaimTypes.HomePhone,"666 666"),
                new Claim(ClaimTypes.OtherPhone,"666 666 666"),
                new Claim(ClaimTypes.Email, "noobie@noob.com"),
                new Claim(ClaimTypes.Webpage,"http://webpage.com"),

                new Claim(ClaimTypes.Thumbprint,"Thumbprint"),
                new Claim(ClaimTypes.UserData,"UserData"),
                new Claim(ClaimTypes.Version,"Version"),

                new Claim(ClaimTypes.WindowsFqbnVersion,"WindowsFqbnVersion"),
                new Claim(ClaimTypes.WindowsSubAuthority,"WindowsSubAuthority"),
                new Claim(ClaimTypes.X500DistinguishedName,"X500DistinguishedName"),

            };
            */


            System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler tokenHandler = 
                new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();


            System.DateTime now = System.DateTime.UtcNow;

            Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor desc = 
                new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
            {
                //Subject = new System.Security.Claims.ClaimsIdentity(claimList),
                Subject = (System.Security.Claims.ClaimsIdentity)data.Principal.Identity,
                Issuer = SecurityConstants.TokenIssuer,
                Audience = SecurityConstants.TokenAudience,
                IssuedAt = now,
                Expires = now.AddMinutes(SecurityConstants.TokenLifetimeMinutes),
                NotBefore = now.AddTicks(-1),
                SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                    SecurityConstants.SecurityKey
                    , Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256
                )
            };

            System.IdentityModel.Tokens.Jwt.JwtSecurityToken tok = 
                tokenHandler.CreateJwtSecurityToken(desc);
            // tok.Header.Add("jti", "foo");
            // tok.Payload.Add("jti", "foobar");

            System.Console.WriteLine(tok.Id);

            string tokk = tok.ToString();
            System.Console.WriteLine(tokk);

            return tokenHandler.CreateEncodedJwt(desc);
        } // End Function IssueToken 


        public static System.Security.Claims.ClaimsPrincipal 
            ValidateJwtToken(string jwtToken, out Microsoft.IdentityModel.Tokens.SecurityToken token)
        {
            Microsoft.IdentityModel.Tokens.SecurityKey sSKey = SecurityConstants.SecurityKey;
            System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler tokenHandler = 
                new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();

            // Parse JWT from the Base64UrlEncoded wire form 
            //(<Base64UrlEncoded header>.<Base64UrlEncoded body>.<signature>)
            System.IdentityModel.Tokens.Jwt.JwtSecurityToken parsedJwt = tokenHandler.ReadToken(jwtToken) 
                as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;

            Microsoft.IdentityModel.Tokens.TokenValidationParameters validationParams =
                new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidAudience = SecurityConstants.TokenAudience,
                    ValidIssuers = new System.Collections.Generic.List<string>()
                        { SecurityConstants.TokenIssuer },
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = sSKey,

                };

            return tokenHandler.ValidateToken(jwtToken, validationParams, out token);
        } // End Function ValidateJwtToken 


    } // End Class AuthHelper 


} // End Namespace NiHaoCookie 
