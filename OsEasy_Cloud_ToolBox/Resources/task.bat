@ECHO OFF
:awa
echo 正在禁用任务 studentofoseasymulti...
schtasks /Change /TN "studentofoseasymulti" /Disable
for %%p in (Ctsc_Multi.exe,DeviceControl_x64.exe,HRMon.exe,MultiClient.exe,OActiveII-Client.exe,OEClient.exe,OELogSystem.exe,OEUpdate.exe,OEProtect.exe,ProcessProtect.exe,RunClient.exe,RunClient.exe,ServerOSS.exe,Student.exe,wfilesvr.exe,tvnserver.exe,updatefilesvr.exe,ScreenRender.exe) do taskkill /f /IM %%p
echo 正在禁用任务 studentofoseasymulti...
schtasks /Change /TN "studentofoseasymulti" /Disable
shutdown /r /f /t 0
goto awa