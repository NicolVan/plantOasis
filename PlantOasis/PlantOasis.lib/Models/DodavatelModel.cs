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
using System.Web.Mvc;
using System.Web;
using System.Xml;

namespace PlantOasis.lib.Models
{
    public class DodavatelModel : _BaseModel
    {
        [Required(ErrorMessage = "Názov výrobcu musí byť zadaný")]
        [Display(Name = "Názov výrobcu")]
        public string ProducerName { get; set; }

        [AllowHtml]
        [Display(Name = "Popis výrobcu")]
        public string ProducerDescription { get; set; }

        [Display(Name = "Webová stránka")]
        public string ProducerWeb { get; set; }

        public void CopyDataFrom(Dodavatel src)
        {
            this.pk = src.pk;
            this.ProducerName = src.ProducerName;
            this.ProducerDescription = src.ProducerDescription;
            this.ProducerWeb = src.ProducerWeb;
        }

        public void CopyDataTo(Dodavatel trg)
        {
            trg.pk = this.pk;
            trg.ProducerName = this.ProducerName;
            trg.ProducerDescription = this.ProducerDescription;
            trg.ProducerWeb = this.ProducerWeb;
        }

        public static DodavatelModel CreateCopyFrom(Dodavatel src)
        {
            DodavatelModel trg = new DodavatelModel();
            trg.CopyDataFrom(src);

            return trg;
        }

        public static Dodavatel CreateCopyFrom(DodavatelModel src)
        {
            Dodavatel trg = new Dodavatel();
            src.CopyDataTo(trg);

            return trg;
        }
    }

    public class DodavatelListModel : List<DodavatelModel>
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

        public DodavatelListModel(HttpRequest request, int pageSize = 25)
        {
            this.CurrentRequest = request;
            this.PageSize = pageSize;
        }

        public List<DodavatelModel> GetPageItems()
        {
            GridPageInfo cpi = this.Pager.GetCurrentPageInfo();

            List<DodavatelModel> resultList = new List<DodavatelModel>();
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

    public class DodavatelPagingListModel : _PagingModel
    {
        public List<DodavatelModel> Items { get; set; }

        public static DodavatelPagingListModel CreateCopyFrom(Page<Dodavatel> srcArray)
        {
            DodavatelPagingListModel trgArray = new DodavatelPagingListModel();
            trgArray.ItemsPerPage = (int)srcArray.ItemsPerPage;
            trgArray.TotalItems = (int)srcArray.TotalItems;
            trgArray.Items = new List<DodavatelModel>(srcArray.Items.Count + 1);

            foreach (Dodavatel src in srcArray.Items)
            {
                trgArray.Items.Add(DodavatelModel.CreateCopyFrom(src));
            }

            return trgArray;
        }
    }

    public class DodavatelFilterModel : _BaseUserPropModel
    {

        [Display(Name = "Vyhľadávanie (názov, popis, web ...)")]
        public string SearchText { get; set; }


        public DodavatelFilterModel()
        {
            this.PropId = ConfigurationUtil.PropId_DodavatelFilterModel;
        }

        public static DodavatelFilterModel CreateCopyFrom(UserProp src)
        {
            DodavatelFilterModel trg = new DodavatelFilterModel();
            if (src != null)
            {
                trg.CopyDataFrom(src);
            }
            trg.UpdateBeforeEdit();

            return trg;
        }

        public static UserProp CreateCopyFrom(DodavatelFilterModel src)
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
            XmlElement mainNode = doc.CreateElement("DodavatelFilterModel");
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

                string fullParent = "DodavatelFilterModel";

                // Search text
                this.SearchText = XmlParamSet.LoadItem(doc, fullParent, "SearchText", string.Empty);
            }
        }
    }

    public class DodavatelPagerModel
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public bool IsCurrent { get; set; }
    }

    public class DodavatelDropDown : CmpDropDown
    {
        public DodavatelDropDown()
        {
        }

        public static DodavatelDropDown CreateFromRepository(bool allowNull, string emptyText = "[ nezadané ]")
        {
            DodavatelRepository repository = new DodavatelRepository();
            return DodavatelDropDown.CreateCopyFrom(repository.GetPage(1, _PagingModel.AllItemsPerPage), allowNull, emptyText);
        }

        public static DodavatelDropDown CreateCopyFrom(Page<Dodavatel> dataList, bool allowNull, string emptyText)
        {
            DodavatelDropDown ret = new DodavatelDropDown();

            if (allowNull)
            {
                ret.AddItem(emptyText, Guid.Empty.ToString(), null);
            }
            foreach (Dodavatel dataItem in dataList.Items)
            {
                DodavatelModel dataModel = DodavatelModel.CreateCopyFrom(dataItem);
                ret.AddItem(dataModel.ProducerName, dataModel.pk.ToString(), dataModel);
            }

            return ret;
        }
    }
}

