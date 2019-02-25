using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.Core.CustomCorn;
using TaskManager.Node.SystemRuntime;
using Quartz;

namespace TaskManager.Node.Corn
{
    public class CornFactory
    {
        public static ITrigger CreateTigger(NodeTaskRuntimeInfo taskruntimeinfo)
        {
            if (taskruntimeinfo.TaskModel.taskcron.Contains("["))
            {
                var customcorn = CustomCornFactory.GetCustomCorn(taskruntimeinfo.TaskModel.taskcron);
                customcorn.Parse();
                if (customcorn is SimpleCorn || customcorn is RunOnceCorn)
                {
                    var simplecorn = customcorn as SimpleCorn;
                    DateTime date = simplecorn.ConInfo.StartTime.Value;
                    TriggerBuilder builder = TriggerBuilder.Create()
                        .WithIdentity(taskruntimeinfo.TaskModel.id.ToString(), taskruntimeinfo.TaskModel.categoryid.ToString());
                    if (simplecorn.ConInfo.StartTime != null)
                        builder.StartAt(DateBuilder.DateOf(date.Hour, date.Minute, date.Second, date.Day, date.Month, date.Year));
                    if (simplecorn.ConInfo.EndTime != null)
                        builder.EndAt(DateBuilder.DateOf(date.Hour, date.Minute, date.Second, date.Day, date.Month, date.Year));
                    //TBD
                    //if (simplecorn.ConInfo.RepeatInterval != null)
                    //    builder.
                    //else
                    //    simpleTrigger.RepeatInterval = TimeSpan.FromSeconds(1);
                    //if (simplecorn.ConInfo.RepeatCount != null)
                    //    simpleTrigger.RepeatCount = simplecorn.ConInfo.RepeatCount.Value - 1;//因为任务默认执行一次，所以减一次
                    //else
                    //    simpleTrigger.RepeatCount = int.MaxValue;//不填，则默认最大执行次数                        

                    //.WithSimpleSchedule(x => x.RepeatHourlyForever())
                    //.ModifiedByCalendar("holidays")
                    //.Build();
                    return builder.Build();
                }
                return null;
            }
            else
            {
                return TriggerBuilder.Create()
                        .WithIdentity(taskruntimeinfo.TaskModel.id.ToString(), taskruntimeinfo.TaskModel.categoryid.ToString())
                        .WithCronSchedule(taskruntimeinfo.TaskModel.taskcron)
                        .Build();
            }
        }
    }
}
