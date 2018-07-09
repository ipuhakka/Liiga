REM builds a new production version of the ReactJS-client app without installing dependencies.
@setlocal enableextensions
@cd /d "%~dp0"
cd ../react-client/liiga-result-data
call npm run build
cd../../scripts