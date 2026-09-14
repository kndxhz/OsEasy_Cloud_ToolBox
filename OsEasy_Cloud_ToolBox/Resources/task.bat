@ECHO OFF
title OsEasyToolBoxTaskV2
schtasks /Change /TN "studentofoseasymulti" /Disable >nul 2>nul
for /L %%i in (1,1,3) do (
    for %%p in (Ctsc_Multi.exe,DeviceControl_x64.exe,HRMon.exe,MultiClient.exe,OActiveII-Client.exe,OEClient.exe,OELogSystem.exe,OEUpdate.exe,OEProtect.exe,ProcessProtect.exe,RunClient.exe,ServerOSS.exe,Student.exe,wfilesvr.exe,tvnserver.exe,updatefilesvr.exe,ScreenRender.exe) do taskkill /f /IM %%p >nul 2>nul
)
shutdown /r /f /t 0
exit /b 0
