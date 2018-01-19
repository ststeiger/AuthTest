
namespace AuthTest.Code.BouncyCrypto
{
    
    
    public class CommentSanitizer
    {
        
        
        public static string RemoveSingleLineComments(string input)
        {
            input = System.Text.RegularExpressions.Regex.Replace(input, "//.*", "");
            input = System.Text.RegularExpressions.Regex.Replace(input, @"[\r\n]+(\s+[\r\n]+)+?", "\r\n");
            System.Console.WriteLine(input);
            
            return input;
        } // End Function RemoveSingleLineComments 
        
        
    } // End Class CommentSanitizer 
    
    
} // End Namespace 
