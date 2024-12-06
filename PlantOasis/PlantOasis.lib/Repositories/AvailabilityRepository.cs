using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantOasis.lib.Repositories
{
    public class AvailabilityRepository : _BaseRepository
    {
        public Page<Availability> GetPage(long page, long itemsPerPage, string sortBy = "AvailabilityName", string sortDir = "ASC")
        {
            var sql = GetBaseQuery();
            sql.Append(string.Format("ORDER BY {0} {1}", sortBy, sortDir));

            return GetPage<Availability>(page, itemsPerPage, sql);
        }

        public Availability Get(Guid key)
        {
            var sql = GetBaseQuery().Where(GetBaseWhereClause(), new { Key = key });

            return Fetch<Availability>(sql).FirstOrDefault();
        }

        public bool Save(Availability dataRec)
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

        bool Insert(Availability dataRec)
        {
            dataRec.pk = Guid.NewGuid();

            object result = InsertInstance(dataRec);
            if (result is Guid)
            {
                return (Guid)result == dataRec.pk;
            }

            return false;
        }

        bool Update(Availability dataRec)
        {
            return UpdateInstance(dataRec);
        }

        public bool Delete(Availability dataRec)
        {
            return DeleteInstance(dataRec);
        }

        Sql GetBaseQuery()
        {
            return new Sql(string.Format("SELECT * FROM {0}", Availability.DbTableName));
        }

        string GetBaseWhereClause()
        {
            return string.Format("{0}.pk = @Key", Availability.DbTableName);
        }
    }


    [TableName(Availability.DbTableName)]
    [PrimaryKey("pk", AutoIncrement = false)]
    public class Availability : _BaseRepositoryRec
    {
        public const string DbTableName = "poAvailability";

        public string AvailabilityName { get; set; }
    }
}
