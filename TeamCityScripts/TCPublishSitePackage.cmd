%~d0
CD "%~dp0"

SET DestStorage=DefaultEndpointsProtocol=https;AccountName=%StorageAccountName%;AccountKey=%StorageAccountKey%
SET DestUri=https://%StorageAccountName%.blob.core.windows.net/
%BuildVCSRootFolder%\Tools\Ext\CloudCopy.exe "D:\TBTemp\%DeploymentType%.zip" "%DestUri%tb-instancesites" "%DestStorage%"

exit 0
