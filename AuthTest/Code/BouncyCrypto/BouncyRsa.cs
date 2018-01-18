
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;


namespace AuthTest.Code.BouncyCrypto
{


    public class BouncyRsa : System.Security.Cryptography.RSA
    {


        private Org.BouncyCastle.Crypto.AsymmetricKeyParameter m_keyParameter;
        private Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair m_keyPair;


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
            return kp;
            // Org.BouncyCastle.Crypto.AsymmetricKeyParameter priv = (Org.BouncyCastle.Crypto.AsymmetricKeyParameter)kp.Private;
        } // End Sub GenerateRsaKeyPair 



        public BouncyRsa(string publicKey, bool b)
        {
            using (System.IO.StringReader keyReader = new System.IO.StringReader(publicKey))
            {
                m_keyParameter = (Org.BouncyCastle.Crypto.AsymmetricKeyParameter)new Org.BouncyCastle.OpenSsl.PemReader(keyReader).ReadObject();
            }
        }


        public BouncyRsa()
        {
            m_keyPair = GenerateRsaKeyPair(2048);
            m_keyParameter = m_keyPair.Public;
        }


        public BouncyRsa(string privateKey)
        {

            using (System.IO.StringReader txtreader = new System.IO.StringReader(privateKey))
            {
                m_keyPair = (Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair)new Org.BouncyCastle.OpenSsl.PemReader(txtreader).ReadObject();
            }

            m_keyParameter = m_keyPair.Public;
        }


        // Abstract
        public override RSAParameters ExportParameters(bool includePrivateParameters)
        {
            throw new NotImplementedException();
        }

        // Abstract 
        public override void ImportParameters(RSAParameters parameters)
        {
            throw new NotImplementedException();
        }
    }


    /*
        public abstract class RSA : AsymmetricAlgorithm
        {
            protected RSA();

            public static RSA Create(string algName);
            public static RSA Create(RSAParameters parameters);
            public static RSA Create(int keySizeInBits);
            public static RSA Create();

            public override string KeyExchangeAlgorithm { get; }
            public override string SignatureAlgorithm { get; }
            public override void FromXmlString(string xmlString);
            public override string ToXmlString(bool includePrivateParameters);

            public abstract void ImportParameters(RSAParameters parameters);
            public abstract RSAParameters ExportParameters(bool includePrivateParameters);

            // public byte[] SignData(byte[] data, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding);
            // public bool VerifyData(byte[] data, byte[] signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding);
            // public bool VerifyData(Stream data, byte[] signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding);

            public virtual byte[] Decrypt(byte[] data, RSAEncryptionPadding padding);
            public virtual byte[] DecryptValue(byte[] rgb);
            public virtual byte[] Encrypt(byte[] data, RSAEncryptionPadding padding);
            public virtual byte[] EncryptValue(byte[] rgb);
            public virtual byte[] SignData(byte[] data, int offset, int count, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding);
            public virtual byte[] SignData(Stream data, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding);
            public virtual byte[] SignHash(byte[] hash, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding);

            public virtual bool VerifyData(byte[] data, int offset, int count, byte[] signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding);
            public virtual bool VerifyHash(byte[] hash, byte[] signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding);
            protected virtual byte[] HashData(byte[] data, int offset, int count, HashAlgorithmName hashAlgorithm);
            protected virtual byte[] HashData(Stream data, HashAlgorithmName hashAlgorithm);
        }
        */



}
