using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantOasis.lib.Util
{
    public class ConfigurationUtil
    {
        public const string PlantOasis_Quote_TransportItemCode = "DOPRAVA";
        public const string PlantOasis_Quote_PaymentItemCode = "PLATBA";
        public const string PlantOasis_Quote_DiscountItemCode = "ZĽAVA";

        public const string LoginFormId = "plantoasis.LoginFormId";
        public const string AfterLoginFormId = "plantoasis.AfterLoginFormId";
        public const string AfterPasswordResetFormId = "plantoasis.AfterPasswordResetFormId";
        public const string RegistrationOkFormId = "plantoasis.RegistrationOkFormId";
        public const string MembersFormId = "plantoasis.MembersFormId";
        public const string CustomersFormId = "plantoasis.CustomersFormId";
        public const string QuoteStatesFormId = "plantoasis.QuoteStatesFormId";
        public const string QuotesFormId = "plantoasis.QuotesFormId";
        public const string QuotesEditFormId = "plantoasis.QuotesEditFormId";
        public const string Quote_InitialState = "plantoasis.Quote.InitialState";
        public const string TransportTypesFormId = "plantoasis.TransportTypesFormId";
        public const string DodavatelFormId = "plantoasis.DodavatelFormId";
        public const string ProductsFormId = "plantoasis.ProductFormId";
        public const string ProductAttributesFormId = "plantoasis.ProductAttributesFormId";
        public const string PlantoasisProduktPricesFormId = "plantoasis.ProduktPricesFormId";
        public const string CategoriesFormId = "plantoasis.CategoriesFormId";
        public const string AvailabilitiesFormId = "plantoasis.AvailabilitiesFormId";
        public const string Plantoasis_ProductPublic_DetailPageId = "plantoasis.ProductPublic_DetailPageId";
        public const string Plantoasis_Quote_DiscountItemCode = "plantoasis.Quote_DiscountItemCode ";
        public const string Plantoasis_Quote_PaymentItemCode = "plantoasis.Quote_PaymentItemCode";
        public const string PlantOasis_ProductPublic_CategoryPageId = "plantoasis.ProductPublic_CategoryPageId";
        public const string Plantoasis_Quote_TransportItemCode = "plantoasis.Quote_TransportItemCode ";
        public const string CountriesFormId = "plantoasis.CountriesFormId";
        public const string PaymentTypesFormId = "plantoasis.PaymentTypesFormId";
        public const string PaymentStatesFormId = "plantoasis.PaymentStatesFormId";

        public const string PropId_DodavatelFilterModel = "PropId_DodavatelFilterModel";
        public const string PropId_CustomerFilterModel = "PropId_CustomerFilterModel";
        public const string PropId_ProductFilterModel = "PropId_ProductFilterModel";
        public const string PropId_ProductInCategoryFilterModel = "PropId_ProductInCategoryFilterModel";
        public const string PropId_ProductNotInCategoryFilterModel = "PropId_ProductNotInCategoryFilterModel";
        public const string PropId_CategoryPublicFilterModel_ProductAttribute = "PropId_CategoryPublicFilterModel_ProductAttribute";
        public const string PropId_CategoryPublicFilterModel_Producer = " PropId_CategoryPublicFilterModel_Producer";
        public const string PropId_CategoryPublicFilterModel_CurrentCategory = "PropId_CategoryPublicFilterModel_CurrentCategory";
        public const string PropId_CategoryPublicFilterModel_ProductSort = "PropId_CategoryPublicFilterModel_ProductSort";
        public const string PropId_CategoryPublicFilterModel_PageSize = "PropId_CategoryPublicFilterModel_PageSize";
        public const string PropId_CategoryPublicFilterModel_ProductView = "PropId_CategoryPublicFilterModel_ProductView";
        public const string PropId_ProductAttributeFilterModel = "PropId_ProductAttributeFilterModel";
        public const string PropId_QuoteListFilterModel = "PropId_QuoteListFilterModel";

        public const string PlantOasis_Quote_InitialState = "plantoasis.Quote.InitialState";
        public const string PlantOasis_Quote_PaidPriceState = "plantoasis.Quote.PaidPriceState";
        public const string PlantOasis_Basket_DeliveryDataPageId = "plantoasis.Basket_DeliveryDataPageId";
        public const string PlantOasis_Basket_ReviewAndSendPageId = "plantoasis.Basket_ReviewAndSendPageId";
        public const string PlantOasis_Basket_FinishedPageId = "plantoasis.Basket_FinishedPageId";
        public const string PlantOasis_Quote_ModalMsgId = "quote-modal-msg";
        public const string PlantOasis_Quote_InfMsgId = "quote-info-msg";
        public static int GetPageId(string pageKey)
        {
            return int.Parse(ConfigurationManager.AppSettings[pageKey]);
        }
        public static string PaiedQuotePriceState()
        {
            return ConfigurationManager.AppSettings[ConfigurationUtil.PlantOasis_Quote_PaidPriceState];
        }
        public static string GetCfgValue(string cfgKey)
        {
            return ConfigurationManager.AppSettings[cfgKey];
        }
        public static string InitialQuoteState()
        {
            return ConfigurationManager.AppSettings[ConfigurationUtil.Quote_InitialState];
        }
        public static string EshopRootUrl
        {
            get
            {
                return "/eshop/kategoria/-vsetko-";
            }
        }
        public static string QuoteViewUrl
        {
            get
            {
                return "/home/zakaznicka-sekcia/moje-objednavky/detail-objednavky";
            }
        }
    }
}