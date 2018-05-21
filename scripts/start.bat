@setlocal enableextensions
@cd /d "%~dp0"
cd../API
start API.exe
cd../Client
start node server.js
cd../scripts