using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PlantOasis.lib.Util
{
    public class HtmlUtil
    {
        public static MvcHtmlString BreadcrumbsSeparator()
        {
            return MvcHtmlString.Create("<span class='separator'><img src='/Styles/images/ikona-breadcrumb-separator.png' /></span>");
        }

        public static string TextToHtml(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }
            return text.Replace("\r\n", "<br/>").Replace("\n", "<br/>");
        }
    }
}
