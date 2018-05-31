DESCRIPTION
------------------

This project includes an SQLite-database containing results from Finnish hockey league matches from year 2000 to present. Project has also a web api
to expose the data, and an HTML/JavaScript client to use it. 

DEPENDENCIES
---------------

Client application runs on Node.js so it needs to be installed on the machine. https://nodejs.org/en/download/

API dll's are included in API folder, but machine needs to have csc.exe setup to compile the project.

BUILD
----------
If node and c# compiler have been installed on the machine, 
both client and the api can be built by executing build.bat in scripts folder. 
This installs needed dependencies for client and compiles the api.

USE
-----------
Once api and client are ready, api and client can be started by running start.bat as admin (as the api requires admin privileges). 

Alternatively, client can be started by going to Client folder in command line and typing in command:
```
node server.js
```
Client runs in localhost:3001, so after running the command this can be opened on a browser. 

Api can also be started from API folder, by running API.exe as admin. 

TESTING
-----------
Database tests can be run by opening project in Visual Studio and building Liiga project, which build the dependencies to right folder. 
After that tests can be run with Test Explorer.