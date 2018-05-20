DESCRIPTION
------------------

This project includes an SQLite-database containing results from Finnish hockey league matches from year 2000 to present. Project has also a web api
to expose the data, and an HTML/JavaScript client to use it. 

DEPENDENCIES
---------------

Client application runs on Node.js so it needs to be installed on the machine. https://nodejs.org/en/download/

Client server uses express v14.5.2. This can be installed by going to Client folder and typing in command
```
npm install --save express@4.15.2
```

USE
-----------
API cannot be used without admin-privileges so it needs to be started by going to folder API->bin->Debug and running API.exe as admininstrator.

Client can be started from scripts folder by clicking client_start.bat-file(on windows).

Alternatively, client can be started by going to Client folder in command line and typing in command:
```
node server.js
```
	
Client runs in localhost:3001, so after running the command this can be opened on a browser. 