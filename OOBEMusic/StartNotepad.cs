using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

public class Impersonation
{
    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
        int dwLogonType, int dwLogonProvider, out IntPtr phToken);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public extern static bool CloseHandle(IntPtr handle);

    public static void ImpersonateUser(string userName, string domain, string password)
    {
        IntPtr userToken = IntPtr.Zero;
        bool success = LogonUser(userName, domain, password, 2, 0, out userToken);
        if (!success)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        using (WindowsImpersonationContext impersonationContext = new WindowsIdentity(userToken).Impersonate())
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("notepad.exe");
            processStartInfo.UseShellExecute = false;
            Process.Start(processStartInfo);
            impersonationContext.Undo();
        }

        CloseHandle(userToken);
    }
}
