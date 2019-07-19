@echo off
setlocal enabledelayedexpansion
FOR /F "tokens=3" %%a in ('REG QUERY "HKLM\Software\Microsoft\Internet Explorer" /v Version ^| FIND "REG_SZ"') DO (    
    set v=%%a 
    echo !v!
    FOR /F "tokens=1 delims=." %%m in ('echo !v!') do (
      SET /A m=%%m
    )
    IF !m! EQU 8 (
      echo it's 8
    ) ELSE (
      IF !m! EQU 9 (
        echo it's 9
      ) ELSE (
       echo norrrrr
      )
    )
)
endlocal