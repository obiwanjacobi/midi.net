CALL "C:\Program Files (x86)\Microsoft Visual Studio 10.0\VC\vcvarsall.bat" x86

REM Copy in CannedBytes library assemlbies
xcopy /E /Y ..\..\..\..\CannedBytes\Source\Build\_Packages\Code\%1\*.* ..\..\_SharedAssemblies\CannedBytes\

MSBUILD /m /t:Clean /p:Configuration=%1 ..\..\Code\CannedBytes.MIDI.NET.sln
MSBUILD /m /p:Configuration=%1 ..\..\Code\CannedBytes.MIDI.NET.sln
