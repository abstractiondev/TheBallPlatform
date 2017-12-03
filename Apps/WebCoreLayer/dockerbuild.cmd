dotnet publish -f netcoreapp2.0 -o pub
docker build -t theballweb .
docker rm tbwebapp
rem docker run -d -p 8080:443 --name tbwebapp theballweb
docker run -p 8080:443 --name tbwebapp theballweb