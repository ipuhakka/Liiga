@setlocal enableextensions
@cd /d "%~dp0"
cd../API/build
start API.exe
cd../../react-client/liiga-result-data
start serve -s build
cd../scripts