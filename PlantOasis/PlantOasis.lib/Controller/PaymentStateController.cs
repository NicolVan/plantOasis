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
    public class PaymentStateController : _BaseController
    {
        public ActionResult GetRecords(string tableId, string sort = "code", string sortDir = "ASC")
        {
            PaymentStateRepository repository = new PaymentStateRepository();

            return View(PaymentStateListModel.CreateCopyFrom(repository.GetRecords(sort, sortDir)));
        }

        public ActionResult InsertRecord()
        {
            return View("EditRecord", new PaymentStateModel());
        }

        public ActionResult EditRecord(string id)
        {
            PaymentStateModel model = PaymentStateModel.CreateCopyFrom(new PaymentStateRepository().Get(new Guid(id)));

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveRecord(PaymentStateModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            if (!new PaymentStateRepository().Save(PaymentStateModel.CreateCopyFrom(model)))
            {
                ModelState.AddModelError("", "Nastala chyba pri zápise záznamu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
            }
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            return this.RedirectToUmbracoPage(ConfigurationUtil.PaymentStatesFormId);
        }

        public ActionResult ConfirmDeleteRecord(string id)
        {
            PaymentStateModel model = PaymentStateModel.CreateCopyFrom(new PaymentStateRepository().Get(new Guid(id)));

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRecord(PaymentStateModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            if (!new PaymentStateRepository().Delete(PaymentStateModel.CreateCopyFrom(model)))
            {
                ModelState.AddModelError("", "Nastala chyba pri zápise záznamu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
            }
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            return this.RedirectToUmbracoPage(ConfigurationUtil.PaymentStatesFormId);
        }
    }
}
