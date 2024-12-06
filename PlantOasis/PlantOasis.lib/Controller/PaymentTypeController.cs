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
    public class PaymentTypeController : _BaseController
    {
        public ActionResult GetRecords(int page = 1, string sort = "PaymentOrder", string sortDir = "ASC")
        {
            PaymentTypeRepository repository = new PaymentTypeRepository();
            PaymentTypePagingListModel model = PaymentTypePagingListModel.CreateCopyFrom(
                repository.GetPage(page, _PagingModel.DefaultItemsPerPage, sort, sortDir));

            return View(model);
        }

        public ActionResult InsertRecord()
        {
            return View("EditRecord", new PaymentTypeModel());
        }

        public ActionResult EditRecord(string id)
        {
            PaymentTypeModel model = PaymentTypeModel.CreateCopyFrom(new PaymentTypeRepository().Get(new Guid(id)));

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveRecord(PaymentTypeModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            PaymentTypeRepository repository = new PaymentTypeRepository();
            if (!repository.Save(PaymentTypeModel.CreateCopyFrom(model)))
            {
                ModelState.AddModelError("", "Nastala chyba pri zápise záznamu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
            }
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            return this.RedirectToUmbracoPage(ConfigurationUtil.PaymentTypesFormId);
        }

        public ActionResult ConfirmDeleteRecord(string id)
        {
            PaymentTypeModel model = PaymentTypeModel.CreateCopyFrom(new PaymentTypeRepository().Get(new Guid(id)));

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRecord(PaymentTypeModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            PaymentTypeRepository repository = new PaymentTypeRepository();
            if (!repository.Delete(PaymentTypeModel.CreateCopyFrom(model)))
            {
                ModelState.AddModelError("", "Nastala chyba pri zápise záznamu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
            }
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            return this.RedirectToUmbracoPage(ConfigurationUtil.PaymentTypesFormId);
        }
    }
}

