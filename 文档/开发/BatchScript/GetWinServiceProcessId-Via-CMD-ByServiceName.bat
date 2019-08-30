@echo off
rem 功能 ： 获取 bat执行时指定的 servicename 参数 的进程id;
rem 参数 1：servicename ;
rem 输出 如果服务存在且在运行中 输出 对应的进程id 及 后续一个空行;
rem 输出 如果服务存在且不在运行中 输出 0 及 后续一个空行;
rem 输出 如果服务不存在 输出 一个空行    或者    No Instance(s) Available. 及 后续一个空行;
rem 运行方式  cmd>a.bat servicename


rem echo %1
set servicename=%1
rem echo %servicename%
for /f "tokens=1" %%# in ('
  wmic service where "name='%servicename%'" get ProcessId /format:value
') do (
	REM echo %%#
  for /f "tokens=2 delims==" %%$ in ("%%#") do echo %%$
)