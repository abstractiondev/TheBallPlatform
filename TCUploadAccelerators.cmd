%~d0
CD "%~dp0"

SET DestStorage=DefaultEndpointsProtocol=https;AccountName=%StorageAccountName%;AccountKey=%StorageAccountKey%
SET DestUri=https://%StorageAccountName%.blob.core.windows.net/
Tools\Ext\CloudCopy.exe "%~dp0Apps\AzureConfiguration\bin\Release\app.publish\AzureConfiguration.cspkg" "%DestUri%aaa-theball-infra-package" "%DestStorage%"
Tools\Ext\CloudCopy.exe "C:\SecureParts\%AzureConfigurationName%.cscfg" "%DestUri%aaa-theball-infra-package" "%DestStorage%"

exit 0
