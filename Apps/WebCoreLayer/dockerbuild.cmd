rmdir /s /q pub
dotnet publish -f netcoreapp2.0 -o pub
kubectl delete deployment tbweb
docker rmi --force theballweb
robocopy /MIR x:\Configs_DevTest Configs_DevTest_tmp
robocopy /MIR x:\TheBallCerts TheBallCerts_tmp
docker build -f Dockerfile_linux -t theballweb . 
rmdir /s /q Configs_DevTest_tmp
rmdir /s /q TheBallCerts_tmp

