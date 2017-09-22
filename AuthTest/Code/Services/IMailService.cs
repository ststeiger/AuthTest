
using System.Threading.Tasks;


namespace AuthTest.Services
{


    public interface IMailService
    {
        Task SendMail(string senderEmail, string name, string email, string subject, string msg);
    }


} 
