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
    public class ProductPriceController : _BaseController
    {      
        public ActionResult GetRecords(string productId)
        {
            ProductPriceListModel model = ProductPriceListModel.CreateCopyFrom(
                new ProductPriceRepository().GetForProduct(new Guid(productId)));
            model.Product = ProductModel.CreateCopyFrom(new ProductRepository().Get(new Guid(productId)), null);

            return View(model);
        }
        public ActionResult InsertRecord(string productId)
        {
            return View("EditRecord", GetProductPriceForEdit(null, productId));
        }

        public ActionResult EditRecord(string id)
        {
            return View(GetProductPriceForEdit(id, null));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveRecord(ProductPriceModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            model.ModelErrors.Clear();
            ProductPriceRepository repository = new ProductPriceRepository();
           ProductPrice dataRec = ProductPriceModel.CreateCopyFrom(model);
            if (dataRec.ValidFrom < DateTime.Today)
            {
                ModelState.AddModelError("ValidFrom", "Dátum platí od nie je možné zadávať do minulosti. Najnižší možný dátum je dnešný dátum.");
            }
            if (dataRec.ValidTo != null)
            {
                if (dataRec.ValidTo.Value < dataRec.ValidFrom)
                {
                    ModelState.AddModelError("ValidTo", "Dátum platí do nemôže byť menší ako dátum platí od.");
                }
            }

            if (ModelState.IsValid)
            {
                if (!repository.Save(dataRec))
                {
                    ModelState.AddModelError("", "Nastala chyba pri zápise záznamu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
                }
            }
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            return this.RedirectToUmbracoPage(ConfigurationUtil.PlantoasisProduktPricesFormId, string.Format("productId={0}", model.ProductKey.ToString()));
        }

        public ActionResult ConfirmDeleteRecord(string id)
        {
            return View(GetProductPriceForEdit(id, null));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRecord(ProductPriceModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            ProductPriceRepository repository = new ProductPriceRepository();
            if (!repository.Delete(ProductPriceModel.CreateCopyFrom(model)))
            {
                ModelState.AddModelError("", "Nastala chyba pri zápise záznamu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
            }
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            return this.RedirectToUmbracoPage(ConfigurationUtil.PlantoasisProduktPricesFormId, string.Format("productId={0}", model.ProductKey.ToString()));
        }


        ProductPriceModel GetProductPriceForEdit(string pk, string productId)
        {
            ProductPriceModel model;
            if (string.IsNullOrEmpty(pk))
            {
                // Create new price as a copy of current standard price 
                model = ProductPriceModel.CreateCopyFrom(new ProductPriceRepository().GetStandardPrice(new Guid(productId)));
                model.pk = Guid.Empty;
                model.ProductKey = new Guid(productId);
                model.ValidFrom = DateTimeUtil.GetDisplayDate(DateTime.Today.AddDays(1));
            }
            else
            {
                model = ProductPriceModel.CreateCopyFrom(new ProductPriceRepository().Get(new Guid(pk)));
            }

            if (model.Product == null)
            {
                model.Product = ProductModel.CreateCopyFrom(new ProductRepository().Get(model.ProductKey), null);
            }

            return model;
        }
    }
}

