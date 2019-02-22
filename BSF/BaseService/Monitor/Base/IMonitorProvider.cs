using BSF.BaseService.Monitor.Base.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSF.BaseService.Monitor.Base
{
    public interface IMonitorProvider
    {
         void AddCommonLog(CommonLogInfo log);


         void AddErrorLog(ErrorLogInfo log);


         void AddTimeWatchLog(TimeWatchLogInfo log);

         void AddTimeWatchApiLog(TimeWatchLogApiInfo log);
        
    }
}
