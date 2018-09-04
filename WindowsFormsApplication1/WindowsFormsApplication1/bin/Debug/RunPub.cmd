:: OpenDDS configure script: configure 
@echo off
set PATH=%PATH%;%CD%\lib
@echo on
CommandPublisher.exe -DCPSConfigFile rtps.ini DEFENDER
