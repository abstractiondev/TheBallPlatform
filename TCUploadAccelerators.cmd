%~d0
CD "%~dp0"

SET DestStorage=DefaultEndpointsProtocol=https;AccountName=%TBStorageAccountName%;AccountKey=%TBStorageAccountKey%
SET DestUri=https://%TBStorageAccountName%.blob.core.windows.net/
Tools\Ext\CloudCopy.exe "%~dp0Apps\AzureConfiguration\bin\Release\app.publish\AzureConfiguration.cspkg" "%DestUri%theball-infra-package" "%DestStorage%"
Tools\Ext\CloudCopy.exe "C:\SecureParts\ServiceConfiguration.%TBAzureConfigName%.cscfg" "%DestUri%theball-infra-package" "%DestStorage%"

exit 0
