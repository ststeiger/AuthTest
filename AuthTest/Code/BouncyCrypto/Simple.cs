
namespace AuthTest.Crypto 
{
    
    
    // https://stackoverflow.com/questions/41986995/implement-rsa-in-net-core
    public class Simple
    {
        
        
        private static void Test()
        {
            Simple.GetRsaKey();
            Simple.GetECDsaKey();
            
            Simple.GetBouncyRsaKey();
            Simple.GetBouncyEcdsaKey();
        }
        
        
        public static Microsoft.IdentityModel.Tokens.SecurityKey GetSymmetricKey()
        {
            return new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                // The algorithm: 'HS256' requires the SecurityKey.KeySize to be 
                // greater than '128' bits. KeySize reported: '32'.
                // ==> key-text >= 16 bytes
                // System.Text.Encoding.UTF8.GetBytes("test")
                System.Text.Encoding.UTF8.GetBytes("i am a top secret password") 
            );
        }
        
        
        // Removed
        // public static Microsoft.IdentityModel.Tokens.SecurityKey GetInMemoryKey()
        // {
        //     return new Microsoft.IdentityModel.Tokens.InMemorySymmetricSecurityKey
        //         (System.Text.Encoding.UTF8.GetBytes("sec"));
        // }
        
        
        public static Microsoft.IdentityModel.Tokens.SecurityKey GetBouncyRsaKey()
        {
            BouncyRsa rsa = new BouncyRsa();
            return new Microsoft.IdentityModel.Tokens.RsaSecurityKey( rsa );
        }
        
        
        public static Microsoft.IdentityModel.Tokens.SecurityKey GetBouncyEcdsaKey()
        {
            BouncyECDSA ecdsa = new BouncyECDSA();
            return new Microsoft.IdentityModel.Tokens.ECDsaSecurityKey( ecdsa );
        }


        public static Microsoft.IdentityModel.Tokens.SecurityKey GetMsRsaKey(
            System.Security.Cryptography.RSAParameters rsaParameters)
        {
            System.Security.Cryptography.RSA rsa = System.Security.Cryptography.RSA.Create();
            rsa.ImportParameters(rsaParameters);

            // rsa.KeySize = desiredKeySizeInBits;
            // rsa.ToXmlString(true);
            // rsa.FromXmlString("");
            
            return new Microsoft.IdentityModel.Tokens.RsaSecurityKey( rsa );
        } // End Function GetMsRsaKey 
        
        
        public static Microsoft.IdentityModel.Tokens.SecurityKey GetMsRsaKey(
           System.Security.Cryptography.X509Certificates.X509Certificate2 cert
           )
        {
            // RSA rsaPublic = System.Security.Cryptography.X509Certificates.RSACertificateExtensions
            //     .GetRSAPublicKey(cert);

            System.Security.Cryptography.RSA rsa = System.Security.Cryptography
                .X509Certificates.RSACertificateExtensions.GetRSAPrivateKey(cert);
            
            return new Microsoft.IdentityModel.Tokens.RsaSecurityKey( rsa );
        } // End Function GetMsRsaKey 
        
        
        public static Microsoft.IdentityModel.Tokens.SecurityKey GetRsaKey()
        {
            return new Microsoft.IdentityModel.Tokens.RsaSecurityKey( GetMsRsaProvider() );
        } // End Function GetRsaKey 
        
        
        public static Microsoft.IdentityModel.Tokens.SecurityKey GetMsECDsaKey(
            System.Security.Cryptography.ECParameters ecParameters)
        {
            System.Security.Cryptography.ECDsa ecdsa = 
                System.Security.Cryptography.ECDsa.Create();
            
            ecdsa.ImportParameters(ecParameters);
            
            // ecdsa.ToXmlString(true);
            // ecdsa.FromXmlString("");
            // ecdsa.KeySize = desiredKeySizeInBits;
            
            return new Microsoft.IdentityModel.Tokens.ECDsaSecurityKey( ecdsa );
        } // End Function GetMsECDsaKey 
        
        
        public static Microsoft.IdentityModel.Tokens.SecurityKey GetMsECDsaKey(
        System.Security.Cryptography.X509Certificates.X509Certificate2 cert
        )
        {
            // System.Security.Cryptography.ECDsa ecdsaPublic = System.Security.Cryptography
            //    .X509Certificates.ECDsaCertificateExtensions.GetECDsaPublicKey(cert);
            
            System.Security.Cryptography.ECDsa ecdsa = System.Security.Cryptography
                .X509Certificates.ECDsaCertificateExtensions.GetECDsaPrivateKey(cert);
            
            return new Microsoft.IdentityModel.Tokens.ECDsaSecurityKey( ecdsa );
        } // End Function GetMsECDsaKey        
        
        
        public static Microsoft.IdentityModel.Tokens.SecurityKey GetECDsaKey()
        {
            return new Microsoft.IdentityModel.Tokens.ECDsaSecurityKey( GetMsEcdsaProvider() );
        } // End Function GetECDsaKey 
        
        
        public static Microsoft.IdentityModel.Tokens.SecurityKey KeyFromX509(
            System.Security.Cryptography.X509Certificates.X509Certificate2 cert
            )
        {
            return new Microsoft.IdentityModel.Tokens.X509SecurityKey(cert);
        } // End Function KeyFromX509 



        public static System.Security.Cryptography.ECDsa GetMsEcdsaProvider()
        {
            return System.Security.Cryptography.ECDsa.Create();
        }
        
        
        public static System.Security.Cryptography.ECDsa GetMsEcdsaProvider(System.Security.Cryptography.ECParameters pars)
        {
            return System.Security.Cryptography.ECDsa.Create(pars);    
        }
        
        
        public static System.Security.Cryptography.ECDsa GetMsEcdsaProviderFromBouncy()
        {
            string namedCurve = "prime256v1";
            Org.BouncyCastle.Crypto.Generators.ECKeyPairGenerator pGen =
                new Org.BouncyCastle.Crypto.Generators.ECKeyPairGenerator();

            Org.BouncyCastle.Crypto.Parameters.ECKeyGenerationParameters genParam =
                new Org.BouncyCastle.Crypto.Parameters.ECKeyGenerationParameters(
              Org.BouncyCastle.Asn1.X9.X962NamedCurves.GetOid(namedCurve),
              new Org.BouncyCastle.Security.SecureRandom()
            );
            
            pGen.Init(genParam);
            
            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair keyPair = pGen.GenerateKeyPair();
            
            Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters pub =
                (Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters)keyPair.Public;
            
            Org.BouncyCastle.Crypto.Parameters.ECPrivateKeyParameters priv =
                (Org.BouncyCastle.Crypto.Parameters.ECPrivateKeyParameters)keyPair.Private;


            System.Security.Cryptography.ECParameters pars =
                new System.Security.Cryptography.ECParameters();
            
            // string str = priv.Parameters.Curve.ToString();
            // System.Console.WriteLine(str);
            
            pars.Curve = System.Security.Cryptography.ECCurve.CreateFromFriendlyName("secp256r1");
            
            pars.D = priv.D.ToByteArray();
            pars.Q = new System.Security.Cryptography.ECPoint();
            pars.Q.X = pub.Q.X.GetEncoded();
            pars.Q.Y = pub.Q.Y.GetEncoded();
            
            return System.Security.Cryptography.ECDsa.Create(pars);
            
            
            // The CngKey can be created by importing the key using the Der encoded bytes:
            Org.BouncyCastle.Asn1.Pkcs.PrivateKeyInfo bcKeyInfo =
                Org.BouncyCastle.Pkcs.PrivateKeyInfoFactory
                    .CreatePrivateKeyInfo(keyPair.Private)
            ;
            
            
            /*
            // Require assembly System.Security.Cryptography.Cng
            byte[] pkcs8Blob = bcKeyInfo.GetDerEncoded();
            
            System.Security.Cryptography.CngKey importedKey =
                System.Security.Cryptography.CngKey.Import(pkcs8Blob,
                System.Security.Cryptography.CngKeyBlobFormat.Pkcs8PrivateBlob
            );
            
            //return new System.Security.Cryptography.ECDsaCng(importedKey);
            */
            
        } // End Function GetMsEcdsaProvider 
        
        
        //////////////////////////////////////////////////////
        
        
        private static System.Security.Cryptography.RSA CreateRSAProvider(
            System.Security.Cryptography.RSAParameters rp)
        {
            /*
            System.Security.Cryptography.CspParameters csp = 
                new System.Security.Cryptography.CspParameters();
            
            csp.KeyContainerName = string.Format("BouncyCastle-{0}", System.Guid.NewGuid());
            
            System.Security.Cryptography.RSACryptoServiceProvider rsaCsp = 
                new System.Security.Cryptography.RSACryptoServiceProvider(csp);
            
            rsaCsp.ImportParameters(rp);
            */
            
            System.Security.Cryptography.RSA rsa = System.Security.Cryptography.RSA.Create(rp);
            
            return rsa;
        } // End Function CreateRSAProvider 
        
        
        // TODO Move functionality to more general class
        private static byte[] ConvertRSAParametersField(
            Org.BouncyCastle.Math.BigInteger n, int size)
        {
            byte[] bs = n.ToByteArrayUnsigned();

            if (bs.Length == size)
                return bs;

            if (bs.Length > size)
                throw new System.ArgumentException("Specified size too small", "size");

            byte[] padded = new byte[size];
            System.Array.Copy(bs, 0, padded, size - bs.Length, bs.Length);
            return padded;
        } // End Function ConvertRSAParametersField 
        
        
        private static System.Security.Cryptography.RSAParameters ToRSAParameters(
            Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters rsaKey)
        {
            System.Security.Cryptography.RSAParameters rp = 
                new System.Security.Cryptography.RSAParameters();
            
            rp.Modulus = rsaKey.Modulus.ToByteArrayUnsigned();
            
            if (rsaKey.IsPrivate)
                rp.D = ConvertRSAParametersField(rsaKey.Exponent, rp.Modulus.Length);
            else
                rp.Exponent = rsaKey.Exponent.ToByteArrayUnsigned();
            
            return rp;
        } // End Function ToRSAParameters 
        
        
        private static System.Security.Cryptography.RSAParameters ToRSAParameters(
            Org.BouncyCastle.Crypto.Parameters.RsaPrivateCrtKeyParameters privKey)
        {
            System.Security.Cryptography.RSAParameters rp = 
                new System.Security.Cryptography.RSAParameters();
            
            rp.Modulus = privKey.Modulus.ToByteArrayUnsigned();
            rp.Exponent = privKey.PublicExponent.ToByteArrayUnsigned();
            
            rp.P = privKey.P.ToByteArrayUnsigned();
            rp.Q = privKey.Q.ToByteArrayUnsigned();
            rp.D = ConvertRSAParametersField(privKey.Exponent, rp.Modulus.Length);
            rp.DP = ConvertRSAParametersField(privKey.DP, rp.P.Length);
            rp.DQ = ConvertRSAParametersField(privKey.DQ, rp.Q.Length);
            rp.InverseQ = ConvertRSAParametersField(privKey.QInv, rp.Q.Length);
            
            return rp;
        } // End Function ToRSAParameters 
        
        
        // GenerateRsaKeyPair(1024)
        public static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair GenerateRsaKeyPair(
            int strength)
        {
            Org.BouncyCastle.Crypto.Generators.RsaKeyPairGenerator gen = 
                new Org.BouncyCastle.Crypto.Generators.RsaKeyPairGenerator();
            
            // new Org.BouncyCastle.Crypto.Parameters.RsaKeyGenerationParameters()
            
            Org.BouncyCastle.Security.SecureRandom secureRandom =
                new Org.BouncyCastle.Security.SecureRandom(
                    new Org.BouncyCastle.Crypto.Prng.CryptoApiRandomGenerator()
            );
            
            Org.BouncyCastle.Crypto.KeyGenerationParameters keyGenParam =
                new Org.BouncyCastle.Crypto.KeyGenerationParameters(secureRandom, strength);
            
            
            gen.Init(keyGenParam);
            
            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair kp = gen.GenerateKeyPair();
            // Org.BouncyCastle.Crypto.AsymmetricKeyParameter priv = (Org.BouncyCastle.Crypto.AsymmetricKeyParameter)kp.Private;
            return kp;
        } // End Sub GenerateRsaKeyPair 
        
        
        public static System.Security.Cryptography.RSA GetMsRsaProvider()
        {
            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair m_keyPair = 
                GenerateRsaKeyPair(2048);
            // Org.BouncyCastle.Crypto.AsymmetricKeyParameter m_keyParameter = m_keyPair.Public;
            
            // Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters pub = (Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters)m_keyPair.Public;
            Org.BouncyCastle.Crypto.Parameters.RsaPrivateCrtKeyParameters priv = 
                (Org.BouncyCastle.Crypto.Parameters.RsaPrivateCrtKeyParameters)
                m_keyPair.Private;
            
            System.Security.Cryptography.RSAParameters dnPriv = ToRSAParameters(priv);
            // System.Security.Cryptography.RSAParameters dnPub = ToRSAParameters(pub);
            
            return CreateRSAProvider(dnPriv);
        } // End Function GetMsRsaProvider 
        
        
    } // End Class 
    
    
}
