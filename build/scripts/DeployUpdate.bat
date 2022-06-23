(
    robocopy [source] [destination]] /s
    rmdir [source] /s /q

    C:\Windows\System32\inetsrv\appcmd.exe recycle apppool [apppool]

) ^& IF %ERRORLEVEL% LEQ 1 exit 0