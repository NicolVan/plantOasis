using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantOasis.lib.Repositories
{
    public class Product2AttributeRepository : _BaseRepository
    {
        public List<Product2Attribute> GetForProduct(Guid keyProduct)
        {
            var sql = GetBaseQuery().Where(GetProductWhereClause(), new { KeyProduct = keyProduct });

            return Fetch<Product2Attribute>(sql);
        }

        public List<Product2AttributeEx> GetForProducts(List<string> productKeyList)
        {
            var sql = new Sql();
            sql.Append(string.Format("SELECT {0}.PkAttribute, {0}.PkProduct, {1}.ProductAttributeName, {1}.ProductAttributeOrder FROM {0}, {1}", Product2Attribute.DbTableName, ProductAttribute.DbTableName));
            sql.Where(GetProductInWhereClause(productKeyList));
            sql.Where(string.Format("{0}.PkAttribute = {1}.pk", Product2Attribute.DbTableName, ProductAttribute.DbTableName));
            sql.Append(string.Format("ORDER BY {0}.PkProduct, {1}.ProductAttributeOrder", Product2Attribute.DbTableName, ProductAttribute.DbTableName));


            return Fetch<Product2AttributeEx>(sql);
        }

        public List<Guid> GetProductAttributeKeysForProductCategories(List<string> productCategoryKeyList)
        {
            var sql = new Sql(string.Format("SELECT DISTINCT({0}.PkAttribute) FROM {0}", Product2Attribute.DbTableName));
            if (productCategoryKeyList != null)
            {
                sql.Where(GetProductCategoryInWhereClause(productCategoryKeyList));
            }

            return Fetch<Guid>(sql);
        }


        public Product2Attribute Get(Guid keyAttribute, Guid keyProduct)
        {
            var sql = GetBaseQuery().Where(GetBaseWhereClause(), new { KeyAttribute = keyAttribute, KeyProduct = keyProduct });

            return Fetch<Product2Attribute>(sql).FirstOrDefault();
        }

        public bool Insert(Product2Attribute dataRec)
        {
            var sql = new Sql();
            sql.Append(string.Format("INSERT INTO {0} (PkAttribute, PkProduct) VALUES (@PkAttribute, @PkProduct)",
                Product2Attribute.DbTableName),
                new { PkAttribute = dataRec.PkAttribute, PkProduct = dataRec.PkProduct });

            return Execute(sql) > 0;
        }

        public bool Delete(Product2Attribute dataRec)
        {
            var sql = new Sql();
            sql.Append(string.Format("DELETE {0} WHERE {1}=@PkAttribute AND {2}=@PkProduct",
                Product2Attribute.DbTableName, "PkAttribute", "PkProduct"),
                new { PkAttribute = dataRec.PkAttribute, PkProduct = dataRec.PkProduct });

            return Execute(sql) > 0;
        }

        public bool DeleteForProduct(Guid productKey)
        {
            bool isOK = true;
            List<Product2Attribute> dataList = GetForProduct(productKey);
            foreach (Product2Attribute dataRec in dataList)
            {
                if (!Delete(dataRec))
                {
                    isOK = false;
                }
            }

            return isOK;
        }

        Sql GetBaseQuery()
        {
            return new Sql(string.Format("SELECT * FROM {0}", Product2Attribute.DbTableName));
        }

        string GetBaseWhereClause()
        {
            return string.Format("{0}.PkAttribute = @KeyAttribute AND {0}.PkProduct = @KeyProduct", Product2Attribute.DbTableName);
        }

        string GetProductWhereClause()
        {
            return string.Format("{0}.PkProduct = @KeyProduct", Product2Attribute.DbTableName);
        }
        string GetAttributeWhereClause()
        {
            return string.Format("{0}.PkAttribute = @KeyAttribute", Product2Attribute.DbTableName);
        }
        string GetProductInWhereClause(List<string> productKeyList)
        {
            StringBuilder strIn = new StringBuilder();
            foreach (string productKey in productKeyList)
            {
                if (strIn.Length > 0)
                {
                    strIn.Append(",");
                }
                strIn.Append(string.Format("'{0}'", productKey));
            }
            return string.Format("{0}.PkProduct IN ({1})", Product2Attribute.DbTableName, strIn.ToString());
        }
        string GetProductCategoryInWhereClause(List<string> productCategoryKeyList)
        {
            return string.Format("{0}.PkProduct IN (SELECT {1}.pkProduct FROM {1} WHERE {1}.pkCategory IN ({2}))", Product2Attribute.DbTableName, Product2Category.DbTableName, GetKeysForInClause(productCategoryKeyList));
        }
    }

    [TableName(Product2Attribute.DbTableName)]
    public class Product2Attribute : _BaseRepositoryRec
    {
        public const string DbTableName = "poProduct2Attribute";

        public Guid PkAttribute { get; set; }
        public Guid PkProduct { get; set; }
    }

    public class Product2AttributeEx: _BaseRepositoryRec
    {
        public Guid PkAttribute { get; set; }
        public Guid PkProduct { get; set; }
        public string ProductAttributeName { get; set; }
        public int ProductAttributeOrder { get; set; }
    }
}