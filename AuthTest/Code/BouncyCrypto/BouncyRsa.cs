
using System.Collections.Generic;


namespace AuthTest.Cryptography
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
                new Org.BouncyCastle.Security.SecureRandom(
                    new Org.BouncyCastle.Crypto.Prng.CryptoApiRandomGenerator()
            );

            Org.BouncyCastle.Crypto.KeyGenerationParameters keyGenParam =
                new Org.BouncyCastle.Crypto.KeyGenerationParameters(secureRandom, strength);


            gen.Init(keyGenParam);

            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair kp = gen.GenerateKeyPair();
            return kp;
        } // End Sub GenerateRsaKeyPair 
        
        
        
        public BouncyRsa() : base()
        {
            int keySize = 2048;
            m_keyPair = GenerateRsaKeyPair(keySize);
            //m_keyParameter = m_keyPair.Public;
            m_keyParameter = m_keyPair.Private;
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

            m_keyParameter = m_keyPair.Private;
            // var x = (Org.BouncyCastle.Crypto.Parameters.RsaPrivateCrtKeyParameters)m_keyPair.Public;
            // var x = (Org.BouncyCastle.Crypto.Parameters.RsaKeyGenerationParameters)m_keyPair.Public;
            var x = (Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters)this.m_keyParameter;
            this.KeySizeValue = x.Modulus.BitLength;
        }


        public BouncyRsa(string publicKey, bool b):base()
        {
            using (System.IO.StringReader keyReader = new System.IO.StringReader(publicKey))
            {
                m_keyParameter =
                    (Org.BouncyCastle.Crypto.AsymmetricKeyParameter) new Org.BouncyCastle.OpenSsl.PemReader(keyReader)
                        .ReadObject();
            }

            var x = (Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters)m_keyPair.Public;
            this.KeySizeValue = x.Modulus.BitLength;
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


        private byte[] EncryptInternal(byte[] bytesToEncrypt, bool asOaep)
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
        } // End Function EncryptInternal 


        public override byte[] Encrypt(byte[] data, System.Security.Cryptography.RSAEncryptionPadding padding)
        {
            if (data == null)
                throw new System.ArgumentNullException(nameof(data));
            
            if (padding == null)
                throw new System.ArgumentNullException(nameof(padding));
            
            if (padding == System.Security.Cryptography.RSAEncryptionPadding.Pkcs1)
                return this.EncryptInternal(data, false);
            
            if (padding == System.Security.Cryptography.RSAEncryptionPadding.OaepSHA1)
                return this.EncryptInternal(data, true);

            // throw RSACryptoServiceProvider.PaddingModeNotSupported();
            throw new System.Security.Cryptography.CryptographicException($"Unknown padding mode \"{padding}\".");
        } // End Function Encrypt 


        // Ignored
        //public override byte[] EncryptValue(byte[] bytesToDecrypt)
        //{
        //    return null;
        //}


        private byte[] DecryptInternal(byte[] bytesToDecrypt, bool asOaep)
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
        } // End Function DecryptInternal 


        public override byte[] Decrypt(byte[] data, System.Security.Cryptography.RSAEncryptionPadding padding)
        {
            if (data == null)
                throw new System.ArgumentNullException(nameof(data));
            
            if (padding == null)
                throw new System.ArgumentNullException(nameof(padding));
            
            if (padding == System.Security.Cryptography.RSAEncryptionPadding.Pkcs1)
                return this.DecryptInternal(data, false);
            
            if (padding == System.Security.Cryptography.RSAEncryptionPadding.OaepSHA1)
                return this.DecryptInternal(data, true);
            
            // throw RSACryptoServiceProvider.PaddingModeNotSupported();
            throw new System.Security.Cryptography.CryptographicException($"Unknown padding mode \"{padding}\".");
        } // End Function Decrypt 


        // Ignored
        //public override byte[] DecryptValue(byte[] rgb)
        //{
        //    System.Security.Cryptography.RSACryptoServiceProvider rs;
        //    return null;
        //}


        // call this.SignHash in effect
        // public virtual byte[] SignData(byte[] data, int offset, int count, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding);
        // public virtual byte[] SignData(Stream data, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding);
        // public byte[] SignData(byte[] data, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding);
        


        public override byte[] SignHash(byte[] hash
            , System.Security.Cryptography.HashAlgorithmName hashAlgorithm
            , System.Security.Cryptography.RSASignaturePadding padding)
        {
            if (hash == null)
                throw new System.ArgumentNullException(nameof(hash));

            if (hashAlgorithm == null)
                throw new System.ArgumentNullException(nameof(hashAlgorithm));

            if (padding == null)
                throw new System.ArgumentNullException(nameof(padding));

            Org.BouncyCastle.Crypto.IDigest digest = GetBouncyAlgorithm(hashAlgorithm);

            if (padding == System.Security.Cryptography.RSASignaturePadding.Pkcs1)
                return SignHashInternal(hash, digest, false);

            if (padding == System.Security.Cryptography.RSASignaturePadding.Pss)
                return SignHashInternal(hash, digest, true);

            throw new System.Security.Cryptography.CryptographicException($"Unknown padding mode \"{padding}\".");
        } // End Function SignHash 


        // Signing requires private key
        private byte[] SignHashInternal(
             byte[] hash 
            ,Org.BouncyCastle.Crypto.IDigest digest 
            ,bool asOaep
        )
        {
            byte[] derEncoded = this.DerEncode(hash, digest);

            Org.BouncyCastle.Crypto.IAsymmetricBlockCipher rsaEngine = null;

            if (asOaep) // PSS:
            {
                rsaEngine =
                    new Org.BouncyCastle.Crypto.Encodings.OaepEncoding(
                        new Org.BouncyCastle.Crypto.Engines.RsaBlindedEngine()
                );
            }
            else // Pkcs1 
            {
                rsaEngine =
                new Org.BouncyCastle.Crypto.Encodings.Pkcs1Encoding(
                    new Org.BouncyCastle.Crypto.Engines.RsaBlindedEngine()
                );
            }

            rsaEngine.Init(true, this.m_keyPair.Private);
            
            byte[] encoded = rsaEngine.ProcessBlock(derEncoded, 0, derEncoded.Length);
            return encoded;
        } // End Function SignHashInternal


        private byte[] DerEncode(byte[] hash,
            Org.BouncyCastle.Crypto.IDigest digest
            )
        {
            System.Collections.Generic.Dictionary<string, Org.BouncyCastle.Asn1.DerObjectIdentifier> oidMap = null;
            oidMap = new Dictionary<string, Org.BouncyCastle.Asn1.DerObjectIdentifier>(System.StringComparer.OrdinalIgnoreCase);

            oidMap["RIPEMD128"] = Org.BouncyCastle.Asn1.TeleTrust.TeleTrusTObjectIdentifiers.RipeMD128;
            oidMap["RIPEMD160"] = Org.BouncyCastle.Asn1.TeleTrust.TeleTrusTObjectIdentifiers.RipeMD160;
            oidMap["RIPEMD256"] = Org.BouncyCastle.Asn1.TeleTrust.TeleTrusTObjectIdentifiers.RipeMD256;

            oidMap["SHA-1"] = Org.BouncyCastle.Asn1.X509.X509ObjectIdentifiers.IdSha1;
            oidMap["SHA-224"] = Org.BouncyCastle.Asn1.Nist.NistObjectIdentifiers.IdSha224;
            oidMap["SHA-256"] = Org.BouncyCastle.Asn1.Nist.NistObjectIdentifiers.IdSha256;
            oidMap["SHA-384"] = Org.BouncyCastle.Asn1.Nist.NistObjectIdentifiers.IdSha384;
            oidMap["SHA-512"] = Org.BouncyCastle.Asn1.Nist.NistObjectIdentifiers.IdSha512;

            oidMap["MD2"] = Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.MD2;
            oidMap["MD4"] = Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.MD4;
            oidMap["MD5"] = Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.MD5;

            Org.BouncyCastle.Asn1.DerObjectIdentifier digestOid = oidMap[digest.AlgorithmName];
            Org.BouncyCastle.Asn1.X509.AlgorithmIdentifier algid =
                new Org.BouncyCastle.Asn1.X509.AlgorithmIdentifier(
                    digestOid, Org.BouncyCastle.Asn1.DerNull.Instance
            );

            Org.BouncyCastle.Asn1.X509.DigestInfo di =
                new Org.BouncyCastle.Asn1.X509.DigestInfo(algid, hash);

            return di.GetDerEncoded();
        } // End Function DerEncode 


        // Call this.VerifyData in effect
        // public virtual bool VerifyData(byte[] data, int offset, int count, byte[] signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding);


        private static Org.BouncyCastle.Crypto.IDigest GetBouncyAlgorithm(
           System.Security.Cryptography.HashAlgorithmName hashAlgorithmName)
        {
            if (hashAlgorithmName == System.Security.Cryptography.HashAlgorithmName.MD5)
                return new Org.BouncyCastle.Crypto.Digests.MD5Digest();
            if (hashAlgorithmName == System.Security.Cryptography.HashAlgorithmName.SHA1)
                return new Org.BouncyCastle.Crypto.Digests.Sha1Digest();
            if (hashAlgorithmName == System.Security.Cryptography.HashAlgorithmName.SHA256)
                return new Org.BouncyCastle.Crypto.Digests.Sha256Digest();
            if (hashAlgorithmName == System.Security.Cryptography.HashAlgorithmName.SHA384)
                return new Org.BouncyCastle.Crypto.Digests.Sha384Digest();
            if (hashAlgorithmName == System.Security.Cryptography.HashAlgorithmName.SHA512)
                return new Org.BouncyCastle.Crypto.Digests.Sha512Digest();
            
            throw new System.Security.Cryptography.CryptographicException(
                $"Unknown hash algorithm \"{hashAlgorithmName.Name}\"."
            );
        } // End Function GetBouncyAlgorithm  
        

        // Call VerifyHash internally
        // public bool VerifyData(byte[] data, byte[] signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding);
        // public bool VerifyData(Stream data, byte[] signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding);

        public override bool VerifyHash(byte[] hash
        , byte[] signature
        , System.Security.Cryptography.HashAlgorithmName hashAlgorithm
        , System.Security.Cryptography.RSASignaturePadding padding)
        {
            if (hash == null)
                throw new System.ArgumentNullException(nameof(hash));

            if (signature == null)
                throw new System.ArgumentNullException(nameof(signature));

            if (hashAlgorithm == null)
                throw new System.ArgumentNullException(nameof(hashAlgorithm));

            if (padding == null)
                throw new System.ArgumentNullException(nameof(padding));

            Org.BouncyCastle.Crypto.IDigest digest = GetBouncyAlgorithm(hashAlgorithm);

            if (padding == System.Security.Cryptography.RSASignaturePadding.Pkcs1)
                return VerifyHashInternal(hash, signature, digest, false);

            if (padding == System.Security.Cryptography.RSASignaturePadding.Pss)
                return VerifyHashInternal(hash, signature, digest, true);

            throw new System.Security.Cryptography.CryptographicException($"Unknown padding mode \"{padding}\".");
        } // End Function VerifyHash 


        private bool VerifyHashInternal(
              byte[] hash
            , byte[] signature 
            , Org.BouncyCastle.Crypto.IDigest digest
            , bool asOaep)
        {
            Org.BouncyCastle.Crypto.IAsymmetricBlockCipher rsaEngine = null;

            if (asOaep) // PSS:
            {
                rsaEngine =
                    new Org.BouncyCastle.Crypto.Encodings.OaepEncoding(
                        new Org.BouncyCastle.Crypto.Engines.RsaBlindedEngine()
                );
            }
            else // Pkcs1 
            {
                rsaEngine =
                new Org.BouncyCastle.Crypto.Encodings.Pkcs1Encoding(
                    new Org.BouncyCastle.Crypto.Engines.RsaBlindedEngine()
                );
            }
            rsaEngine.Init(false, this.m_keyPair.Public);
            byte[] a = null;
            byte[] b = null;

            try
            {
                a = rsaEngine.ProcessBlock(signature, 0, signature.Length);
                b = this.DerEncode(hash, digest);
            }
            catch 
            {
                return false;
            }

            if (a.Length == b.Length)
                return Org.BouncyCastle.Utilities.Arrays.ConstantTimeAreEqual(a, b);
            if (a.Length != b.Length - 2)
                return false;

            int num1 = a.Length - hash.Length - 2;
            int num2 = b.Length - hash.Length - 2;

            b[1] -= (byte)2;
            b[3] -= (byte)2;

            int num3 = 0;
            for (int index = 0; index < hash.Length; ++index)
                num3 |= (int)a[num1 + index] ^ (int)b[num2 + index];

            for (int index = 0; index < num1; ++index)
                num3 |= (int)a[index] ^ (int)b[index];

            return num3 == 0;
        } // End Function VerifyHashInternal  


        private static System.Security.Cryptography.HashAlgorithm GetHashAlgorithm(
            System.Security.Cryptography.HashAlgorithmName hashAlgorithmName)
        {
            if (hashAlgorithmName == System.Security.Cryptography.HashAlgorithmName.MD5)
                return  System.Security.Cryptography.MD5.Create();
            if (hashAlgorithmName == System.Security.Cryptography.HashAlgorithmName.SHA1)
                return  System.Security.Cryptography.SHA1.Create();
            if (hashAlgorithmName == System.Security.Cryptography.HashAlgorithmName.SHA256)
                return  System.Security.Cryptography.SHA256.Create();
            if (hashAlgorithmName == System.Security.Cryptography.HashAlgorithmName.SHA384)
                return  System.Security.Cryptography.SHA384.Create();
            if (hashAlgorithmName == System.Security.Cryptography.HashAlgorithmName.SHA512)
                return  System.Security.Cryptography.SHA512.Create();
            
            throw new System.Security.Cryptography.CryptographicException(
                $"Unknown hash algorithm \"{hashAlgorithmName.Name}\"."
            );
        } // End Function GetHashAlgorithm 


        protected override byte[] HashData(byte[] data, int offset, int count,
            System.Security.Cryptography.HashAlgorithmName hashAlgorithm)
        {
            Org.BouncyCastle.Crypto.IDigest digest = GetBouncyAlgorithm(hashAlgorithm);
            
            byte[] retValue = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(data, offset, count);
            digest.DoFinal(retValue, 0);
            return retValue;
            
            // using (System.Security.Cryptography.HashAlgorithm hashAlgorithm1 =
            //     GetHashAlgorithm(hashAlgorithm))
            //     return hashAlgorithm1.ComputeHash(data, offset, count);
        } // End Function HashData 


        protected override byte[] HashData(System.IO.Stream data,
            System.Security.Cryptography.HashAlgorithmName hashAlgorithm)
        {
            Org.BouncyCastle.Crypto.IDigest digest = GetBouncyAlgorithm(hashAlgorithm);
            
            byte[] buffer = new byte[4096];
            int cbSize;
            while ((cbSize = data.Read(buffer, 0, buffer.Length)) > 0)
                digest.BlockUpdate(buffer, 0, cbSize);
            
            byte[] hash = new byte[digest.GetDigestSize()];
            digest.DoFinal(hash, 0);
            return hash;
            
            // using (System.Security.Cryptography.HashAlgorithm hashAlgorithm1 =
            //     GetHashAlgorithm(hashAlgorithm))
            //     return hashAlgorithm1.ComputeHash(data);
        } // End Function HashData 
        
        
    }
    
    
}
