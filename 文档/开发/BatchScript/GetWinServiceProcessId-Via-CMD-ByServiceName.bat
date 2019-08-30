@echo off
rem 功能 ： 获取 bat执行时指定的 servicename 参数 的进程id
rem 参数 1：servicename 
rem 输出 对应的进程id
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