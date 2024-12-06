using dufeksoft.lib.Model.Grid;
using dufeksoft.lib.ParamSet;
using dufeksoft.lib.UI;
using NPoco;
using PlantOasis.lib.Repositories;
using PlantOasis.lib.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace PlantOasis.lib.Models
{
    public class ProductAttributeModel : _BaseModel
    {
        [Display(Name = "Poradie")]
        public int ProductAttributeOrder { get; set; }

        [Required(ErrorMessage = "Skupina musí byť zadaná")]
        [Display(Name = "Skupina")]
        public string ProductAttributeGroup { get; set; }

        [Required(ErrorMessage = "Názov vlastnosti musí byť zadaný")]
        [Display(Name = "Názov vlastnosti")]
        public string ProductAttributeName { get; set; }

        public void CopyDataFrom(ProductAttribute src)
        {
            this.pk = src.pk;
            this.ProductAttributeOrder = src.ProductAttributeOrder;
            this.ProductAttributeName = src.ProductAttributeName;
            this.ProductAttributeGroup = src.ProductAttributeGroup;
        }

        public void CopyDataTo(ProductAttribute trg)
        {
            trg.pk = this.pk;
            trg.ProductAttributeOrder = this.ProductAttributeOrder;
            trg.ProductAttributeName = this.ProductAttributeName;
            trg.ProductAttributeGroup = this.ProductAttributeGroup;
        }

        public static ProductAttributeModel CreateCopyFrom(ProductAttribute src)
        {
            ProductAttributeModel trg = new ProductAttributeModel();
            trg.CopyDataFrom(src);

            return trg;
        }

        public static ProductAttribute CreateCopyFrom(ProductAttributeModel src)
        {
            ProductAttribute trg = new ProductAttribute();
            src.CopyDataTo(trg);

            return trg;
        }
    }

    public class ProductAttributeListModel : List<ProductAttributeModel>
    {
        public HttpRequest CurrentRequest { get; private set; }
        public string SessionId { get; set; }
        public int PageSize { get; private set; }

        private GridPagerModel currentPager;
        public GridPagerModel Pager
        {
            get
            {
                return GetPager();
            }
        }

        public ProductAttributeListModel(HttpRequest request, int pageSize = 25)
        {
            this.CurrentRequest = request;
            this.PageSize = pageSize;
        }

        public List<ProductAttributeModel> GetPageItems()
        {
            GridPageInfo cpi = this.Pager.GetCurrentPageInfo();

            List<ProductAttributeModel> resultList = new List<ProductAttributeModel>();
            for (int i = cpi.FirsItemIndex; i < this.Count && i < cpi.LastItemIndex + 1; i++)
            {
                resultList.Add(this[i]);
            }

            return resultList;
        }

        GridPagerModel GetPager()
        {
            if (this.currentPager == null || this.currentPager.ItemCnt != this.Count)
            {
                this.currentPager = new GridPagerModel(this.CurrentRequest, this.Count, this.PageSize);
            }

            return this.currentPager;
        }
    }

    public class ProduktAttributePagingListModel : _PagingModel
    {
        public List<ProductAttributeModel> Items { get; set; }

        public static ProduktAttributePagingListModel CreateCopyFrom(Page<ProductAttribute> srcArray)
        {
            ProduktAttributePagingListModel trgArray = new ProduktAttributePagingListModel();
            trgArray.ItemsPerPage = (int)srcArray.ItemsPerPage;
            trgArray.TotalItems = (int)srcArray.TotalItems;
            trgArray.Items = new List<ProductAttributeModel>(srcArray.Items.Count + 1);

            foreach (ProductAttribute src in srcArray.Items)
            {
                trgArray.Items.Add(ProductAttributeModel.CreateCopyFrom(src));
            }

            return trgArray;
        }
    }

    public class ProduktAttributeFilterModel : _BaseUserPropModel
    {

        [Display(Name = "Vyhľadávanie (skupina, názov ...)")]
        public string SearchText { get; set; }


        public ProduktAttributeFilterModel()
        {
            this.PropId = ConfigurationUtil.PropId_ProductAttributeFilterModel;
        }

        public static ProduktAttributeFilterModel CreateCopyFrom(UserProp src)
        {
            ProduktAttributeFilterModel trg = new ProduktAttributeFilterModel();
            if (src != null)
            {
                trg.CopyDataFrom(src);
            }
            trg.UpdateBeforeEdit();

            return trg;
        }

        public static UserProp CreateCopyFrom(ProduktAttributeFilterModel src)
        {
            src.UpdateAfterEdit();
            UserProp trg = new UserProp();
            src.CopyDataTo(trg);

            return trg;
        }


        public void UpdateBeforeEdit()
        {
            LoadPropValue(this.PropValue);
        }

        public void UpdateAfterEdit()
        {
            this.PropValue = SavePropValue();
        }

        private string SavePropValue()
        {
            // Create XML document
            XmlDocument doc = new XmlDocument();
            // Create main element
            XmlElement mainNode = doc.CreateElement("ProductAttributeFilterModel");
            mainNode.SetAttribute("version", "1.0");
            doc.AppendChild(mainNode);

            // Search text
            XmlParamSet.SaveItem(doc, mainNode, "SearchText", this.SearchText);

            return doc.InnerXml;
        }

        private void LoadPropValue(string propValue)
        {
            XmlDocument doc = new XmlDocument();
            if (!string.IsNullOrEmpty(propValue))
            {
                doc.LoadXml(propValue);

                string fullParent = "ProductAttributeFilterModel";

                // Search text
                this.SearchText = XmlParamSet.LoadItem(doc, fullParent, "SearchText", string.Empty);
            }
        }
    }

    public class ProduktAttributeDropDown : CmpDropDown
    {
        public ProduktAttributeDropDown()
        {
        }

        public static ProduktAttributeDropDown CreateFromRepository(bool allowNull, string emptyText = "< žiadna vlastnosť >")
        {
            ProductAttributeRepository repository = new ProductAttributeRepository();
            return ProduktAttributeDropDown.CreateCopyFrom(repository.GetPage(1, _PagingModel.AllItemsPerPage), allowNull, emptyText);
        }

        public static ProduktAttributeDropDown CreateCopyFrom(Page<ProductAttribute> dataList, bool allowNull, string emptyText)
        {
            ProduktAttributeDropDown ret = new ProduktAttributeDropDown();

            if (allowNull)
            {
                ret.AddItem(emptyText, Guid.Empty.ToString(), null);
            }
            foreach (ProductAttribute dataItem in dataList.Items)
            {
                ProductAttributeModel dataModel = ProductAttributeModel.CreateCopyFrom(dataItem);
                ret.AddItem(string.Format("{0} / {1}", dataModel.ProductAttributeGroup, dataModel.ProductAttributeName), dataModel.pk.ToString(), dataModel);
            }

            return ret;
        }
    }
}
