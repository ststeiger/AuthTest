
namespace AuthTest.Services
{


    public class PathProvider : IPathProvider
    {
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;

        public PathProvider(Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            _hostingEnvironment = environment;
        }

        public string MapPath(string path)
        {
            string filePath = null;

            if(path != null)
                filePath = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, path.Replace('/', System.IO.Path.DirectorySeparatorChar));

            return filePath;
        }

        public string MapPath(Microsoft.AspNetCore.Http.PathString path)
        {
            string retValue = null;

            if (path.HasValue)
            {
                retValue = System.IO.Path.Combine(_hostingEnvironment.WebRootPath
                    , path.Value.Substring(1).Replace('/', System.IO.Path.DirectorySeparatorChar));
            } // End if (path.HasValue) 

            return retValue;
        }


    }


}
