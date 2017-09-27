
namespace AuthTest
{


    public static class ETagGenerator
    {


        public static string GetETag(string key, byte[] contentBytes)
        {
            byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] combinedBytes = Combine(keyBytes, contentBytes);

            return GenerateETag(combinedBytes);
        } // End Function GetETag


        public static string GetETag(string key, string fileName)
        {
            byte[] contentBytes = HashFile(fileName);

            byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] combinedBytes = Combine(keyBytes, contentBytes);

            return GenerateETag(combinedBytes);
        } // End Function GetETag


        private static string GenerateETag(byte[] data)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] hash = md5.ComputeHash(data);
                string hex = System.BitConverter.ToString(hash);
                return hex.Replace("-", "");
            }
        } // End Function GenerateETag 


        private static byte[] Combine(byte[] a, byte[] b)
        {
            byte[] c = new byte[a.Length + b.Length];
            System.Buffer.BlockCopy(a, 0, c, 0, a.Length);
            System.Buffer.BlockCopy(b, 0, c, a.Length, b.Length);
            return c;
        } // End Function Combine 


        private static byte[] HashFile(string filename)
        {
            byte[] retValue = null;

            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                using (System.IO.Stream stream = System.IO.File.OpenRead(filename))
                {
                    retValue = md5.ComputeHash(stream);
                } // End Using stream 

            } // End Using md5 

            return retValue;
        } // End Function HashFile 


    } // End Class ETagGenerator 


} // End Namespace AuthTest 
