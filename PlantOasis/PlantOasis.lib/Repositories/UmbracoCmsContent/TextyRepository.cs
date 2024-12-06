using PlantOasis.lib.Models.UmbracoCmsContent;
using PlantOasis.lib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace PlantOasis.lib.Repositories.UmbracoCmsContent
{
    public class TextyRepository
    {
        public const int TextyId_Sk = 1143;
        public const int TextyId_En = 1144;
        public static Texty GetFromUmbraco(UmbracoHelper umbraco)
        {
            string cultureId = CurrentLang.GetCurrentCultureId();

            IPublishedContent content = umbraco.Content(cultureId == CurrentLang.CultureId_En ? TextyId_En : TextyId_Sk);

            return content == null ? null : new Texty(content);
        }
    }

}