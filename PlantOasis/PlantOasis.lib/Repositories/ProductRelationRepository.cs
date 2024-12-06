using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantOasis.lib.Repositories
{
    public class ProductRelationRepository : _BaseRepository
    {
        public List<ProductRelation> GetForProduct(Guid productKey)
        {
            var sql = GetBaseQuery().Where(GetProductMainWhereClause(), new { KeyProductMain = productKey });

            return Fetch<ProductRelation>(sql);
        }

        public bool Insert(ProductRelation dataRec)
        {
            var sql = new Sql();
            sql.Append(string.Format("INSERT INTO {0} (pkProductMain, pkProductRelated) VALUES (@PkProductMain, @PkProductRelated)",
                ProductRelation.DbTableName),
                new { PkProductMain = dataRec.PkProductMain, PkProductRelated = dataRec.PkProductRelated });

            return Execute(sql) > 0;
        }

        public bool Delete(ProductRelation dataRec)
        {
            var sql = new Sql();
            sql.Append(string.Format("DELETE {0} WHERE pkProductMain=@PkProductMain AND pkProductRelated=@PkProductRelated", ProductRelation.DbTableName),
                new { PkProductMain = dataRec.PkProductMain, PkProductRelated = dataRec.PkProductRelated });

            return Execute(sql) > 0;
        }

        public bool DeleteForProduct(Guid productKey)
        {
            bool isOK = true;
            List<ProductRelation> dataList = GetForProduct(productKey);
            foreach (ProductRelation dataRec in dataList)
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
            return new Sql(string.Format("SELECT * FROM {0}", ProductRelation.DbTableName));
        }

        string GetProductMainWhereClause()
        {
            return string.Format("{0}.pkProductMain = @KeyProductMain", ProductRelation.DbTableName);
        }
    }

    [TableName(ProductRelation.DbTableName)]
    public class ProductRelation : _BaseRepositoryRec
    {
        public const string DbTableName = "poProductRelation";

        public Guid PkProductMain { get; set; }
        public Guid PkProductRelated { get; set; }
    }
}