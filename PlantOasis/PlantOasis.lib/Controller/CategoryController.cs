using PlantOasis.lib.Models.Pdf;
using PlantOasis.lib.Models;
using PlantOasis.lib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web;
using Umbraco.Web.Mvc;
using PlantOasis.lib.Repositories;
using PlantOasis.lib.Task;
using System.Diagnostics;

namespace PlantOasis.lib.Controller
{
    public class _BaseCategoryController : _BaseController
    {
        public const string ReturnToCategoryKey = "ReturnToCategory";

        public const string ReturnToTabKey = "ReturnToTab";
        public const string ReturnToTabVal_Subcateg = "Subcategories";
        public const string ReturnToTabVal_ProdInCat = "ProductsInCategory";
        public const string ReturnToTabVal_ProdNotInCat = "ProductsNotInCategory";

        public const string ReturnToPageKey = "ReturnToPage";

        public void SetReturnToCategory(Guid categoryKey, string tab)
        {
            TempData[_BaseCategoryController.ReturnToCategoryKey] = categoryKey;
            TempData[_BaseCategoryController.ReturnToTabKey] = tab;
        }
        public void SetReturnToPage(int page)
        {
            TempData[_BaseCategoryController.ReturnToCategoryKey] = page.ToString();
        }
        public string GetReturnToCategory()
        {
            Guid categoryKey = TempData[_BaseCategoryController.ReturnToCategoryKey] == null ? Guid.Empty : (Guid)TempData[_BaseCategoryController.ReturnToCategoryKey];

            return categoryKey == null || categoryKey == Guid.Empty ? null : categoryKey.ToString();
        }
        public string GetReturnToTab()
        {
            return TempData[_BaseCategoryController.ReturnToTabKey] == null ? _BaseCategoryController.ReturnToTabVal_Subcateg : (string)TempData[_BaseCategoryController.ReturnToTabKey];
        }
        public string GetReturnToPage()
        {
            return TempData[_BaseCategoryController.ReturnToPageKey] == null ? "1" : (string)TempData[_BaseCategoryController.ReturnToPageKey];
        }

        public string GetReturnToCategoryQueryString()
        {
            return string.Format("id={0}&tab={1}&page={2}", GetReturnToCategory(), GetReturnToTab(), GetReturnToPage());
        }
    }

    [PluginController("PlantOasis")]
    [Authorize(Roles = MemberRepository.PlantOasisMemberAdminRole)]

    public class CategoryController : _BaseCategoryController
    {

        public ActionResult GetRecords(string id, string tab)
        {
            if (string.IsNullOrEmpty(id))
            {
                // Try to get category ID after category edit event
                id = GetReturnToCategory();
            }
            CategoryRepository repository = new CategoryRepository();
            CategoryModel model = string.IsNullOrEmpty(id) ? new CategoryModel() : CategoryModel.CreateCopyFrom(repository.Get(new Guid(id)));
            model.LoadRelatives(repository);
            model.TabId = string.IsNullOrEmpty(tab) ? GetReturnToTab() : tab;

            return View(model);
        }

        public ActionResult InsertRecord(string id)
        {
            CategoryModel model = GetCategoryForEdit();
            if (!string.IsNullOrEmpty(id))
            {
                model.ParentCategoryKey = new Guid(id);
                model.LoadRelatives(new CategoryRepository());
            }
            return View("EditRecord", model);
        }

        public ActionResult EditRecord(string id)
        {
            CategoryRepository repository = new CategoryRepository();
            CategoryModel model = string.IsNullOrEmpty(id) ? GetCategoryForEdit() : CategoryModel.CreateCopyFrom(repository.Get(new Guid(id)));
            if (model.Children == null || model.Parents == null)
            {
                model.LoadRelatives(repository);
            }

            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveRecord(CategoryModel model)
        {
            SetReturnToCategory(model.ParentCategoryKey, _BaseCategoryController.ReturnToTabVal_Subcateg);

            if (!ModelState.IsValid)
            {
                return RedirectToCurrentUmbracoPageAfterSaveRecord(model);
            }

            model.ModelErrors.Clear();

            CategoryRepository repository = new CategoryRepository();
            // check category CODE
            Category dupl = repository.GetForCategoryCode(model.CategoryCode);
            if (dupl != null && dupl.pk != model.pk)
            {
                model.ModelErrors.Add("Zadaný kód kategórie už je použitý pre inú kategóriu.");
            }
            // check category URL
            dupl = repository.GetForCategoryUrl(model.CategoryUrl);
            if (dupl != null && dupl.pk != model.pk)
            {
                model.ModelErrors.Add("Zadané URL už je použité pre inú kategóriu.");
            }
            if (model.ModelErrors.Count == 0)
            {
                if (!repository.Save(CategoryModel.CreateCopyFrom(model)))
                {
                    model.ModelErrors.Add("Nastala chyba pri zápise záznamu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
                }
            }
            if (model.ModelErrors.Count > 0)
            {
                return RedirectToCurrentUmbracoPageAfterSaveRecord(model);
            }

            return this.RedirectToUmbracoPage(ConfigurationUtil.CategoriesFormId, GetReturnToCategoryQueryString());
        }

        public ActionResult ConfirmDeleteRecord(string id)
        {
          CategoryRepository repository = new CategoryRepository();
            CategoryModel model = string.IsNullOrEmpty(id) ? GetCategoryForEdit() : CategoryModel.CreateCopyFrom(repository.Get(new Guid(id)));

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRecord(CategoryModel model)
        {
            SetReturnToCategory(model.ParentCategoryKey, _BaseCategoryController.ReturnToTabVal_Subcateg);

            if (!ModelState.IsValid)
            {
                return RedirectToCurrentUmbracoPageAfterSaveRecord(model);
            }

            model.ModelErrors.Clear();

            if (model.ModelErrors.Count == 0)
            {
                CategoryRepository repository = new CategoryRepository();
                if (!repository.DeleteRecursive(CategoryModel.CreateCopyFrom(model), true))
                {
                    model.ModelErrors.Add("Nastala chyba pri zápise záznamu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
                }
            }
            if (model.ModelErrors.Count > 0)
            {
                return RedirectToCurrentUmbracoPageAfterSaveRecord(model);
            }

            return this.RedirectToUmbracoPage(ConfigurationUtil.CategoriesFormId, GetReturnToCategoryQueryString());
        }

        RedirectToUmbracoPageResult RedirectToCurrentUmbracoPageAfterSaveRecord(CategoryModel rec = null)
        {
            SetCategoryForEdit(rec);
            return RedirectToCurrentUmbracoPage();
        }
        void SetCategoryForEdit(CategoryModel rec = null)
        {
            TempData["CategoryForEdit"] = rec;
        }
        CategoryModel GetCategoryForEdit()
        {
            CategoryModel model = TempData["CategoryForEdit"] == null ? new CategoryModel() : (CategoryModel)TempData["CategoryForEdit"];
            if (model.Children == null || model.Parents == null)
            {
                model.LoadRelatives(new CategoryRepository());
            }

            return model;
        }

        public ActionResult GetCategoryOfferPdf(string id)
        {
            PdfFilePrintResult pdfResult = new CategoryOfferToPdf(new Guid(id), this.DefaultImgPath).GetPdf();

            ActionResult ret = PdfDownloadResult.GetActionResult(pdfResult.FileContent, pdfResult.FileName);
            if (ret == null)
            {
                throw new HttpException(404, "Error generating PDF");
            }

            return ret;
        }
    }
}

