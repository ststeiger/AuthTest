
namespace AuthTest.Cryptography
{

    // https://stackoverflow.com/questions/37526036/how-to-determine-the-public-key-size-from-the-csr-file-using-bouncy-castle-in-ja
    // https://searchcode.com/file/94872444/crypto/src/asn1/sec/SECNamedCurves.cs
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
            //Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp256k1");
            Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp521r1");
            

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
            this.KeySizeValue = this.m_privKey.Parameters.Curve.FieldSize;
        } // End Constructor 


        public BouncyECDSA(Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters pubKey)
            :base()
        {
            this.m_pubKey = pubKey;
            this.KeySizeValue = this.m_pubKey.Parameters.Curve.FieldSize;
        } // End Constructor 
        
        
        public BouncyECDSA(Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair kp)
            :base()
        {
            this.m_privKey = (Org.BouncyCastle.Crypto.Parameters.ECPrivateKeyParameters) kp.Private;
            this.m_pubKey = (Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters) kp.Public;
            //this.KeySizeValue = keySize;

            //var x = (Org.BouncyCastle.Crypto.Parameters.ECKeyParameters)kp.Public;

            // var x = (Org.BouncyCastle.Crypto.Parameters.DsaPublicKeyParameters)kp.Public;
            // var y = (Org.BouncyCastle.Crypto.Parameters.DsaPrivateKeyParameters)kp.Private;

            // this.KeySizeValue = x.Y.BitCount;
            // this.KeySizeValue = y.X.BitCount;
            this.KeySizeValue = this.m_privKey.Parameters.Curve.FieldSize;
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
            return new Org.BouncyCastle.Asn1.DerSequence(
                new Org.BouncyCastle.Asn1.Asn1Encodable[2]
                {
                    new Org.BouncyCastle.Asn1.DerInteger(r),
                    new Org.BouncyCastle.Asn1.DerInteger(s)
                }
            ).GetDerEncoded();
        } // End Function DerEncode 


        private Org.BouncyCastle.Math.BigInteger[] DerDecode(byte[] encoding)
        {
            Org.BouncyCastle.Asn1.Asn1Sequence asn1Sequence = 
                (Org.BouncyCastle.Asn1.Asn1Sequence)
                Org.BouncyCastle.Asn1.Asn1Object.FromByteArray(encoding);

            return new Org.BouncyCastle.Math.BigInteger[2]
            {
                ((Org.BouncyCastle.Asn1.DerInteger) asn1Sequence[0]).Value,
                ((Org.BouncyCastle.Asn1.DerInteger) asn1Sequence[1]).Value
            };
        } // End Function DerDecode 


        // Abstract    
        // throw new InvalidKeyException("EC private key required for signing");
        public override byte[] SignHash(byte[] hash)
        {
            byte[] encoded = SignHashInternal(hash);

            return AsymmetricAlgorithmHelpers.ConvertDerToIeee1363(encoded, 0, encoded.Length, this.KeySize);
        } // End Function SignHash 


        public byte[] SignHashInternal(byte[] hash)
        {
            if (hash == null)
                throw new System.ArgumentNullException(nameof(hash));

            Org.BouncyCastle.Crypto.Signers.ECDsaSigner signer = new Org.BouncyCastle.Crypto.Signers.ECDsaSigner();
            signer.Init(true, this.m_privKey);

            Org.BouncyCastle.Math.BigInteger[] signature = signer.GenerateSignature(hash);
            byte[] encoded = this.DerEncode(signature[0], signature[1]);

            return encoded;
        } // End Function SignHashInternal


        /*
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
        } // End Function ByteArrayAreEqual 
        */


        // Abstract
        public override bool VerifyHash(byte[] hash, byte[] signature)
        {
            if (hash == null)
                throw new System.ArgumentNullException(nameof(hash));

            if (signature == null)
                throw new System.ArgumentNullException(nameof(signature));

            int num = 2 * AsymmetricAlgorithmHelpers.BitsToBytes(this.KeySize);
            if (signature.Length != num)
                return false;

            byte[] derSignature = AsymmetricAlgorithmHelpers.ConvertIeee1363ToDer(signature);
            
            Org.BouncyCastle.Crypto.Signers.ECDsaSigner signer = 
                new Org.BouncyCastle.Crypto.Signers.ECDsaSigner();
            signer.Init(false, this.m_pubKey);
            
            Org.BouncyCastle.Math.BigInteger[] bigIntegerArray = this.DerDecode(derSignature);
            return signer.VerifySignature(hash, bigIntegerArray[0], bigIntegerArray[1]);
        } // End Function VerifyHash 


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
                return System.Security.Cryptography.MD5.Create();
            if (hashAlgorithmName == System.Security.Cryptography.HashAlgorithmName.SHA1)
                return System.Security.Cryptography.SHA1.Create();
            if (hashAlgorithmName == System.Security.Cryptography.HashAlgorithmName.SHA256)
                return System.Security.Cryptography.SHA256.Create();
            if (hashAlgorithmName == System.Security.Cryptography.HashAlgorithmName.SHA384)
                return System.Security.Cryptography.SHA384.Create();
            if (hashAlgorithmName == System.Security.Cryptography.HashAlgorithmName.SHA512)
                return System.Security.Cryptography.SHA512.Create();

            throw new System.Security.Cryptography.CryptographicException($"Unknown hash algorithm \"{hashAlgorithmName.Name}\".");
        }
        
        
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
        }
        
        
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
        }
        
        
    }
    
    
}
