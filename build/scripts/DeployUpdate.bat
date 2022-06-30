(
    C:\Windows\system32\inetsrv\appcmd.exe stop apppool /apppool.name:"{apppool}"

    robocopy {source} {destination} /s
    rmdir {source} /s /q

    C:\Windows\System32\inetsrv\appcmd.exe start apppool /apppool.name:"{apppool}"

) ^& IF %ERRORLEVEL% LEQ 1 exit 0