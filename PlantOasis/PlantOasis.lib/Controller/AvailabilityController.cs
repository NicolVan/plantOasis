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
    public class AvailabilityController : _BaseController
    {
        public ActionResult GetRecords(int page = 1, string sort = "AvailabilityName", string sortDir = "ASC")
        {
           AvailabilityRepository repository = new AvailabilityRepository();
            AvailabilityPagingListModel model = AvailabilityPagingListModel.CreateCopyFrom(
                repository.GetPage(page, _PagingModel.DefaultItemsPerPage, sort, sortDir));

            return View(model);
        }

        public ActionResult InsertRecord()
        {
            return View("EditRecord", new AvailabilityModel());
        }

        public ActionResult EditRecord(string id)
        {
            AvailabilityModel model = AvailabilityModel.CreateCopyFrom(new AvailabilityRepository().Get(new Guid(id)));

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveRecord(AvailabilityModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            AvailabilityRepository repository = new AvailabilityRepository();
            if (!repository.Save(AvailabilityModel.CreateCopyFrom(model)))
            {
                ModelState.AddModelError("", "Nastala chyba pri zápise záznamu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
            }
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            return this.RedirectToUmbracoPage(ConfigurationUtil.AvailabilitiesFormId);
        }



        public ActionResult ConfirmDeleteRecord(string id)
        {
            AvailabilityModel model = AvailabilityModel.CreateCopyFrom(new AvailabilityRepository().Get(new Guid(id)));

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRecord(AvailabilityModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            AvailabilityRepository repository = new AvailabilityRepository();
            if (!repository.Delete(AvailabilityModel.CreateCopyFrom(model)))
            {
                ModelState.AddModelError("", "Nastala chyba pri zápise záznamu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
            }
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            return this.RedirectToUmbracoPage(ConfigurationUtil.AvailabilitiesFormId);
        }
    }
}
