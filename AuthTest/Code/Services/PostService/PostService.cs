
using System.Collections.Generic;
using System.Threading.Tasks;


// Wilder:
// using System.Net.Http;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace AuthTest.Services
{


    public class PostService: IPostService
    {
        private Microsoft.Extensions.Configuration.IConfiguration _config;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;
        private Microsoft.Extensions.Logging.ILogger<PostService> _logger;

        public PostService(
            Microsoft.AspNetCore.Hosting.IHostingEnvironment env 
            , Microsoft.Extensions.Configuration.IConfiguration config
            , Microsoft.Extensions.Logging.ILogger<PostService> logger
            )
        {
            _env = env;
            _config = config;
            _logger = logger;
        }

        public async Task SendMail(string template, string name, string email, string subject, string msg)
        {
            try
            {
                string path = $"{_env.ContentRootPath}\\EmailTemplates\\{template}";
                string body = System.IO.File.ReadAllText(path, System.Text.Encoding.UTF8);

                string key = _config["MailService:ApiKey"];

                string uri = "https://api.sendgrid.com/api/mail.send.json";
                KeyValuePair<string, string>[] post = new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>("api_user", _config["MailService:ApiUser"]),
                    new KeyValuePair<string, string>("api_key", _config["MailService:ApiKey"]),
                    new KeyValuePair<string, string>("to", _config["MailService:Receiver"]),
                    new KeyValuePair<string, string>("toname", name),
                    new KeyValuePair<string, string>("subject", "Wildermuth.com Site Mail"),
                    new KeyValuePair<string, string>("text", string.Format(body, email, name, subject, msg)),
                    new KeyValuePair<string, string>("from", _config["MailService:Receiver"])
                };

                using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
                {
                    using (System.Net.Http.HttpResponseMessage response = 
                        await client.PostAsync(uri, new System.Net.Http.FormUrlEncodedContent(post)))
                    { 

                        if (!response.IsSuccessStatusCode)
                        {
                            string result = await response.Content.ReadAsStringAsync();
                            _logger.LogError($"Failed to send message via SendGrid: {System.Environment.NewLine}Body: {post}{System.Environment.NewLine}Result: {result}");
                        } // End if (!response.IsSuccessStatusCode)

                    } // End Using response

                } // End Using client 
            
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Exception Thrown sending message via SendGrid", ex);
            }
        }


    }


}