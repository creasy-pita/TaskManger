
�Ż�
	* ��ʾ����

	������ƽṹ�Ż�
		2019��4��16��
		*ITaskProvider   ʹ������ע�� ��ʽ�����Լ���ÿ���ж�����ڵ�Ĳ���ϵͳ����
		*ProcessHelper  ���Ը�дΪ�ӿڷ�ʽ ����������ע�룬��ITaskProvider ����ͬ����
		*TimeJob �ڵĶ�ʱ���� ���� ���Բ�ʹ�� Quartz�Ķ�ʱ����ʽ�����Լ�ʹ���߳��ඨʱ�����
		* BSF.BaseService.TaskManager.Dal.Dal �� model ���Ƿ���� ��  TaskManager.Domain���ظ� 
	�����Ż�

	��������
		2019��4��16��
		* �ڵ����ܷ����б��е�ͳ������  ��ͳ������������ڴ�ʹ�ã����Ѿ�ֹͣ���е�����Ҳ��ȥͳ��֮ǰ���ڴ�ʹ����
	����
		2019��4��17��
		*���� ��������������ʱ ����״̬�ϵĸ��»���쳣

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


2019��4��12��
	TBD lastTimeDic �������ǰ���pid�ķ�ʽά���ģ��������Ž��̹رն��������� ����ʹ�õ����⣬��Ҫ


2019��3��26��
1 ����ƽ̨  ���� ��TaskManage.node �������ԣ� TaskMnager.web ����������
2 ����ƽ̨  ���� TaskManage.node ����ͳһ�ļ���  ���� �������dll
3 ����ƽ̨  ʹ�� ���������ͬ�Ĳ�������  ����   StartTaskCommand  StopTaskCommand
��������
 ��������ʱ��֮ǰ����־�ᱻ���������   

 
2019��3��25��
1 ����ƽ̨   ���� ��������Ƿ��Ѿ����ڲ�����  pid
	ps aux | grep name | grep -v grep | awk '{print $2}'
	
	
2 ����ƽ̨ TaskManager.Node ʹ�� Process �����������ʱ    ��������
	No Such file or  directory
	ԭ�� �������ֿ�����ʽ�� �Գ���ĵ�ǰ����Ŀ¼��Ӱ�죬���� Process ������ processinfo ��WorkingDirectory ����
		��1��dotnet TaskManager.Node/TaskManager.Node.dll 
			
		��2��dotnet TaskManager.Node.dll 
			���ԣ�1��ʱ TaskManager.Node.dll ����ʱ�����Ŀ¼��Ϣ
				AppDomain.CurrentDomain.BaseDirectory:/root/Software/netcore/TaskManager.Node/
				Directory.GetCurrentDirectory():/root/Software/netcore
				this.GetType().Assembly.Location:/root/Software/netcore/TaskManager.Node/TaskManager.Node.dll
				System.AppContext.BaseDirectory:/root/Software/netcore/TaskManager.Node/
			���ԣ�2��ʱ TaskManager.Node.dll ����ʱ�����Ŀ¼��Ϣ
				AppDomain.CurrentDomain.BaseDirectory:/root/Software/netcore/TaskManager.Node/
				Directory.GetCurrentDirectory():/root/Software/netcore/TaskManager.Node
				this.GetType().Assembly.Location:/root/Software/netcore/TaskManager.Node/TaskManager.Node.dll
				System.AppContext.BaseDirectory:/root/Software/netcore/TaskManager.Node/




2019��2��28��
run microsoft/mssql-server  container
	docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=Str0ngPassword!' -p 1401:1433 -v=/docker/mssql/data:/var/opt/mssql -d --name=tomssl_sql microsoft/mssql-server-linux:latest


1 
dotnet workingdirectory �� arguments 

���� ִ��  a/test.dll
���
	filename = "dotnet"
	workingdirectory="a/"
	arguments ="a/test.dll" 
	�����ʾ  No executable found matching command "dotnet-ProccessStartConsoleWithShell.dll"
ԭ�� workingdirectory  ����ָ�� dotnet ִ�е�Ŀ¼


���Բ鿴Դ����������� 
	https://github.com/dotnet/corefx/blob/master/src/System.Diagnostics.Process/src/System/Diagnostics/Process.cs




	���ܴ������� 
		kill  û����ȫɾ�� ���̣� ��Ҫ��һ������
���Ż�
	���ֽڵ���  linux ��window ����
	����������� �� �ر�  ��  �������Ż��� ������ͨ�� �̵߳ȴ��̶�ʱ����������һ���İ�

����
	���� taskmanager.node �ڵ�