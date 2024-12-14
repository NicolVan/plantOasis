using PlantOasis.lib.Models;
using PlantOasis.lib.Repositories;
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
    public class ProductPublicController : _BaseController
    {
        public ActionResult ProductDetail()
        {
            string productUrl = ProductContentFinder.GetProductUrl(this.CurrentRequest.Url);
            ProductPublicModel model = new ProductPublicModel(productUrl);
            model.SessionId = this.CurrentSessionId;
            //model.CategoryForFilterMenu = new CategoryPublicModel(this.CurrentSessionId);
            if (model.SeoData != null)
            {
                this.SetSeoModel(model.SeoData);
            }

            return View(model);
        }

        //[HttpPost]
        //public ActionResult ProductSearch(ProductSearchModel model)
        //{
        //    if (string.IsNullOrEmpty(model.ProductToSearch))
        //    {
        //        // Nothing to search
        //        return this.RedirectToCurrentUmbracoUrl();
        //    }

        //    return this.RedirectToplantoasisUmbracoPage(ConfigurationUtil.Ecommerce_ProductPublic_SearchPageId, string.Format("srchprod={0}", model.ProductToSearch));
        //}
        //[HttpPost]
        //public ActionResult ProductGlobalSearch(string productToGlobalSearch)
        //{
        //    if (string.IsNullOrEmpty(productToGlobalSearch))
        //    {
        //        // Nothing to search
        //        return this.RedirectToCurrentUmbracoUrl();
        //    }

        //    return this.RedirectToplantoasisUmbracoPage(ConfigurationUtil.Ecommerce_ProductPublic_SearchPageId, string.Format("srchprod={0}", productToGlobalSearch));
        //}
    }
}


