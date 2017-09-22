
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AuthTest.Mail
{


    public class Mail
    {

        public static void SendMail(SmtpConfig smtpConfig, string strContactName
            , string strContactEmail, string strContactMessage)
        {
            MimeKit.MimeMessage message = new MimeKit.MimeMessage();

            message.From.Add(new MimeKit.MailboxAddress(strContactEmail, strContactEmail));
            message.To.Add(new MimeKit.MailboxAddress("Don Danillo", "info@daniel-steiger.ch"));
            message.To.Add(new MimeKit.MailboxAddress("Don Danillo", "stefan.steiger@rsnweb.ch"));

            message.Subject = "Neue Nachricht von www.daniel-steiger.ch";
            message.Body = new MimeKit.TextPart("plain") { Text = strContactMessage };

            using (MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient())
            {
                // client.Connect("smtp.gmail.com", 587);
                ////Note: only needed if the SMTP server requires authentication
                // client.Authenticate("mymail@gmail.com", "mypassword");

                client.Connect(smtpConfig.Server, smtpConfig.Port, false);
                client.Authenticate(smtpConfig.User, smtpConfig.Password);

                client.Send(message);
                client.Disconnect(true);
            }


        }


    }


}
