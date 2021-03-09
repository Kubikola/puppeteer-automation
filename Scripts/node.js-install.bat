@echo off

if exist "%programfiles(x86)%" (
	:: 64-bit
	set "prodcode={179B1CCA-65E3-4A97-8208-590C0160E347}"
	set "nodejsurl=https://nodejs.org/download/release/v15.5.0/node-v15.5.0-x64.msi"
) else (
	:: 32-bit
	set "prodcode={7136155B-06A7-43C8-BB15-C01141CBBB8C}"
	set "nodejsurl=https://nodejs.org/download/release/v15.5.0/node-v15.5.0-x86.msi"
)

:: replace / with space
for %%a in (%nodejsurl:/= %) do set "nodejsfile=%%a"

:: check if installed
reg query "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall" | findstr /I /C:"%prodcode%" >nul

if %ERRORLEVEL% equ 0 (
	:: already installed, exiting
	echo %nodejsfile% with GUID: %prodcode% is installed.
	exit /b
)

set "created=0"
:: download suitable node.js installer
if not exist "%~dp0%nodejsfile%" (
	set "created=1"
	bitsadmin /transfer "Downloading %nodejsfile%" "%nodejsurl%" "%~dp0%nodejsfile%"
)

:: installation file already exists
if %created% equ 0 (
	echo Found existing %nodejsfile%, will be used for installation.
)
echo Installing... Please wait...
:: install it on the background
%nodejsfile% /quiet /qn /norestart

if %ERRORLEVEL%% equ 0 (
	echo Installation done.
) else (
	echo Installation failed.
	if %created% equ 0 (
		:: installation failed with an pre-existing installer
		echo Incorrect existing %nodejsfile%. Remove it and run %0 again.
	)
)

:: remove downloaded installer
if %created% equ 1 (
	del /Q /F "%~dp0%nodejsfile%"
	echo Downloaded installer file "%nodejsfile%" removed.
)