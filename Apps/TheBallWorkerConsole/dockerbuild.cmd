rmdir /s /q pub
dotnet publish -f netcoreapp2.0 -o pub
kubectl delete deployment tbwrk
docker rmi --force theballwrk
robocopy /MIR x:\Configs_DevTest Configs_DevTest_tmp
docker build -f Dockerfile_linux -t theballwrk . 
rmdir /s /q Configs_DevTest_tmp


