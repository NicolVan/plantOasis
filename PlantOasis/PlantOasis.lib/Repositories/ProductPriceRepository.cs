using NPoco;
using PlantOasis.lib.Models;
using PlantOasis.lib.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Composing;

namespace PlantOasis.lib.Repositories
{
    public class ProductPriceRepository : _BaseRepository
    {
        public List<ProductPrice> GetForProduct(Guid productKey)
        {
            var sql = GetBaseQuery().Where(GetProductWhereClause(), new { ProductKey = productKey });
            sql.Append("ORDER BY ValidFrom DESC, ValidTo DESC");

            return Fetch<ProductPrice>(sql);
        }

        public List<ProductPrice> GetForDate(DateTime dateAt)
        {
            var sql = GetBaseQuery().Where(GetValidAtWhereClause(), new { ValidAt = dateAt });

            return Fetch<ProductPrice>(sql);
        }

        public ProductPrice Get(Guid key)
        {
            var sql = GetBaseQuery().Where(GetBaseWhereClause(), new { Key = key });

            return Fetch<ProductPrice>(sql).FirstOrDefault();
        }

        public ProductPrice GetStandardPrice(Guid productKey)
        {
            var sql = GetBaseQuery().Where(GetProductWhereClause(), new { ProductKey = productKey });
            sql.Where(GetStandardPriceWhereClause());

            return Fetch<ProductPrice>(sql).FirstOrDefault();
        }

        public bool Save(ProductPrice dataRec)
        {
            ProductPriceCache.ClearCache();

            if (IsNew(dataRec))
            {
                return Insert(dataRec);
            }
            else
            {
                return Update(dataRec);
            }
        }

        bool Insert(ProductPrice dataRec)
        {
            bool finalResult = false;

            dataRec.pk = Guid.NewGuid();

            using (var scope = Current.ScopeProvider.CreateScope())
            {
                bool transOk = false;
                try
                {
                    scope.Database.BeginTransaction();
                    object result = scope.Database.Insert<ProductPrice>(dataRec);

                    if (result is Guid)
                    {
                        if ((Guid)result == dataRec.pk)
                        {
                            if (dataRec.ValidTo == null)
                            {
                                // New standard price
                                // Set valid to date for previous standard price
                                var sql = new Sql(string.Format("UPDATE {0} SET validTo=@ValidTo", ProductPrice.DbTableName),
                                    new { ValidTo = dataRec.ValidFrom.AddDays(-1) });
                                sql.Where(GetNegativeBaseWhereClause(), new { Key = dataRec.pk });
                                sql.Where(GetProductWhereClause(), new { ProductKey = dataRec.ProductKey });
                                sql.Where(GetStandardPriceWhereClause());
                                scope.Database.Execute(sql);
                            }
                            scope.Database.CompleteTransaction();
                            transOk = true;
                            finalResult = true;
                        }
                    }

                }
                finally
                {
                    if (!transOk)
                    {
                        scope.Database.AbortTransaction();
                    }
                    scope.Complete();
                }
            }

            return finalResult;
        }

        bool Update(ProductPrice dataRec)
        {
            return UpdateInstance(dataRec);
        }

        public bool Delete(ProductPrice dataRec)
        {
            ProductPriceCache.ClearCache();

            return DeleteInstance(dataRec);
        }

        Sql GetBaseQuery()
        {
            return new Sql(string.Format("SELECT * FROM {0}", ProductPrice.DbTableName));
        }

        string GetBaseWhereClause()
        {
            return string.Format("{0}.pk = @Key", ProductPrice.DbTableName);
        }
        string GetNegativeBaseWhereClause()
        {
            return string.Format("{0}.pk <> @Key", ProductPrice.DbTableName);
        }
        string GetProductWhereClause()
        {
            return string.Format("{0}.productKey = @ProductKey", ProductPrice.DbTableName);
        }
        string GetStandardPriceWhereClause()
        {
            return string.Format("{0}.validTo IS NULL", ProductPrice.DbTableName);
        }
        string GetValidAtWhereClause()
        {
            return string.Format("{0}.validFrom <= @ValidAt AND ({0}.validTo IS NULL OR {0}.validTo >= @ValidAt)", ProductPrice.DbTableName);
        }
    }

    [TableName(ProductPrice.DbTableName)]
    [PrimaryKey("pk", AutoIncrement = false)]
    public class ProductPrice : _BaseRepositoryRec
    {
        public const string DbTableName = "poProductPrice";

        public Guid ProductKey { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public decimal VatRate { get; set; }
        public decimal Price_1_NoVat { get; set; }
        public decimal Price_1_WithVat { get; set; }

        public static ProductPrice CreateUnknownPrice(Guid procutKey)
        {
            return new ProductPrice()
            {
                ProductKey = procutKey,
                ValidFrom = new DateTime(2025, 1, 1),
                Price_1_NoVat = 1,
                Price_1_WithVat = 1,
            };
        }
    }

    public class ProductPriceCache
    {
        static readonly object _cacheLock = new object();
        private static Hashtable htCache = null;
        private static DateTime htCreatedAt;

        public static void ClearCache()
        {
            lock (_cacheLock)
            {
                ProductPriceCache.htCache = null;
            }
        }

        public static ProductPriceInfo GetProductPrice(Guid productKey)
        {
           ProductPricesAtDate pricesCollection;

            lock (_cacheLock)
            {
                if (htCache != null && htCreatedAt < DateTime.Now.AddDays(-1))
                {
                  
                    htCache = null;
                }
                if (htCache == null)
                {
                    htCache = new Hashtable();
                    htCreatedAt = DateTime.Now;
                }

                DateTime today = DateTime.Today;
                string key = DateTimeUtil.GetDateId(today);
                if (!htCache.ContainsKey(key))
                {
                    htCache.Add(key, new ProductPricesAtDate(today));
                }

                pricesCollection = (ProductPricesAtDate)htCache[key];
            }

            return pricesCollection.GetProductPrice(productKey);
        }
    }

    public class ProductPricesAtDate
    {
        private Hashtable htProduct;

        public ProductPricesAtDate(DateTime dateAt)
        {
            this.htProduct = new Hashtable();
            foreach (ProductPrice price in new ProductPriceRepository().GetForDate(dateAt))
            {
                ProductPriceInfo priceInfo;
                string key = price.ProductKey.ToString();
                if (!this.htProduct.ContainsKey(key))
                {
                    this.htProduct.Add(key, priceInfo = new ProductPriceInfo(dateAt));
                }
                else
                {
                    priceInfo = (ProductPriceInfo)htProduct[key];
                }

                priceInfo.SetPrice(price);
            }
        }

        public ProductPriceInfo GetProductPrice(Guid productKey)
        {
            string key = productKey.ToString();

            ProductPriceInfo productPriceInfo = null;
            if (!htProduct.ContainsKey(key))
            {
                this.htProduct.Add(key, productPriceInfo = new ProductPriceInfo(DateTime.Today));
                productPriceInfo.SetPrice(ProductPrice.CreateUnknownPrice(productKey));
            }

            productPriceInfo = (ProductPriceInfo)htProduct[key];
            productPriceInfo.EnsureStandardPrice();

            return productPriceInfo;
        }
    }

    public class ProductPriceInfo
    {
        public DateTime ValidAt { get; private set; }
        public ProductPrice StandardPrice { get; private set; }
        public ProductPriceModel StandardPriceModel
        {
            get
            {
                return ProductPriceModel.CreateCopyFrom(this.StandardPrice);
            }
        }
        public ProductPrice ActionPrice { get; private set; }
        public ProductPriceModel ActionPriceModel
        {
            get
            {
                return ProductPriceModel.CreateCopyFrom(this.ActionPrice);
            }
        }
        public ProductPrice CurrentPrice
        {
            get
            {
                return this.ActionPrice != null ? this.ActionPrice : this.StandardPrice;
            }
        }
        public ProductPriceModel CurrentPriceModel
        {
            get
            {
                return ProductPriceModel.CreateCopyFrom(this.CurrentPrice);
            }
        }

        public ProductPriceInfo(DateTime dateAt)
        {
            this.ValidAt = dateAt;
        }

        public void SetPrice(ProductPrice price)
        {
            if (price.ValidTo == null)
            {
                // Standard price
                if (this.StandardPrice == null || this.StandardPrice.ValidFrom < price.ValidFrom)
                {
                    this.StandardPrice = price;
                }
            }
            else
            {
                // Action price
                if (this.ActionPrice == null || this.ActionPrice.ValidFrom < price.ValidFrom)
                {
                    this.ActionPrice = price;
                }
            }
        }
        public void EnsureStandardPrice()
        {
            if (this.StandardPrice == null)
            {
                if (this.ActionPrice == null)
                {
                    this.StandardPrice = new ProductPrice();
                }
                else
                {
                    this.StandardPrice = this.ActionPrice;
                }
            }
        }

        public decimal GetCurrentPriceNoVat()
        {
            return this.CurrentPrice.Price_1_NoVat;
        }
        public decimal GetCurrentPriceVatPerc()
        {
            return this.CurrentPrice.VatRate;
        }

        public decimal GetCurrentPriceWithVat()
        {
            return this.CurrentPrice.Price_1_WithVat;
        }
    }

    public class ProductPriceComparer : IComparer<ProductPrice>
    {
        public int Compare(ProductPrice x, ProductPrice y)
        {
            if (x == null)
            {
                return -1;
            }
            if (y == null)
            {
                return 1;
            }

            return decimal.Compare(x.Price_1_NoVat, y.Price_1_NoVat);
        }
    }
}