cd../API
csc -r:Liiga.exe -r:System.Net.Http.dll -r:System.Net.Http.Formatting.dll -r:System.Web.Http.dll -r:System.Web.Http.Cors.dll -r:System.Web.Http.SelfHost.dll -r:Newtonsoft.Json.dll -out:API.exe -recurse:*.cs
cd../Client
npm install --save express@4.15.2
cd../scripts
