using PlantOasis.lib.Models;
using PlantOasis.lib.Repositories;
using PlantOasis.lib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace PlantOasis.lib.Controller
{
    [PluginController("PlantOasis")]
    [Authorize(Roles = MemberRepository.PlantOasisMemberAdminRole)]
    public class SysConstController : _BaseController
    {
        public ActionResult EditRecord()
        {
            return View(GetSysConstForEdit());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveRecord(SysConstModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            if (!new SysConstRepository().Save(SysConstModel.CreateCopyFrom(model)))
            {
                ModelState.AddModelError("", "Nastala chyba pri zápise záznamu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
            }
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            return this.RedirectToUmbracoPage(ConfigurationUtil.AfterLoginFormId);
        }


        SysConstModel GetSysConstForEdit()
        {
            return SysConstModel.CreateCopyFrom(new SysConstRepository().Get());
        }
    }
}

