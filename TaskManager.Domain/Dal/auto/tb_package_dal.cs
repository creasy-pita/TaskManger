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
	public partial class tb_package_dal
    {
        public virtual bool Add(DbConn PubConn, tb_package_model model)
        {

            List<ProcedureParameter> Par = new List<ProcedureParameter>()
                {
					new ProcedureParameter("@packagename",    model.packagename),
					new ProcedureParameter("@createtime",    model.createtime),
					new ProcedureParameter("@remark",    model.remark),
                };
            int rev = PubConn.ExecuteSql(@"insert into tb_version(packagename,createtime,remark)
										   values(@packagename,@createtime,@remark)", Par);
            return rev == 1;

        }

        public virtual bool Edit(DbConn PubConn, tb_package_model model)
        {
            List<ProcedureParameter> Par = new List<ProcedureParameter>()
            {
                    new ProcedureParameter("@packagename",    model.packagename),
                    new ProcedureParameter("@remark",    model.remark),
            };
			Par.Add(new ProcedureParameter("@id",  model.id));

            int rev = PubConn.ExecuteSql("update tb_package set packagename=@packagename,remark=@remark where id=@id", Par);
            return rev == 1;

        }

        public virtual bool Delete(DbConn PubConn, int id)
        {
            List<ProcedureParameter> Par = new List<ProcedureParameter>();
            Par.Add(new ProcedureParameter("@id",  id));

            string Sql = "delete from tb_package where id=@id";
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

        public virtual tb_package_model Get(DbConn PubConn, int id)
        {
            List<ProcedureParameter> Par = new List<ProcedureParameter>();
            Par.Add(new ProcedureParameter("@id", id));
            StringBuilder stringSql = new StringBuilder();
            stringSql.Append(@"select s.* from tb_package s where s.id=@id");
            DataSet ds = new DataSet();
            PubConn.SqlToDataSet(ds, stringSql.ToString(), Par);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
				return CreateModel(ds.Tables[0].Rows[0]);
            }
            return null;
        }


        public virtual tb_package_model CreateModel(DataRow dr)
        {
            var o = new tb_package_model();
			
			//
			if(dr.Table.Columns.Contains("id"))
			{
				o.id = dr["id"].Toint();
			}
			//
			if(dr.Table.Columns.Contains("packagename"))
			{
				o.packagename = dr["packagename"].ToString();
			}
			//
			if(dr.Table.Columns.Contains("remark"))
			{
				o.remark = dr["remark"].ToString();
			}
			//
			if(dr.Table.Columns.Contains("createtime"))
			{
				o.createtime = dr["createtime"].ToDateTime();
			}

			return o;
        }
    }
}