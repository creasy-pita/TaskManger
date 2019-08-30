using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BSF.Extensions;
using BSF.Db;
using TaskManager.Domain.Model;

using TaskManager.Core;

namespace TaskManager.Domain.Dal
{
	
	public partial class tb_webtask_dal
    {
        public List<int> GetTaskIDsByState(DbConn PubConn, int taskstate,int nodeid)
        {
            return SqlHelper.Visit(ps =>
            {
                ps.Add("@taskstate", taskstate);
                ps.Add("@nodeid", nodeid);
                StringBuilder stringSql = new StringBuilder();
                stringSql.Append(@"select id from tb_webtask s where s.taskstate=@taskstate and s.nodeid=@nodeid");
                DataSet ds = new DataSet();
                PubConn.SqlToDataSet(ds, stringSql.ToString(), ps.ToParameters());
                List<int> rs = new List<int>();
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        rs.Add(Convert.ToInt32(dr[0]));
                    }
                }
                return rs;
            });

        }

        public List<tb_webtask_model> GetLongRunningTaskIDs(DbConn PubConn, int maxrunningtimeseconds)
        {
            return SqlHelper.Visit(ps =>
            {
                ps.Add("@maxrunningtime", DateTime.Parse("1900-01-01").AddSeconds((int)maxrunningtimeseconds));
                ps.Add("@taskstate", (int)EnumTaskState.Running);
                StringBuilder stringSql = new StringBuilder();
                stringSql.Append(@"select * from tb_webtask s where (s.tasklastendtime-s.tasklaststarttime)>@maxrunningtime and taskstate=@taskstate");
                DataSet ds = new DataSet();
                PubConn.SqlToDataSet(ds, stringSql.ToString(), ps.ToParameters());
                List<tb_webtask_model> rs = new List<tb_webtask_model>();
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        rs.Add(CreateModel(dr));
                    }
                }
                return rs;
            });

        }

        public List<tb_webtasklist_model> GetList(DbConn PubConn, string taskid, string keyword, string CStime, string CEtime, int categoryid, int nodeid ,int userid, int state , int pagesize, int pageindex, out int count)
        {
            int _count = 0;
            List<tb_webtasklist_model> model = new List<tb_webtasklist_model>();
            DataSet dsList = SqlHelper.Visit<DataSet>(ps =>
            {
                string sqlwhere = "";
                StringBuilder sql = new StringBuilder();
                //sql.Append("select ROW_NUMBER() over(order by T.id desc) as rownum,T.*,C.categoryname,N.nodename,U.username from tb_webtask T ");
                sql.Append("select T.*,C.categoryname,N.nodename,U.username from tb_webtask T ");
                sql.Append("left join tb_category C on C.id=T.categoryid ");
                sql.Append("left join tb_user U on U.id=T.taskcreateuserid ");
                sql.Append("left join tb_node N on N.id=T.nodeid where 1=1 ");
                if (!string.IsNullOrWhiteSpace(taskid))
                {
                    ps.Add("taskid", taskid);
                    sqlwhere += " and ( T.id =@taskid )";
                }
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    ps.Add("keyword", keyword);
                    sqlwhere += " and ( T.taskname like '%'+@keyword+'%' or T.taskremark like '%'+@keyword+'%' )";
                }
                if (categoryid != -1)
                {
                    ps.Add("categoryid", categoryid);
                    sqlwhere += " and T.categoryid=@categoryid";
                }
                if (nodeid != -1)
                {
                    ps.Add("nodeid", nodeid);
                    sqlwhere += " and T.nodeid=@nodeid";
                }
                if (state != -999)
                {
                    ps.Add("taskstate", state);
                    sqlwhere += " and T.taskstate=@taskstate";
                }
                if (userid != -1)
                {
                    ps.Add("taskcreateuserid", userid);
                    sqlwhere += " and T.taskcreateuserid=@taskcreateuserid";
                }
                DateTime d=DateTime.Now;
                if (DateTime.TryParse(CStime, out d))
                {
                    ps.Add("CStime", Convert.ToDateTime(CStime));
                    sqlwhere += " and T.taskcreatetime>=@CStime";
                }
                if (DateTime.TryParse(CEtime, out d))
                {
                    ps.Add("CEtime", Convert.ToDateTime(CEtime));
                    sqlwhere += " and T.taskcreatetime<=@CEtime";
                }
                _count = Convert.ToInt32(PubConn.ExecuteScalar("select count(1) from tb_webtask T where 1=1 " + sqlwhere, ps.ToParameters()));
                DataSet ds = new DataSet();
                //string sqlSel = "select * from (" + sql + sqlwhere + ") A where rownum between " + ((pageindex - 1) * pagesize + 1) + " and " + pagesize * pageindex;
                string sqlSel = sql + sqlwhere + " order by T.id desc limit " + ((pageindex - 1) * pagesize ) + "," + pagesize;
                PubConn.SqlToDataSet(ds, sqlSel, ps.ToParameters());
                return ds;
            });
            foreach (DataRow dr in dsList.Tables[0].Rows)
            {
                tb_webtasklist_model m = CreateModelList(dr);
                model.Add(m);
            }
            count = _count;
            return model;
        }

        public List<tb_webtask_model> GetListAll(DbConn PubConn)
        {
            List<tb_webtask_model> model = new List<tb_webtask_model>();
            DataSet dsList = SqlHelper.Visit<DataSet>(ps =>
            {
                string sql = "select id,taskname from tb_webtask";
                DataSet ds = new DataSet();
                PubConn.SqlToDataSet(ds, sql, ps.ToParameters());
                return ds;
            });
            foreach (DataRow dr in dsList.Tables[0].Rows)
            {
                tb_webtask_model m = CreateModelList(dr);
                model.Add(m);
            }
            return model;
        }

        public virtual tb_webtasklist_model CreateModelList(DataRow dr)
        {
            var o = new tb_webtasklist_model();

            //
            if (dr.Table.Columns.Contains("id"))
            {
                o.id = dr["id"].Toint();
            }
            //
            if (dr.Table.Columns.Contains("taskname"))
            {
                o.taskname = dr["taskname"].Tostring();
            }
            //
            if (dr.Table.Columns.Contains("categoryid"))
            {
                o.categoryid = dr["categoryid"].Toint();
            }
            //
            if (dr.Table.Columns.Contains("nodeid"))
            {
                o.nodeid = dr["nodeid"].Toint();
            }
            //
            if (dr.Table.Columns.Contains("taskcreatetime"))
            {
                o.taskcreatetime = dr["taskcreatetime"].ToDateTime();
            }
            //
            if (dr.Table.Columns.Contains("taskupdatetime"))
            {
                o.taskupdatetime = dr["taskupdatetime"].ToDateTime();
            }
            //
            if (dr.Table.Columns.Contains("tasklaststarttime"))
            {
                o.tasklaststarttime = dr["tasklaststarttime"].ToDateTime();
            }
            //
            if (dr.Table.Columns.Contains("tasklastendtime"))
            {
                o.tasklastendtime = dr["tasklastendtime"].ToDateTime();
            }
            //
            if (dr.Table.Columns.Contains("tasklasterrortime"))
            {
                o.tasklasterrortime = dr["tasklasterrortime"].ToDateTime();
            }
            //
            if (dr.Table.Columns.Contains("taskerrorcount"))
            {
                o.taskerrorcount = dr["taskerrorcount"].Toint();
            }
            //
            if (dr.Table.Columns.Contains("taskruncount"))
            {
                o.taskruncount = dr["taskruncount"].Toint();
            }
            //
            if (dr.Table.Columns.Contains("taskcreateuserid"))
            {
                o.taskcreateuserid = dr["taskcreateuserid"].Toint();
            }
            //
            if (dr.Table.Columns.Contains("taskstate"))
            {
                o.taskstate = dr["taskstate"].ToByte();
            }
            //
            if (dr.Table.Columns.Contains("taskport"))
            {
                o.taskport = dr["taskport"].Toint();
            }
            //
            if (dr.Table.Columns.Contains("taskhealthcheckurl"))
            {
                o.taskhealthcheckurl = dr["taskhealthcheckurl"].Tostring();
            }
            //
            if (dr.Table.Columns.Contains("taskpath"))
            {
                o.taskpath = dr["taskpath"].Tostring();
            }
            //
            if (dr.Table.Columns.Contains("taskstartfilename"))
            {
                o.taskstartfilename = dr["taskstartfilename"].Tostring();
            }
            //
            if (dr.Table.Columns.Contains("taskarguments"))
            {
                o.taskarguments = dr["taskarguments"].Tostring();
            }
            //
            if (dr.Table.Columns.Contains("taskstopfilename"))
            {
                o.taskstopfilename = dr["taskstopfilename"].Tostring();
            }
            //
            if (dr.Table.Columns.Contains("taskstoparguments"))
            {
                o.taskstoparguments = dr["taskstoparguments"].Tostring();
            }
            //
            if (dr.Table.Columns.Contains("taskremark"))
            {
                o.taskremark = dr["taskremark"].Tostring();
            }
            //
            if (dr.Table.Columns.Contains("nodename"))
            {
                o.nodename = dr["nodename"].Tostring();
            }
            //
            if (dr.Table.Columns.Contains("categoryname"))
            {
                o.categoryname = dr["categoryname"].Tostring();
            }
            //
            if (dr.Table.Columns.Contains("username"))
            {
                o.username = dr["username"].Tostring();
            }
            return o;
        }

        public tb_webtask_model GetOneTask(DbConn PubConn, int taskid)
        {
            return SqlHelper.Visit(ps =>
            {
                ps.Add("id", taskid);
                string sql = "select * from tb_webtask where id=@id";
                DataSet ds = new DataSet();
                PubConn.SqlToDataSet(ds, sql, ps.ToParameters());
                tb_webtask_model model = CreateModel(ds.Tables[0].Rows[0]);
                return model;
            });
        }

        public List<tb_webtask_model> GetTaskByNodeID(DbConn PubConn,int nodeID)
        {
            return SqlHelper.Visit(ps =>
            {
                ps.Add("nodeid", nodeID);
                string sql = "select * from tb_webtask where nodeid=@nodeid";
                DataSet ds = new DataSet();
                PubConn.SqlToDataSet(ds, sql, ps.ToParameters());
                List<tb_webtask_model> model = new List<tb_webtask_model>();
                foreach(DataRow dr in ds.Tables[0].Rows)
                {
                    model.Add(CreateModel(dr));
                }
                return model;
            });
        }

        public int AddTask(DbConn PubConn, tb_webtask_model model)
        {
            return SqlHelper.Visit<int>(ps =>
            {
                ps.Add("@taskname", model.taskname);
                ps.Add("@categoryid", model.categoryid);
                ps.Add("@nodeid", model.nodeid);
                ps.Add("@taskcreatetime", model.taskcreatetime);
                ps.Add("@taskupdatetime", model.taskcreatetime);
                ps.Add("@taskerrorcount", 0);
                ps.Add("@taskruncount", 0);
                ps.Add("@taskcreateuserid", model.taskcreateuserid);
                ps.Add("@taskstate",0);
                ps.Add("@taskport", model.taskport);
                ps.Add("@taskhealthcheckurl", model.taskhealthcheckurl.NullToEmpty());
                ps.Add("@taskpath", model.taskpath);
                ps.Add("@taskarguments", model.taskarguments);
                ps.Add("@taskremark", model.taskremark);
                ps.Add("@taskstartfilename", model.taskstartfilename);
                ps.Add("@taskstopfilename", model.taskstopfilename);
                ps.Add("@taskstoparguments", model.taskstoparguments);
                //   int rev = Convert.ToInt32(PubConn.ExecuteScalar(@"insert into tb_webtask(taskname,categoryid,nodeid,taskcreatetime,taskruncount,taskcreateuserid,taskstate,taskport,taskhealthcheckurl,taskpath,taskarguments,taskremark,taskstartfilename)
                //values(@taskname,@categoryid,@nodeid,@taskcreatetime,@taskruncount,@taskcreateuserid,@taskstate,@taskport,@taskhealthcheckurl,@taskpath,@taskarguments,@taskremark,@taskstartfilename) select @@IDENTITY", ps.ToParameters()));
                PubConn.ExecuteScalar(@"insert into tb_webtask(taskname,categoryid,nodeid,taskcreatetime,taskupdatetime,taskruncount,taskcreateuserid,taskstate,taskport,taskhealthcheckurl,taskpath,taskarguments,taskremark,taskstartfilename,taskstopfilename,taskstoparguments)
										   values(@taskname,@categoryid,@nodeid,@taskcreatetime,@taskupdatetime,@taskruncount,@taskcreateuserid,@taskstate,@taskport,@taskhealthcheckurl,@taskpath,@taskarguments,@taskremark,@taskstartfilename,@taskstopfilename,@taskstoparguments)", ps.ToParameters());
                //TBD ´Ë´¦
                DataTable dt = PubConn.SqlToDataTable("SELECT LAST_INSERT_ID() AS id_value", null);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(dt.Rows[0][0]);
                }
                return -1;
            });
        }

        public int UpdateTask(DbConn PubConn, tb_webtask_model model)
        {
            return SqlHelper.Visit<int>(ps =>
            {
                ps.Add("@id", model.id);
                ps.Add("@taskname", model.taskname);
                ps.Add("@categoryid", model.categoryid);
                ps.Add("@nodeid", model.nodeid);
                ps.Add("@taskupdatetime", model.taskupdatetime);
                ps.Add("@tasklaststarttime", model.tasklaststarttime);
                ps.Add("@tasklastendtime", model.tasklastendtime);
                ps.Add("@taskcreateuserid", model.taskcreateuserid);
                ps.Add("@taskhealthcheckurl", model.taskhealthcheckurl.NullToEmpty());
                ps.Add("@taskpath", model.taskpath);
                ps.Add("@taskstartfilename", model.taskstartfilename);
                ps.Add("@taskarguments", model.taskarguments);
                ps.Add("@taskremark", model.taskremark);
                ps.Add("@taskstopfilename", model.taskstopfilename);
                ps.Add("@taskstoparguments", model.taskstoparguments);
                ps.Add("@taskport", model.taskport);
                ps.Add("@taskstate", model.taskstate);
                string sql = "Update tb_webtask Set taskname=@taskname,categoryid=@categoryid,nodeid=@nodeid,taskupdatetime=@taskupdatetime,tasklaststarttime=@tasklaststarttime,";
                sql += "tasklastendtime=@tasklastendtime,taskhealthcheckurl=@taskhealthcheckurl,taskpath=@taskpath,taskcreateuserid=@taskcreateuserid,";
                sql += "taskarguments=@taskarguments,taskremark=@taskremark,taskstartfilename=@taskstartfilename,taskstopfilename=@taskstopfilename,taskstoparguments=@taskstoparguments,taskport=@taskport,taskstate=@taskstate";
                sql += " where id=@id";
                int i = PubConn.ExecuteSql(sql, ps.ToParameters());
                return i;
            });
        }

        public int CheckTaskState(DbConn PubConn, int id)
        {
            return SqlHelper.Visit(ps =>
            {
                ps.Add("id", id);
                string sql = "select taskstate from tb_webtask where id=@id";
                int i = Convert.ToInt32(PubConn.ExecuteScalar(sql, ps.ToParameters()));
                return i;
            });
        }

        public int ChangeTaskState(DbConn PubConn, int id, int state)
        {
            return SqlHelper.Visit(ps =>
            {
                ps.Add("taskstate", state);
                ps.Add("id", id);
                string sql = "update tb_webtask set taskstate=@taskstate where id=@id";
                return PubConn.ExecuteSql(sql, ps.ToParameters());
            });
        }

        public int UpdateTaskState(DbConn PubConn, int taskid, int taskstate)
        {
            return SqlHelper.Visit(ps =>
            {
                ps.Add("@taskstate", taskstate);
                ps.Add("@id", taskid);
                StringBuilder stringSql = new StringBuilder();
                stringSql.Append(@"update tb_webtask set taskstate=@taskstate where id=@id");
                return PubConn.ExecuteSql(stringSql.ToString(), ps.ToParameters());
            });
        }

        public bool DeleteOneTask(DbConn PubConn, int id)
        {
            return SqlHelper.Visit<bool>(ps =>
            {
                ps.Add("id", id);
                string sql = "delete from tb_webtask where taskstate=0 and id=@id";
                int i = PubConn.ExecuteSql(sql, ps.ToParameters());
                return i > 0;
            });
        }
        
    }
}