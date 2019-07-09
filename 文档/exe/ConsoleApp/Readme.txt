1 startprocess.exe :execute this project exe ,you can start a process which you want by setting  the filename, arguments,workdirectory of ProcessStartInfo.
2 findprocess.exe  :find process by filename, arguments

使用startprocess.exe 打开指定的进程
使用 查找指定的进程， 通过 commandline 信息信息匹配

示例：cmd 执行 开启 BackgroundTasksSample.dll  注意 开启时 还传入了 参数0 ： v1
E:\work\myproject\netcore\ConsoleApp\StartProcess\bin\Debug\netcoreapp2.1\win10-x64\StartProcess dotnet "E:\work\myproject\netcore\BackgroundTasksSample-GenericHost\bin\Debug\netcoreapp2.1\publish\BackgroundTasksSample.dll v1" "E:\work\myproject\netcore\BackgroundTasksSample-GenericHost\bin\Debug\netcoreapp2.1\publish"

：执行 cmd  查找 commandline 信息带有 BackgroundTasksSample.dll v1的进程
E:\work\myproject\netcore\ConsoleApp\ConsoleApp\FindProcess\bin\Debug\netcoreapp2.1\win10-x64\FindProcess dotnet "E:\work\myproject\netcore\BackgroundTasksSample-GenericHost\bin\Debug\netcoreapp2.1\publish\BackgroundTasksSample.dll v1"

