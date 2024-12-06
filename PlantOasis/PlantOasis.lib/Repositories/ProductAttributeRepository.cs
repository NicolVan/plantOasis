using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantOasis.lib.Repositories
{
    public class ProductAttributeRepository : _BaseRepository
    {
        static readonly object _productAttributeOrderLock = new object();

        public Page<ProductAttribute> GetPage(long page, long itemsPerPage, string sortBy = null, string sortDir = null, ProductAttributeFilter filter = null)
        {
            var sql = GetBaseQuery();
            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.SearchText))
                {
                    sql.Where(GetSearchTextWhereClause(filter.SearchText), new { SearchText = filter.SearchText });
                }
            }
            if (string.IsNullOrEmpty(sortBy))
            {
                sortBy = "ProductAttributeOrder";
            }
            if (string.IsNullOrEmpty(sortDir))
            {
                sortDir = "ASC";
            }
            sql.Append(string.Format("ORDER BY {0} {1}", sortBy, sortDir));

            return GetPage<ProductAttribute>(page, itemsPerPage, sql);
        }

        public ProductAttribute Get(Guid key)
        {
            var sql = GetBaseQuery().Where(GetBaseWhereClause(), new { Key = key });

            return Fetch<ProductAttribute>(sql).FirstOrDefault();
        }

        public bool Save(ProductAttribute dataRec)
        {
            if (IsNew(dataRec))
            {
                return Insert(dataRec);
            }
            else
            {
                return Update(dataRec);
            }
        }

        bool Insert(ProductAttribute dataRec)
        {
            dataRec.pk = Guid.NewGuid();

            object result = InsertInstance(dataRec);
            if (result is Guid)
            {
                if ((Guid)result == dataRec.pk)
                {
                    // Record saved
                    // Set product attribute order
                    bool isOk = SetNewProductAttributeOrder(dataRec);

                    return isOk;
                }
                return (bool)result;
            }

            return false;
        }

        bool Update(ProductAttribute dataRec)
        {
            return UpdateInstance(dataRec);
        }

        public bool Delete(ProductAttribute dataRec)
        {
            if (DeleteInstance(dataRec))
            {
                return ReorderProductAttributes(dataRec.ProductAttributeOrder, -1);
            }

            return false;
        }

        public bool SetNewProductAttributeOrder(ProductAttribute dataRec)
        {
            return true;
        }
        public bool ReorderProductAttributes(int orderFrom, int orderOffset)
        {
            return true;
        }

        public bool SetProductAttributeOrder(Guid productAttributeKey, int order)
        {
            var sql = new Sql();
            sql.Append(string.Format("UPDATE {0} SET {1}=@Order WHERE {2}=@Key",
                ProductAttribute.DbTableName, "ProductAttributeOrder", "pk"),
                new { Order = order, Key = productAttributeKey });

            return Execute(sql) > 0;
        }
        public bool SetProductAttributeOrder(int oldOrder, int newOrder)
        {
            var sql = new Sql();
            sql.Append(string.Format("UPDATE {0} SET {1}=@NewOrder WHERE {1}=@OldOrder",
                ProductAttribute.DbTableName, "ProductAttributeOrder"),
                new { NewOrder = newOrder, OldOrder = oldOrder });

            return Execute(sql) > 0;
        }

        public int GetProductAttributeOrder(Guid productAttributeKey)
        {
            var sql = new Sql();
            sql.Append(
                string.Format("SELECT ProductAttributeOrder FROM {0} WHERE pk=@ProductAttributeKey",
                ProductAttribute.DbTableName),
                new { ProductAttributeKey = productAttributeKey });
            return ExecuteScalar<int>(sql);
        }

        public int GetMaxOrder()
        {
            var sql = new Sql();
            sql.Append(
                string.Format("SELECT MAX(ProductAttributeOrder) FROM {0}",
                ProductAttribute.DbTableName));
            return ExecuteScalar<int>(sql);
        }
        public bool MoveProductAttributeUp(Guid productAttributeKey)
        {
            lock (_productAttributeOrderLock)
            {
                int oldOrder = GetProductAttributeOrder(productAttributeKey);
                if (oldOrder > 1)
                {
                    SetProductAttributeOrder(oldOrder - 1, oldOrder);
                    SetProductAttributeOrder(productAttributeKey, oldOrder - 1);
                }
            }

            return true;
        }
        public bool MoveProductAttributeDown(Guid productAttributeKey)
        {
            lock (_productAttributeOrderLock)
            {
                int oldOrder = GetProductAttributeOrder(productAttributeKey);
                if (oldOrder < GetMaxOrder())
                {
                    SetProductAttributeOrder(oldOrder + 1, oldOrder);
                    SetProductAttributeOrder(productAttributeKey, oldOrder + 1);
                }
            }

            return true;
        }

        Sql GetBaseQuery()
        {
            return new Sql(string.Format("SELECT * FROM {0}", ProductAttribute.DbTableName));
        }

        string GetBaseWhereClause()
        {
            return string.Format("{0}.pk = @Key", ProductAttribute.DbTableName);
        }
        string GetSearchTextWhereClause(string searchText)
        {
            return string.Format("{0}.productAttributeGroup LIKE '%{1}%' collate Latin1_general_CI_AI OR {0}.productAttributeName LIKE '%{1}%' collate Latin1_general_CI_AI", ProductAttribute.DbTableName, searchText);
        }
    }


    [TableName(ProductAttribute.DbTableName)]
    [PrimaryKey("pk", AutoIncrement = false)]
    public class ProductAttribute : _BaseRepositoryRec
    {
        public const string DbTableName = "poProductAttribute";

        public string ProductAttributeGroup { get; set; }
        public string ProductAttributeName { get; set; }
        public int ProductAttributeOrder { get; set; }
    }

    public class ProductAttributeFilter
    {
        public string SearchText { get; set; }
    }
}
