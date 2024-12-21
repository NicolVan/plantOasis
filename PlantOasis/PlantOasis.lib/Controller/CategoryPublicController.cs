using PlantOasis.lib.Models;
using PlantOasis.lib.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace PlantOasis.lib.Controller
{

    [PluginController("PlantOasis")]
    public class CategoryPublicController : _BaseController
    {
        public ActionResult PromotedCategory(string categoryCode)
        {
            CategoryModel model = CategoryModel.CreateCopyFrom(new CategoryRepository().GetForCategoryCode(categoryCode));

            return View(model);
        }

        public ActionResult QuickOrder(string categoryCode)
        {
            new CategoryPublicFilterModel().SetProductView(this.CurrentSessionId, CategoryPublicFilterModel_ProductView.ProductView_List);

            return Redirect(CategoryModel.CreateCopyFrom(Category.RootCategory()).Url);
        }

        public ActionResult CategoryDetail()
        {
            if (this.CurrentRequest.Params[CategoryPublicModel.cQuickOrderParamName] == "1")
            {
                // Switch to list view
                new CategoryPublicFilterModel().SetProductView(this.CurrentSessionId, CategoryPublicFilterModel_ProductView.ProductView_List);
            }
            // Load category data
            CategoryPublicModel model = new CategoryPublicModel(this, null);
            // Set current category data for eshop model
            this.SetCurrentProductCategoryModel(model);
            // Set SEO data
            if (model.SeoData != null)
            {
                this.SetSeoModel(model.SeoData);
            }
            // Set eshop global data
            model.EshopData = this.GetCurrentEshopModel();

            return View(model);
        }
        public ActionResult GetProductsFavorite()
        {
            ProductSearchModel searchModel = new ProductSearchModel()
            {
                CustomerKey = CustomerModel.IsUserAuthenticated() ? CustomerModel.GetCurrentCustomer().pk : Guid.Empty,
                Action = ProductSearchModel.ModelType.Favorite
            };
            CategoryPublicModel model = new CategoryPublicModel(this, searchModel);
            // Set eshop global data
            model.EshopData = this.GetCurrentEshopModel();

            return View("CategoryDetail", model);
        }
        public ActionResult GetProductsForSearch()
        {
            string productToSearch = this.CurrentRequest.Params["srchprod"];
            if (Regex.IsMatch(productToSearch ?? string.Empty, @"[\'\""]"))
            {
                ModelState.AddModelError("ProductToSearch", "Úvodzovky a apostrofy nie sú povolené.");
                return View("CategoryDetail");
            }
            productToSearch = Regex.Replace(productToSearch ?? string.Empty, @"[^a-zA-Z0-9\s]", "");

            ProductSearchModel searchModel = new ProductSearchModel()
            {
                ProductToSearch = productToSearch,
                Action = ProductSearchModel.ModelType.Search
            };

            CategoryPublicModel model = new CategoryPublicModel(this, searchModel);

            model.EshopData = this.GetCurrentEshopModel();

            return View("CategoryDetail", model);
        }


        public ActionResult GetProductsSearchResult(string id)
        {
            string[] forbiddenChars = { "'", "\"", "*", "%", ";", "--", " ' " };

            // Skontroluj, či id nie je null
            if (id != null)
            {
                foreach (var forbiddenChar in forbiddenChars)
                {
                    if (id.Contains(forbiddenChar))
                    {
                        ProductSearchModel searchModel = new ProductSearchModel()
                        {
                            ProductToSearch = id,
                            Action = ProductSearchModel.ModelType.Search,
                            NoResultsFound = true
                        };
                        CategoryPublicModel model = new CategoryPublicModel(this, searchModel);

                        return View(model);
                    }
                }
            }
    

        ProductSearchModel searchModelNormal = new ProductSearchModel()
            {
                ProductToSearch = id,
                Action = ProductSearchModel.ModelType.Search
            };
            CategoryPublicModel modelNormal = new CategoryPublicModel(this, searchModelNormal);

            return View(modelNormal);
        }

        public ActionResult GetProductsDiscounted()
            {
                CategoryPromoModel model = new CategoryPromoModel(this.CurrentSessionId, CategoryPromoModel.PromoType.Discounted);
              model.Title = "Zľavné produkty";

            return View("CategoryPromo", model);
            }

            public ActionResult GetProductsBestseller()
            {
                CategoryPromoModel model = new CategoryPromoModel(this.CurrentSessionId, CategoryPromoModel.PromoType.Bestseller);
                model.Title = "Obľúbené produkty";

                return View("CategoryPromo", model);
            }

        public ActionResult GetFavoriteBestProducts()
        {
            CategoryPromoModel model = new CategoryPromoModel(this.CurrentSessionId, "OBLUBENE", 8);
            model.Title = "Obľúbené";

            return View("CategoryPromo", model);
        }
        public ActionResult GetNewsProducts()
        {
            CategoryPromoModel model = new CategoryPromoModel(this.CurrentSessionId, "NOVINKY", 8);
            model.Title = "Novinky";

            return View("CategoryPromo", model);
        }
        public ActionResult GetProductsRecomended()
            {
                CategoryPromoModel model = new CategoryPromoModel(this.CurrentSessionId, "ODPORUCANE", 8);
                model.Title = "Odporúčané produkty";

                return View("CategoryPromo", model);
            }

        public ActionResult GetProductsPromoted(string id)
        {
            CategoryPromoModel model = new CategoryPromoModel(this.CurrentSessionId, id, 4);
            model.Title = model.Category.CategoryName;

            return View("CategoryPromo", model);
        }

        public ActionResult GetCategoriesPromoted(string id)
            {
                CategoryModel model = GetCurrentEshopModel().CategoryTreeData.GetCategoryNode(id);

                return View(model);
            }

            public ActionResult HomeCategoryBanners()
            {
                return View(GetCurrentEshopModel().CategoryTreeData);
            }
        }
    }
