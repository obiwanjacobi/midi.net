REM Copy assembly files from their projects directories into the package folder structure

xcopy /E /Y ..\..\Code\CannedBytes.Midi\bin\%1\CannedBytes.Midi.* ..\_Packages\Code\%1\
xcopy /E /Y ..\..\Code\CannedBytes.Midi.Components\bin\%1\CannedBytes.Midi.Components.* ..\_Packages\Code\%1\
xcopy /E /Y ..\..\Code\CannedBytes.Midi.Message\bin\%1\CannedBytes.Midi.Message.* ..\_Packages\Code\%1\
xcopy /E /Y ..\..\Code\CannedBytes.Midi.IO\bin\%1\CannedBytes.Midi.IO.* ..\_Packages\Code\%1\
xcopy /E /Y ..\..\Code\CannedBytes.Midi.Xml\bin\%1\CannedBytes.Midi.Xml.* ..\_Packages\Code\%1\

xcopy /E /Y ..\..\_SharedAssemblies\CannedBytes\*.* ..\_Packages\Code\%1\

REM Cleanup some garbage that came with it

Del ..\_Packages\Code\%1\*.old
Del ..\_Packages\Code\%1\*.CodeAnalysisLog.xml
Del ..\_Packages\Code\%1\*.lastcodeanalysissucceeded
