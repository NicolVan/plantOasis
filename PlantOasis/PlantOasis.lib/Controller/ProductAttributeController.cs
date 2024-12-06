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
    public class ProductAttributeController : _BaseController
    {
        public ActionResult GetRecords(int page = 1, string sort = null, string sortDir = null)
        {
            try
            {
                return GetRecordsView(page, sort, sortDir);
            }
            catch
            {
                ProduktAttributeFilterModel filter = GetProductAttributeFilterForEdit();
                if (filter != null)
                {
                    filter.SearchText = string.Empty;
                    UserPropRepository repository = new UserPropRepository();
                    repository.Save(this.CurrentSessionId, ProduktAttributeFilterModel.CreateCopyFrom(filter));
                }
                return GetRecordsView(page, sort, sortDir);
            }
        }
        ActionResult GetRecordsView(int page, string sort, string sortDir)
        {
            ProduktAttributeFilterModel filter = GetProductAttributeFilterForEdit();

           ProductAttributeRepository repository = new ProductAttributeRepository();
            ProduktAttributePagingListModel model = ProduktAttributePagingListModel.CreateCopyFrom(
                repository.GetPage(page, _PagingModel.DefaultItemsPerPage, sort, sortDir,
                    new ProductAttributeFilter()
                    {
                        SearchText = filter.SearchText
                    })
                );

            return View(model);
        }

        public ActionResult InsertRecord()
        {
            return View("EditRecord", new ProductAttributeModel());
        }

        public ActionResult EditRecord(string id)
        {
            ProductAttributeModel model = ProductAttributeModel.CreateCopyFrom(new ProductAttributeRepository().Get(new Guid(id)));

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveRecord(ProductAttributeModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            ProductAttributeRepository repository = new ProductAttributeRepository();
            if (!repository.Save(ProductAttributeModel.CreateCopyFrom(model)))
            {
                ModelState.AddModelError("", "Nastala chyba pri zápise záznamu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
            }
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            return this.RedirectToUmbracoPage(ConfigurationUtil.ProductAttributesFormId);
        }

        public ActionResult ConfirmDeleteRecord(string id)
        {
            ProductAttributeModel model = ProductAttributeModel.CreateCopyFrom(new ProductAttributeRepository().Get(new Guid(id)));

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRecord(ProductAttributeModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            ProductAttributeRepository repository = new ProductAttributeRepository();
            if (!repository.Delete(ProductAttributeModel.CreateCopyFrom(model)))
            {
                ModelState.AddModelError("", "Nastala chyba pri zápise záznamu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
            }
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            return this.RedirectToUmbracoPage(ConfigurationUtil.ProductAttributesFormId);
        }

        public ActionResult GetFilter()
        {
            return View(GetProductAttributeFilterForEdit());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveFilter(ProduktAttributeFilterModel model)
        {
            UserPropRepository repository = new UserPropRepository();
            if (!repository.Save(this.CurrentSessionId, ProduktAttributeFilterModel.CreateCopyFrom(model)))
            {
                ModelState.AddModelError("", "Nastala chyba pri zápise záznamu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
            }
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            return RedirectToCurrentUmbracoPage();
        }
        ProduktAttributeFilterModel GetProductAttributeFilterForEdit()
        {
            return ProduktAttributeFilterModel.CreateCopyFrom(new UserPropRepository().Get(this.CurrentSessionId, ConfigurationUtil.PropId_ProductAttributeFilterModel));
        }
    }
}
