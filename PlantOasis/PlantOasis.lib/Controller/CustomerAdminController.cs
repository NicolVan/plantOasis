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
    [Authorize(Roles = MemberRepository.PlantOasisMemberCustomerRole)]
    public class CustomerAdminController : _BaseController
    {
        public ActionResult GetRecords(int page = 1, string sort = "Name", string sortDir = "ASC")
        {
            try
            {
                return GetRecordsView(page, sort, sortDir);
            }
            catch
            {
                CustomerFilterModel filter = GetCustomerFilterForEdit();
                if (filter != null)
                {
                    filter.SearchText = string.Empty;
                    UserPropRepository repository = new UserPropRepository();
                    repository.Save(this.CurrentSessionId, CustomerFilterModel.CreateCopyFrom(filter));
                }
                return GetRecordsView(page, sort, sortDir);
            }
        }
        ActionResult GetRecordsView(int page, string sort, string sortDir)
        {
            CustomerFilterModel filter = GetCustomerFilterForEdit();

           CustomerRepository repository = new CustomerRepository();
            CustomerListModel model = CustomerListModel.CreateCopyFrom(
                repository.GetPage(page, _PagingModel.DefaultItemsPerPage, sort, sortDir,
                    new CustomerFilter()
                    {
                        SearchText = filter.SearchText,
                    }),
                    new CustomerDropDowns()
                );

            return View(model);
        }

        public ActionResult InsertRecord()
        {
            return View("EditRecord", GetCustomerForEdit());
        }

        public ActionResult EditRecord(string id)
        {
            CustomerRepository repository = new CustomerRepository();
            CustomerModel model = CustomerModel.CreateCopyFrom(repository.Get(new Guid(id)), new CustomerDropDowns());

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveRecord(CustomerModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.IsDeliveryAddress)
                {
                    if (string.IsNullOrEmpty(model.DeliveryName))
                    {
                        ModelState.AddModelError("DeliveryName", "Doručovacia firma (meno a priezvisko) musí byť zadané.");
                    }
                    if (string.IsNullOrEmpty(model.DeliveryCountryCollectionKey) || model.DeliveryCountryCollectionKey == Guid.Empty.ToString())
                    {
                        ModelState.AddModelError("DeliveryCountryCollectionKey", "Doručovacia krajina musí byť zadaná.");
                    }
                    if (string.IsNullOrEmpty(model.DeliveryStreet))
                    {
                        ModelState.AddModelError("DeliveryStreet", "Doručovacia ulica a číslo domu musí byť zadané.");
                    }
                    if (string.IsNullOrEmpty(model.DeliveryCity))
                    {
                        ModelState.AddModelError("DeliveryCity", "Doručovacia obec musí byť zadaná.");
                    }
                    if (string.IsNullOrEmpty(model.DeliveryZip))
                    {
                        ModelState.AddModelError("DeliveryZip", "Doručovacie PSČ musí byť zadané.");
                    }
                }
            }
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            CustomerRepository repository = new CustomerRepository();
            if (repository.Save(CustomerModel.CreateCopyFrom(model, new CustomerDropDowns())))
            {
                //model.SaveNewsletterSettings();
            }
            else
            {
                ModelState.AddModelError("", "Nastala chyba pri zápise záznamu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
            }
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            return this.RedirectToUmbracoPage(ConfigurationUtil.CustomersFormId);
        }

        public ActionResult ConfirmDeleteRecord(string id)
        {
           CustomerRepository repository = new CustomerRepository();
            CustomerModel model = CustomerModel.CreateCopyFrom(repository.Get(new Guid(id)), new CustomerDropDowns());

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRecord(string keepMember, CustomerModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            CustomerRepository repository = new CustomerRepository();
            if (!repository.Delete(CustomerModel.CreateCopyFrom(model, new CustomerDropDowns())))
            {
                ModelState.AddModelError("", "Nastala chyba pri zápise záznamu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
            }
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            if (keepMember == "no")
            {
                MemberRepository memberRep = new MemberRepository();
                Member member = memberRep.Get(model.OwnerId);
                if (member != null)
                {
                    memberRep.Delete(member);
                }
            }

            return this.RedirectToUmbracoPage(ConfigurationUtil.CustomersFormId);
        }

        public ActionResult GetFilter()
        {
            return View(GetCustomerFilterForEdit());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveFilter(CustomerFilterModel model)
        {
            model.ModelErrors.Clear();
            if (model.ModelErrors.Count == 0)
            {
                UserPropRepository repository = new UserPropRepository();
                if (!repository.Save(this.CurrentSessionId, CustomerFilterModel.CreateCopyFrom(model)))
                {
                    model.ModelErrors.Add("Nastala chyba pri zápise záznamu do systému. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
                }
            }
            if (model.ModelErrors.Count > 0)
            {
                return RedirectToCurrentUmbracoPageAfterSaveFilter(model);
            }

            return RedirectToCurrentUmbracoPageAfterSaveFilter();
        }


        CustomerModel GetCustomerForEdit()
        {
            CustomerModel model = new CustomerModel();
            model.DropDowns = new CustomerDropDowns();

            return model;
        }

        RedirectToUmbracoPageResult RedirectToCurrentUmbracoPageAfterSaveFilter(CustomerFilterModel rec = null)
        {
            SetCustomerFilterForEdit(rec);
            return RedirectToCurrentUmbracoPage();
        }
        void SetCustomerFilterForEdit(CustomerFilterModel rec = null)
        {
            TempData["CustomerFilterForEdit"] = rec;
        }
        CustomerFilterModel GetCustomerFilterForEdit()
        {
            if (TempData["CustomerFilterForEdit"] == null)
            {
                UserPropRepository repository = new UserPropRepository();
                TempData["CustomerFilterForEdit"] = CustomerFilterModel.CreateCopyFrom(repository.Get(this.CurrentSessionId, ConfigurationUtil.PropId_CustomerFilterModel));
            }

            return (CustomerFilterModel)TempData["CustomerFilterForEdit"];
        }
    }
}

