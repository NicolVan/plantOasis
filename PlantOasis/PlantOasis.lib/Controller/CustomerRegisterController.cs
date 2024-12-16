using dufeksoft.lib.Mail;
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
using Umbraco.Web.Mvc;
using Mailer = PlantOasis.lib.Util.Mailer;


namespace PlantOasis.lib.Controller
{
    [PluginController("PlantOasis")]
    public class CustomerRegisterController : _BaseController
    {
        public ActionResult Registration()
        {
            return View(GetRegisterModelForEdit());
        }

        [HttpPost]
        public ActionResult SubmitRegistration(RegisterModel model)
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
                    if (string.IsNullOrEmpty(model.DeliveryPhone))
                    {
                        ModelState.AddModelError("DeliveryPhone", "Doručovací telefón musí byť zadaný.");
                    }
                }
                if (model.RegisterPassword != model.RepeatRegisterPassword)
                {
                    ModelState.AddModelError("RegisterPassword", "Heslo a Opakujte heslo musia byť rovnaké.");
                    ModelState.AddModelError("RepeatRegisterPassword", "Heslo a Opakujte heslo musia byť rovnaké.");
                }
                if (!model.AgreePersonalDataProfiling)
                {
                    ModelState.AddModelError("AgreePersonalDataProfiling", "Musíte označiť súhlas so spracovaním osobných údajov profilovaním.");
                }
                if (!new ApiKeyValidator().IsValid(model.Password, model.ConfirmPassword))
                {
                    ModelState.AddModelError("", "Musíte označiť, že nie ste robot.");
                }
            }
            if (ModelState.IsValid)
            {
                Member newMember = MemberModel.CreateCopyFrom(model);

                MemberRepository memberRep = new MemberRepository();
                Member member = null;
                MembershipCreateStatus status = memberRep.Save(this, newMember);
                if (status != MembershipCreateStatus.Success)
                {
                    ModelState.AddModelError("", string.Format("Nastala chyba pri zápise záznamu do systému. {0}. Opravte chyby a skúste akciu zopakovať. Ak sa chyba vyskytne znovu, kontaktujte nás prosím.", memberRep.GetErrorMessage(status)));
                }
                else
                {
                    member = memberRep.GetMemberByEmail(model.Email);
                }
                if (member != null)
                {
                    CustomerRepository customerRep = new CustomerRepository();
                    EshopCustomer customer = null;
                    if (customerRep.Save(EshopCustomer.CreateCopyFrom(model, member)))
                    {
                        //new NewsletterRepository().SetForEmail(model.Email, model.AgreePersonalDataNewsletter);
                        customer = customerRep.GetForOwner(member.MemberId);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Nastala chyba pri zápise údajov. Skúste akciu zopakovať a ak sa chyba vyskytne znovu, kontaktujte nás prosím.");
                    }
                }
                if (!ModelState.IsValid)
                {
                    if (status == MembershipCreateStatus.Success && member != null)
                    {
                        // On error delete member
                        memberRep.Delete(member);
                    }
                    return CurrentUmbracoPage();
                }


                List<TextTemplateParam> paramList = new List<TextTemplateParam>();
                paramList.Add(new TextTemplateParam("LOGIN", newMember.Email));
                paramList.Add(new TextTemplateParam("LOGIN_URL", string.Format("{0}/sk/zakaznicka-sekcia/prihlasenie", new _BaseControllerUtil().SiteRootUrl)));

                // Odoslanie uzivatelovi
                Util.Mailer.SendMailTemplateWithoutBcc(
                     "Potvrdenie registrácie",
                     TextTemplate.GetTemplateText("NewRegistration_Sk", paramList),
                     newMember.Email);

                return this.RedirectToUmbracoPage(ConfigurationUtil.RegistrationOkFormId);
            }

            return CurrentUmbracoPage();
        }

        RegisterModel GetRegisterModelForEdit()
        {
            RegisterModel model = new RegisterModel();
            model.DropDowns = new RegisterDropDowns();

            return model;
        }


        public ActionResult LostPassword()
        {
            return View(new LostPasswordModel());
        }


        [HttpPost]
        public ActionResult SubmitLostPassword(LostPasswordModel model)
        {
            TempData["success"] = null;

            if (ModelState.IsValid)
            {
                var memberService = Services.MemberService;
                var member = memberService.GetByEmail(model.Email);

                if (member == null)
                {
                    ModelState.AddModelError("", "Užívateľ pre zadanú e-mailovú adresu neexistuje.");
                }
                else
                {
                    string temporaryPassword = GenerateTemporaryPassword();

                    try
                    {
                        memberService.SavePassword(member, temporaryPassword); // Automaticky hashuje heslo
                        List<TextTemplateParam> paramList = new List<TextTemplateParam>
                {
                    new TextTemplateParam("LOGIN", member.Email),
                    new TextTemplateParam("PASSWORD", temporaryPassword)
                };

                        Mailer.SendMailTemplateWithoutBcc(
                            "Obnovenie prístupu na plant-oasis.sk",
                            TextTemplate.GetTemplateText("LostPassword_Sk", paramList),
                            member.Email
                        );

                        TempData["success"] = true;
                    }
                    catch (Exception exc)
                    {
                        ModelState.AddModelError("", $"Chyba pri odosielaní e-mailu: {exc.Message}");
                    }
                }
            }

            return CurrentUmbracoPage();
        }

        private string GenerateTemporaryPassword()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8);
        }
    }
}