@echo off
rem 功能 ： 获取servicename 为 GtCxService_LSSJ 的进程id
rem 运行方式  cmd>a.bat
rem 解读 ： 使用FOR 对 in 中的 命令得到的集合结果进行遍历 并取出每个遍历结果分割后的第三项（默认分割符是空格或者tab  default delimiter set of space and tab）并输出
setlocal enabledelayedexpansion
FOR /F "tokens=3" %%a in ('sc queryex GtCxService_LSSJ ^| FIND "PID"') DO (    
    set v=%%a 
    echo !v!
)
endlocal