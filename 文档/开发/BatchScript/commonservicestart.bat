@echo off
rem setlocal enabledelayedexpansion
set v=
FOR /F "tokens=3" %%a in ('sc queryex GtCxService_LSSJ ^| FIND "PID"') DO (    
	set v=%%a 
	rem echo %v%
)
echo %1
echo %2
if "%v%" == "" (
	echo "this service begin installed "
	%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe %cd%\GisqLandSystem.GtCxService_ZWFWW.exe
	echo "this service end installed "
rem	goto startservice
)

rem :startservice
echo "begin  start"
Net Start GtCxService_LSSJ
rem set _STARTSERVICE=Net Start %1
rem %_STARTSERVICE%
sc config GtCxService_LSSJ start= auto
echo "end  start"
rem endlocal