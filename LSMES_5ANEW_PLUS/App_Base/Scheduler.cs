using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using Quartz.Impl;
using System.Threading.Tasks;

namespace LSMES_5ANEW_PLUS.App_Base
{
    /// <summary>
    /// Job 类
    /// </summary>
    public class TestJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() =>
            {
                SysLog log = new SysLog($"Job 测试，当前时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
            });
        }
    }
    public class Scheduler
    {
        static bool state = false;
        static IScheduler scheduler;
        static public void Stop()
        {
            state = false;
            scheduler.Shutdown();
        }
        static public void Run()
        {
            if (state) return;
            try
            {
                //1.创建调度单元
                Task<IScheduler> tsk = StdSchedulerFactory.GetDefaultScheduler();
                scheduler = tsk.Result;
                //2.创建一个具体的作业即job(具体的job需要单独在一个文件中执行)
                IJobDetail job = JobBuilder.Create<TestJob>().WithIdentity("完成").Build();
                //3.创建并配置一个触发器trigger,
                ITrigger _CtroTrigger = TriggerBuilder.Create()
                    .WithIdentity("定时确认")
                    .WithCronSchedule("0 * 10 * * ?")
                    .Build()
                    as ITrigger;
                //4.将job和trigger加入到作业调度池中
                scheduler.ScheduleJob(job, _CtroTrigger);
                //5.开启调度
                scheduler.Start();
                state = true;
            }
            catch(Exception ex)
            {
                SysLog log = new SysLog(ex.Message);
                state = false;
            }
        }
        static public bool IsAlive()
        {
            return state;
        }
    }
}