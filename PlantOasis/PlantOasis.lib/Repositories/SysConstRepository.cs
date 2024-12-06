using NPoco;
using PlantOasis.lib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantOasis.lib.Repositories
{
    public class SysConstRepository : _BaseRepository
    {
        public Guid SysConstKey
        {
            get
            {
                return new Guid("11111111-1111-1111-1111-111111111111");
            }
        }
        public SysConst Get()
        {
            var sql = GetBaseQuery().Where(GetBaseWhereClause(), new { Key = this.SysConstKey });

            SysConst ret = Fetch<SysConst>(sql).FirstOrDefault();
            return ret != null ? ret : new SysConst();
        }

        public bool Save(SysConst dataRec)
        {
            SysConstUtil.Clear();

            if (IsNew(dataRec))
            {
                return Insert(dataRec);
            }
            else
            {
                return Update(dataRec);
            }
        }

        bool Insert(SysConst dataRec)
        {
            dataRec.pk = this.SysConstKey;

            object result = InsertInstance(dataRec);
            if (result is Guid)
            {
                return (Guid)result == dataRec.pk;
            }

            return false;
        }

        bool Update(SysConst dataRec)
        {
            return UpdateInstance(dataRec);
        }

        Sql GetBaseQuery()
        {
            return new Sql(string.Format("SELECT * FROM {0}", SysConst.DbTableName));
        }

        string GetBaseWhereClause()
        {
            return string.Format("{0}.pk = @Key", SysConst.DbTableName);
        }
    }

    [TableName(SysConst.DbTableName)]
    [PrimaryKey("pk", AutoIncrement = false)]
    public class SysConst : _BaseRepositoryRec
    {
        public const string DbTableName = "poSysConst";

        public string CompanyName { get; set; }
        public string CompanyIco { get; set; }
        public string CompanyDic { get; set; }
        public string CompanyIcdph { get; set; }

        public string AddressStreet { get; set; }
        public string AddressCity { get; set; }
        public string AddressZip { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public string Bank { get; set; }
        public string Iban { get; set; }
        public string Currency { get; set; }
        public decimal FreeTransportPrice { get; set; }

        public SysConst()
        {
            CompanyName = "Plant Oasis, s. r. o.";
            CompanyIco = "";
            CompanyDic = "";
            CompanyIcdph = "";

            AddressStreet = "";
            AddressCity = "";
            AddressZip = "";
            Phone = "";
            Email = "info@plantoasis.sk";

            Bank = "";
            Iban = "";
            Currency = "€";
            FreeTransportPrice = 60;
        }
    }
}

