:: OpenDDS configure script: configure 
@echo off
set PATH=%PATH%;%CD%\lib
@echo on
DefenderInitialSubscriber.exe -DCPSConfigFile rtps.ini
