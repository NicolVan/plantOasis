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
    public class Product2CategoryController : _BaseCategoryController
    {
        #region Products in category
        [Authorize(Roles = MemberRepository.PlantOasisMemberAdminRole)]
        public ActionResult GetRecordsInCategory(string id, int page = 1, string sort = "ProductOrder", string sortDir = "ASC")
        {
            try
            {
                return GetRecordsInCategoryView(id, page, sort, sortDir);
            }
            catch
            {
                ProductInCategoryFilterModel filter = GetProductInCategoryFilterForEdit(id);
                if (filter != null)
                {
                    filter.SearchText = string.Empty;
                    UserPropRepository repository = new UserPropRepository();
                    repository.Save(this.CurrentSessionId, ProductInCategoryFilterModel.CreateCopyFrom(filter));
                }

                return GetRecordsInCategoryView(id, page, sort, sortDir);
            }
        }
        ActionResult GetRecordsInCategoryView(string id, int page, string sort, string sortDir)
        {
            Guid keyCategory = new Guid(id);
            Product2CategoryRepository repository = new Product2CategoryRepository();

            ProductInCategoryFilterModel filter = GetProductInCategoryFilterForEdit(id);

            Product2CategoryItemsModel model = Product2CategoryItemsModel.CreateCopyFrom(
                repository.GetPage(page, _PagingModel.DefaultItemsPerPage, keyCategory, sort, sortDir,
                    new Product2CategoryFilter()
                    {
                        ProductCode = filter.ProductCode,
                        SearchText = filter.SearchText
                    })
                );
            model.CategoryKey = keyCategory;

            if (model.Items.Count > 0)
            {
                ProductRepository prodRep = new ProductRepository();
                model.BindProducts(
                    prodRep.GetPageForCategory(1, _PagingModel.AllItemsPerPage, model.CategoryKey).Items,
                    new ProductModelDropDowns());
            }

            return View(model);
        }

        [Authorize(Roles = MemberRepository.PlantOasisMemberAdminRole)]
        public ActionResult GetInCategoryFilter(string id)
        {
            return View(GetProductInCategoryFilterForEdit(id));
        }
        [HttpPost]
        [Authorize(Roles = MemberRepository.PlantOasisMemberAdminRole)] 
        [ValidateAntiForgeryToken]
        public ActionResult SaveInCategoryFilter(ProductInCategoryFilterModel model)
        {
            SetReturnToCategory(model.PkCategory, _BaseCategoryController.ReturnToTabVal_ProdInCat);

            model.ModelErrors.Clear();
            if (model.ModelErrors.Count == 0)
            {
                UserPropRepository repository = new UserPropRepository();
                if (!repository.Save(this.CurrentSessionId, ProductInCategoryFilterModel.CreateCopyFrom(model)))
                {
                    model.ModelErrors.Add("Nastala chyba pri zápise záznamu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
                }
            }
            if (model.ModelErrors.Count > 0)
            {
                return RedirectToCurrentUmbracoPageAfterSaveRecordInCategoryFilter(model);
            }

            return RedirectToCurrentUmbracoPageAfterSaveRecordInCategoryFilter();
        }
        RedirectToUmbracoPageResult RedirectToCurrentUmbracoPageAfterSaveRecordInCategoryFilter(ProductInCategoryFilterModel rec = null)
        {
            SetProductInCategoryFilterForEdit(rec);
            return RedirectToCurrentUmbracoPage(GetReturnToCategoryQueryString());
        }
        void SetProductInCategoryFilterForEdit(ProductInCategoryFilterModel rec = null)
        {
            TempData["ProductInCategoryFilterForEdit"] = rec;
        }
        ProductInCategoryFilterModel GetProductInCategoryFilterForEdit(string id)
        {
            if (TempData["ProductInCategoryFilterForEdit"] == null)
            {
              UserPropRepository repository = new UserPropRepository();
                TempData["ProductInCategoryFilterForEdit"] = ProductInCategoryFilterModel.CreateCopyFrom(repository.Get(this.CurrentSessionId, ConfigurationUtil.PropId_ProductInCategoryFilterModel));
            }

            ProductInCategoryFilterModel ret = (ProductInCategoryFilterModel)TempData["ProductInCategoryFilterForEdit"];
            ret.PkCategory = string.IsNullOrEmpty(id) ? Guid.Empty : new Guid(id);

            return ret;
        }
        #endregion

        #region Products not in category
        [Authorize(Roles = MemberRepository.PlantOasisMemberAdminRole)]
        public ActionResult GetRecordsNotInCategory(string id, int page = 1, string sort = "ProductOrder", string sortDir = "ASC")
        {
            try
            {
                return GetRecordsNotInCategoryView(id, page, sort, sortDir);
            }
            catch
            {
                ProductNotInCategoryFilterModel filter = GetProductNotInCategoryFilterForEdit(id);
                if (filter != null)
                {
                    filter.SearchText = string.Empty;
                    UserPropRepository repository = new UserPropRepository();
                    repository.Save(this.CurrentSessionId, ProductNotInCategoryFilterModel.CreateCopyFrom(filter));
                }

                return GetRecordsNotInCategoryView(id, page, sort, sortDir);
            }
        }
        ActionResult GetRecordsNotInCategoryView(string id, int page, string sort, string sortDir)
        {
            Guid keyCategory = new Guid(id);
          ProductRepository repository = new ProductRepository();

            ProductNotInCategoryFilterModel filter = GetProductNotInCategoryFilterForEdit(id);

            Product2CategoryItemsModel model = Product2CategoryItemsModel.CreateCopyFrom(keyCategory,
                repository.GetPageForNotInCategory(keyCategory, page, _PagingModel.DefaultItemsPerPage, sort, sortDir,
                    new Product2CategoryFilter()
                    {
                        ProductCode = filter.ProductCode,
                        SearchText = filter.SearchText
                    }),
                new ProductModelDropDowns()
                );
            model.CategoryKey = keyCategory;

            return View(model);
        }

        [Authorize(Roles = MemberRepository.PlantOasisMemberAdminRole)]
        public ActionResult GetNotInCategoryFilter(string id)
        {
            return View(GetProductNotInCategoryFilterForEdit(id));
        }
        [HttpPost]
        [Authorize(Roles = MemberRepository.PlantOasisMemberAdminRole)]
        [ValidateAntiForgeryToken]
        public ActionResult SaveNotInCategoryFilter(ProductNotInCategoryFilterModel model)
        {
            SetReturnToCategory(model.PkCategory, _BaseCategoryController.ReturnToTabVal_ProdNotInCat);

            model.ModelErrors.Clear();
            if (model.ModelErrors.Count == 0)
            {
                UserPropRepository repository = new UserPropRepository();
                if (!repository.Save(this.CurrentSessionId, ProductNotInCategoryFilterModel.CreateCopyFrom(model)))
                {
                    model.ModelErrors.Add("Nastala chyba pri zápise záznamu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
                }
            }
            if (model.ModelErrors.Count > 0)
            {
                return RedirectToCurrentUmbracoPageAfterSaveRecordNotInCategoryFilter(model);
            }

            return RedirectToCurrentUmbracoPageAfterSaveRecordNotInCategoryFilter();
        }
        RedirectToUmbracoPageResult RedirectToCurrentUmbracoPageAfterSaveRecordNotInCategoryFilter(ProductNotInCategoryFilterModel rec = null)
        {
            SetProductNotInCategoryFilterForEdit(rec);
            return RedirectToCurrentUmbracoPage(GetReturnToCategoryQueryString());
        }
        void SetProductNotInCategoryFilterForEdit(ProductNotInCategoryFilterModel rec = null)
        {
            TempData["ProductNotInCategoryFilterForEdit"] = rec;
        }
        ProductNotInCategoryFilterModel GetProductNotInCategoryFilterForEdit(string id)
        {
            if (TempData["ProductNotInCategoryFilterForEdit"] == null)
            {
                UserPropRepository repository = new UserPropRepository();
                TempData["ProductNotInCategoryFilterForEdit"] = ProductNotInCategoryFilterModel.CreateCopyFrom(repository.Get(this.CurrentSessionId, ConfigurationUtil.PropId_ProductNotInCategoryFilterModel));
            }

            ProductNotInCategoryFilterModel ret = (ProductNotInCategoryFilterModel)TempData["ProductNotInCategoryFilterForEdit"];
            ret.PkCategory = string.IsNullOrEmpty(id) ? Guid.Empty : new Guid(id);

            return ret;
        }
        #endregion


        [Authorize(Roles = MemberRepository.PlantOasisMemberAdminRole)]
        public ActionResult InsertProduct(string id, string prodid)
        {
            SetReturnToCategory(new Guid(id), _BaseCategoryController.ReturnToTabVal_ProdNotInCat);

            return this.RedirectToUmbracoPage(ConfigurationUtil.CategoriesFormId, GetReturnToCategoryQueryString());
        }

        [Authorize(Roles = MemberRepository.PlantOasisMemberAdminRole)]
        public ActionResult DeleteProduct(string id, string prodid)
        {
            SetReturnToCategory(new Guid(id), _BaseCategoryController.ReturnToTabVal_ProdInCat);

            return this.RedirectToUmbracoPage(ConfigurationUtil.CategoriesFormId, GetReturnToCategoryQueryString());
        }
    }
}


