rmdir /s /q pub
dotnet publish -f netcoreapp2.0 -o pub
docker rmi --force theballwrk-az
docker build -f Dockerfile_azure -t theballwrk-az . 
rmdir /s /q pub


