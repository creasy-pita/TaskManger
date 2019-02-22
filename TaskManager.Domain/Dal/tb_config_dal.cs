using BSF.Db;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TaskManager.Domain.Model;

namespace TaskManager.Domain.Dal
{
    public partial class tb_config_dal
    {
        public virtual List<tb_config_model> GetList(DbConn PubConn)
        {
            List<ProcedureParameter> Par = new List<ProcedureParameter>();
            StringBuilder stringSql = new StringBuilder();
            stringSql.Append(@"select s.* from tb_config s");
            DataSet ds = new DataSet();
            PubConn.SqlToDataSet(ds, stringSql.ToString(), Par);
            List<tb_config_model> rs = new List<tb_config_model>();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    rs.Add(CreateModel(dr));
                }
            }
            return rs;
        }

        public virtual tb_config_model Get(DbConn PubConn, string configkey)
        {
            List<ProcedureParameter> Par = new List<ProcedureParameter>();
            Par.Add(new ProcedureParameter("@configkey", configkey));
            StringBuilder stringSql = new StringBuilder();
            stringSql.Append(@"select s.* from tb_config s where s.configkey=@configkey");
            DataSet ds = new DataSet();
            PubConn.SqlToDataSet(ds, stringSql.ToString(), Par);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return CreateModel(ds.Tables[0].Rows[0]);
            }
            return null;
        }
    }
}
