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
	public partial class tb_webtask_dal
    {
        public virtual bool Add(DbConn PubConn, tb_webtask_model model)
        {

            List<ProcedureParameter> Par = new List<ProcedureParameter>()
                {
					
					//
					new ProcedureParameter("@taskname",    model.taskname),
					//
					new ProcedureParameter("@categoryid",    model.categoryid),
					//
					new ProcedureParameter("@nodeid",    model.nodeid),
					//
					new ProcedureParameter("@taskcreatetime",    model.taskcreatetime),
					//
					new ProcedureParameter("@taskupdatetime",    model.taskupdatetime),
					//
					new ProcedureParameter("@tasklaststarttime",    model.tasklaststarttime),
					//
					new ProcedureParameter("@tasklastendtime",    model.tasklastendtime),
					//
					new ProcedureParameter("@tasklasterrortime",    model.tasklasterrortime),
					//
					new ProcedureParameter("@taskerrorcount",    model.taskerrorcount),
					//
					new ProcedureParameter("@taskruncount",    model.taskruncount),
					//
					new ProcedureParameter("@taskcreateuserid",    model.taskcreateuserid),
					//
					new ProcedureParameter("@taskstate",    model.taskstate),
					//
					new ProcedureParameter("@taskport",    model.taskport),
					//
					new ProcedureParameter("@taskhealthcheckurl",    model.taskhealthcheckurl),
					//
					new ProcedureParameter("@taskpath",    model.taskpath),
					//
					new ProcedureParameter("@taskstartfilename",    model.taskstartfilename),
					//
					new ProcedureParameter("@taskarguments",    model.taskarguments),
					//
					new ProcedureParameter("@taskremark",    model.taskremark)   
                };
            int rev = PubConn.ExecuteSql(@"insert into tb_webtask(taskname,categoryid,nodeid,taskcreatetime,taskupdatetime,tasklaststarttime,tasklastendtime,tasklasterrortime,taskerrorcount,taskruncount,taskcreateuserid,taskstate,taskport,taskhealthcheckurl,taskpath,taskstartfilename,taskarguments,taskremark)
										   values(@taskname,@categoryid,@nodeid,@taskcreatetime,@taskupdatetime,@tasklaststarttime,@tasklastendtime,@tasklasterrortime,@taskerrorcount,@taskruncount,@taskcreateuserid,@taskstate,@taskport,@taskhealthcheckurl,@taskpath,@taskstartfilename,@taskarguments,@taskremark)", Par);
            return rev == 1;

        }

        public virtual bool Edit(DbConn PubConn, tb_webtask_model model)
        {
            List<ProcedureParameter> Par = new List<ProcedureParameter>()
            {
                    
					//
					new ProcedureParameter("@taskname",    model.taskname),
					//
					new ProcedureParameter("@categoryid",    model.categoryid),
					//
					new ProcedureParameter("@nodeid",    model.nodeid),
					//
					new ProcedureParameter("@taskcreatetime",    model.taskcreatetime),
					//
					new ProcedureParameter("@taskupdatetime",    model.taskupdatetime),
					//
					new ProcedureParameter("@tasklaststarttime",    model.tasklaststarttime),
					//
					new ProcedureParameter("@tasklastendtime",    model.tasklastendtime),
					//
					new ProcedureParameter("@tasklasterrortime",    model.tasklasterrortime),
					//
					new ProcedureParameter("@taskerrorcount",    model.taskerrorcount),
					//
					new ProcedureParameter("@taskruncount",    model.taskruncount),
					//
					new ProcedureParameter("@taskcreateuserid",    model.taskcreateuserid),
					//
					new ProcedureParameter("@taskstate",    model.taskstate),
					//
					new ProcedureParameter("@taskport",    model.taskport),
					//
					new ProcedureParameter("@taskhealthcheckurl",    model.taskhealthcheckurl),
					//
					new ProcedureParameter("@taskpath",    model.taskpath),
					//
					new ProcedureParameter("@taskstartfilename",    model.taskstartfilename),
					//
					new ProcedureParameter("@taskarguments",    model.taskarguments),
					//
					new ProcedureParameter("@taskremark",    model.taskremark)
            };
			Par.Add(new ProcedureParameter("@id",  model.id));

            int rev = PubConn.ExecuteSql("update tb_webtask set taskname=@taskname,categoryid=@categoryid,nodeid=@nodeid,taskcreatetime=@taskcreatetime,taskupdatetime=@taskupdatetime,tasklaststarttime=@tasklaststarttime,tasklastendtime=@tasklastendtime,tasklasterrortime=@tasklasterrortime,taskerrorcount=@taskerrorcount,taskruncount=@taskruncount,taskcreateuserid=@taskcreateuserid,taskstate=@taskstate,taskport=@taskport,taskhealthcheckurl=@taskhealthcheckurl,taskpath=@taskpath,taskstartfilename=@taskstartfilename,taskarguments=@taskarguments,taskremark=@taskremark where id=@id", Par);
            return rev == 1;

        }

        public virtual bool Delete(DbConn PubConn, int id)
        {
            List<ProcedureParameter> Par = new List<ProcedureParameter>();
            Par.Add(new ProcedureParameter("@id",  id));

            string Sql = "delete from tb_webtask where id=@id";
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

        public virtual tb_webtask_model Get(DbConn PubConn, int id)
        {
            List<ProcedureParameter> Par = new List<ProcedureParameter>();
            Par.Add(new ProcedureParameter("@id", id));
            StringBuilder stringSql = new StringBuilder();
            stringSql.Append(@"select s.* from tb_webtask s where s.id=@id");
            int rev = PubConn.ExecuteSql(stringSql.ToString(), Par);
            DataSet ds = new DataSet();
            PubConn.SqlToDataSet(ds, stringSql.ToString(), Par);
            if (ds != null && ds.Tables.Count > 0)
            {
				return CreateModel(ds.Tables[0].Rows[0]);
            }
            return null;
        }

		public virtual tb_webtask_model CreateModel(DataRow dr)
        {
            var o = new tb_webtask_model();
			
			//
			if(dr.Table.Columns.Contains("id"))
			{
				o.id = dr["id"].Toint();
			}
			//
			if(dr.Table.Columns.Contains("taskname"))
			{
				o.taskname = dr["taskname"].Tostring();
			}
			//
			if(dr.Table.Columns.Contains("categoryid"))
			{
				o.categoryid = dr["categoryid"].Toint();
			}
			//
			if(dr.Table.Columns.Contains("nodeid"))
			{
				o.nodeid = dr["nodeid"].Toint();
			}
			//
			if(dr.Table.Columns.Contains("taskcreatetime"))
			{
				o.taskcreatetime = dr["taskcreatetime"].ToDateTime();
			}
			//
			if(dr.Table.Columns.Contains("taskupdatetime"))
			{
				o.taskupdatetime = dr["taskupdatetime"].ToDateTime();
			}
			//
			if(dr.Table.Columns.Contains("tasklaststarttime"))
			{
				o.tasklaststarttime = dr["tasklaststarttime"].ToDateTime();
			}
			//
			if(dr.Table.Columns.Contains("tasklastendtime"))
			{
				o.tasklastendtime = dr["tasklastendtime"].ToDateTime();
			}
			//
			if(dr.Table.Columns.Contains("tasklasterrortime"))
			{
				o.tasklasterrortime = dr["tasklasterrortime"].ToDateTime();
			}
			//
			if(dr.Table.Columns.Contains("taskerrorcount"))
			{
				o.taskerrorcount = dr["taskerrorcount"].Toint();
			}
			//
			if(dr.Table.Columns.Contains("taskruncount"))
			{
				o.taskruncount = dr["taskruncount"].Toint();
			}
			//
			if(dr.Table.Columns.Contains("taskcreateuserid"))
			{
				o.taskcreateuserid = dr["taskcreateuserid"].Toint();
			}
			//
			if(dr.Table.Columns.Contains("taskstate"))
			{
				o.taskstate = dr["taskstate"].ToByte();
			}
			//
			if(dr.Table.Columns.Contains("taskport"))
			{
				o.taskport = dr["taskport"].Toint();
			}
			//
			if(dr.Table.Columns.Contains("taskhealthcheckurl"))
			{
				o.taskhealthcheckurl = dr["taskhealthcheckurl"].Tostring();
			}
			//
			if(dr.Table.Columns.Contains("taskpath"))
			{
				o.taskpath = dr["taskpath"].Tostring();
			}
			//
			if(dr.Table.Columns.Contains("taskarguments"))
			{
				o.taskarguments = dr["taskarguments"].Tostring();
			}
            //
            if (dr.Table.Columns.Contains("taskstartfilename"))
            {
                o.taskstartfilename = dr["taskstartfilename"].ToString();
            }
			//
			if(dr.Table.Columns.Contains("taskremark"))
			{
				o.taskremark = dr["taskremark"].Tostring();
			}
			return o;
        }
    }
}