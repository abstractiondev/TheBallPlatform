%~d0
CD "%~dp0"

SET DestStorage=DefaultEndpointsProtocol=https;AccountName=%StorageAccountName%;AccountKey=%StorageAccountKey%
SET DestUri=https://%StorageAccountName%.blob.core.windows.net/
Tools\Ext\CloudCopy.exe "%~dp0bin\CaloomWorkerRole\*.dll" "%DestUri%worker-role-accelerator" "%DestStorage%"
Tools\Ext\CloudCopy.exe "%~dp0bin\CaloomWorkerRole\CaloomWorkerRole.dll" "%DestUri%worker-role-accelerator" "%DestStorage%"

exit 0
