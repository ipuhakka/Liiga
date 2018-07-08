cd../API
csc -r:Liiga.exe -r:System.Net.Http.dll -r:System.Net.Http.Formatting.dll -r:System.Web.Http.dll -r:System.Web.Http.Cors.dll -r:System.Web.Http.SelfHost.dll -r:Newtonsoft.Json.dll -out:API.exe -recurse:*.cs

cd../react-client/liiga-result-data
call npm install --save react
call npm install --save react-dom
call npm install --save react-scripts
call npm install --save react-bootstrap
call npm install --save serve -g
call npm run build
cd../../scripts
