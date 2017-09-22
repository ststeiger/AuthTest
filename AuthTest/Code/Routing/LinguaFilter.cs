
namespace CoreCms
{


    // https://msdn.microsoft.com/en-us/magazine/mt767699.aspx
    // https://stackoverflow.com/questions/8786192/asp-net-mvc-register-action-filter-without-modifying-controller
    public class LanguageConfiguration
    {
        public static NonLockingThreadSafeHashSet<string> s_hs;
        public static string s_NameForDefaultSelector;


        // We should be able to read languages without lock, 
        // and add/remove languages in a thread-safe manner 
        static LanguageConfiguration()
        {
            s_NameForDefaultSelector = "default";

            s_hs = new NonLockingThreadSafeHashSet<string>(System.StringComparer.OrdinalIgnoreCase);

            s_hs.Add("de");
            s_hs.Add("en");
            s_hs.Add("fr");
            s_hs.Add("it");

            s_hs.Add(s_NameForDefaultSelector);
        } // End Static Constructor 

        public string DefaultSelector = s_NameForDefaultSelector;
        public string DefaultLanguage { get; set; } = "EN";


        public bool Contains(string item)
        {
            return s_hs.Contains(item);
        } // End Function Contains 

        public void Add(string item)
        {
            s_hs.Add(item);
        } // End Sub Add 


    } // End Class LanguageConfiguration 





    // System.Web.Mvc.ActionFilterAttribute
    public class LocalizationAttribute : Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute
    {

        LanguageConfiguration m_validLanguages;

        public LocalizationAttribute(LanguageConfiguration languageConfig)
        {
            this.m_validLanguages = languageConfig;
        } // End Constructor 


        private static string[] ParseAccept(string accept)
        {
            // accept = " ";

            if (!string.IsNullOrEmpty(accept))
            {
                string[] languages = accept.Split(',');

                for (int i = 0; i < languages.Length; ++i)
                {
                    if (languages[i] == null)
                        languages[i] = "";

                    int semicolonPosition = languages[i].IndexOf(";");
                    if (semicolonPosition != -1)
                        languages[i] = languages[i].Substring(0, semicolonPosition);

                    languages[i] = languages[i].Trim();

                    if (languages[i].Length > 2)
                        languages[i] = languages[i].Substring(0, 2);

                    // languages[i] = "en-US";
                } // Next i 

                return languages;
            } // End if (!string.IsNullOrEmpty(accept))

            return null;
        } // End Function ParseAccept 


        public override void OnActionExecuting(
            Microsoft.AspNetCore.Mvc.Filters.
            ActionExecutingContext filterContext
            )
        {
            // Here, we set the language to 
            // - the currently selected language (if present and valid)
            // - else the first valid language in accept-header (if not present or invalid)
            // - else the default language EN (English), if no supported language in accept-header 

            string culture = filterContext.RouteData.Values["culture"] as string;
            if (
                    System.StringComparer.OrdinalIgnoreCase.Equals(culture, this.m_validLanguages.DefaultSelector) 
                || !this.m_validLanguages.Contains(culture)
                )
                culture = null;


            if (culture == null)
            {
                // string[] userLanguage = HttpContext.Current.Request.UserLanguages;
                string accept = filterContext.HttpContext.Request.Headers["Accept-Language"];
                string[] userLanguages = ParseAccept(accept);
                // userLanguages = new string[] { "en-US", "en-GB" };

                culture = this.m_validLanguages.DefaultLanguage; // default
                if (userLanguages != null)
                {
                    for (int i = 0; i < userLanguages.Length; ++i)
                    {
                        if (this.m_validLanguages.Contains(userLanguages[i]))
                        {
                            culture = userLanguages[i];
                            break;
                        } // End if (this.m_validLanguages.Contains(userLanguages[i]))

                    } // Next i 

                } // End if (userLanguages != null) 

            } // End if (culture == null)


            culture = culture.ToUpper();

            filterContext.RouteData.Values["culture"] = culture;
            // System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo(culture);
            // System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;
            // System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture(cultureInfo.Name);

            base.OnActionExecuting(filterContext);
        } // End Sub OnActionExecuting 


    } // End Class LanguageConfiguration 


} // End Namespace CoreCms 
