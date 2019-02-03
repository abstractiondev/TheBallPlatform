@echo off
@pushd %~dp0

mkdir bin\Debug\ 2> nul
@echo Creating bin\Debug\abscompiler.exe
al /out:bin\Debug\Resources.dll /embed:OperationABS\bin\Debug\OperationABS.dll /embed:DocumentationABS\bin\Debug\DocumentationABS.dll /embed:OperationToDocumentationTRANS\bin\Debug\OperationToDocumentationTRANS.dll /embed:OperationToStatusTrackingTRANS\bin\debug\OperationToStatusTrackingTRANS.dll /embed:StatusTrackingABS\bin\Debug\StatusABS.dll /embed:StatusTrackingToDocumentationTRANS\bin\Debug\StatusTrackingToDocumentationTRANS.dll /embed:TheBallCoreABS\bin\Debug\TheBallCoreABS.dll /embed:TheBallCoreToOperationTRANS\bin\Debug\TheBallCoreToOperationTRANS.dll /embed:absbuilder\AbstractionBuilder\bin\Debug\Mono.TextTemplating.dll
copy absbuilder\AbstractionBuilder\bin\Debug\AbstractionBuilder.exe bin\Debug\ > nul
copy absbuilder\AbstractionBuilder\bin\Debug\Mono.TextTemplating.dll bin\Debug\ > nul
copy AbstractionContent\absbuilder\In\Content_v1_0\AbstractionBuilderContent_v1_0.xml bin\Debug\ > nul
ilmerge.2.14.1208\tools\ILMerge.exe /out:bin\Debug\abscompiler.exe bin\Debug\AbstractionBuilder.exe bin\Debug\Resources.dll bin\Debug\Mono.TextTemplating.dll
del bin\Debug\AbstractionBuilder.exe > nul
del bin\Debug\*.dll > nul

mkdir bin\Release\ 2> nul
@echo Creating bin\Release\abscompiler.exe
al /out:bin\Release\Resources.dll /embed:OperationABS\bin\Release\OperationABS.dll /embed:DocumentationABS\bin\Release\DocumentationABS.dll /embed:OperationToDocumentationTRANS\bin\Release\OperationToDocumentationTRANS.dll /embed:OperationToStatusTrackingTRANS\bin\Release\OperationToStatusTrackingTRANS.dll /embed:StatusTrackingABS\bin\Release\StatusABS.dll /embed:StatusTrackingToDocumentationTRANS\bin\Release\StatusTrackingToDocumentationTRANS.dll /embed:TheBallCoreABS\bin\Release\TheBallCoreABS.dll /embed:TheBallCoreToOperationTRANS\bin\Release\TheBallCoreToOperationTRANS.dll /embed:absbuilder\AbstractionBuilder\bin\Release\Mono.TextTemplating.dll
copy absbuilder\AbstractionBuilder\bin\Release\AbstractionBuilder.exe bin\Release\ > nul
copy absbuilder\AbstractionBuilder\bin\Release\Mono.TextTemplating.dll bin\Release\ > nul
copy AbstractionContent\absbuilder\In\Content_v1_0\AbstractionBuilderContent_v1_0.xml bin\Release\ > nul
ilmerge.2.14.1208\tools\ILMerge.exe /out:bin\Release\abscompiler.exe bin\Release\AbstractionBuilder.exe bin\Release\Resources.dll bin\Release\Mono.TextTemplating.dll
del bin\Release\AbstractionBuilder.exe > nul
del bin\Release\*.dll > nul

@popd