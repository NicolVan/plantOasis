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
    public class QuoteStateController : _BaseController
    {
        public ActionResult GetRecords(string tableId, string sort = "code", string sortDir = "ASC")
        {
            QuoteStateRepository repository = new QuoteStateRepository();

            return View(QuoteStateListModel.CreateCopyFrom(repository.GetRecords(sort, sortDir)));
        }

        public ActionResult InsertRecord()
        {
            return View("EditRecord", new QuoteStateModel());
        }

        public ActionResult EditRecord(string id)
        {
            QuoteStateModel model = QuoteStateModel.CreateCopyFrom(new QuoteStateRepository().Get(new Guid(id)));

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveRecord(QuoteStateModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            if (!new QuoteStateRepository().Save(QuoteStateModel.CreateCopyFrom(model)))
            {
                ModelState.AddModelError("", "Nastala chyba pri zápise záznamu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
            }
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            return this.RedirectToUmbracoPage(ConfigurationUtil.QuoteStatesFormId);
        }

        public ActionResult ConfirmDeleteRecord(string id)
        {
            QuoteStateModel model = QuoteStateModel.CreateCopyFrom(new QuoteStateRepository().Get(new Guid(id)));

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRecord(QuoteStateModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            if (!new QuoteStateRepository().Delete(QuoteStateModel.CreateCopyFrom(model)))
            {
                ModelState.AddModelError("", "Nastala chyba pri zápise záznamu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
            }
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            return this.RedirectToUmbracoPage(ConfigurationUtil.QuoteStatesFormId);
        }
    }
}

