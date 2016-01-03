REM   *** Mount X: as infra utility drive for role ***
net use x: \\%CoreFileShareAccountName%.file.core.windows.net\tbcore /u:%CoreFileShareAccountName% %CoreFileShareAccountKey%  >> "%~dp0\NetOutput.txt" 2>&1
dir x: >> "%~dp0\NetOutput.txt"
robocopy /MIR x:\Infra e:\TheBallInfra

IF %ERRORLEVEL% EQU 183 DO VERIFY > NUL

REM   If the ERRORLEVEL is not zero at this point, some other error occurred.
IF %ERRORLEVEL% NEQ 0 (
   ECHO Error mounting tbcore to x:. >> "%TEMP%\StartupLog.txt" 2>&1
   GOTO ErrorExit
)

REM   *** Exit batch file. ***
EXIT /b 0


:ErrorExit
REM   Report the date, time, and ERRORLEVEL of the error.
DATE /T >> "%TEMP%\StartupLog.txt" 2>&1
TIME /T >> "%TEMP%\StartupLog.txt" 2>&1
ECHO An error occurred during startup. ERRORLEVEL = %ERRORLEVEL% >> "%TEMP%\StartupLog.txt" 2>&1
EXIT %ERRORLEVEL%
