@setlocal enableextensions
@cd /d "%~dp0"
cd../API/build
start API.exe
cd../../react-client/liiga-result-data
call npm start
cd../scripts