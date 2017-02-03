import-module ServerManager
<# Add-WindowsFeature Web-CertProvider #>
<# pkgmgr.exe /iu:IIS-ApplicationInit #>
dism.exe /enable-feature /online /featureName:IIS-ApplicationInit
mkdir D:\TheBallCerts
robocopy.exe /MIR X:\TheBallCerts D:\TheBallCerts
<#
Enable-WebCentralCertProvider -CertStoreLocation D:\TheBallCerts -UserName $env:IISCertAccountName -Password $env:IISCertAccountPassword -PrivateKeyPassword T11r1kka
#>