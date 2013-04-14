CALL CreatePackagesDir.cmd

cd Code
CALL BuildAll.cmd
CALL PackageCopyAll.cmd
cd ..

pause