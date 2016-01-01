%~d0
CD "%~dp0"

SET DestStorage=DefaultEndpointsProtocol=https;AccountName=%StorageAccountName%;AccountKey=%StorageAccountKey%
SET DestUri=https://%StorageAccountName%.blob.core.windows.net/
%BuildVCSRootFolder%\Tools\Ext\CloudCopy.exe "D:\TBTemp\BindingData.txt" "%DestUri%tb-config" "%DestStorage%"

exit 0
