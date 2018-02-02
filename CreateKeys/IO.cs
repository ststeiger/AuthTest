using System;
using System.Collections.Generic;
using System.Text;

namespace CreateKeys
{
    class IO
    {



        public static void WritePrivatePublic(Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair keyPair, string fn)
        {
            string privateKey = null;
            string publicKey = null;
            string bothKeys = null;

            // id_rsa
            using (System.IO.TextWriter textWriter = new System.IO.StringWriter())
            {
                Org.BouncyCastle.OpenSsl.PemWriter pemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(textWriter);
                pemWriter.WriteObject(keyPair.Private);
                pemWriter.Writer.Flush();

                privateKey = textWriter.ToString();
            } // End Using textWriter 

            // id_rsa.pub
            using (System.IO.TextWriter textWriter = new System.IO.StringWriter())
            {
                Org.BouncyCastle.OpenSsl.PemWriter pemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(textWriter);
                pemWriter.WriteObject(keyPair.Public);
                pemWriter.Writer.Flush();

                publicKey = textWriter.ToString();
            } // End Using textWriter 


            //  // This writes the same as private key, not both
            //using (System.IO.TextWriter textWriter = new System.IO.StringWriter())
            //{
            //    Org.BouncyCastle.OpenSsl.PemWriter pemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(textWriter);
            //    pemWriter.WriteObject(keyPair);
            //    pemWriter.Writer.Flush();

            //    bothKeys = textWriter.ToString();
            //} // End Using textWriter 

            System.Console.WriteLine(privateKey);
            System.Console.WriteLine(publicKey);
            //System.Console.WriteLine(bothKeys);
            System.IO.File.WriteAllText("id_" + fn + ".priv", privateKey, Encoding.UTF8);
            System.IO.File.WriteAllText("id_" + fn + ".pub", privateKey, Encoding.UTF8);
            
            // Org.BouncyCastle.Crypto.AsymmetricKeyParameter pk = ReadPrivateKey(privateKey);
            // Org.BouncyCastle.Crypto.AsymmetricKeyParameter pubKey = ReadPublicKey(publicKey);

            // ReadPublicKey(privateKey); // Cannot read this
            // ReadPrivateKey(publicKey); // Cannot read this either...

            // CerKeyInfo(keyPair);

        } // End Sub WritePrivatePublic



        public static Org.BouncyCastle.Crypto.AsymmetricKeyParameter ReadPublicKey(string publicKey)
        {
            Org.BouncyCastle.Crypto.AsymmetricKeyParameter keyParameter = null;

            using (System.IO.TextReader reader = new System.IO.StringReader(publicKey))
            {
                Org.BouncyCastle.OpenSsl.PemReader pemReader =
                    new Org.BouncyCastle.OpenSsl.PemReader(reader);

                object obj = pemReader.ReadObject();

                if ((obj is Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair))
                    throw new System.ArgumentException("The given publicKey is actually a private key.", "publicKey");

                if (!(obj is Org.BouncyCastle.Crypto.AsymmetricKeyParameter))
                    throw new System.ArgumentException("The given publicKey is not a valid assymetric key.", "publicKey");

                keyParameter = (Org.BouncyCastle.Crypto.AsymmetricKeyParameter)obj;
                if (keyParameter.IsPrivate)
                    throw new System.ArgumentException("The given publicKey is actually a private key.", "publicKey");
            } // End Using reader 

            if (!(keyParameter is Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters))
                throw new System.ArgumentException("The given privateKey is an asymmetric publicKey, but not an ECDSA public key.", "publicKey");

            return keyParameter;
        } // End Function ReadPublicKey 



        public static Org.BouncyCastle.Crypto.AsymmetricKeyParameter ReadPrivateKey(string privateKey)
        {
            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair keyPair = null;

            using (System.IO.TextReader reader = new System.IO.StringReader(privateKey))
            {
                Org.BouncyCastle.OpenSsl.PemReader pemReader =
                    new Org.BouncyCastle.OpenSsl.PemReader(reader);

                object obj = pemReader.ReadObject();

                if (obj is Org.BouncyCastle.Crypto.AsymmetricKeyParameter)
                    throw new System.ArgumentException("The given privateKey is a public key, not a privateKey...", "privateKey");

                if (!(obj is Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair))
                    throw new System.ArgumentException("The given privateKey is not a valid assymetric key.", "privateKey");

                keyPair = (Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair)obj;
            } // End using reader 

            // Org.BouncyCastle.Crypto.AsymmetricKeyParameter priv = keyPair.Private;
            // Org.BouncyCastle.Crypto.AsymmetricKeyParameter pub = keyPair.Public;

            // Note: 
            // cipher.Init(false, key);
            // !!!

            return keyPair.Private;
        } // End Function ReadPrivateKey


        public static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair ReadKeyPair(string privateKey)
        {
            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair keyPair = null;

            using (System.IO.TextReader reader = new System.IO.StringReader(privateKey))
            {
                Org.BouncyCastle.OpenSsl.PemReader pemReader =
                    new Org.BouncyCastle.OpenSsl.PemReader(reader);

                object obj = pemReader.ReadObject();

                if (obj is Org.BouncyCastle.Crypto.AsymmetricKeyParameter)
                    throw new System.ArgumentException("The given privateKey is a public key, not a privateKey...", "privateKey");

                if (!(obj is Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair))
                    throw new System.ArgumentException("The given privateKey is not a valid assymetric key.", "privateKey");

                keyPair = (Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair)obj;
            }

            // Org.BouncyCastle.Crypto.AsymmetricKeyParameter priv = keyPair.Private;
            // Org.BouncyCastle.Crypto.AsymmetricKeyParameter pub = keyPair.Public;

            // Note: 
            // cipher.Init(false, key);
            // !!!

            return keyPair;
        } // End Function ReadPrivateKey


        public static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair ReadKeyPairFromFile(string fileName)
        {
            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair KeyPair = null;

            //  Stream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            using (System.IO.FileStream fs = System.IO.File.OpenRead(fileName))
            {

                using (System.IO.StreamReader sr = new System.IO.StreamReader(fs))
                {
                    Org.BouncyCastle.OpenSsl.PemReader pemReader = new Org.BouncyCastle.OpenSsl.PemReader(sr);
                    KeyPair = (Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair)pemReader.ReadObject();
                    // System.Security.Cryptography.RSAParameters rsa = Org.BouncyCastle.Security.
                    //     DotNetUtilities.ToRSAParameters((Org.BouncyCastle.Crypto.Parameters.RsaPrivateCrtKeyParameters)KeyPair.Private);
                } // End Using sr 

            } // End Using fs 

            return KeyPair;
        } // End Function ImportKeyPair 


        //public static void ReadPrivateKeyFile(string privateKeyFileName)
        //{
        //    Org.BouncyCastle.Crypto.Parameters.RsaPrivateCrtKeyParameters key = null;

        //    using (System.IO.StreamReader streamReader = System.IO.File.OpenText(privateKeyFileName))
        //    {
        //        Org.BouncyCastle.OpenSsl.PemReader pemReader = 
        //            new Org.BouncyCastle.OpenSsl.PemReader(streamReader);
        //        key = (Org.BouncyCastle.Crypto.Parameters.RsaPrivateCrtKeyParameters) pemReader.ReadObject();
        //    } // End Using streamReader 

        //    // Note: 
        //    // cipher.Init(false, key);
        //    // !!!
        //} // End Function ReadPrivateKey


        public Org.BouncyCastle.Crypto.AsymmetricKeyParameter ReadPublicKeyFile(string pemFilename)
        {
            Org.BouncyCastle.Crypto.AsymmetricKeyParameter keyParameter = null;

            using (System.IO.StreamReader streamReader = System.IO.File.OpenText(pemFilename))
            {
                Org.BouncyCastle.OpenSsl.PemReader pemReader = new Org.BouncyCastle.OpenSsl.PemReader(streamReader);
                keyParameter = (Org.BouncyCastle.Crypto.AsymmetricKeyParameter)pemReader.ReadObject();
            } // End Using fileStream 

            return keyParameter;
        } // End Function ReadPublicKey 


        public static void ExportKeyPair(Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair keyPair)
        {
            string privateKey = null;

            using (System.IO.TextWriter textWriter = new System.IO.StringWriter())
            {
                Org.BouncyCastle.OpenSsl.PemWriter pemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(textWriter);
                pemWriter.WriteObject(keyPair.Private);
                pemWriter.Writer.Flush();

                privateKey = textWriter.ToString();
            } // End Using textWriter 

            System.Console.WriteLine(privateKey);
        } // End Sub ExportKeyPair 



    }
}
