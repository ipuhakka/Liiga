DESCRIPTION
------------------

This project includes an SQLite-database containing results from Finnish hockey league matches from year 2000 to present. Project has also a web api
to expose the data, and a react webclient to use it. 

DEPENDENCIES
---------------

Client application runs on Node.js so it needs to be installed on the machine. https://nodejs.org/en/download/

API dll's are included in API folder, but machine needs to have csc.exe setup to compile the project.

BUILD
----------
Build files work only for windows.

If node and c# compiler have been installed on the machine, 
both client and the api can be built by executing **build.bat** in scripts folder. 
This installs needed dependencies for client and compiles the api. Client 
dependency installation can take several minutes.

USE
-----------
Once api and client are ready, api and client can be started by running **start_production.bat** or **start_development.bat**. 
These files need to be run as admininstrator.

**start_production.bat** uses serve to serve the optimized version of the react-client which runs in port 5000, 
while **start_development.bat** starts the react start script. This opens the right page in a browser and tells 
the user the correct address to use.  

TESTING
-----------
Database tests can be run by opening project in Visual Studio and building Liiga project, which build the dependencies to right folder. 
After that tests can be run with Test Explorer.