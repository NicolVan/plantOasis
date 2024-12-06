using PlantOasis.lib.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantOasis.lib.Models
{
    public class ProductRelationModel : _BaseModel
    {
        public List<ProductRelationItem> Items { get; set; }

        public static List<ProductRelationItem> LoadItems(Guid productKey)
        {
            List<ProductRelationItem> allItems = new List<ProductRelationItem>();

            // Load all relations
            foreach (ProductRelation relation in new ProductRelationRepository().GetForProduct(productKey))
            {
                ProductRelationItem item = new ProductRelationItem()
                {
                    PkProductMain = productKey,
                    PkProductRelated = relation.PkProductRelated,
                };

                allItems.Add(item);
            }

            // Set related products
            SetRelatedProducts(allItems);

            // Sort result list by product name
            allItems.Sort(new ProductRelationItemComparer());

            return allItems;
        }

        public void SetRelatedProducts()
        {
            SetRelatedProducts(this.Items);
        }

        public static void SetRelatedProducts(List<ProductRelationItem> itemList)
        {
            List<string> relatedProductsKeyList = new List<string>();
            Hashtable htRelated = new Hashtable();
            foreach (ProductRelationItem item in itemList)
            {
                htRelated.Add(item.PkProductRelated, item);
                relatedProductsKeyList.Add(item.PkProductRelated.ToString());
            }


            List<Product> dataList = new ProductRepository().GetPage(1, _PagingModel.AllItemsPerPage,
                filter: new ProductFilter()
                {
                    ProductKeyList = relatedProductsKeyList
                }).Items;
            foreach (Product relatedProduct in dataList)
            {
                if (htRelated.ContainsKey(relatedProduct.pk))
                {
                    ProductRelationItem item = (ProductRelationItem)htRelated[relatedProduct.pk];
                    item.RelatedProduct = relatedProduct;
                }
            }
        }
    }

    public class ProductRelationItem : _BaseModel
    {
        public Guid PkProductMain { get; set; }
        public Guid PkProductRelated { get; set; }
        public Product RelatedProduct { get; set; }

        public ProductRelationItem()
        {
        }

        public void CopyDataFrom(ProductRelation src)
        {
            this.pk = src.pk;
            this.PkProductMain = src.PkProductMain;
            this.PkProductRelated = src.PkProductRelated;
        }

        public void CopyDataTo(ProductRelation trg)
        {
            trg.pk = this.pk;
            trg.PkProductMain = this.PkProductMain;
            trg.PkProductRelated = this.PkProductRelated;
        }

        public static ProductRelationItem CreateCopyFrom(ProductRelation src)
        {
            ProductRelationItem trg = new ProductRelationItem();
            trg.CopyDataFrom(src);

            return trg;
        }

        public static ProductRelation CreateCopyFrom(ProductRelationItem src)
        {
            ProductRelation trg = new ProductRelation();
            src.CopyDataTo(trg);

            return trg;
        }
    }

    public class ProductRelationItemComparer : IComparer<ProductRelationItem>
    {
        public int Compare(ProductRelationItem x, ProductRelationItem y)
        {
            return string.Compare(x.RelatedProduct.ProductName, y.RelatedProduct.ProductName);
        }
    }
}
