import-module ServerManager
Add-WindowsFeature Web-CertProvider
mkdir D:\TheBallCerts
Enable-WebCentralCertProvider -CertStoreLocation D:\TheBallCerts -UserName env:$IISCertAccountName -Password env:$IISCertAccountPassword