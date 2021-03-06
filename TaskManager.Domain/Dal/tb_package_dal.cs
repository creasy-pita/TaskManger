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
        public int AddOrUpdate(DbConn PubConn, tb_package_model model)
        {
            return SqlHelper.Visit(ps =>
            {
                ps.Add("@packagename", model.packagename);
                ps.Add("@remark", model.remark);
                ps.Add("@createtime", model.createtime);
                ps.Add("@id", model.id);
                string updatecmd = "update tb_package set packagename=@packagename,remark=@remark where id=@id";
                string insertcmd = @"insert into tb_package(packagename,remark,createtime)
										   values(@packagename,@remark,@createtime)";
                if (PubConn.ExecuteSql(updatecmd, ps.ToParameters()) <= 0)
                {
                    PubConn.ExecuteSql(insertcmd, ps.ToParameters());
                }
                return 1;
            });
        }

        public List<tb_package_model> GetList(DbConn PubConn, string keyword, string cstime, string cetime, int pagesize, int pageindex, out int count)
        {
            int _count = 0;
            List<tb_package_model> Model = new List<tb_package_model>();
            DataSet dsList = SqlHelper.Visit<DataSet>(ps =>
            {
                string sqlwhere = "";
                //string sql = "select ROW_NUMBER() over(order by id desc) as rownum,id,nodename,nodeostype,nodecreatetime,nodeip,nodelastupdatetime,ifcheckstate from tb_node where 1=1 ";
                string sql = "select * from tb_package where 1=1 ";
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    ps.Add("keyword", keyword);
                    sqlwhere = " and (packagename like '%'+@keyword+'%' or remark like '%'+@keyword+'%') ";
                }
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
                _count = Convert.ToInt32(PubConn.ExecuteScalar("select count(1) from tb_package where 1=1 " + sqlwhere, ps.ToParameters()));
                DataSet ds = new DataSet();
                //string sqlSel = "select * from (" + sql + sqlwhere + ") A where rownum between " + ((pageindex - 1) * pagesize + 1) + " and " + pagesize * pageindex;
                string sqlSel = sql + sqlwhere + " order by id desc limit " + ((pageindex - 1) * pagesize) + "," + pagesize;
                PubConn.SqlToDataSet(ds, sqlSel, ps.ToParameters());
                return ds;
            });
            foreach (DataRow dr in dsList.Tables[0].Rows)
            {
                tb_package_model n = CreateModel(dr);
                Model.Add(n);
            }
            count = _count;
            return Model;
        }

        public List<tb_package_model> GetListAll(DbConn PubConn)
        {
            return SqlHelper.Visit<List<tb_package_model>>(ps =>
            {
                List<tb_package_model> Model = new List<tb_package_model>();
                string sql = "select * from tb_package";
                DataSet ds = new DataSet();
                PubConn.SqlToDataSet(ds, sql, ps.ToParameters());
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    tb_package_model n = CreateModel(dr);
                    Model.Add(n);
                }
                return Model;
            });
        }
    }
}