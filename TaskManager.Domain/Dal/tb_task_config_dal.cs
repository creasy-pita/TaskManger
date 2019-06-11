using BSF.Db;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TaskManager.Domain.Model;

namespace TaskManager.Domain.Dal
{
    public partial class tb_task_config_dal
    {
        public virtual List<tb_task_config_model> GetList(DbConn PubConn, int taskId)
        {
            List<ProcedureParameter> Par = new List<ProcedureParameter>();
            Par.Add(new ProcedureParameter("@taskid", taskId));
            StringBuilder stringSql = new StringBuilder();
            stringSql.Append(@"select s.* from tb_task_config s where s.taskid=@taskid");
            DataSet ds = new DataSet();
            PubConn.SqlToDataSet(ds, stringSql.ToString(), Par);
            List<tb_task_config_model> rs = new List<tb_task_config_model>();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    rs.Add(CreateModel(dr));
                }
            }
            return rs;
        }
    }
}
