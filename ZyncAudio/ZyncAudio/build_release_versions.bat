@echo off

echo Building Windows 64-Bit release..
echo.
dotnet publish -c Release
echo Done.
echo.

echo.
echo.
echo Finished all builds.

timeout /t -1