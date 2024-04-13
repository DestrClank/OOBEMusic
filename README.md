# OOBE Music Player
## Introduction 
The era of the Windows XP OOBE's music misses you ? Well I have the perfect solution FOR YOU !

This program is a Windows service that plays music in background during the OOBE phase and during the First Logon Animation when your local account is being made.

It works for Windows 10 and Windows 11.

# Installation
To enjoy the maximum benefits of the program, you will have to install Windows for the first time and get into the audit mode of Windows when you get into the OOBE phase. You can use a virtual machine and do all the steps described below.

1. When you install Windows for the first time, wait until you are into the OOBE phase (where you have to configure Windows for the first time) and press `Ctrl + Shift + F3` to reboot into audit mode (this can take a while).
2. At some point, you will get into the Desktop and a pop-up will appear, it's the System Preparation program, for now , close it.
3. Download the ZIP file in Releases and extract it into the root of the `C:\` drive (don't extract into `C:\Windows\system32\oobe` because the program won't start here)
4. Open Powershell with administrative privileges and type this command :
   - Powershell : `New-Service -Name "OOBEMusic" -BinaryPathName "C:\OOBEMusic\OOBEMusic.exe"`
   - Alternatively, an `install.ps1` file installs automatically the program into the Windows Services. To use it, you will have to type this command first : `Set-ExecutionPolicy RemoteSigned`.
5. Use the combination `Windows + R` ane type in the box `services.msc`
6. Scroll down until you see OOBEMusic and verify that the startup type is set to "Automatic". (Be careful : dont change anything in here if you don't know what you are doing ! Your computer may not boot again if you disable a service essential for Windows to run !)
7. Reboot your computer and open Task Manager.
   - Use `Ctrl + Shift + Esc` or type `taskmgr.exe` into the Run box to open it.
8. In the Details panel, you should see a new process called `OOBEMusic.exe` running in the background, if you see this, congratulations ! You installed it successfully !
   - Note : You can use the audit mode to install all the drivers for your computer or install the guests additions (WMware Tools or VirtualBox Guests Additions). You can let Windows Update install the drivers if it finds some. Don't forget, if you don't have any audio coming out of your computer, install the audio drivers !
9. Now you can use `Windows + R` and type `sysprep.exe` to open System Preparation. In the window, select Reboot into OOBE and click OK.
10. Normally, when Windows reboots, the installation phase will play the music ! Congratulations and enjoy !

## Customization
You can change the default music that plays when the OOBE starts. But there is requirements for the music file.
- The music file has to be in .wav format for it to work into the program. You can use any converters like ffmpeg to convert your file into .wav file.

To change the music you have 2 ways :
- Note : you have to do this into the audit mode **BEFORE** completing the OOBE phase !
### Via Registry Editor
1. When you first execute the service, the program makes an entry into the registry at
   - `HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\DestrClank\OOBEMusic\MusicFile`
2. Use the `Windows + R` and type `regedit.exe` into the Run box.
3. Navigate to the path specified and you will see a `MusicFile` setting.
4. Go into Windows Explorer and locate where your .wav file is located.
   - In Windows 11, right-click on the file and select `Copy path location`
   - In Windows 10, while maintaining the Shift key, right-click on the file and select `Copy path location`
5. Go into Task Manager and go into the Services tab.
6. Search for OOBEMusic and click on it. Right-click and select `Restart`. This will restart the program.
7. Do the steps for step 9 in the installation's guide to reboot into OOBE.
   - **WARNING** : If you already passed the OOBE phase, **don't do this** ! Sysprep will **RESET YOUR PC AND DELETE ALL YOUR FILES** !!
### Replace the defaut music file
1. Rename your .wav file to `music.wav`.
2. Into the root of your C: drive, you should find an OOBEMusic folder.
3. Put your music.wav file into OOBEMusic folder, Windows Explorer will tell you to replace the file, say yes.

## How it works
The program analyses if one of these two processes are running to determine when to play the music.
- WWAHost.exe : the program responsible for the OOBE user interface.
- FirstLogonAnim.exe : after you completed the setup phase, your computer will show an animation, this program is responsible for it.

## Side effects
1. When you create a new user, the window that prompts you to put your credentials of your Microsoft account plays the music because WWAHost.exe is used here for this interface.
   - Sometimes, WWAHost can still be running even if the window is closed, use Task Manager and kill WWAHost.exe and the music should stop.

2. When a new user logs on for the first time, they will be greeted by the music playing in the background.
   - The same problem as the first side effect can occur at the end of the preparation phase when you get to the Desktop.
3. Any program that needs WWAHost.exe will play the music.
   - In that case, you can disable the OOBEMusic Windows Service when you have finished installing Windows.
4. Any program named WWAHost.exe or FirstLogonAnim.exe will play the music.


##end
