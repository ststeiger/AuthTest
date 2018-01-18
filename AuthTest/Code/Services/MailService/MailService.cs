
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


// Wilder:
// using System.Net.Http;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;



namespace AuthTest.Services
{



    public class MailInfo
    {
        public System.Net.Mail.MailAddress From;
        public System.Net.Mail.MailAddress To;
        
        public string Subject;
        public MessageFormat_t MessageFormat;
        public string Message;


        public MailInfo(string from, string to)
            : this(new System.Net.Mail.MailAddress(from)
                  , new System.Net.Mail.MailAddress(to)
                  , null, MessageFormat_t.Html, null)
        { }

        public MailInfo(string from, string to, string subject, MessageFormat_t format, string message)
            :this(new System.Net.Mail.MailAddress(from), new System.Net.Mail.MailAddress(to), subject, format, message)
        { }


        public MailInfo(System.Net.Mail.MailAddress from, System.Net.Mail.MailAddress to
            , string subject, MessageFormat_t format, string message)
        {
            this.From = from;
            this.To = to;
            this.Subject = subject;
            this.MessageFormat = format;
            this.Message = message;
        }


        public enum MessageFormat_t : int 
        {
            Plain = 0,
            // Text = 0,
            // Flowed = 1,
            Html = 2,
            // Enriched = 3,
            // CompressedRichText = 4,
            // RichText = 5
        }

        public static void Test()
        {
            MailService ms = new MailService(null, null, null, null);
            IMailService mss = ms;
            mss.SendMail("", "", "", "", "");
            new MailInfo("from", "to")
            {
                Subject="",
                Message="",
                MessageFormat = MessageFormat_t.Plain
            };
        }

    }


    public class MailService: IMailService
    {
        // private Microsoft.Extensions.Configuration.IConfigurationRoot _config;
        private Microsoft.Extensions.Configuration.IConfiguration m_config;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment m_env;
        private Microsoft.Extensions.Logging.ILogger<MailService> m_logger;
        private Mail.SmtpConfig m_smtpConfig;

        public MailService(
            Microsoft.AspNetCore.Hosting.IHostingEnvironment env
            //, Microsoft.Extensions.Configuration.IConfigurationRoot config
            , Microsoft.Extensions.Configuration.IConfiguration config
            , Microsoft.Extensions.Logging.ILogger<MailService> logger
            , Microsoft.Extensions.Options.IOptions<Mail.SmtpConfig> smtpConfig 
            )
        {
            m_env = env;
            m_config = config;
            m_logger = logger;
            m_smtpConfig = smtpConfig.Value;
        } // End Constructor 
        
        
        async Task IMailService.SendMail(string senderEmail, string name, string email, string subject, string msg)
        {
            await this.SendMailImpl(true, senderEmail, name, email, subject, msg);
        }
        

        private async Task SendMailImpl(bool repeat, string senderEmail, string name, string email, string subject, string msg)
        {
            try
            {
                MimeKit.MimeMessage message = new MimeKit.MimeMessage();

                message.From.Add(new MimeKit.MailboxAddress(senderEmail, senderEmail));
                message.To.Add(new MimeKit.MailboxAddress(name, email));

                message.Subject = subject;
                // message.Body = new MimeKit.TextPart(MimeKit.Text.TextFormat.Plain) { Text = msg };
                message.Body = new MimeKit.TextPart(MimeKit.Text.TextFormat.Html) { Text = msg };

                using (MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient())
                {
                    // client.Connect("smtp.gmail.com", 587);
                    // client.Authenticate("mymail@gmail.com", "mypassword");

                    // client.Connect(smtpConfig.Server, smtpConfig.Port, false); // Insecure
                    // client.Connect(smtpConfig.Server, smtpConfig.Port, true); // SSL
                    client.Connect(this.m_smtpConfig.Server, this.m_smtpConfig.Port, MailKit.Security.SecureSocketOptions.StartTls);

                    // // Note: only needed if the SMTP server requires authentication
                    client.Authenticate(this.m_smtpConfig.User, this.m_smtpConfig.Password);

                    await client.SendAsync(message);
                    client.Disconnect(true);
                } // End Using Client 
            }
            catch (System.Exception ex)
            {
                // OMG: Recursive...

                try
                {
                    // this.m_logger.LogError("Exception Thrown sending message via SendGrid", ex);
                    
                    if(repeat)
                        await this.SendMailImpl(false, senderEmail, name, email, subject, msg);
                    else
                        System.Console.WriteLine("Exception Thrown sending message via SendGrid\n{0}", ex.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
            }

        } // End function IMailService.SendMail 


    } // End Class MailService : IMailService 


} // End Namespace AuthTest.Services 
