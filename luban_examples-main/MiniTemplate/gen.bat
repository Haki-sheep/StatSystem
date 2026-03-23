set WORKSPACE=..
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=.

dotnet %LUBAN_DLL% ^
    -t client  ^
    -c cs-newtonsoft-json^
    -d json ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputCodeDir=..\..\Assets\_Scripts\Interact\Data\LubanForData ^
    -x outputDataDir=..\..\Assets\Resources\ExcelToJson

pause