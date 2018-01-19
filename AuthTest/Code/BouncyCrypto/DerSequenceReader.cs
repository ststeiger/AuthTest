
// Decompiled with JetBrains decompiler
// Type: System.Security.Cryptography.DerSequenceReader
// Assembly: System.Security.Cryptography.Algorithms, Version=4.3.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 79445BD0-C274-4B93-85E4-91E39E026034
// Assembly location: /usr/share/dotnet/shared/Microsoft.NETCore.App/2.0.0/System.Security.Cryptography.Algorithms.dll

using System.Security.Cryptography;


namespace AuthTest.Cryptography 
{


    internal class DerSequenceReader
    {
        private readonly byte[] _data;
        private int _contentLength;
        private readonly int _end;
        private int _position;
        

        private int ContentLength
        {
            set
            {
                this._contentLength = value;
            }
        }


        internal DerSequenceReader(byte[] data, int offset, int length)
          : this(DerTag.Sequence, data, offset, length)
        { }


        internal DerSequenceReader(DerTag tagToEat, byte[] data, int offset, int length)
        {
            this._data = data;
            this._end = offset + length;

            if (length < 2 || length > data.Length - offset)
                throw new CryptographicException("Invalid Der-Encoding");

            this._position = offset;
            this.EatTag(tagToEat);
            this.ContentLength = this.EatLength();
        }


        internal bool HasData
        {
            get
            {
                return this._position < this._end;
            }
        }


        internal byte[] ReadIntegerBytes()
        {
            this.EatTag(DerTag.Integer);
            return this.ReadContentAsBytes();
        }


        private byte[] ReadContentAsBytes()
        {
            int count = this.EatLength();
            byte[] numArray = new byte[count];
            System.Buffer.BlockCopy(this._data, this._position, numArray, 0, count);
            this._position += count;
            return numArray;
        }


        private void EatTag(DerTag expected)
        {
            if (!this.HasData)
                throw new CryptographicException("Invalid Der-Encoding");

            CheckTag(expected, this._data, this._position);
            ++this._position;
        }


        private static void CheckTag(DerTag expected, byte[] data, int position)
        {
            if (position >= data.Length)
                throw new CryptographicException("Invalid Der-Encoding");

            byte num1 = data[position];
            if (((int)num1 & 128) != 0)
                return;

            byte num2 = (byte)((uint)num1 & 31U);

            if ((expected & (DerTag)31) != (DerTag)num2)
                throw new CryptographicException("Invalid Der-Encoding");
        }


        private int EatLength()
        {
            int bytesConsumed;
            int num = ScanContentLength(this._data, this._position, out bytesConsumed);
            this._position += bytesConsumed;

            if (num > this._end - this._position)
                throw new CryptographicException("Invalid Der-Encoding");

            return num;
        }
        private static int ScanContentLength(byte[] data, int offset, out int bytesConsumed)
        {
            if (offset >= data.Length)
                throw new CryptographicException("Invalid Der-Encoding");

            byte num1 = data[offset];
            if ((int)num1 < 128)
            {
                if ((int)num1 > data.Length - offset)
                    throw new CryptographicException("Invalid Der-Encoding");

                bytesConsumed = 1;
                return (int)num1;
            }

            bytesConsumed = 1 + ((int)num1 & (int)sbyte.MaxValue);

            if (bytesConsumed > data.Length - offset)
                throw new CryptographicException("Invalid Der-Encoding");

            if (bytesConsumed == 1)
                throw new CryptographicException("Invalid Der-Encoding");

            int num2 = offset + bytesConsumed;
            int num3 = 0;
            for (int index = offset + 1; index < num2; ++index)
                num3 = (num3 << 8) + (int)data[index];

            if (num3 > data.Length - offset - bytesConsumed)
                throw new CryptographicException("Invalid Der-Encoding");

            return num3;
        }


        internal enum DerTag : byte
        {
            Boolean = 1,
            Integer = 2,
            BitString = 3,
            OctetString = 4,
            Null = 5,
            ObjectIdentifier = 6,
            UTF8String = 12, // 0x0C
            Sequence = 16, // 0x10
            Set = 17, // 0x11
            PrintableString = 19, // 0x13
            T61String = 20, // 0x14
            IA5String = 22, // 0x16
            UTCTime = 23, // 0x17
            GeneralizedTime = 24, // 0x18
            BMPString = 30, // 0x1E
        }


    }


}
