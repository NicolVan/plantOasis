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
    public class CountryController : _BaseController
    {
        public ActionResult GetRecords(int page = 1, string sort = "Code", string sortDir = "ASC")
        {
            CountryPagingListModel model = CountryPagingListModel.CreateCopyFrom(
                new CountryRepository().GetPage(page, _PagingModel.DefaultItemsPerPage, sort, sortDir));

            return View(model);
        }

        public ActionResult InsertRecord()
        {
            return View("EditRecord", GetCountryForEdit(null));
        }
        public ActionResult EditRecord(string id)
        {
            return View(GetCountryForEdit(id));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveRecord(CountryModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            if (!new CountryRepository().Save(CountryModel.CreateCopyFrom(model)))
            {
                ModelState.AddModelError("", "Nastala chyba pri zápise záznamu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
            }
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            return this.RedirectToUmbracoPage(ConfigurationUtil.CountriesFormId);
        }

        public ActionResult ConfirmDeleteRecord(string id)
        {
            return View(GetCountryForEdit(id));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRecord(CountryModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            if (!new CountryRepository().Delete(CountryModel.CreateCopyFrom(model)))
            {
                ModelState.AddModelError("", "Nastala chyba pri zápise záznamu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
            }

            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            return this.RedirectToUmbracoPage(ConfigurationUtil.CountriesFormId);
        }


        CountryModel GetCountryForEdit(string id)
        {
            return string.IsNullOrEmpty(id) ? new CountryModel() : CountryModel.CreateCopyFrom(new CountryRepository().Get(new Guid(id)));
        }
    }
}
