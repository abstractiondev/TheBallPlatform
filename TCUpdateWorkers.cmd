%~d0
CD "%~dp0"

SET DestStorage=DefaultEndpointsProtocol=https;AccountName=%TBSTORAGEACCOUNTNAME%;AccountKey=%TBSTORAGEACCOUNTKEY%
SET DestUri=https://%TBSTORAGEACCOUNTNAME%.blob.core.windows.net/
Tools\Ext\CloudCopy.exe "%~dp0bin\CaloomWorkerRole\*.dll" "%DestUri%worker-role-accelerator" "%DestStorage%"
Tools\Ext\CloudCopy.exe "%~dp0bin\CaloomWorkerRole\CaloomWorkerRole.dll" "%DestUri%worker-role-accelerator" "%DestStorage%"

