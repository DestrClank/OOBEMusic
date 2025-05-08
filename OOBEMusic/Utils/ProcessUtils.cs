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
        public Dictionary<string, (string, int)> processes = new Dictionary<string, (string, int)>
        {
            { "WWAHost", ("ActivateWWAHostMusic", RegHelper.CheckActivationState("ActivateWWAHostMusic")) },
            { "OobeShellHost", ("ActivateOOBEHostMusic", RegHelper.CheckActivationState("ActivateOOBEHostMusic")) },
            { "FirstLogonAnim", ("ActivateFirstLogonMusic" , RegHelper.CheckActivationState("ActivateFirstLogonMusic")) },
            { "windeploy", ("ActivateWinDeployMusic" , RegHelper.CheckActivationState("ActivateWinDeployMusic")) }
        };

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

        public (List<string> processeslist, bool exists) CheckIfProcessExists()
        {
            // Correction 1: Use KeyValuePair<string, int> in the foreach loop

            List<string> processeslists = new List<string>();

            // Correction 2: Iterate over KeyValuePair<string, int>
            foreach (KeyValuePair<string, (string, int)> processName in processes)
            {
                // Correction 3: Use processName.Key to get the process name
                Process[] process = Process.GetProcessesByName(processName.Key);

                bool interrupted = CheckProcesses(process);

                if (!interrupted)
                {
                    if (process.Length > 0 && processName.Value.Item2 == 1)
                    {
                        processeslists.Add(process[0].ProcessName);
                        return (processeslists, true);
                    }
                }
            }

            return (processeslists, false);
        }

        public bool CheckIfAnyOptionIsEnabled()
        {
            foreach (KeyValuePair<string, (string, int)> processName in processes)
            {
                if (processName.Value.Item2 == 1)
                {
                    return true; // Si au moins une option est activée, retourne true
                }
            }
            return false; // Si aucune option n'est activée, retourne false
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
