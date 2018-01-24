
namespace AuthTest.WebClient 
{


    // http://codecaster.nl/blog/2015/11/webclient-httpwebrequest-httpclient-perform-web-requests-net/
    public class HtmlFetcher
    {


        public class Parameter
        {
            public System.Uri Uri;
            public System.Net.Http.HttpMethod RequestMethod;
            public string ContentType;

            private byte[] m_Bytes;
            public byte[] GetFriendBytes()
            {
                return m_Bytes;
            } // End Function GetFriendBytes

        } // End Class Parameter 


        public static string GetData()
        {
            Parameter parameters = new Parameter {
                Uri= new System.Uri(""),
                RequestMethod = System.Net.Http.HttpMethod.Get
            };
            return GetData(parameters);
        } // End Function GetData 


        public static string GetData(Parameter parameters)
        {
            string result = null;

            using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
            {

                //HttpRequestMessage request = new HttpRequestMessage
                //{
                //    RequestUri = parameters.Uri,
                //    Method = parameters.requestMethod,
                //};

                // https://stackoverflow.com/questions/29801195/adding-headers-when-using-httpclient-getasync
                //request.Content = new ByteArrayContent(parameters.GetFriendBytes());
                //request.Content.Headers.Add("", "");
                //request.Content.Headers.ContentType =
                //    new System.Net.Http.Headers.MediaTypeHeaderValue(parameters.ContentType);


                // sendResult = client.SendAsync(request).Result;

                using (System.Net.Http.HttpResponseMessage response = 
                    client.GetAsync(parameters.Uri).Result)
                {
                    // http://codecaster.nl/blog/2015/11/webclient-httpwebrequest-httpclient-perform-web-requests-net/
                    // result.EnsureSuccessStatusCode();
                    // res = client.GetStringAsync(parameters.Uri).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        System.Net.HttpStatusCode code = response.StatusCode;

                        if (code == System.Net.HttpStatusCode.NotFound)
                        {
                            result = response.Content.ReadAsStringAsync().Result;
                            throw new System.Exception(result);
                        } // End if (code == HttpStatusCode.NotFound) 

                    } // End if (!response.IsSuccessStatusCode) 

                    
                    using (System.IO.Stream stream = response.Content.ReadAsStreamAsync().Result)
                    {
                        using (System.IO.StreamReader reader = new System.IO.StreamReader(stream, System.Text.Encoding.UTF8))
                        {
                            using (Newtonsoft.Json.JsonTextReader jsonReader =
                                new Newtonsoft.Json.JsonTextReader(reader))
                            {
                                Newtonsoft.Json.JsonSerializer serializer =
                                    new Newtonsoft.Json.JsonSerializer();
                                serializer.Deserialize(jsonReader);
                                // serializer.Deserialize<string>(jsonReader);
                            } // End Using jsonReader 

                        } // End Using reader 

                    } // End Using stream 
                    

                    result = response.Content.ReadAsStringAsync().Result;
                } // End Using response 

            } // End Using client 
            
            return result;
        } // End Function GetData 


        public static void Serialize(object value, System.IO.Stream s)
        {
            using (System.IO.StreamWriter writer = 
                new System.IO.StreamWriter(s))
            using (Newtonsoft.Json.JsonTextWriter jsonWriter = 
                new Newtonsoft.Json.JsonTextWriter(writer))
            {
                Newtonsoft.Json.JsonSerializer ser = new Newtonsoft.Json.JsonSerializer();
                ser.Serialize(jsonWriter, value);
                jsonWriter.Flush();
            } // End Using jsonWriter 

        } // End Sub Serialize 


        public static T Deserialize<T>(System.IO.Stream s)
        {
            using (System.IO.StreamReader reader = new System.IO.StreamReader(s))
            using (Newtonsoft.Json.JsonTextReader jsonReader = new Newtonsoft.Json.JsonTextReader(reader))
            {
                Newtonsoft.Json.JsonSerializer ser = new Newtonsoft.Json.JsonSerializer();
                return ser.Deserialize<T>(jsonReader);
            } // End Using jsonReader 

        } // End Sub Deserialize 


        public class ProxyServerSettings
        {
            public string Address;
            public int Port;
            public string UserName;
            public string Password;
        } // End Class ProxyServerSettings 


        // https://stackoverflow.com/questions/29856543/httpclient-and-using-proxy-constantly-getting-407
        public static System.Net.Http.HttpClient 
            CreateProxiedClient(ProxyServerSettings proxyServerSettings)
        {
            string proxyUri = $"{proxyServerSettings.Address}:{proxyServerSettings.Port}";

            System.Net.NetworkCredential proxyCreds = new System.Net.NetworkCredential(
                proxyServerSettings.UserName,
                proxyServerSettings.Password
            );

            System.Net.WebProxy proxy = new System.Net.WebProxy(proxyUri, false)
            {
                UseDefaultCredentials = false,
                Credentials = proxyCreds,
            };

            // Now create a client handler which uses that proxy

            System.Net.Http.HttpClient client = null;
            System.Net.Http.HttpClientHandler httpClientHandler = 
                new System.Net.Http.HttpClientHandler()
            {
                Proxy = proxy,
                PreAuthenticate = true,
                UseDefaultCredentials = false,
            };

            // You only need this part if the server wants a username and password:

            string httpUserName = "?????", httpPassword = "secret";
            httpClientHandler.Credentials = 
                new System.Net.NetworkCredential(httpUserName, httpPassword);

            client = new System.Net.Http.HttpClient(httpClientHandler);
            return client;
        } // End Sub CreateProxiedClient 


    } // End Class HtmlFetcher 


} // End Namespace AuthTest.WebClient 
