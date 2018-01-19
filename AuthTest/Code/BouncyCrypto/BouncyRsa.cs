
using System.Security.Cryptography;

namespace AuthTest.Crypto 
{


    public class BouncyRsa : System.Security.Cryptography.RSA // abstract class RSA : AsymmetricAlgorithm
    {
        // protected RSA();

        private Org.BouncyCastle.Crypto.AsymmetricKeyParameter m_keyParameter;
        private Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair m_keyPair;
        
        
        // GenerateRsaKeyPair(1024)
        public static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair GenerateRsaKeyPair(int strength)
        {
            Org.BouncyCastle.Crypto.Generators.RsaKeyPairGenerator gen =
                new Org.BouncyCastle.Crypto.Generators.RsaKeyPairGenerator();

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
        
        
        
        public BouncyRsa() : base()
        {
            int keySize = 2048;
            m_keyPair = GenerateRsaKeyPair(keySize);
            m_keyParameter = m_keyPair.Public;
            this.KeySizeValue = keySize;
        }
        
        
        public BouncyRsa(string privateKey):base()
        {

            using (System.IO.StringReader txtreader = new System.IO.StringReader(privateKey))
            {
                m_keyPair =
                    (Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair) new Org.BouncyCastle.OpenSsl.PemReader(txtreader)
                        .ReadObject();
            }

            m_keyParameter = m_keyPair.Public;
        }
        
        
        public BouncyRsa(string publicKey, bool b):base()
        {
            using (System.IO.StringReader keyReader = new System.IO.StringReader(publicKey))
            {
                m_keyParameter =
                    (Org.BouncyCastle.Crypto.AsymmetricKeyParameter) new Org.BouncyCastle.OpenSsl.PemReader(keyReader)
                        .ReadObject();
            }
        }
        
        
        // public static RSA Create(string algName);
        // public static RSA Create(RSAParameters parameters);
        // public static RSA Create(int keySizeInBits);
        // public static RSA Create();

        // Implemented by RSA
        // public override string KeyExchangeAlgorithm { get; }
        // public override string SignatureAlgorithm { get; }

        // Not required
        // public override void FromXmlString(string xmlString);
        // public override string ToXmlString(bool includePrivateParameters);


        // abstract
        public override void ImportParameters(System.Security.Cryptography.RSAParameters parameters)
        {
            throw new System.NotImplementedException("ImportParameters");
        }

        // abstract 
        public override System.Security.Cryptography.RSAParameters ExportParameters(bool includePrivateParameters)
        {
            throw new System.NotImplementedException("ExportParameters");
        }

        // public byte[] SignData(byte[] data, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding);
        // public bool VerifyData(byte[] data, byte[] signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding);
        // public bool VerifyData(Stream data, byte[] signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding);



        public byte[] Encrypt(byte[] bytesToEncrypt, bool asOaep)
        {
            if (bytesToEncrypt == null)
                throw new System.ArgumentNullException(nameof(bytesToEncrypt));
            
            Org.BouncyCastle.Crypto.IAsymmetricBlockCipher encryptEngine = null;
            
            if (asOaep)
            {
                //Org.BouncyCastle.Crypto.Encodings.OaepEncoding encryptEngine =
                encryptEngine = new Org.BouncyCastle.Crypto.Encodings.OaepEncoding(new Org.BouncyCastle.Crypto.Engines.RsaEngine());
            }
            else
            {
                // Org.BouncyCastle.Crypto.Encodings.Pkcs1Encoding encryptEngine =
                encryptEngine = new Org.BouncyCastle.Crypto.Encodings.Pkcs1Encoding(new Org.BouncyCastle.Crypto.Engines.RsaEngine());
            }
            
            encryptEngine.Init(true, this.m_keyParameter);
            return encryptEngine.ProcessBlock(bytesToEncrypt, 0, bytesToEncrypt.Length);
        }
        
        
        public override byte[] Encrypt(byte[] data, System.Security.Cryptography.RSAEncryptionPadding padding)
        {
            if (data == null)
                throw new System.ArgumentNullException(nameof(data));
            
            if (padding == null)
                throw new System.ArgumentNullException(nameof(padding));
            
            if (padding == System.Security.Cryptography.RSAEncryptionPadding.Pkcs1)
                return this.Encrypt(data, false);
            
            if (padding == System.Security.Cryptography.RSAEncryptionPadding.OaepSHA1)
                return this.Encrypt(data, true);

            // throw RSACryptoServiceProvider.PaddingModeNotSupported();
            throw new System.Security.Cryptography.CryptographicException($"Unknown padding mode \"{padding}\".");
        }


        // Ignored
        //public override byte[] EncryptValue(byte[] bytesToDecrypt)
        //{
        //    return null;
        //}
        
        
        public byte[] Decrypt(byte[] bytesToDecrypt, bool asOaep)
        {
            if (bytesToDecrypt == null)
                throw new System.ArgumentNullException(nameof(bytesToDecrypt));

            if (bytesToDecrypt.Length > this.KeySize / 8)
                throw new System.Security.Cryptography.CryptographicException(
                    $"Padding: data too big - key size in bytes: \"{System.Convert.ToString(this.KeySize / 8)}\".");
            
            Org.BouncyCastle.Crypto.IAsymmetricBlockCipher decryptionEngine = null;
            
            if (asOaep)
            {
                //Org.BouncyCastle.Crypto.Encodings.OaepEncoding decryptionEngine =
                decryptionEngine = new Org.BouncyCastle.Crypto.Encodings.OaepEncoding(new Org.BouncyCastle.Crypto.Engines.RsaEngine());
            }
            else
            {
                // Org.BouncyCastle.Crypto.Encodings.Pkcs1Encoding decryptionEngine =
                decryptionEngine = new Org.BouncyCastle.Crypto.Encodings.Pkcs1Encoding(new Org.BouncyCastle.Crypto.Engines.RsaEngine());
            }
            
            decryptionEngine.Init(false, m_keyParameter);
            return decryptionEngine.ProcessBlock(bytesToDecrypt, 0, bytesToDecrypt.Length);
        }
        
        
        public override byte[] Decrypt(byte[] data, System.Security.Cryptography.RSAEncryptionPadding padding)
        {
            if (data == null)
                throw new System.ArgumentNullException(nameof(data));
            
            if (padding == null)
                throw new System.ArgumentNullException(nameof(padding));
            
            if (padding == System.Security.Cryptography.RSAEncryptionPadding.Pkcs1)
                return this.Decrypt(data, false);
            
            if (padding == System.Security.Cryptography.RSAEncryptionPadding.OaepSHA1)
                return this.Decrypt(data, true);
            
            // throw RSACryptoServiceProvider.PaddingModeNotSupported();
            throw new System.Security.Cryptography.CryptographicException($"Unknown padding mode \"{padding}\".");
        }


        // Ignored
        //public override byte[] DecryptValue(byte[] rgb)
        //{
        //    System.Security.Cryptography.RSACryptoServiceProvider rs;
        //    return null;
        //}


        // call this.SignHash in effect
        // public virtual byte[] SignData(byte[] data, int offset, int count, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding);
        // public virtual byte[] SignData(Stream data, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding);
        // public virtual byte[] SignHash(byte[] hash, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding);

        // Call this.VerifyData in effect
        // public virtual bool VerifyData(byte[] data, int offset, int count, byte[] signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding);
        // public virtual bool VerifyHash(byte[] hash, byte[] signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding);


        private static System.Security.Cryptography.HashAlgorithm GetHashAlgorithm(
            System.Security.Cryptography.HashAlgorithmName hashAlgorithmName)
        {
            if (hashAlgorithmName == System.Security.Cryptography.HashAlgorithmName.MD5)
                return (System.Security.Cryptography.HashAlgorithm) System.Security.Cryptography.MD5.Create();
            if (hashAlgorithmName == System.Security.Cryptography.HashAlgorithmName.SHA1)
                return (System.Security.Cryptography.HashAlgorithm) System.Security.Cryptography.SHA1.Create();
            if (hashAlgorithmName == System.Security.Cryptography.HashAlgorithmName.SHA256)
                return (System.Security.Cryptography.HashAlgorithm) System.Security.Cryptography.SHA256.Create();
            if (hashAlgorithmName == System.Security.Cryptography.HashAlgorithmName.SHA384)
                return (System.Security.Cryptography.HashAlgorithm) System.Security.Cryptography.SHA384.Create();
            if (hashAlgorithmName == System.Security.Cryptography.HashAlgorithmName.SHA512)
                return (System.Security.Cryptography.HashAlgorithm) System.Security.Cryptography.SHA512.Create();
            
            throw new System.Security.Cryptography.CryptographicException(
                $"Unknown hash algorithm \"{hashAlgorithmName.Name}\"."
            );
        }
        
        
        protected override byte[] HashData(byte[] data, int offset, int count,
            System.Security.Cryptography.HashAlgorithmName hashAlgorithm)
        {
            using (System.Security.Cryptography.HashAlgorithm hashAlgorithm1 =
                GetHashAlgorithm(hashAlgorithm))
                return hashAlgorithm1.ComputeHash(data);
        }
        
        
        protected override byte[] HashData(System.IO.Stream data,
            System.Security.Cryptography.HashAlgorithmName hashAlgorithm)
        {
            using (System.Security.Cryptography.HashAlgorithm hashAlgorithm1 =
                GetHashAlgorithm(hashAlgorithm))
                return hashAlgorithm1.ComputeHash(data);
        }
        
        
    }
    
    
}
