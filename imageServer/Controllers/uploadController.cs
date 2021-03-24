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
        [Route("api/Files/Upload")]
        public async Task<string> Post()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count > 0)
                {
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFiles = httpRequest.Files[file];

                        var fileName = postedFiles.FileName.Split('\\').LastOrDefault().Split('/').LastOrDefault();

                        var filePath = HttpContext.Current.Server.MapPath("~/Uploads/" + fileName);

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