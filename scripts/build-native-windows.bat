@echo off
set "ROOT=%~dp0.."

if not exist "%ROOT%\DeliveryApp\bin\Debug\net8.0" mkdir "%ROOT%\DeliveryApp\bin\Debug\net8.0"
if not exist "%ROOT%\DeliveryTests\bin\Debug\net8.0" mkdir "%ROOT%\DeliveryTests\bin\Debug\net8.0"

gcc -std=c11 -O2 -shared "%ROOT%\native\delivery_native.c" -o "%ROOT%\DeliveryApp\bin\Debug\net8.0\delivery_native.dll"

if errorlevel 1 exit /b 1

copy /Y "%ROOT%\DeliveryApp\bin\Debug\net8.0\delivery_native.dll" "%ROOT%\DeliveryTests\bin\Debug\net8.0\delivery_native.dll"
