REM Use Powershell to run IIS configs
PowerShell -Version 4.0 -Command "Set-Executionpolicy Unrestricted"
PowerShell -Version 4.0 .\InitializeIIS.ps1
PowerShell -Version 4.0 .\InstallCerts.ps1