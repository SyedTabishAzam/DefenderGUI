:: OpenDDS configure script: configure 
@echo off
set PATH=%PATH%;%CD%\lib
@echo on
DefenderCommandPublisher.exe -DCPSConfigFile rtps.ini DEFENDER
