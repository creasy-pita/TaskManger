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
	public partial class tb_packageversion_dal
    {
        public virtual bool Add(DbConn PubConn, tb_packageversion_model model)
        {

            List<ProcedureParameter> Par = new List<ProcedureParameter>()
                {
					
					//
					new ProcedureParameter("@packageid",    model.packageid),
					//
					new ProcedureParameter("@versionno",    model.versionno),
					//
					new ProcedureParameter("@createtime",    model.createtime),
					new ProcedureParameter("@zipfile",    model.zipfile),
					//
					new ProcedureParameter("@zipfilename",    model.zipfilename)   
                };
            int rev = PubConn.ExecuteSql(@"insert into tb_packageversion(packageid,versionno,createtime,zipfile,zipfilename)
										   values(@packageid,@versionno,@createtime,@zipfile,@zipfilename)", Par);
            return rev == 1;

        }

        public virtual bool Edit(DbConn PubConn, tb_packageversion_model model)
        {
            List<ProcedureParameter> Par = new List<ProcedureParameter>()
            {
                    
					//
					new ProcedureParameter("@packageid",    model.packageid),
					//
					new ProcedureParameter("@versionno",    model.versionno),
					//压缩文件二进制文件
					new ProcedureParameter("@zipfile",    model.zipfile),
					//
					new ProcedureParameter("@zipfilename",    model.zipfilename)
            };
			Par.Add(new ProcedureParameter("@id",  model.id));

            int rev = PubConn.ExecuteSql("update tb_packageversion set packageid=@packageid,versionno=@versionno,zipfile=@zipfile,zipfilename=@zipfilename where id=@id", Par);
            return rev == 1;

        }

        public virtual bool Delete(DbConn PubConn, int id)
        {
            List<ProcedureParameter> Par = new List<ProcedureParameter>();
            Par.Add(new ProcedureParameter("@id",  id));

            string Sql = "delete from tb_packageversion where id=@id";
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

        public virtual tb_packageversion_model Get(DbConn PubConn, int id)
        {
            List<ProcedureParameter> Par = new List<ProcedureParameter>();
            Par.Add(new ProcedureParameter("@id", id));
            StringBuilder stringSql = new StringBuilder();
            stringSql.Append(@"select s.* from tb_packageversion s where s.id=@id");
            DataSet ds = new DataSet();
            PubConn.SqlToDataSet(ds, stringSql.ToString(), Par);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
				return CreateModel(ds.Tables[0].Rows[0]);
            }
            return null;
        }

        public List<tb_packageversion_model> GetVersionByPackageID(DbConn pubConn, int packageid)
        {
            return SqlHelper.Visit(ps =>
            {
                ps.Add("@packageid", packageid);
                string sql = "select versionno,zipfilename from tb_packageversion where packageid=@packageid";
                DataSet ds = new DataSet();
                pubConn.SqlToDataSet(ds, sql, ps.ToParameters());
                List<tb_packageversion_model> model = new List<tb_packageversion_model>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    tb_packageversion_model m = CreateModel(dr);
                    model.Add(m);
                }
                return model;
            });
        }

        public virtual tb_packageversion_model CreateModel(DataRow dr)
        {
            var o = new tb_packageversion_model();
			
			//
			if(dr.Table.Columns.Contains("id"))
			{
				o.id = dr["id"].Toint();
			}
			//
			if(dr.Table.Columns.Contains("packageid"))
			{
				o.packageid = dr["packageid"].Toint();
			}
			//
			if(dr.Table.Columns.Contains("versionno"))
			{
				o.versionno = dr["versionno"].ToString();
			}
			//
			if(dr.Table.Columns.Contains("createtime"))
			{
				o.createtime = dr["createtime"].ToDateTime();
			}
			//压缩文件二进制文件
			if(dr.Table.Columns.Contains("zipfile"))
			{
				o.zipfile = dr["zipfile"].ToBytes();
			}
			//
			if(dr.Table.Columns.Contains("zipfilename"))
			{
				o.zipfilename = dr["zipfilename"].Tostring();
			}
			return o;
        }
    }
}