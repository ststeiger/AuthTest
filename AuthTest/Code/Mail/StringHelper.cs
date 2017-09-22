
namespace AuthTest
{


    internal class StringHelper
    {

        private static System.Collections.Generic.List<string> GraphemeClusters(string s)
        {
            System.Collections.Generic.List<string> ls = new System.Collections.Generic.List<string>();

            System.Globalization.TextElementEnumerator enumerator = System.Globalization.StringInfo.GetTextElementEnumerator(s);
            while (enumerator.MoveNext())
            {
                ls.Add((string)enumerator.Current);
            }

            return ls;
        }


        public static string ReverseGraphemeClusters(string s)
        {
            if (string.IsNullOrEmpty(s) || s.Length == 1)
                return s;

            System.Collections.Generic.List<string> ls = GraphemeClusters(s);
            ls.Reverse();

            return string.Join("", ls.ToArray());
        }


        public static string TrimSpecial(System.Object obj)
        {
            if (obj == null)
                return null;

            if (obj == System.DBNull.Value)
                return null;

            string str = obj.ToString();
            return str.Trim(new char[] { '\r', '\n', ' ', '\t' });
        } // End Function TrimSpecial 


    } /// End Class StringHelper 


} // End Namespace CoreCms 
