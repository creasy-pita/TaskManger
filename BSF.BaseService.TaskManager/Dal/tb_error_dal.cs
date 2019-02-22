using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BSF.Extensions;
using BSF.Db;
using BSF.BaseService.TaskManager.Model;


namespace BSF.BaseService.TaskManager.Dal
{
	/*代码自动生成工具自动生成,不要在这里写自己的代码，否则会被自动覆盖哦 - 车毅*/
	public partial class tb_error_dal
    {
        public int Add(DbConn PubConn, tb_error_model model)
        {
            return SqlHelper.Visit(ps =>
            {
                ps.Add("@msg", model.msg);
                ps.Add("@errortype", model.errortype);
                ps.Add("@errorcreatetime", model.errorcreatetime);
                ps.Add("@taskid", model.taskid);
                ps.Add("@nodeid", model.nodeid);
                return PubConn.ExecuteSql(@"insert into tb_error(msg,errortype,errorcreatetime,taskid,nodeid)
										   values(@msg,@errortype,@errorcreatetime,@taskid,@nodeid)", ps.ToParameters()) ;
            });
        }
    }
}