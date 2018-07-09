@setlocal enableextensions
@cd /d "%~dp0"
cd../API
csc -r:Liiga.exe -r:System.Net.Http.dll -r:System.Net.Http.Formatting.dll -r:System.Web.Http.dll -r:System.Web.Http.Cors.dll -r:System.Web.Http.SelfHost.dll -r:Newtonsoft.Json.dll -out:API.exe -recurse:*.cs
cd../
