: start

tasklist /FI "IMAGENAME eq git.exe" 2>NUL | find /I /N "git.exe">NUL
if "%ERRORLEVEL%" NEQ "0" goto unlock

taskkill /f /im git.exe

goto start

: unlock

del .git\index.lock
