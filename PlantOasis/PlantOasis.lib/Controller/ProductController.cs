
using PlantOasis.lib.Models;
using PlantOasis.lib.Repositories;
using PlantOasis.lib.Util;
using System;
using System.Collections;
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
    public class ProductController : _BaseController
    {

            public ActionResult GetRecords(int page = 1, string sort = "ProductOrder", string sortDir = "DESC")
            {
                try
                {
                    return GetRecordsView(page, sort, sortDir);
                }
                catch
                {
                    ProductFilterModel filter = GetProductFilterForEdit();
                    if (filter != null)
                    {
                        filter.SearchText = string.Empty;
                        UserPropRepository repository = new UserPropRepository();
                        repository.Save(this.CurrentSessionId, ProductFilterModel.CreateCopyFrom(filter));
                    }
                    return GetRecordsView(page, sort, sortDir);
                }
            }
            ActionResult GetRecordsView(int page, string sort, string sortDir)
            {
                ProductFilterModel filter = GetProductFilterForEdit();

                ProductRepository repository = new ProductRepository();
                ProductPagingListModel model = ProductPagingListModel.CreateCopyFrom(
                    repository.GetPage(page, _PagingModel.DefaultItemsPerPage, sort, sortDir,
                        new ProductFilter()
                        {
                            ProductCode = filter.ProductCode,
                            SearchText = filter.SearchText
                        }),
                    GetProductDropDowns()
                    );

                return View(model);
            }

            public ActionResult InsertRecord()
            {
                return View("EditRecord", GetProductForEdit());
            }

            public ActionResult EditRecord(string id)
            {
                ProductRepository repository = new ProductRepository();
                ProductModel model;
                if (string.IsNullOrEmpty(id))
                {
                    model = GetProductForEdit();
                }
                else
                {
                    model = ProductModel.CreateCopyFrom(repository.Get(new Guid(id)), GetProductDropDowns());
                    model.ProductAttributes.pk = model.pk;
                    model.ProductAttributes.Items = Product2AttributeModel.LoadItems(model.pk);
                    model.ProductRelations.pk = model.pk;
                    model.ProductRelations.Items = ProductRelationModel.LoadItems(model.pk);
                    model.ProductCategories.LoadCategories(this, model.pk);
                }

                return View(model);
            }
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult SaveRecord(ProductModel model)
            {
                if (!ModelState.IsValid)
                {
                    return CurrentUmbracoPage();
                }

                model.ModelErrors.Clear();

                ProductRepository repository = new ProductRepository();
                // check product CODE
                Product dupl = repository.GetForProductCode(model.ProductCode);
                if (dupl != null && dupl.pk != model.pk)
                {
                    model.ModelErrors.Add("Zadaný kód produktu už je použitý pre iný produkt.");
                }
                // check product URL
                dupl = repository.GetForProductUrl(model.ProductUrl);
                if (dupl != null && dupl.pk != model.pk)
                {
                    model.ModelErrors.Add("Zadané URL už je použité pre iný produkt.");
                }

                if (model.ModelErrors.Count == 0)
                {
                    Product dataRecord = ProductModel.CreateCopyFrom(model, GetProductDropDowns());
                    if (repository.Save(dataRecord))
                    {
                        model.pk = dataRecord.pk;
                    }
                    else
                    {
                        model.ModelErrors.Add("Nastala chyba pri zápise záznamu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
                    }
                    if (model.ModelErrors.Count == 0)
                    {
                        #region Product attributes
                        model.ProductAttributes.pk = dataRecord.pk; // set the current product key

                        Product2AttributeRepository repAttr = new Product2AttributeRepository();
                        if (!repAttr.DeleteForProduct(model.ProductAttributes.pk))
                        {
                            model.ModelErrors.Add("Nastala chyba pri zápise vlastností produktu systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
                        }
                        else
                        {
                            if (model.ProductAttributes.Items != null)
                            {
                                foreach (Product2AttributeItem item in model.ProductAttributes.Items)
                                {
                                    item.ProductKey = dataRecord.pk; // set the current product key
                                    if (item.IsSelected)
                                    {
                                        if (!repAttr.Insert(Product2AttributeItem.CreateCopyFrom(item)))
                                        {
                                            model.ModelErrors.Add("Nastala chyba pri zápise vlastností produktu systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                    if (model.ModelErrors.Count == 0)
                    {
                        #region Product relations
                        model.ProductRelations.pk = dataRecord.pk; // set the current product key

                        ProductRelationRepository repRel = new ProductRelationRepository();
                        if (!repRel.DeleteForProduct(model.ProductRelations.pk))
                        {
                            model.ModelErrors.Add("Nastala chyba pri zápise súvisiacich produktov do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
                        }
                        else
                        {
                            if (model.ProductRelations.Items != null)
                            {
                                Hashtable htDupl = new Hashtable();
                                foreach (ProductRelationItem item in model.ProductRelations.Items)
                                {
                                    if (htDupl.ContainsKey(item.PkProductRelated))
                                    {
                                        continue;
                                    }
                                    htDupl.Add(item.PkProductRelated, item);
                                    item.PkProductMain = dataRecord.pk; // set the current product key
                                    item.PkProductRelated = item.PkProductRelated; // set the related product key
                                    if (!repRel.Insert(ProductRelationItem.CreateCopyFrom(item)))
                                    {
                                        model.ModelErrors.Add("Nastala chyba pri zápise súvisiacich produktov do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
                                        break;
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                    if (model.ModelErrors.Count == 0)
                    {
                        #region Product categories
                        Guid pkProduct = model.pk;
                        Product2CategoryRepository repCat = new Product2CategoryRepository();
                        if (model.ProductCategories.SelectedCategories != null && model.ProductCategories.SelectedCategories.Count > 0)
                        {
                            // Set product to selected categories
                            Hashtable htCatSaved = new Hashtable();
                            foreach (string categoryKey in model.ProductCategories.SelectedCategories)
                            {
                                if (htCatSaved.ContainsKey(categoryKey))
                                {
                                    continue;
                                }
                                htCatSaved.Add(categoryKey, categoryKey);

                                Guid pkCategory = new Guid(categoryKey);
                                if (repCat.Get(pkCategory, pkProduct) == null)
                                {
                                    // Insert product to category
                                    Product2Category item = new Product2Category();
                                    item.PkProduct = pkProduct;
                                    item.PkCategory = pkCategory;
                                    if (!repCat.Insert(item))
                                    {
                                        model.ModelErrors.Add("Nastala chyba pri zápise kategórií produktu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
                                        break;
                                    }
                                }
                            }

                            // Delete product from not selected categories
                            foreach (Product2Category item in repCat.GetForProduct(pkProduct))
                            {
                                if (!htCatSaved.ContainsKey(item.PkCategory.ToString()))
                                {
                                    // Delete product from category
                                    repCat.Delete(item);
                                }
                            }
                        }
                        else
                        {
                            // Remove product from all categories
                            if (!repCat.DeleteForProduct(model.pk))
                            {
                                model.ModelErrors.Add("Nastala chyba pri zápise kategórií produktu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
                            }
                        }
                        #endregion
                    }
                }

                if (model.ModelErrors.Count > 0)
                {
                    if (model.ProductRelations.Items != null)
                    {
                        model.ProductRelations.SetRelatedProducts();
                    }
                    return RedirectToCurrentUmbracoPageAfterSaveRecord(model);
                }

                ProductFilterModel filter = GetProductFilterForEdit();
                if (filter != null)
                {
                    filter.SearchText = string.Empty;
                    filter.ProductCode = string.Empty;
                    new UserPropRepository().Save(this.CurrentSessionId, ProductFilterModel.CreateCopyFrom(filter));
                }


                return this.RedirectToUmbracoPage(ConfigurationUtil.ProductsFormId);
            }

            public ActionResult ConfirmDeleteRecord(string id)
            {
                ProductRepository repository = new ProductRepository();
                ProductModel model = string.IsNullOrEmpty(id) ? GetProductForEdit() : ProductModel.CreateCopyFrom(repository.Get(new Guid(id)), GetProductDropDowns());

                return View(model);
            }
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult DeleteRecord(ProductModel model)
            {
                if (!ModelState.IsValid)
                {
                    return RedirectToCurrentUmbracoPageAfterSaveRecord(model);
                }

                model.ModelErrors.Clear();

                if (model.ModelErrors.Count == 0)
                {
                    ProductRepository repository = new ProductRepository();
                    if (!repository.Delete(ProductModel.CreateCopyFrom(model, GetProductDropDowns())))
                    {
                        model.ModelErrors.Add("Nastala chyba pri zápise záznamu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
                    }
                }
                if (model.ModelErrors.Count > 0)
                {
                    return RedirectToCurrentUmbracoPageAfterSaveRecord(model);
                }

                return this.RedirectToUmbracoPage(ConfigurationUtil.ProductsFormId);
            }


            RedirectToUmbracoPageResult RedirectToCurrentUmbracoPageAfterSaveRecord(ProductModel rec = null)
            {
                SetProductForEdit(rec);
                return RedirectToCurrentUmbracoPage();
            }
            void SetProductForEdit(ProductModel rec = null)
            {
                TempData["ProductForEdit"] = rec;
            }
            ProductModel GetProductForEdit()
            {
                ProductModel model = TempData["ProductForEdit"] == null ? new ProductModel() : (ProductModel)TempData["ProductForEdit"];
                if (model.ProductAttributes.Items == null)
                {
                    model.ProductAttributes.pk = model.pk;
                    model.ProductAttributes.Items = Product2AttributeModel.LoadItems(model.ProductAttributes.pk);
                }
                if (model.ProductRelations.Items == null)
                {
                    model.ProductRelations.pk = model.pk;
                    model.ProductRelations.Items = ProductRelationModel.LoadItems(model.ProductRelations.pk);
                }
                if (model.ProductCategories.SelectedCategories == null)
                {
                    model.ProductCategories.LoadCategories(this, model.pk);
                }
                if (model.ProductCategories.AllCategories == null)
                {
                    model.ProductCategories.UpdateSelectedCategories(this);
                }
                if (model.DropDowns == null)
                {
                    model.DropDowns = GetProductDropDowns();
                }

                return model;
            }
            ProductModelDropDowns GetProductDropDowns()
            {
                return new ProductModelDropDowns();
            }

            public ActionResult GetFilter()
            {
                return View(GetProductFilterForEdit());
            }
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult SaveFilter(ProductFilterModel model)
            {
                model.ModelErrors.Clear();
                if (model.ModelErrors.Count == 0)
                {
                    UserPropRepository repository = new UserPropRepository();
                    if (!repository.Save(this.CurrentSessionId, ProductFilterModel.CreateCopyFrom(model)))
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
            RedirectToUmbracoPageResult RedirectToCurrentUmbracoPageAfterSaveRecordFilter(ProductFilterModel rec = null)
            {
                SetProductFilterForEdit(rec);
                return RedirectToCurrentUmbracoPage();
            }
            void SetProductFilterForEdit(ProductFilterModel rec = null)
            {
                TempData["ProductFilterForEdit"] = rec;
            }
            ProductFilterModel GetProductFilterForEdit()
            {
                if (TempData["ProductFilterForEdit"] == null)
                {
                    UserPropRepository repository = new UserPropRepository();
                    TempData["ProductFilterForEdit"] = ProductFilterModel.CreateCopyFrom(repository.Get(this.CurrentSessionId, ConfigurationUtil.PropId_ProductFilterModel));
                }

                return (ProductFilterModel)TempData["ProductFilterForEdit"];
            }

            public ActionResult EditImages(string id)
            {
                ProductImagesModel model = ProductImagesModel.LoadModel(new Guid(id));

                return View(model);
            }
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult SaveImages(ProductImagesModel model)
            {
                if (!ModelState.IsValid)
                {
                    return CurrentUmbracoPage();
                }

                model.Save();

                return this.RedirectToUmbracoPage(ConfigurationUtil.ProductsFormId);
            }
        }
    }

