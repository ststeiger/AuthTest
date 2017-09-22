using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Xml;
using Microsoft.AspNetCore.Authentication.JwtBearer;


namespace NiHaoCookie
{


    public class AuthHelper
    {

        class SecurityConstants
        {
            public static string TokenIssuer = "nihao.lol";
            public static string TokenAudience = "nobody";
            public static int TokenLifetimeMinutes = 720; // 60*12;

            public static SecurityKey SecurityKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes("i am a top secret password")
            );

        }


        public static string IssueToken()
        {
            var claimList = new List<Claim>()
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

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityTokenDescriptor desc = makeSecurityTokenDescriptor(SecurityConstants.SecurityKey, claimList);

            JwtSecurityToken tok = tokenHandler.CreateJwtSecurityToken(desc);
            // tok.Header.Add("jti", "foo");
            // tok.Payload.Add("jti", "foobar");

            System.Console.WriteLine(tok.Id);

            string tokk = tok.ToString();
            System.Console.WriteLine(tokk);

            return tokenHandler.CreateEncodedJwt(desc);
        } // End Function IssueToken 


        private static SecurityTokenDescriptor makeSecurityTokenDescriptor(SecurityKey sSKey, List<Claim> claimList)
        {
            claimList.Add(new System.Security.Claims.Claim("jti", System.Guid.NewGuid().ToString()));

            System.DateTime now = DateTime.UtcNow;
            Claim[] claims = claimList.ToArray();

            return new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(claims),
                Issuer = SecurityConstants.TokenIssuer,
                Audience = SecurityConstants.TokenAudience,
                IssuedAt = System.DateTime.UtcNow,
                Expires = System.DateTime.UtcNow.AddMinutes(SecurityConstants.TokenLifetimeMinutes),
                NotBefore = System.DateTime.UtcNow.AddTicks(-1),

                SigningCredentials = new SigningCredentials(sSKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256)
            };

        } // End Function makeSecurityTokenDescriptor 
        

        public static ClaimsPrincipal ValidateJwtToken(string jwtToken, out SecurityToken token)
        {
            SecurityKey sSKey = SecurityConstants.SecurityKey;
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            // Parse JWT from the Base64UrlEncoded wire form 
            //(<Base64UrlEncoded header>.<Base64UrlEncoded body>.<signature>)
            JwtSecurityToken parsedJwt = tokenHandler.ReadToken(jwtToken) as JwtSecurityToken;

            TokenValidationParameters validationParams =
                new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidAudience = SecurityConstants.TokenAudience,
                    ValidIssuers = new List<string>() { SecurityConstants.TokenIssuer },
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = sSKey,

                };

            return tokenHandler.ValidateToken(jwtToken, validationParams, out token);
        } // End Function ValidateJwtToken 





    }
}
