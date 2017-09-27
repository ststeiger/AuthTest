
using System.Threading.Tasks;


namespace AuthTest.Services
{


    public interface IPostService
    {
        Task SendMail(string template, string name, string email, string subject, string msg);
    }


}
