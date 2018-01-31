
namespace CreateKeys
{


    class ECDSA
    {


        // https://stackoverflow.com/questions/18244630/elliptic-curve-with-digital-signature-algorithm-ecdsa-implementation-on-bouncy
        public static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair GenerateKeyPair(string curveName)
        {
            Org.BouncyCastle.Crypto.Generators.ECKeyPairGenerator gen =
                new Org.BouncyCastle.Crypto.Generators.ECKeyPairGenerator();

            Org.BouncyCastle.Security.SecureRandom secureRandom = CoreCMS.JWT.NonBackdooredPrng.SecureRandom;

            // https://github.com/bcgit/bc-csharp/blob/master/crypto/src/asn1/sec/SECNamedCurves.cs#LC1096
            Org.BouncyCastle.Asn1.X9.X9ECParameters ps =
                Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName(curveName); // not "secp256k1"

            Org.BouncyCastle.Crypto.Parameters.ECDomainParameters ecParams =
                new Org.BouncyCastle.Crypto.Parameters.ECDomainParameters(ps.Curve, ps.G, ps.N, ps.H);

            Org.BouncyCastle.Crypto.Parameters.ECKeyGenerationParameters keyGenParam =
                new Org.BouncyCastle.Crypto.Parameters.ECKeyGenerationParameters(ecParams, secureRandom);

            gen.Init(keyGenParam);
            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair kp = gen.GenerateKeyPair();

            // Org.BouncyCastle.Crypto.Parameters.ECPrivateKeyParameters priv = 
            //     (Org.BouncyCastle.Crypto.Parameters.ECPrivateKeyParameters)kp.Private;

            return kp;
        } // End Function GenerateEcdsaKeyPair 


    }


}
