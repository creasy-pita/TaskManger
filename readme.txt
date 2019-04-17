
优化
	* 表示待做

	代码设计结构优化
		2019年4月16日
		*ITaskProvider   使用依赖注入 方式，可以减少每次判断任务节点的操作系统类型
		*ProcessHelper  可以改写为接口方式 并采用依赖注入，与ITaskProvider 做相同处理
		*TimeJob 内的定时任务 代码 可以不使用 Quartz的定时任务方式，可以简单使用线程类定时来完成
		* BSF.BaseService.TaskManager.Dal.Dal 与 model 类是否多余 与  TaskManager.Domain中重复 
	功能优化

	功能问题
		2019年4月16日
		* 节点性能分析列表中的统计问题  会统计所有任务的内存使用，但已经停止运行的任务也会去统计之前的内存使用量
	测试
		2019年4月17日
		*测试 启动大的任务服务时 任务状态上的更新会否异常

select p.*, n.nodename,'' as taskname 
	from ( 
			select nodeid,0 as taskid,sum(cpu) as cpu,sum(memory) as memory,sum(installdirsize) as installdirsize,max(lastupdatetime) as lastupdatetime 
				from tb_performance 
				where 1=1  and lastupdatetime>@lastupdatetime  group by nodeid 
		) p,
		tb_node n 
	where p.nodeid=n.id order by  p.nodeid desc

select p.*, n.nodename,'' as taskname 
	from ( 
			select p.nodeid,0 as taskid,sum(p.cpu) as cpu,sum(p.memory) as memory,sum(p.installdirsize) as installdirsize,max(p.lastupdatetime) as lastupdatetime 
				from tb_performance p
				,tb_task t
				where 1=1 and t.id = p.taskid and lastupdatetime>@lastupdatetime  group by nodeid 
		) p,
		tb_node n 
	where p.nodeid=n.id order by  p.nodeid desc
----------------------------------------------------------------		


2019年4月12日
	TBD lastTimeDic 的数据是按照pid的方式维护的，可能随着进程关闭而过期引起 混乱使用的问题，需要


2019年3月26日
1 调度平台  增加 对TaskManage.node 心跳测试， TaskMnager.web 的心跳测试
2 调度平台  增加 TaskManage.node 增加统一文件夹  放置 任务程序dll
3 调度平台  使用 命令工厂处理不同的操作命令  比如   StartTaskCommand  StopTaskCommand
？？？？
 重新启动时，之前的日志会被清除的问题   

 
2019年3月25日
1 调度平台   查找 程序进程是否已经存在并返回  pid
	ps aux | grep name | grep -v grep | awk '{print $2}'
	
	
2 调度平台 TaskManager.Node 使用 Process 开启任务进程时    报错问题
	No Such file or  directory
	原因 以下两种开启方式， 对程序的当前工作目录有影响，导致 Process 类启动 processinfo 的WorkingDirectory 不对
		（1）dotnet TaskManager.Node/TaskManager.Node.dll 
			
		（2）dotnet TaskManager.Node.dll 
			调试（1）时 TaskManager.Node.dll 运行时输出的目录信息
				AppDomain.CurrentDomain.BaseDirectory:/root/Software/netcore/TaskManager.Node/
				Directory.GetCurrentDirectory():/root/Software/netcore
				this.GetType().Assembly.Location:/root/Software/netcore/TaskManager.Node/TaskManager.Node.dll
				System.AppContext.BaseDirectory:/root/Software/netcore/TaskManager.Node/
			调试（2）时 TaskManager.Node.dll 运行时输出的目录信息
				AppDomain.CurrentDomain.BaseDirectory:/root/Software/netcore/TaskManager.Node/
				Directory.GetCurrentDirectory():/root/Software/netcore/TaskManager.Node
				this.GetType().Assembly.Location:/root/Software/netcore/TaskManager.Node/TaskManager.Node.dll
				System.AppContext.BaseDirectory:/root/Software/netcore/TaskManager.Node/




2019年2月28日
run microsoft/mssql-server  container
	docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=Str0ngPassword!' -p 1401:1433 -v=/docker/mssql/data:/var/opt/mssql -d --name=tomssl_sql microsoft/mssql-server-linux:latest


1 
dotnet workingdirectory 和 arguments 

加入 执行  a/test.dll
如果
	filename = "dotnet"
	workingdirectory="a/"
	arguments ="a/test.dll" 
	则会提示  No executable found matching command "dotnet-ProccessStartConsoleWithShell.dll"
原因 workingdirectory  可以指定 dotnet 执行的目录


可以查看源码来具体分析 
	https://github.com/dotnet/corefx/blob/master/src/System.Diagnostics.Process/src/System/Diagnostics/Process.cs




	可能存在问题 
		kill  没有完全删除 进程， 需要进一步处理
待优化
	区分节点是  linux ，window 类型
	启动任务进程 和 关闭  是  操作的优化， 现在是通过 线程等待固定时间来进入下一步的啊

功能
	监视 taskmanager.node 节点