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
    public class DodavatelController : _BaseController
    {
        public ActionResult GetRecords(int page = 1, string sort = "ProducerName", string sortDir = "ASC")
        {
            try
            {
                return GetRecordsView(page, sort, sortDir);
            }
            catch
            {
                DodavatelFilterModel filter = GetProducerFilterForEdit();
                if (filter != null)
                {
                    filter.SearchText = string.Empty;
                    UserPropRepository repository = new UserPropRepository();
                    repository.Save(this.CurrentSessionId, DodavatelFilterModel.CreateCopyFrom(filter));
                }
                return GetRecordsView(page, sort, sortDir);
            }
        }
        ActionResult GetRecordsView(int page, string sort, string sortDir)
        {
            DodavatelFilterModel filter = GetProducerFilterForEdit();

           DodavatelRepository repository = new DodavatelRepository();
            DodavatelPagingListModel model = DodavatelPagingListModel.CreateCopyFrom(
                repository.GetPage(page, _PagingModel.DefaultItemsPerPage, sort, sortDir,
                    new DodavatelFilter()
                    {
                        SearchText = filter.SearchText
                    })
                );

            return View(model);
        }

        public ActionResult InsertRecord()
        {
            return View("EditRecord", new DodavatelModel());
        }

        public ActionResult EditRecord(string id)
        {
            DodavatelModel model = DodavatelModel.CreateCopyFrom(new DodavatelRepository().Get(new Guid(id)));

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveRecord(DodavatelModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            if (!new DodavatelRepository().Save(DodavatelModel.CreateCopyFrom(model)))
            {
                ModelState.AddModelError("", "Nastala chyba pri zápise záznamu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
            }
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            return this.RedirectToUmbracoPage(ConfigurationUtil.DodavatelFormId);
        }

        public ActionResult ConfirmDeleteRecord(string id)
        {
            DodavatelModel model = DodavatelModel.CreateCopyFrom(new DodavatelRepository().Get(new Guid(id)));

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRecord(DodavatelModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            DodavatelRepository repository = new DodavatelRepository();
            try
            {
                if (!repository.Delete(DodavatelModel.CreateCopyFrom(model)))
                {
                    ModelState.AddModelError("", "Nastala chyba pri zápise záznamu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
                }
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("", "Výrobcu nie je možné odstrániť pretože je priradený k niektorým produktom.");
                this.Logger.Error(typeof(DodavatelController), "DeleteRecord error", exc);
            }
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            return this.RedirectToUmbracoPage(ConfigurationUtil.DodavatelFormId);
        }


        public ActionResult GetFilter()
        {
            return View(GetProducerFilterForEdit());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveFilter(DodavatelFilterModel model)
        {
            model.ModelErrors.Clear();
            if (model.ModelErrors.Count == 0)
            {
                UserPropRepository repository = new UserPropRepository();
                if (!repository.Save(this.CurrentSessionId, DodavatelFilterModel.CreateCopyFrom(model)))
                {
                    model.ModelErrors.Add("Nastala chyba pri zápise záznamu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
                }
            }
            if (model.ModelErrors.Count > 0)
            {
                return RedirectToCurrentUmbracoPageAfterSaveRecordFilter(model);
            }

            return RedirectToCurrentUmbracoPageAfterSaveRecordFilter();
        }
        RedirectToUmbracoPageResult RedirectToCurrentUmbracoPageAfterSaveRecordFilter(DodavatelFilterModel rec = null)
        {
            SetProducerFilterForEdit(rec);
            return RedirectToCurrentUmbracoPage();
        }
        void SetProducerFilterForEdit(DodavatelFilterModel rec = null)
        {
            TempData["stirilabProducerFilterForEdit"] = rec;
        }
        DodavatelFilterModel GetProducerFilterForEdit()
        {
            if (TempData["stirilabProducerFilterForEdit"] == null)
            {
                UserPropRepository repository = new UserPropRepository();
                TempData["stirilabProducerFilterForEdit"] = DodavatelFilterModel.CreateCopyFrom(repository.Get(this.CurrentSessionId, ConfigurationUtil.PropId_DodavatelFilterModel));
            }

            return (DodavatelFilterModel)TempData["stirilabProducerFilterForEdit"];
        }
    }
}
