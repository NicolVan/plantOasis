using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantOasis.lib.Repositories
{
 public class DodavatelRepository : _BaseRepository
    {
        public Page<Dodavatel> GetPage(long page, long itemsPerPage, string sortBy = "ProducerName", string sortDir = "ASC", DodavatelFilter filter = null)
        {
            var sql = GetBaseQuery();
            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.SearchText))
                {
                    sql.Where(GetSearchTextWhereClause(filter.SearchText), new { SearchText = filter.SearchText });
                }
            }
            sql.Append(string.Format("ORDER BY {0} {1}", sortBy, sortDir));

            return GetPage<Dodavatel>(page, itemsPerPage, sql);
        }

        public Dodavatel Get(Guid key)
        {
            var sql = GetBaseQuery().Where(GetBaseWhereClause(), new { Key = key });

            return Fetch<Dodavatel>(sql).FirstOrDefault();
        }

        public bool Save(Dodavatel dataRec)
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

        bool Insert(Dodavatel dataRec)
        {
            dataRec.pk = Guid.NewGuid();

            object result = InsertInstance(dataRec);
            if (result is Guid)
            {
                return (Guid)result == dataRec.pk;
            }

            return false;
        }

        bool Update(Dodavatel dataRec)
        {
            return UpdateInstance(dataRec);
        }

        public bool Delete(Dodavatel dataRec)
        {
            return DeleteInstance(dataRec);
        }

        Sql GetBaseQuery()
        {
            return new Sql(string.Format("SELECT * FROM {0}", Dodavatel.DbTableName));
        }

        string GetBaseWhereClause()
        {
            return string.Format("{0}.pk = @Key", Dodavatel.DbTableName);
        }
        string GetSearchTextWhereClause(string searchText)
        {
            return string.Format("{0}.producerName LIKE '%{1}%' collate Latin1_general_CI_AI OR {0}.producerDescription LIKE '%{1}%' collate Latin1_general_CI_AI OR {0}.producerWeb LIKE '%{1}%' collate Latin1_general_CI_AI", Dodavatel.DbTableName, searchText);
        }
    }


    [TableName(Dodavatel.DbTableName)]
    [PrimaryKey("pk", AutoIncrement = false)]
    public class Dodavatel : _BaseRepositoryRec
    {
        public const string DbTableName = "poProducer";

        public string ProducerName { get; set; }
        public string ProducerDescription { get; set; }
        public string ProducerWeb { get; set; }
    }

    public class DodavatelFilter
    {
        public string SearchText { get; set; }
    }
}