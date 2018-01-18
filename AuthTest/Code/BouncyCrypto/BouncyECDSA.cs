using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthTest.Code.BouncyCrypto
{
    public class BouncyECDSA : System.Security.Cryptography.ECDsa
    {


        Org.BouncyCastle.Crypto.Parameters.ECPrivateKeyParameters m_privKey;
        Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters m_pubKey;


        public BouncyECDSA(Org.BouncyCastle.Crypto.Parameters.ECPrivateKeyParameters privKey)
        {
            this.m_privKey = privKey;
        } // End Constructor 


        public BouncyECDSA(Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters pubKey)
        {
            this.m_pubKey = pubKey;
        } // End Constructor 


        public BouncyECDSA(Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair kp)
        {
            this.m_privKey = (Org.BouncyCastle.Crypto.Parameters.ECPrivateKeyParameters)kp.Private;
            this.m_pubKey = (Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters)kp.Public;
        } // End Constructor 

        //
        // Zusammenfassung:
        //     Generates a digital signature for the specified hash value.
        //
        // Parameter:
        //   hash:
        //     The hash value of the data that is being signed.
        //
        // Rückgabewerte:
        //     A digital signature that consists of the given hash value encrypted with the
        //     private key.
        //
        // Ausnahmen:
        //   T:System.ArgumentNullException:
        //     The hash parameter is null.
        public override byte[] SignHash(byte[] hash)
        {
            return null;
        }

        //
        // Zusammenfassung:
        //     Verifies a digital signature against the specified hash value.
        //
        // Parameter:
        //   hash:
        //     The hash value of a block of data.
        //
        //   signature:
        //     The digital signature to be verified.
        //
        // Rückgabewerte:
        //     true if the hash value equals the decrypted signature; otherwise, false.
        public override bool VerifyHash(byte[] hash, byte[] signature)
        {
            return true;
        }
        





    }





    /*
        public abstract class ECDsa : AsymmetricAlgorithm
        {
            protected ECDsa();
            public override string KeyExchangeAlgorithm { get; }
            public override string SignatureAlgorithm { get; }
            public override string ToXmlString(bool includePrivateParameters);
            public override void FromXmlString(string xmlString);

            public static ECDsa Create(ECParameters parameters);
            public static ECDsa Create(string algorithm);
            public static ECDsa Create(ECCurve curve);
            public static ECDsa Create();

            public abstract byte[] SignHash(byte[] hash);
            public abstract bool VerifyHash(byte[] hash, byte[] signature);

            public virtual ECParameters ExportExplicitParameters(bool includePrivateParameters);
            public virtual ECParameters ExportParameters(bool includePrivateParameters);

            
            public virtual void GenerateKey(ECCurve curve);
            public virtual void ImportParameters(ECParameters parameters);
            public virtual byte[] SignData(byte[] data, HashAlgorithmName hashAlgorithm);
            public virtual byte[] SignData(Stream data, HashAlgorithmName hashAlgorithm);
            public virtual byte[] SignData(byte[] data, int offset, int count, HashAlgorithmName hashAlgorithm);
        
        
            public bool VerifyData(byte[] data, byte[] signature, HashAlgorithmName hashAlgorithm);
            public virtual bool VerifyData(byte[] data, int offset, int count, byte[] signature, HashAlgorithmName hashAlgorithm);
            public bool VerifyData(Stream data, byte[] signature, HashAlgorithmName hashAlgorithm);
            
            protected virtual byte[] HashData(byte[] data, int offset, int count, HashAlgorithmName hashAlgorithm);
            protected virtual byte[] HashData(Stream data, HashAlgorithmName hashAlgorithm);
        }
 */


}
