using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web;
using System.Threading.Tasks;



namespace imageServer.Controllers
{
    public class uploadController : ApiController
    {
        // route to upload the files
        [Route("api/Files/Upload")]
        public async Task<string> Post()
        {
            try
            {
                // catch the http post
                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count > 0)
                {
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFiles = httpRequest.Files[file];

                        // find the file name
                        var fileName = postedFiles.FileName.Split('\\').LastOrDefault().Split('/').LastOrDefault(); 

                        // direct the file to wanted directory
                        var filePath = HttpContext.Current.Server.MapPath("~/Uploads/" + fileName);

                        // save the file in the directory
                        postedFiles.SaveAs(filePath);

                        return "/Uploads/" + fileName;
                    }
                }
            }
            catch (Exception e)
            {

                return e.Message;
            }
            return "no files";
        }

    }
}