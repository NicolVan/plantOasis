using iTextSharp.text.pdf;
using iTextSharp.text;
using NPoco;
using PlantOasis.lib.Models.Pdf;
using PlantOasis.lib.Models;
using PlantOasis.lib.Pdf;
using PlantOasis.lib.Repositories;
using PlantOasis.lib.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PlantOasis.lib.Task
{
    public class CategoryOfferToPdf
    {
        private static float widthMargin = 20;
        private static float widthPadding = 0;

        public DateTime PrintDateTime { get; private set; }

        private CategoryOfferModel DataModel;

        public CategoryOfferToPdf(Guid categoryKey, string imgPath)
        {
            this.PrintDateTime = DateTime.Now;
            this.DataModel = new CategoryOfferModel(categoryKey, imgPath);
        }

        public PdfFilePrintResult GetPdf()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (Document doc = new Document(PageSize.A4))
                {
                    using (PdfWriter writer = PdfWriter.GetInstance(doc, ms))
                    {
                        doc.Open();
                        PdfFile pdf = new PdfFile(doc, writer, new PdfFonts());

                        float pageBottom = pdf.PageHeight - 30;
                        float itemHeight = 14;

                        int cnt = 0;
                        int pagenb = 1;
                        float y = PageHeader(pdf, pagenb);

                        //for (int i = 0; i < 10; i++)
                        {
                            foreach (ProductModel product in this.DataModel.Products.Items)
                            {
                                if (y + itemHeight > pageBottom)
                                {
                                    pdf.NewPage();
                                    y = PageHeader(pdf, ++pagenb);
                                }

                                OneIteData(pdf, product, y, ++cnt);
                                y += itemHeight;
                            }
                        }

                        doc.Close();
                        writer.Close();
                    }
                }

                return new PdfFilePrintResult("Produkty.pdf", ms.ToArray());
            }
        }

        private float PageHeader(PdfFile pdf, int pagenb)
        {
            float left = widthMargin + widthPadding;
            float right = pdf.PageWidth - widthMargin - widthPadding;
            float headImgWith = 200;
            float y = 20;
            float x = left;

            float headImgRatio = 150f / 800f;
            float headImgHeight = headImgRatio * headImgWith;
            float headImgRatio2 = 200f / 800f;
            float headImgHeight2 = headImgRatio2 * headImgWith;
            float topHeight = headImgHeight + headImgHeight2;

            if (pagenb > 1)
            {
                topHeight = 20;
                pdf.ChangeDrawColor(pdf.Fonts.SecondaryColor);
            }
            else
            {
                pdf.ChangeDrawColor(pdf.Fonts.PrimaryColor);

                // Logo
                pdf.AddImgAtPosition(string.Format("{0}/{1}", this.DataModel.ImgPath, "Logo1.png"), headImgWith, headImgHeight, 10, 10, headImgHeight);

                // Nadpis
                pdf.CenterTextAtPosition(50, new PdfTextItem(this.DataModel.CategoryData.CategoryName.ToUpper(), PdfFonts.F_BOLD_24));

                // Podnadpis
                pdf.CenterTextAtPosition(90, new PdfTextItem("Kvety do každej domácnosti", PdfFonts.F_BOLD_10));

                // Kontaktné informácie
                pdf.CenterTextAtPosition(110, new PdfTextItem("Objednávku si spravíte jednoducho telefonicky,", PdfFonts.F_NORMAL_11));
                pdf.CenterTextAtPosition(125, new PdfTextItem("ale aj cez SMS, email alebo online na eshope.", PdfFonts.F_NORMAL_11));
                pdf.CenterTextAtPosition(150, new PdfTextItem(string.Format("{0}      {1}      www.plant-oasis.sk", this.DataModel.PlantOasis.Phone, this.DataModel.PlantOasis.Email), PdfFonts.F_BOLD_11));

                // Dodatočný text
                pdf.CenterParagraphAtRectangle(20, headImgHeight + headImgHeight2 / 2f, pdf.PageWidth - 40, headImgHeight2 / 2f, new PdfTextItem(this.DataModel.HeadInfoMsg, PdfFonts.F_NORMAL_11));
            }

            // Medzera pred tabuľkou
            y += 150; // Zvýšenie medzery pred tabuľkou

            // Hlavička tabuľky
            pdf.RightTextAtPosition(left + 30, y, new PdfTextItem("Kód", PdfFonts.F_BOLD_10));
            pdf.WriteTextAtPosition(left + 60, y, new PdfTextItem("Názov", PdfFonts.F_BOLD_10));

            pdf.RightTextAtPosition(right - 60, y, new PdfTextItem("Cena", PdfFonts.F_BOLD_10));
            pdf.RightTextAtPosition(right - 20, y, new PdfTextItem("MJ", PdfFonts.F_BOLD_10));

            x = right;

            // Kreslenie horizontálnej čiary
            pdf.DrawHorizontalLine(left, y + 3, pdf.PageWidth - 2 * (widthMargin + widthPadding));

            // Footer
            headImgRatio = 50f / 800f;
            headImgWith = pdf.PageWidth;
            headImgHeight = headImgRatio * headImgWith;
            pdf.ChangeDrawColor(pdf.Fonts.PrimaryColor);
            pdf.CenterTextAtPosition(pdf.PageHeight - 30f, new PdfTextItem("... viac informácií na www.plant-oasis.sk", PdfFonts.F_BOLD_10));

            return y + 20; 
        }


        private float OneIteData(PdfFile pdf, ProductModel product, float y, int cnt)
        {
            pdf.ChangeDrawColor(pdf.Fonts.DefaultColor);

            float left = widthMargin + widthPadding;
            float right = pdf.PageWidth - widthMargin - widthPadding;

            float x = left;

            pdf.RightTextAtPosition(x + 30, y, new PdfTextItem(product.ProductCode, PdfFonts.F_NORMAL_10));
            pdf.WriteTextAtPosition(x + 40, y, new PdfTextItem(product.ProductName, PdfFonts.F_NORMAL_10));

            x = right;
            pdf.RightTextAtPosition(x - 30, y, new PdfTextItem(PriceUtil.NumberToTwoDecString(product.GetCurrentPrice_WithVat()), PdfFonts.F_NORMAL_10));
            pdf.RightTextAtPosition(x, y, new PdfTextItem(product.UnitTypeName, PdfFonts.F_NORMAL_10));

            pdf.ChangeDrawColor(pdf.Fonts.PrimaryColor);
            pdf.DrawHorizontalLine(left, y + 3, pdf.PageWidth - 2 * (widthMargin + widthPadding), 0.2f);

            return y;
        }
    }

    public class CategoryOfferModel
    {
        public string ImgPath { get; private set; }
        public SysConstModel PlantOasis { get; private set; }
        public CategoryModel CategoryData { get; private set; }
        public ProductPagingListModel Products { get; private set; }

        public string HeadInfoMsg
        {
            get
            {
                return GetHeadInfoMsg();
            }
        }

        public CategoryOfferModel(Guid pkCaregory, string imgPath)
        {
            this.ImgPath = imgPath;
            this.PlantOasis= SysConstModel.CreateCopyFrom(new SysConstRepository().Get());
            this.CategoryData = CategoryModel.CreateCopyFrom(new CategoryRepository().Get(pkCaregory));


          ProductFilter filter = new ProductFilter();
            filter.OnlyIsVisible = true;
            filter.ProductCategoryKeyList = new List<string>();
            filter.ProductCategoryKeyList.Add(pkCaregory.ToString());

            Page<Product> productsPage = new ProductRepository().GetPage(
            page: 1,
                        itemsPerPage: _PagingModel.AllItemsPerPage,
            filter: filter);

            this.Products = ProductPagingListModel.CreateCopyFrom(
                productsPage,
                new ProductModelDropDowns(),
                loadPrices: true);
            this.Products.Items.Sort(new ProductModelRecomendationComparer(filter));
        }

        private string GetHeadInfoMsg()
        {
            return this.CategoryData.CategoryOfferText;
        }
    }
}

