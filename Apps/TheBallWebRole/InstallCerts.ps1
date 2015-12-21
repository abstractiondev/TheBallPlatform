Get-ChildItem "D:\TheBallCerts" -Filter *.pfx | `
Foreach-Object{
    $certFileName = $_.FullName
    Write-Host $certFileName
    $myPwd = ConvertTo-SecureString -String "T11r1kka" -Force -AsPlainText
    Import-PfxCertificate -FilePath $certFileName -Password $myPwd -CertStoreLocation Cert:\LocalMachine\My
}
