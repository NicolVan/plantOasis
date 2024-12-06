using PlantOasis.lib.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace PlantOasis.lib.Controller
{
    [PluginController("PlantOasis")]
    public class EshopApiController : UmbracoApiController
    {
        // ~/Umbraco/EshopApi/ClearSessionData
        public string ClearSessionData()
        {
            try
            {
                UserPropRepository rep = new UserPropRepository();
                rep.DeleteOldSessionData(DateTime.Now.AddDays(-1));

                QuoteRepository quoteRep = new QuoteRepository();
                quoteRep.DeleteOldSessionData(DateTime.Now.AddDays(-7));

                DeleteOldLogFiles(DateTime.Now.AddDays(-7));
            }
            catch (Exception exc)
            {
                this.Logger.Error(typeof(EshopApiController), "ClearSessionData error", exc);
                return "ERR";
            }

            return "OK";
        }

        private void DeleteOldLogFiles(DateTime dt)
        {
            string logPath = string.Format("{0}\\App_Data\\Logs",
                HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath));

            foreach (FileInfo fi in new DirectoryInfo(logPath).GetFiles())
            {
                if (fi.LastWriteTime < dt)
                {
                    fi.Delete();
                }
            }
        }
    }
}

