REM installs all client dependencies for ReactJS -client application and build the client and api. Installing dependencies can take several minutes.
@setlocal enableextensions
@cd /d "%~dp0"
cd ../react-client/liiga-result-data
call npm install --save react
call npm install --save react-dom
call npm install --save react-scripts
call npm install --save react-bootstrap
call npm install --save serve -g
call npm run build
cd../../scripts

cd../API/build
csc -r:Liiga.exe -r:System.Net.Http.dll -r:System.Net.Http.Formatting.dll -r:System.Web.Http.dll -r:System.Web.Http.Cors.dll -r:System.Web.Http.SelfHost.dll -r:Newtonsoft.Json.dll -out:API.exe -recurse:..\src\*.cs
cd../../scripts