
namespace AuthTest.Services
{


    public interface IPathProvider
    {
        string MapPath(string path);
        string MapPath(Microsoft.AspNetCore.Http.PathString path);
    }


}
