
namespace CreateKeys
{


    class Program
    {


        public static string GenerateECDSA()
        {
            // http://safecurves.cr.yp.to/
            var kpECDSA = ECDSA.GenerateKeyPair("sect571r1");
            IO.WritePrivatePublic(kpECDSA, "ecdsa");

            return "";
        } // End Function GenerateECDSA 


        public static string GenerateRSA()
        {
            var kpRSA = RSA.GenerateKeyPair(4096);
            IO.WritePrivatePublic(kpRSA, "rsa");

            return "";
        } // End Function GenerateRSA 


        static void Main(string[] args)
        {
            GenerateECDSA();
            GenerateRSA();

            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        } // End Sub Main 


    } // End Class Program 


} // End Namespace CreateKeys 
