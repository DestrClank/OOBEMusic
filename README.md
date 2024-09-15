# OOBE Music Player
## Introduction 
The era of the Windows XP OOBE's music misses you ? Well I have the perfect solution FOR YOU !

This program is a Windows service that plays music in background during the OOBE phase and during the First Logon Animation when your local account is being made.

It works for Windows 10 and Windows 11.
**Note** : It has not been tested with Windows 8.1 and older. My theory is :
- On Windows 8.1, it would work because it's the first Windows version to implement the "Web-Based" OOBE that modern Windows uses.
- On Windows 7 and older, it would not work because the OOBE seems to be app-based instead.

# Installation
To enjoy the maximum benefits of the program, you will have to install Windows for the first time and get into the audit mode of Windows when you get into the OOBE phase. You can use a virtual machine and do all the steps described below.

0. Use your installation media (ISO, DVD, or USB key) and install Windows 10 or 11 as you normally would. It will copy the files onto your harddrive and then do a reboot. The Preparation phase will be launched. Wait even more, then you PC will reboot again.
1. At this point, after some other preparation, you will be into the OOBE phase (where you have to configure Windows for the first time, and Cortana or the Windows 11 video introduction starts). Press `Ctrl + Shift + F3` to reboot into audit mode (the reboot can take a while).
2. At some point, you will be logged as "Administrator" and you will get into the Desktop, a little window will appear, it's normal, it's the System Preparation program, for now, close it.
3. Download the ZIP file in "Releases" and extract it into the root of the `C:\` drive (don't extract into `C:\Windows\system32\oobe` because the program won't start here)
4. Open Powershell with administrative privileges and type this command :
   - Powershell : `New-Service -Name "OOBEMusic" -BinaryPathName "C:\OOBEMusic\OOBEMusic.exe"`
   - Alternatively, an `install.ps1` file installs automatically the program into the Windows Services. To use it, you will have to type this command first : `Set-ExecutionPolicy RemoteSigned` or use the `install.bat` file. It will launch the Powershell script automatically.
5. Use the key combination `Windows + R` ane type in the box `services.msc`
6. Scroll down until you see OOBEMusic or "OOBE Music Player" and verify that the startup type is set to "Automatic". (Be careful : dont change anything in here if you don't know what you are doing ! Your computer may not boot again if you disable a service essential for Windows to run !)
7. Now, convert any music file with a file converter (ffmpeg for example) to a `.wav` file and put it inside `C:\OOBEMusic`.
   - Note : You need to rename the file `music.wav` in order for the file to be recognized by the program.
8. Reboot your computer with the Start Menu or `Alt + F4` or with `Ctrl + Alt + Delete` and when it has finished to reboot and you are on the Desktop again, open the Task Manager.
   - Use `Ctrl + Shift + Esc` or type `taskmgr.exe` into the Run box to open it.
11. In the Details panel, you should see a new process called `OOBEMusic.exe` running in the background, if you see this, congratulations ! You installed it successfully !
   - Note : You can use the audit mode to install all the drivers for your computer or install the guests additions (WMware Tools or VirtualBox Guests Additions). You can let Windows Update install the drivers if it finds some. Don't forget, try to play any sound from your computer (Try to play a YouTube video for example), if you don't have any audio coming out of your computer, install the audio drivers !
12. Now you can use `Windows + R` and type `sysprep.exe` to open System Preparation. In the window, select Reboot into OOBE and click OK.
13. Normally, when Windows reboots, the installation phase will play the music ! Congratulations and enjoy !
   - If nothing is playing, no panic ! You can go back to the audit mode by pressing `Ctrl + Shift + F3` and try again !
   - Sometimes, the service starts very late because of Windows, maybe too late at this point, you can alternatively reboot the computer by pressing the power button.
   - Verify that you have a music file encoded in `.wav` format and present into `C:\OOBEMusic` !

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
5. Now with your path into the clipboard, modify the registry key `MusicFile` and replace the default one with the one you want. Remove the quotations marks if there is any.
6. Go into Task Manager and go into the Services tab.
7. Search for OOBEMusic and click on it. Right-click and select `Restart`. This will restart the program.
8. Do the steps for step 9 in the installation's guide to reboot into OOBE.
   - Note : Now, the service will play without restarting the new file you specified into the registry key.
   - **WARNING** : If you already passed the OOBE phase, **don't do this** ! Sysprep will **RESET YOUR PC AND DELETE ALL YOUR FILES** !!
### Replace the defaut music file
1. Rename your .wav file to `music.wav`.
2. Into the root of your C: drive, you should find an OOBEMusic folder.
3. Put your music.wav file into OOBEMusic folder, Windows Explorer will tell you to replace the file or to make a copy, select "Replace".

## How it works
The program analyses if one of these two processes are running to determine when to play the music.
- WWAHost.exe : the program responsible for the OOBE user interface.
- FirstLogonAnim.exe : after you completed the setup phase, your computer will show an animation, this program is responsible for it.

## Side effects
1. When you create a new user, the window that prompts you to put your credentials of your Microsoft account plays the music because WWAHost.exe is used here for this interface.
   - Sometimes, WWAHost can still be running even if the window is closed, use Task Manager and kill WWAHost.exe and the music should stop.
   - Note : now the service checks if the process is in "Suspended" mode. If it is suspended, the music should not play. But if you want to maximize your performance, try the solution above.
2. When a new user logs on for the first time, they will be greeted by the music playing in the background.
   - The same problem as the first side effect can occur at the end of the preparation phase when you get to the Desktop.
   - Note : for some reason, after analysis, WWAHost.exe is not suspended after the first logon of the user. Try the solution described in point 1 of Side effects.
3. Any program that needs WWAHost.exe will play the music.
   - In that case, you can disable the OOBEMusic Windows Service when you have finished installing Windows.
4. Any program named WWAHost.exe or FirstLogonAnim.exe will play the music.

## Optional registry keys
You can use some registry keys implemented into the program alternatively to control when OOBEMusic should play the music.
   - Location : `HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\DestrClank\OOBEMusic\`
### How to put these keys
For these keys, you have to make them into `HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\DestrClank\OOBEMusic\` as `DWORD (32 bits)`.

1. In Registry Editor, navigate to the location showed, and on the right side where you see `MusicFile`, right-click then `New > DWORD 32 bits value`.
2. Name the new key with one of the names below.
3. After that, double click on your new key and set the value to `0` or `1`.
   - `0` means Disabled.
   - `1` means Enabled.
   - Do not set any other value other than these 2 values.

### List of optional keys
- `ActivateWWAHostMusic` : This will enable the playback when WWAHost.exe is detected. It will play on the OOBE screens and on the new user window when you make a new user account on the computer.
- `ActivateFirstLogonMusic` : This will enable the playback when you finish the OOBE phase and the preparation screen with the text passing and the animation background starts. This screen is caused by `FirstLogonAnim.exe`.

### Disable the service with registry
You can use these 2 keys described earlier and set them as `0` to completely disable the service. In this case, the service will automatically close, freeing some RAM in the process.
