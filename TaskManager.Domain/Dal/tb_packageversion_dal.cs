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
        public int AddOrUpdate(DbConn PubConn, tb_packageversion_model model)
        {
            return SqlHelper.Visit(ps =>
            {
                ps.Add("@packageid", model.packageid);
                ps.Add("@versionno", model.versionno);
                ps.Add("@createtime", model.createtime);
                ps.Add("@zipfile", model.zipfile);
                ps.Add("@zipfilename", model.zipfilename);
                ps.Add("@id", model.id);
                string updatecmd = "update tb_packageversion set packageid=@packageid,versionno=@versionno,zipfile=@zipfile,zipfilename=@zipfilename where id=@id";
                string insertcmd = @"insert into tb_packageversion(packageid,versionno,createtime,zipfile,zipfilename)
										   values(@packageid,@versionno,@createtime,@zipfile,@zipfilename)";
                if (PubConn.ExecuteSql(updatecmd, ps.ToParameters()) <= 0)
                {
                    PubConn.ExecuteSql(insertcmd, ps.ToParameters());
                }
                return 1;
            });
        }

        public List<tb_packageversion_model> GetListByPackageId(DbConn PubConn, int packageId)
        {
            List<tb_packageversion_model> modelList = new List<tb_packageversion_model>();
            DataSet dsList = SqlHelper.Visit<DataSet>(ps =>
            {
                string sql = "select id,versionno,zipfilename from tb_packageversion where packageid=@packageid";
                DataSet ds = new DataSet();
                ps.Add("packageid", packageId);
                PubConn.SqlToDataSet(ds, sql, ps.ToParameters());
                return ds;
            });
            foreach (DataRow dr in dsList.Tables[0].Rows)
            {
                tb_packageversion_model m = CreateModel(dr);
                modelList.Add(m);
            }
            return modelList;
        }

        public List<tb_packageversion_model> GetListByPackageId(DbConn PubConn, int packageId, string cstime, string cetime, int pagesize, int pageindex, out int count)
        {
            int _count = 0;
            List<tb_packageversion_model> Model = new List<tb_packageversion_model>();
            DataSet dsList = SqlHelper.Visit<DataSet>(ps =>
            {
                string sqlwhere = "";
                //string sql = "select ROW_NUMBER() over(order by id desc) as rownum,id,nodename,nodeostype,nodecreatetime,nodeip,nodelastupdatetime,ifcheckstate from tb_node where 1=1 ";
                string sql = "select * from tb_packageversion where 1=1 ";

                    ps.Add("packageid", packageId);
                    sqlwhere = " and packageid =@packageid ";

                DateTime d = DateTime.Now;
                if (DateTime.TryParse(cstime, out d))
                {
                    ps.Add("CStime", Convert.ToDateTime(cstime));
                    sqlwhere += " and createtime>=@CStime";
                }
                if (DateTime.TryParse(cetime, out d))
                {
                    ps.Add("CEtime", Convert.ToDateTime(cetime));
                    sqlwhere += " and createtime<=@CEtime";
                }
                _count = Convert.ToInt32(PubConn.ExecuteScalar("select count(1) from tb_packageversion where 1=1 " + sqlwhere, ps.ToParameters()));
                DataSet ds = new DataSet();
                //string sqlSel = "select * from (" + sql + sqlwhere + ") A where rownum between " + ((pageindex - 1) * pagesize + 1) + " and " + pagesize * pageindex;
                string sqlSel = sql + sqlwhere + " order by id desc limit " + ((pageindex - 1) * pagesize) + "," + pagesize;
                PubConn.SqlToDataSet(ds, sqlSel, ps.ToParameters());
                return ds;
            });
            foreach (DataRow dr in dsList.Tables[0].Rows)
            {
                tb_packageversion_model n = CreateModel(dr);
                Model.Add(n);
            }
            count = _count;
            return Model;
        }
    }
}