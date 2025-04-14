using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOBEMusic.Utils
{
    public class ProcessUtils
    {
        public string ParseProcesses(List<string> processeslist)
        {
            string processLogsStrings = string.Empty;
            if (processeslist.Count > 0)
            {
                foreach (string process in processeslist)
                {
                    processLogsStrings += process + ".exe" + ", ";
                }
                processLogsStrings = processLogsStrings.TrimEnd(',', ' ');
            }
            else
            {
                processLogsStrings = processeslist[0];
            }

            return processLogsStrings;
        }

        public (List<string> processeslist, bool exists) CheckIfProcessExists(int WWAHostSetting, int FirstLogonSetting, int OobeShellSetting)
        {
            string wwahost = "WWAHost";
            string firstbootanim = "FirstLogonAnim";
            string oobehost = "OobeShellHost";

            Process[] wwahostlist = Process.GetProcessesByName(wwahost);
            Process[] firstlogonlist = Process.GetProcessesByName(firstbootanim);
            Process[] oobehostlist = Process.GetProcessesByName(oobehost);

            List<string> processeslists = new List<string>();

            bool exists = false;

            if (wwahostlist.Length > 0 && WWAHostSetting == 1)
            {

                bool interrupted = CheckProcesses(wwahostlist);

                if (!interrupted)
                {
                    processeslists.Add(wwahostlist[0].ProcessName);
                    exists = true;
                }
            }

            if (firstlogonlist.Length > 0 && FirstLogonSetting == 1)
            {

                bool interrupted = CheckProcesses(firstlogonlist);

                if (!interrupted)
                {
                    processeslists.Add(firstlogonlist[0].ProcessName);
                    exists = true;
                }

            }

            if (oobehostlist.Length > 0 && OobeShellSetting == 1)
            {
                bool interrupted = CheckProcesses(oobehostlist);
                if (!interrupted)
                {
                    processeslists.Add(oobehostlist[0].ProcessName);
                    exists = true;
                }
            }

            return (processeslists, exists);
        }

        public static bool CheckProcesses(Process[] processes)
        {
            foreach (Process process in processes)
            {
                bool isSuspended = process.Threads.Cast<ProcessThread>()
                    .Any(thread => thread.ThreadState == System.Diagnostics.ThreadState.Wait && thread.WaitReason == ThreadWaitReason.Suspended);
                if (!isSuspended)
                {
                    return false; // Si un processus n'est pas suspendu, retourne false
                } //
            }
            return true; // Si tous les processus sont suspendus, retourne true
        }
    }
}
