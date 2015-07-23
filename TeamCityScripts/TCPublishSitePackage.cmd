%~d0
CD "%~dp0"

SET DestStorage=DefaultEndpointsProtocol=https;AccountName=%StorageAccountName%;AccountKey=%StorageAccountKey%
SET DestUri=https://%StorageAccountName%.blob.core.windows.net/
%BuildVCSRootFolder%\Tools\Ext\CloudCopy.exe "D:\TBTemp\%HostName%.zip" "%DestUri%tb-sites" "%DestStorage%"

exit 0
