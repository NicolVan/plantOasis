using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PlantOasis.lib.Util
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString RegisterScript(this HtmlHelper htmlHelper, MvcHtmlString template)
        {
            htmlHelper.ViewContext.HttpContext.Items["_script_" + Guid.NewGuid()] = template.ToString();
            return MvcHtmlString.Empty;
        }

        public static MvcHtmlString RenderScripts(this HtmlHelper htmlHelper)
        {
            StringBuilder resultScript = new StringBuilder();

            foreach (object key in htmlHelper.ViewContext.HttpContext.Items.Keys)
            {
                if (key.ToString().StartsWith("_script_"))
                {
                    var template = htmlHelper.ViewContext.HttpContext.Items[key] as string;
                    if (template != null)
                    {
                        resultScript.Append((string)template);
                    }
                }
            }
            return new MvcHtmlString(resultScript.ToString());
        }
    }
}

