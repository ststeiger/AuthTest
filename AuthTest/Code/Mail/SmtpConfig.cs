
namespace AuthTest.Mail
{


    // https://stackoverflow.com/questions/39083372/how-to-read-connection-string-in-net-core/40844342#40844342
    public class SmtpConfig
    {
        public string Server { get; set; }
        public string User { get; set; }

        private string m_Password;
        public string Password 
        {
            get { return this.m_Password; }
            set { this.m_Password = StringHelper.ReverseGraphemeClusters(value); }

        }
        public int Port { get; set; }
    }


}
