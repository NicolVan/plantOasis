using PlantOasis.lib.Controller;
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
    public class TransportTypeController : _BaseController
    {
        public ActionResult GetRecords(int page = 1, string sort = "TransportOrder", string sortDir = "ASC")
        {
            TransportTypeRepository repository = new TransportTypeRepository();
            TransportTypePagingListModel model = TransportTypePagingListModel.CreateCopyFrom(
                repository.GetPage(page, _PagingModel.DefaultItemsPerPage, sort, sortDir));

            return View(model);
        }

        public ActionResult InsertRecord()
        {
            return View("EditRecord", new TransportTypeModel());
        }

        public ActionResult EditRecord(string id)
        {
            TransportTypeModel model = TransportTypeModel.CreateCopyFrom(new TransportTypeRepository().Get(new Guid(id)));

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveRecord(TransportTypeModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            TransportTypeRepository repository = new TransportTypeRepository();
            if (!repository.Save(TransportTypeModel.CreateCopyFrom(model)))
            {
                ModelState.AddModelError("", "Nastala chyba pri zápise záznamu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
            }
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            return this.RedirectToUmbracoPage(ConfigurationUtil.TransportTypesFormId);
        }

        public ActionResult ConfirmDeleteRecord(string id)
        {
            TransportTypeModel model = TransportTypeModel.CreateCopyFrom(new TransportTypeRepository().Get(new Guid(id)));

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRecord(TransportTypeModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            model.ModelErrors.Clear();

            TransportTypeRepository repository = new TransportTypeRepository();
            if (!repository.Delete(TransportTypeModel.CreateCopyFrom(model)))
            {
                ModelState.AddModelError("", "Nastala chyba pri zápise záznamu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
            }
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            return this.RedirectToUmbracoPage(ConfigurationUtil.TransportTypesFormId);
        }
    }
}