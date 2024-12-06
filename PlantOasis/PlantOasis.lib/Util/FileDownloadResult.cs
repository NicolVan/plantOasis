using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PlantOasis.lib.Util
{
    public class PdfDownloadResult : ActionResult
    {
        public byte[] _data { get; set; }
        public string _fileName { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Buffer = true;
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.ContentType = "application/pdf";
            //context.HttpContext.Response.AddHeader("content-disposition", string.Format("attachment;filename={0}", _fileName));
            context.HttpContext.Response.OutputStream.Write(_data, 0, _data.Length);
            context.HttpContext.Response.End();
        }

        public static ActionResult GetActionResult(byte[] data, string fileName)
        {
            PdfDownloadResult pdf = new PdfDownloadResult();
            pdf._data = data;
            pdf._fileName = fileName;
            return pdf;
        }
    }
}
