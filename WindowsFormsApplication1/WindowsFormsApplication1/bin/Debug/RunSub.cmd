:: OpenDDS configure script: configure 
@echo off
set PATH=%PATH%;%CD%\lib
@echo on
InitialSubscriber.exe -DCPSConfigFile rtps.ini
