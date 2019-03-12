using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Node
{
    public class ScheduleJob
    {
        /// <summary>
        /// 按照时间执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="seconds"></param>
        public static void ExecuteInterval<T>(int seconds, string IdentityName) where T : IJob
        {
            try
            {
                ISchedulerFactory factory = new StdSchedulerFactory();
                IScheduler scheduler = factory.GetScheduler().Result;
                IJobDetail job = JobBuilder.Create<T>()
                    .WithIdentity(IdentityName)
                    .Build();
                ITrigger trigger = TriggerBuilder.Create()
                    .StartNow()
                    .WithSimpleSchedule(x => x.WithIntervalInSeconds(seconds).RepeatForever())
                    .WithIdentity(IdentityName)
                    .Build();
                scheduler.ScheduleJob(job, trigger);
                scheduler.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
