using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BSF.Extensions;
using BSF.Db;
using TaskManager.Domain.Model;

namespace TaskManager.Domain.Dal
{
	/*代码自动生成工具自动生成,不要在这里写自己的代码，否则会被自动覆盖哦 - 车毅*/
	public partial class tb_config_dal
    {
        public virtual bool Add(DbConn PubConn, tb_config_model model)
        {

            List<ProcedureParameter> Par = new List<ProcedureParameter>()
                {
					
					//
					new ProcedureParameter("@configkey",    model.configkey),
					//
					new ProcedureParameter("@configvalue",    model.configvalue),
					//
					new ProcedureParameter("@lastupdatetime",    model.lastupdatetime),
                    //
					new ProcedureParameter("@remark",    model.remark),
                };
            int rev = PubConn.ExecuteSql(@"insert into tb_config(configkey,configvalue,remark,lastupdatetime)
										   values(@configkey,@configvalue,@remark,@lastupdatetime)", Par);
            return rev == 1;

        }

        public virtual bool Edit(DbConn PubConn, tb_config_model model)
        {
            List<ProcedureParameter> Par = new List<ProcedureParameter>()
            {
                    
					//
					new ProcedureParameter("@configkey",    model.configkey),
					//
					new ProcedureParameter("@configvalue",    model.configvalue),
                    //
					new ProcedureParameter("@remark",    model.remark),
					//
					new ProcedureParameter("@lastupdatetime",    model.lastupdatetime),
            };
			Par.Add(new ProcedureParameter("@id",  model.id));

            int rev = PubConn.ExecuteSql("update tb_config set configkey=@configkey,configvalue=@configvalue,remark=@remark,lastupdatetime=@lastupdatetime where id=@id", Par);
            return rev == 1;

        }

        public virtual bool Delete(DbConn PubConn, int id)
        {
            List<ProcedureParameter> Par = new List<ProcedureParameter>();
            Par.Add(new ProcedureParameter("@id",  id));

            string Sql = "delete from tb_config where id=@id";
            int rev = PubConn.ExecuteSql(Sql, Par);
            if (rev == 1)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public virtual tb_config_model Get(DbConn PubConn, int id)
        {
            List<ProcedureParameter> Par = new List<ProcedureParameter>();
            Par.Add(new ProcedureParameter("@id", id));
            StringBuilder stringSql = new StringBuilder();
            stringSql.Append(@"select s.* from tb_config s where s.id=@id");
            DataSet ds = new DataSet();
            PubConn.SqlToDataSet(ds, stringSql.ToString(), Par);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
				return CreateModel(ds.Tables[0].Rows[0]);
            }
            return null;
        }

		public virtual tb_config_model CreateModel(DataRow dr)
        {
            var o = new tb_config_model();
			
			//
			if(dr.Table.Columns.Contains("id"))
			{
				o.id = dr["id"].Toint();
			}
			//
			if(dr.Table.Columns.Contains("configkey"))
			{
				o.configkey = dr["configkey"].Tostring();
			}
			//
			if(dr.Table.Columns.Contains("configvalue"))
			{
				o.configvalue = dr["configvalue"].Tostring();
			}
            //
			if(dr.Table.Columns.Contains("remark"))
			{
				o.remark = dr["remark"].Tostring();
			}
			//
			if(dr.Table.Columns.Contains("lastupdatetime"))
			{
				o.lastupdatetime = dr["lastupdatetime"].ToDateTime();
			}
			
			return o;
        }
    }
}