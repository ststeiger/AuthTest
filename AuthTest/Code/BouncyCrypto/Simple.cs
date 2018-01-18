
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace AuthTest.Code.BouncyCrypto
{

    // https://stackoverflow.com/questions/41986995/implement-rsa-in-net-core
    public class Simple
    {
        public static string RemoveSingleLineComments(string input)
        {
            input = System.Text.RegularExpressions.Regex.Replace(input, "//.*", "");
            input = System.Text.RegularExpressions.Regex.Replace(input, @"[\r\n]+(\s+[\r\n]+)+?", "\r\n");
            System.Console.WriteLine(input);

            return input;
        }
        
        public static Microsoft.IdentityModel.Tokens.SecurityKey GetMsRsaKey(RSAParameters rsaParameters)
        {
            RSA rsa = RSA.Create();
            rsa.ImportParameters(rsaParameters);

            // rsa.KeySize = desiredKeySizeInBits;
            // rsa.ToXmlString(true);
            // rsa.FromXmlString("");

            return new Microsoft.IdentityModel.Tokens.RsaSecurityKey(
                rsa
            );
        }


        public static Microsoft.IdentityModel.Tokens.SecurityKey GetMsRsaKey(
           System.Security.Cryptography.X509Certificates.X509Certificate2 cert
           )
        {
            // RSA rsaPublic = System.Security.Cryptography.X509Certificates.RSACertificateExtensions
            //     .GetRSAPublicKey(cert);

            RSA rsa = System.Security.Cryptography.X509Certificates.RSACertificateExtensions
                .GetRSAPrivateKey(cert);

            return new Microsoft.IdentityModel.Tokens.RsaSecurityKey(
                rsa
            );
        }


        public static Microsoft.IdentityModel.Tokens.SecurityKey GetRsaKey()
        {
            return new Microsoft.IdentityModel.Tokens.RsaSecurityKey(
                GetMsRsaProvider()
            );
        }



        public static Microsoft.IdentityModel.Tokens.SecurityKey GetMsECDsaKey(ECParameters ecParameters)
        {
            ECDsa ecdsa = ECDsa.Create();
            ecdsa.ImportParameters(ecParameters);


            // ecdsa.ToXmlString(true);
            // ecdsa.FromXmlString("");
            // ecdsa.KeySize = desiredKeySizeInBits;

            return new Microsoft.IdentityModel.Tokens.ECDsaSecurityKey(
                ecdsa
            );
        }


        public static Microsoft.IdentityModel.Tokens.SecurityKey GetMsECDsaKey(
        System.Security.Cryptography.X509Certificates.X509Certificate2 cert
        )
        {
            // ECDsa ecdsaPublic = cert.GetECDsaPublicKey();
            ECDsa ecdsa = cert.GetECDsaPrivateKey();

            return new Microsoft.IdentityModel.Tokens.ECDsaSecurityKey(
                ecdsa
            );
        }
        

        public static Microsoft.IdentityModel.Tokens.SecurityKey GetECDsaKey()
        {
            return new Microsoft.IdentityModel.Tokens.ECDsaSecurityKey(
                       GetMsEcdsaProvider()
            );
        }


        public static Microsoft.IdentityModel.Tokens.SecurityKey KeyFromX509(
            System.Security.Cryptography.X509Certificates.X509Certificate2 cert
            )
        {
            return new Microsoft.IdentityModel.Tokens.X509SecurityKey(cert);
        }


        public static System.Security.Cryptography.ECDsa GetMsEcdsaProvider()
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


            System.Security.Cryptography.ECParameters pars = new System.Security.Cryptography.ECParameters();
            //string str = priv.Parameters.Curve.ToString();
            //System.Console.WriteLine(str);

            //pars.Curve = new ECCurve();


            //pars.D = priv.D.ToByteArray();
            //pars.Q = new System.Security.Cryptography.ECPoint();
            //pars.Q.X = pub.Q.X.GetEncoded();
            //pars.Q.Y = pub.Q.Y.GetEncoded();

            //System.Security.Cryptography.ECDsa.Create(pars);


            // The CngKey can be created by importing the key using the Der encoded bytes:
            Org.BouncyCastle.Asn1.Pkcs.PrivateKeyInfo bcKeyInfo =
                Org.BouncyCastle.Pkcs.PrivateKeyInfoFactory.CreatePrivateKeyInfo(keyPair.Private)
            ;

            // Require assembly System.Security.Cryptography.Cng
            byte[] pkcs8Blob = bcKeyInfo.GetDerEncoded();

            System.Security.Cryptography.CngKey importedKey =
                System.Security.Cryptography.CngKey.Import(pkcs8Blob,
                System.Security.Cryptography.CngKeyBlobFormat.Pkcs8PrivateBlob
            );

            return new System.Security.Cryptography.ECDsaCng(importedKey);
        } // End Function GetMsEcdsaProvider 


        //////////////////////////////////////////////////////



        private static RSA CreateRSAProvider(RSAParameters rp)
        {
            CspParameters csp = new CspParameters();
            csp.KeyContainerName = string.Format("BouncyCastle-{0}", Guid.NewGuid());
            RSACryptoServiceProvider rsaCsp = new RSACryptoServiceProvider(csp);

            rsaCsp.ImportParameters(rp);
            return rsaCsp;
        }


        // TODO Move functionality to more general class
        private static byte[] ConvertRSAParametersField(Org.BouncyCastle.Math.BigInteger n, int size)
        {
            byte[] bs = n.ToByteArrayUnsigned();

            if (bs.Length == size)
                return bs;

            if (bs.Length > size)
                throw new ArgumentException("Specified size too small", "size");

            byte[] padded = new byte[size];
            Array.Copy(bs, 0, padded, size - bs.Length, bs.Length);
            return padded;
        }


        private static RSAParameters ToRSAParameters(Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters rsaKey)
        {
            RSAParameters rp = new RSAParameters();
            rp.Modulus = rsaKey.Modulus.ToByteArrayUnsigned();
            if (rsaKey.IsPrivate)
                rp.D = ConvertRSAParametersField(rsaKey.Exponent, rp.Modulus.Length);
            else
                rp.Exponent = rsaKey.Exponent.ToByteArrayUnsigned();
            return rp;
        }


        private static RSAParameters ToRSAParameters(Org.BouncyCastle.Crypto.Parameters.RsaPrivateCrtKeyParameters privKey)
        {
            RSAParameters rp = new RSAParameters();
            rp.Modulus = privKey.Modulus.ToByteArrayUnsigned();
            rp.Exponent = privKey.PublicExponent.ToByteArrayUnsigned();
            rp.P = privKey.P.ToByteArrayUnsigned();
            rp.Q = privKey.Q.ToByteArrayUnsigned();
            rp.D = ConvertRSAParametersField(privKey.Exponent, rp.Modulus.Length);
            rp.DP = ConvertRSAParametersField(privKey.DP, rp.P.Length);
            rp.DQ = ConvertRSAParametersField(privKey.DQ, rp.Q.Length);
            rp.InverseQ = ConvertRSAParametersField(privKey.QInv, rp.Q.Length);
            return rp;
        }



        // GenerateRsaKeyPair(1024)
        public static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair GenerateRsaKeyPair(int strength)
        {
            Org.BouncyCastle.Crypto.Generators.RsaKeyPairGenerator gen = new Org.BouncyCastle.Crypto.Generators.RsaKeyPairGenerator();

            // new Org.BouncyCastle.Crypto.Parameters.RsaKeyGenerationParameters()

            Org.BouncyCastle.Security.SecureRandom secureRandom =
                new Org.BouncyCastle.Security.SecureRandom(new Org.BouncyCastle.Crypto.Prng.CryptoApiRandomGenerator());

            Org.BouncyCastle.Crypto.KeyGenerationParameters keyGenParam =
                new Org.BouncyCastle.Crypto.KeyGenerationParameters(secureRandom, strength);


            gen.Init(keyGenParam);

            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair kp = gen.GenerateKeyPair();
            // Org.BouncyCastle.Crypto.AsymmetricKeyParameter priv = (Org.BouncyCastle.Crypto.AsymmetricKeyParameter)kp.Private;
            return kp;
        } // End Sub GenerateRsaKeyPair 


        public static RSA GetMsRsaProvider()
        {
            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair m_keyPair = GenerateRsaKeyPair(2048);
            // Org.BouncyCastle.Crypto.AsymmetricKeyParameter m_keyParameter = m_keyPair.Public;

            // Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters pub = (Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters)m_keyPair.Public;
            Org.BouncyCastle.Crypto.Parameters.RsaPrivateCrtKeyParameters priv = (Org.BouncyCastle.Crypto.Parameters.RsaPrivateCrtKeyParameters)m_keyPair.Private;

            var dnPriv = ToRSAParameters(priv);
            //var dnPub = ToRSAParameters(pub);

            return CreateRSAProvider(dnPriv);
        }


    } // End Class 


}
