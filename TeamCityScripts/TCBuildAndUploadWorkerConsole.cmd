msbuild TheBallWorker.sln /t:Rebuild /p:Configuration=Release
Tools\Ext\7z.exe a -y -r build\%~1.zip .\Apps\TheBallWorkerConsole\bin\Release\*
