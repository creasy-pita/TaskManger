
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