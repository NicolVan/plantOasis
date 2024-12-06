using PlantOasis.lib.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace PlantOasis.lib.Controller
{
    [PluginController("PlantOasis")]
    public class FileUploadApiController : UmbracoApiController
    {
        [HttpPost]
        public object UploadFile()
        {
            FileUploadRepository fu = new FileUploadRepository();
            return fu.UploadFile();
        }
        [HttpPost]
        public object ManageFiles(string id)
        {
            string[] items = id.Split('|');
            switch (items[0].ToLower())
            {
                case "delete":
                    {
                        FileUploadRepository fu = new FileUploadRepository();
                        return fu.DeleteFile(items[1]);
                    }
                case "description":
                    {
                        FileUploadRepository fu = new FileUploadRepository();
                        return fu.SetFileDescription(items[1], items[2], items[3]);
                    }
                default:
                    {
                        FileUploadRepository fu = new FileUploadRepository();
                        return fu.GetFiles(items[1]);
                    }
            }
        }
    }
}
