CALL CreatePackagesDir.cmd

REM CALL ..\..\..\CannedBytes\Source\Build\BuildAndPackageRTW.cmd

cd Code
CALL BuildAll.cmd
CALL PackageCopyAll.cmd
cd ..

pause