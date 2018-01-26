
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


// master/src/Common/src/System/Security/Cryptography/AsymmetricAlgorithmHelpers.cs
// https://github.com/dotnet/corefx/tree/master/src/Common/src/Internal/Cryptography


namespace AuthTest.Cryptography
{


    internal static class AsymmetricAlgorithmHelpers
    {


        public static byte[] ConvertIeee1363ToDer(byte[] input)
        {
            int num = input.Length / 2;
            
            return DerEncoder.ConstructSequence(DerEncoder.SegmentedEncodeUnsignedInteger(input, 0, num), DerEncoder.SegmentedEncodeUnsignedInteger(input, num, num));
        }
        

        public static byte[] ConvertDerToIeee1363(byte[] input, int inputOffset, int inputCount, int fieldSizeBits)
        {
            int bytes = BitsToBytes(fieldSizeBits);
            try
            {
                DerSequenceReader derSequenceReader = 
                    new DerSequenceReader(input, inputOffset, inputCount);

                byte[] signatureField1 = derSequenceReader.ReadIntegerBytes();
                byte[] signatureField2 = derSequenceReader.ReadIntegerBytes();
                byte[] response = new byte[2 * bytes];

                CopySignatureField(signatureField1, response, 0, bytes);
                CopySignatureField(signatureField2, response, bytes, bytes);

                return response;
            }
            catch (System.InvalidOperationException ex)
            {
                throw new System.Security.Cryptography.CryptographicException($"Cryptography Exception", ex);
            }
        }

        public static int BitsToBytes(int bitLength)
        {
            return (bitLength + 7) / 8;
        }


        private static void CopySignatureField(
              byte[] signatureField
            , byte[] response
            , int offset
            , int fieldLength)
        {
            if (signatureField.Length > fieldLength)
                System.Buffer.BlockCopy(signatureField, 1, response, offset, fieldLength);
            else if (signatureField.Length == fieldLength)
            {
                System.Buffer.BlockCopy(signatureField, 0, response, offset, fieldLength);
            }
            else
            {
                int num = fieldLength - signatureField.Length;
                System.Buffer.BlockCopy(signatureField, 0, response, offset + num
                    , signatureField.Length);
            }
        }


        public static byte[] HashData(
              byte[] data
            , int offset
            , int count
            , System.Security.Cryptography.HashAlgorithmName hashAlgorithm)
        {
            using (System.Security.Cryptography.HashAlgorithm hashAlgorithm1 =
                GetHashAlgorithm(hashAlgorithm))

                return hashAlgorithm1.ComputeHash(data, offset, count);
        }


        public static byte[] HashData(
              System.IO.Stream data
            , System.Security.Cryptography.HashAlgorithmName hashAlgorithm)
        {
            using (System.Security.Cryptography.HashAlgorithm hashAlgorithm1 = 
                GetHashAlgorithm(hashAlgorithm))
                return hashAlgorithm1.ComputeHash(data);
        }


        private static System.Security.Cryptography.HashAlgorithm GetHashAlgorithm(
            System.Security.Cryptography.HashAlgorithmName hashAlgorithmName)
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


    }


}
