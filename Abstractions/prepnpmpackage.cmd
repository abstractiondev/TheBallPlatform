@echo off
@pushd %~dp0

copy TheBallCoreABS\TheBallCore\Content_v1_0\TheBallCore_v1_0.xsd npmpub\ > nul
copy AbstractionContent\TheBallCore\In\Content_v1_0\TheBallInterface.xml npmpub\ > nul
copy bin\Release\AbstractionBuilderContent_v1_0.xml npmpub\ > nul
copy bin\Release\abscompiler.exe npmpub\ > nul

@popd