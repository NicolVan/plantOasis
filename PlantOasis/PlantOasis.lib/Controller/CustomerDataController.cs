using PlantOasis.lib.Models;
using PlantOasis.lib.Repositories;
using PlantOasis.lib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using System.Web;
using Umbraco.Web.Mvc;


namespace PlantOasis.lib.Controller
{
    [PluginController("PlantOasis")]
    [Authorize(Roles = MemberRepository.PlantOasisMemberCustomerRole)]
    public class CustomerDataController : _BaseController
    {
        public ActionResult EditMyProfile()
        {
          CustomerModel model = GetCurrentCustomerForEdit();

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveMyProfile(CustomerModel model)
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

            if (new CustomerRepository().Save(CustomerModel.CreateCopyFrom(model, new CustomerDropDowns())))
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

            return this.RedirectToUmbracoPage(ConfigurationUtil.AfterLoginFormId);
        }


      CustomerModel GetCurrentCustomerForEdit()
        {
            CustomerModel model = CustomerModel.GetCurrentCustomerModel(new CustomerDropDowns());
            if (model.DropDowns == null)
            {
                model.DropDowns = new CustomerDropDowns();
            }

            return model;
        }


        public ActionResult EditMyPassword()
        {
            return View(new ChangePasswordModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveMyPassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            if (model.NewPassword != model.NewPasswordRepeat)
            {
                ModelState.AddModelError("", "Heslo a zopakované heslo musia byť rovnaké.");
            }
            else
            {
                if (!CustomerModel.VerifyUserPassword(CustomerModel.GetCurrentMemberName(), model.OldPassword))
                {
                    ModelState.AddModelError("", "Staré heslo je nesprávne.");
                }
            }

            if (ModelState.IsValid)
            {
                MemberRepository repository = new MemberRepository();
                MemberModel memberModel = MemberModel.CreateCopyFrom(repository.Get(CustomerModel.GetCurrentMemberId()));
                memberModel.Password = model.NewPassword;
                memberModel.PasswordRepeat = model.NewPasswordRepeat;
                MembershipCreateStatus status = repository.SavePassword(MemberModel.CreateCopyFrom(memberModel));
                if (status != MembershipCreateStatus.Success)
                {
                    ModelState.AddModelError("", string.Format("Nastala chyba pri zápise záznamu do systému. {0}. Opravte chyby a skúste akciu zopakovať. Ak sa chyba vyskytne znovu, kontaktujte nás prosím.", repository.GetErrorMessage(status)));
                }
            }
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            return this.RedirectToUmbracoPage(ConfigurationUtil.AfterLoginFormId);
        }



        public ActionResult GetMyQuoteRecords()
        {
            HttpRequest currentRequest = this.CurrentRequest;

            QuoteForListRepository repository = new QuoteForListRepository();
            QuoteListModel model = QuoteListModel.CreateCopyFrom(currentRequest,
                repository.GetForUser(QuoteListModel.GetCurrentPageNumber(currentRequest), 10,
                    CustomerModel.GetCurrentMemberId(),
                    CustomerModel.GetCurrentMemberName()
                ));

            return View(model);
        }

        public ActionResult MyQuoteDetail(string id)
        {
            BasketModel model = new BasketModel();
            model.CurrentQuote = QuoteController.GetQuoteWithProducts(new Guid(id));
            model.CurrentQuote.User = QuoteController.GetQuoteUser(model.CurrentQuote);
            SetStepQuotes(model);

            return View(model);
        }

        void SetStepQuotes(BasketModel model)
        {
            List<QuoteForList> dataList =
                new QuoteForListRepository().GetForUser(1, _PagingModel.AllItemsPerPage,
                    CustomerModel.GetCurrentMemberId(),
                    CustomerModel.GetCurrentMemberName()
                ).Items;

            for (int idx = 0; idx < dataList.Count; idx++)
            {
                if (dataList[idx].pk == model.CurrentQuote.pk)
                {
                    if (idx > 0)
                    {
                        model.NextQuoteKey = dataList[idx - 1].pk;
                    }
                    if (idx < dataList.Count - 1)
                    {
                        model.PrevQuoteKey = dataList[idx + 1].pk;
                    }
                    break;
                }
            }
        }
    }
}