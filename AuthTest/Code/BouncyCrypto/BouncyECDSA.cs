
namespace AuthTest.Crypto 
{


    public class BouncyECDSA : System.Security.Cryptography.ECDsa // abstract class ECDsa : AsymmetricAlgorithm
    {
        
        Org.BouncyCastle.Crypto.Parameters.ECPrivateKeyParameters m_privKey;
        Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters m_pubKey;

        
        // https://stackoverflow.com/questions/18244630/elliptic-curve-with-digital-signature-algorithm-ecdsa-implementation-on-bouncy
        public static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair GenerateEcdsaKeyPair()
        {
            Org.BouncyCastle.Crypto.Generators.ECKeyPairGenerator gen = 
                new Org.BouncyCastle.Crypto.Generators.ECKeyPairGenerator();

            Org.BouncyCastle.Security.SecureRandom secureRandom = 
                new Org.BouncyCastle.Security.SecureRandom();

            // https://github.com/bcgit/bc-csharp/blob/master/crypto/src/asn1/sec/SECNamedCurves.cs#LC1096
            Org.BouncyCastle.Asn1.X9.X9ECParameters ps = 
                Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp256k1");
            
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

        

        public BouncyECDSA()
            :this(GenerateEcdsaKeyPair())
        { } // End Constructor 
        
        
        public BouncyECDSA(Org.BouncyCastle.Crypto.Parameters.ECPrivateKeyParameters privKey)
            :base()
        {
            this.m_privKey = privKey;
        } // End Constructor 


        public BouncyECDSA(Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters pubKey)
            :base()
        {
            this.m_pubKey = pubKey;
        } // End Constructor 
        
        
        public BouncyECDSA(Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair kp)
            :base()
        {
            this.m_privKey = (Org.BouncyCastle.Crypto.Parameters.ECPrivateKeyParameters) kp.Private;
            this.m_pubKey = (Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters) kp.Public;
            this.KeySizeValue = keySize;
        } // End Constructor 
        
        
        // protected ECDsa();
        // public override string KeyExchangeAlgorithm { get; }
        // public override string SignatureAlgorithm { get; }
        // public override string ToXmlString(bool includePrivateParameters);
        // public override void FromXmlString(string xmlString);

        // public static ECDsa Create(ECParameters parameters);
        // public static ECDsa Create(string algorithm);
        // public static ECDsa Create(ECCurve curve);
        // public static ECDsa Create();

        private byte[] DerEncode(Org.BouncyCastle.Math.BigInteger r, Org.BouncyCastle.Math.BigInteger s)
        {
            return new Org.BouncyCastle.Asn1.DerSequence(new Org.BouncyCastle.Asn1.Asn1Encodable[2]
            {
                (Org.BouncyCastle.Asn1.Asn1Encodable) new Org.BouncyCastle.Asn1.DerInteger(r),
                (Org.BouncyCastle.Asn1.Asn1Encodable) new Org.BouncyCastle.Asn1.DerInteger(s)
            }).GetDerEncoded();
        }
        
        
        // Abstract    
        public override byte[] SignHash(byte[] hash)
        {
            // throw new InvalidKeyException("EC private key required for signing");
            
            Org.BouncyCastle.Crypto.Signers.ECDsaSigner signer = new Org.BouncyCastle.Crypto.Signers.ECDsaSigner();
            signer.Init(true, this.m_privKey);
            
            Org.BouncyCastle.Math.BigInteger[] signature = signer.GenerateSignature(hash);
            return this.DerEncode(signature[0], signature[1]);
        }


        private static bool ByteArrayAreEqual(byte[] a1, byte[] a2)
        {
            if (a1 == null && a2 == null)
                return true;

            if (a1 == null || a2 == null)
                return false;


            // If not same length, done
            if (a1.Length != a2.Length)
            {
                return false;
            }

            // If they are the same object, done
            if (object.ReferenceEquals(a1, a2))
            {
                return true;
            }

            // Loop all values and compare, return false at first non-matching byte 
            for (int i = 0; i < a1.Length; i++)
            {
                if (a1[i] != a2[i])
                {
                    return false;
                }
            } // Next i 
            
            // if there never was a mismatch, a1 and b1 are equal
            return true;
        }
        
        
        // Abstract
        public override bool VerifyHash(byte[] hash, byte[] signature)
        {
            // byte[] msgBytes = hash;
            // Org.BouncyCastle.Crypto.ISigner signer = Org.BouncyCastle.Security.SignerUtilities.GetSigner("SHA-256withECDSA");
            // signer.Init(false, this.m_pubKey);
            // signer.BlockUpdate(msgBytes, 0, msgBytes.Length);
            // return signer.VerifySignature(signature);
            
            byte[] shouldMatch = this.SignHash(hash);
            return ByteArrayAreEqual(shouldMatch, signature);
        }
        
        
        // Not required
        // public virtual ECParameters ExportExplicitParameters(bool includePrivateParameters);
        // public virtual ECParameters ExportParameters(bool includePrivateParameters);

        // Not required
        // public virtual void GenerateKey(ECCurve curve);
        // public virtual void ImportParameters(ECParameters parameters);



        // Calls SignData <offset count>
        // public virtual byte[] SignData(byte[] data, System.Security.Cryptography.HashAlgorithmName hashAlgorithm);

        // Calls this.SignHash(this.HashData(
        //public virtual byte[] SignData(Stream data, System.Security.Cryptography.HashAlgorithmName hashAlgorithm);

        // Calls HashData
        // public virtual byte[] SignData(byte[] data, int offset, int count, System.Security.Cryptography.HashAlgorithmName hashAlgorithm);

        // All calls VerifyHash
        // public bool VerifyData(byte[] data, byte[] signature, System.Security.Cryptography.HashAlgorithmName hashAlgorithm);
        // public virtual bool VerifyData(byte[] data, int offset, int count, byte[] signature, System.Security.Cryptography.HashAlgorithmName hashAlgorithm);
        // // public bool VerifyData(Stream data, byte[] signature, System.Security.Cryptography.HashAlgorithmName hashAlgorithm);

        
        
        private static System.Security.Cryptography.HashAlgorithm GetHashAlgorithm(System.Security.Cryptography.HashAlgorithmName hashAlgorithmName)
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
            throw new System.Security.Cryptography.CryptographicException($"Unknown hash algorithm \"{hashAlgorithmName.Name}\".");
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
