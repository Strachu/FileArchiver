FOR /F "tokens=*" %%G IN ('dir /B /AD /S bin') DO rmdir /S /Q "%%G"
FOR /F "tokens=*" %%G IN ('dir /B /AD /S obj') DO rmdir /S /Q "%%G"

FOR /F "tokens=*" %%G IN ('dir /B /AD /S bin') DO rmdir /S /Q "%%G"
FOR /F "tokens=*" %%G IN ('dir /B /AD /S obj') DO rmdir /S /Q "%%G"
