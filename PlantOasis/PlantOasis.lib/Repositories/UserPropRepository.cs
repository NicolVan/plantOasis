using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantOasis.lib.Repositories
{
    public class UserPropRepository : _BaseRepository
    {
        public UserProp Get(string sessionId, string propId)
        {
            System.Web.Security.MembershipUser user = System.Web.Security.Membership.GetUser();
            if (user == null)
            {
                return Get(0, sessionId, propId);
            }

            return Get((int)user.ProviderUserKey, sessionId, propId);
        }

        public UserProp Get(int userId, string sessionId, string propId)
        {
            var sql = GetBaseQuery().Where(GetBaseWhereClause(), new { PropId = propId });
            // Use session id identifier
            sql.Where(GetSessionWhereClause(), new { SessionId = sessionId });

            return Fetch<UserProp>(sql).FirstOrDefault();
        }

        public bool Save(string sessionId, UserProp dataRec)
        {
            if (IsNew(dataRec))
            {
                return Insert(sessionId, dataRec);
            }
            else
            {
                return Update(dataRec);
            }
        }

        bool Insert(string sessionId, UserProp dataRec)
        {
            // Use session id identifier
            dataRec.UserId = 0;
            dataRec.SessionId = sessionId;

            dataRec.pk = Guid.NewGuid();
            dataRec.DateCreate = DateTime.Now;

            object result = InsertInstance(dataRec);
            if (result is Guid)
            {
                return (Guid)result == dataRec.pk;
            }

            return false;
        }

        bool Update(UserProp dataRec)
        {
            dataRec.DateCreate = DateTime.Now; // update date create
            return UpdateInstance(dataRec);
        }

        public bool Delete(UserProp dataRec)
        {
            return DeleteInstance(dataRec);
        }
        public void Delete(string sessionId, string propId)
        {
            UserProp dataRec = Get(sessionId, propId);
            if (dataRec != null)
            {
                Delete(dataRec);
            }
        }

        Sql GetBaseQuery()
        {
            var sql = new Sql(string.Format("SELECT * FROM {0}", UserProp.DbTableName));

            return sql;
        }

        string GetBaseWhereClause()
        {
            return string.Format("{0}.propId = @PropId", UserProp.DbTableName);
        }
        string GetUserWhereClause()
        {
            return string.Format("{0}.userId = @UserId", UserProp.DbTableName);
        }
        string GetSessionWhereClause()
        {
            return string.Format("{0}.sessionId = @SessionId", UserProp.DbTableName);
        }

        public bool DeleteOldSessionData(DateTime dt)
        {
            var sql = new Sql();
            sql.Append(string.Format("DELETE FROM {0}", UserProp.DbTableName));
            sql.Where(string.Format("{0}.sessionId IS NOT NULL AND {0}.dateCreate < @DateCreate", UserProp.DbTableName), new { DateCreate = dt });
            Execute(sql);

            return true;
        }

    }

    [TableName(UserProp.DbTableName)]
    [PrimaryKey("pk", AutoIncrement = false)]
    public class UserProp : _BaseRepositoryRec
    {
        public const string DbTableName = "poUserProp";

        public int UserId { get; set; }
        public string SessionId { get; set; }
        public DateTime DateCreate { get; set; }
        public string PropId { get; set; }
        public string PropValue { get; set; }
    }
}
