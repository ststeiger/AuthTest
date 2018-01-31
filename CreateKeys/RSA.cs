using System;
using System.Collections.Generic;
using System.Text;

namespace CreateKeys
{
    class RSA
    {

        // GenerateRsaKeyPair(1024)
        public static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair GenerateKeyPair(int strength)
        {
            Org.BouncyCastle.Crypto.Generators.RsaKeyPairGenerator gen = new Org.BouncyCastle.Crypto.Generators.RsaKeyPairGenerator();

            // new Org.BouncyCastle.Crypto.Parameters.RsaKeyGenerationParameters()

            Org.BouncyCastle.Security.SecureRandom secureRandom = CoreCMS.JWT.NonBackdooredPrng.SecureRandom;

            Org.BouncyCastle.Crypto.KeyGenerationParameters keyGenParam =
                new Org.BouncyCastle.Crypto.KeyGenerationParameters(secureRandom, strength);

            gen.Init(keyGenParam);

            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair kp = gen.GenerateKeyPair();
            return kp;
            // Org.BouncyCastle.Crypto.AsymmetricKeyParameter priv = (Org.BouncyCastle.Crypto.AsymmetricKeyParameter)kp.Private;
        } // End Sub GenerateRsaKeyPair 


    }
}
