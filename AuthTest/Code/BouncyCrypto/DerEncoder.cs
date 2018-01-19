// Decompiled with JetBrains decompiler
// Type: System.Security.Cryptography.DerEncoder
// Assembly: System.Security.Cryptography.Algorithms, Version=4.3.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 79445BD0-C274-4B93-85E4-91E39E026034
// Assembly location: /usr/share/dotnet/shared/Microsoft.NETCore.App/2.0.0/System.Security.Cryptography.Algorithms.dll
using System.Collections.Generic;


namespace AuthTest.Cryptography
{


    internal static class DerEncoder
    {


        private static readonly byte[][] s_nullTlv = new byte[3][]
        {
             new byte[1]{ (byte) 5 },
             new byte[1],
             System.Array.Empty<byte>()
        };


        private static byte[] EncodeLength(int length)
        {
            byte num1 = (byte)length;

            if (length < 128)
                return new byte[1] { num1 };

            if (length <= (int)byte.MaxValue)
                return new byte[2] { (byte)129, num1 };

            int num2 = length >> 8;
            byte num3 = (byte)num2;

            if (length <= (int)ushort.MaxValue)
                return new byte[3] { (byte)130, num3, num1 };

            int num4 = num2 >> 8;
            byte num5 = (byte)num4;

            if (length <= 16777215)
                return new byte[4] { (byte)131, num5, num3, num1 };

            return new byte[5]
            {
               (byte) 132,
               (byte) (num4 >> 8),
               num5,
               num3,
               num1
            };
        }


        internal static byte[][] SegmentedEncodeUnsignedInteger(
              byte[] bigEndianBytes
            , int offset
            , int count)
        {
            int srcOffset = offset;
            int num = srcOffset + count;
            while (srcOffset < num && (int)bigEndianBytes[srcOffset] == 0)
                ++srcOffset;
            if (srcOffset == num)
                --srcOffset;
            int count1 = num - srcOffset;
            int dstOffset = 0;
            byte[] numArray;

            if ((int)bigEndianBytes[srcOffset] > (int)sbyte.MaxValue)
            {
                numArray = new byte[count1 + 1];
                dstOffset = 1;
            }
            else
                numArray = new byte[count1];

            System.Buffer.BlockCopy(bigEndianBytes, srcOffset, numArray, dstOffset, count1);

            return new byte[3][]
            {
               new byte[1]{ (byte) 2 },
               EncodeLength(numArray.Length),
               numArray
            };
        }


        internal static byte[] ConstructSequence(params byte[][][] items)
        {
            return ConstructSequence((IEnumerable<byte[][]>)items);
        }


        internal static byte[] ConstructSequence(IEnumerable<byte[][]> items)
        {
            int length = 0;
            foreach (byte[][] numArray1 in items)
            {
                foreach (byte[] numArray2 in numArray1)
                    length += numArray2.Length;
            }

            byte[] numArray3 = EncodeLength(length);
            byte[] numArray4 = new byte[1 + numArray3.Length + length];
            numArray4[0] = (byte)48;

            int dstOffset1 = 1;
            System.Buffer.BlockCopy(numArray3, 0, numArray4, dstOffset1, numArray3.Length);
            int dstOffset2 = dstOffset1 + numArray3.Length;

            foreach (byte[][] numArray1 in items)
            {
                foreach (byte[] numArray2 in numArray1)
                {
                    System.Buffer.BlockCopy(numArray2, 0, numArray4, dstOffset2, numArray2.Length);
                    dstOffset2 += numArray2.Length;
                }
            }

            return numArray4;
        }


    }


}
