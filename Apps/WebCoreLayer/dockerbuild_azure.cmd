rmdir /s /q pub
dotnet publish -f netcoreapp2.0 -o pub
docker rmi --force theballweb-az
docker build -f Dockerfile_azure -t theballweb-az . 
rmdir /s /q pub

