using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ParseNetshModeBss
{
    internal static class RunCommandLine
    {
        public static string RunNetsh(string program = "netsh", string args = "wlan show networks mode=bssid")
        {
            string retval = "";
            var start = new ProcessStartInfo()
            {
                FileName = program,
                Arguments = args,
                RedirectStandardOutput = true,
                WindowStyle = ProcessWindowStyle.Hidden,
            };
            using (Process? proc = Process.Start(start))
            {
                retval = proc?.StandardOutput.ReadToEnd() ?? "";
                proc!.WaitForExit();
            }
            return retval;
        }

        public static string RunNetshG(string program = "netsh", string args = "wlan show networks mode=bssid")
        {
            string retval = "";
            var start = new ProcessStartInfo()
            {
                FileName = program,
                Arguments = args,
                RedirectStandardOutput = true,
                WindowStyle = ProcessWindowStyle.Hidden,
            };

            // Changes for G
            //start.UseShellExecute = true;
            //start.WindowStyle = ProcessWindowStyle.Minimized;
            //start.RedirectStandardOutput = false;
            start.CreateNoWindow = true;

            using (Process? proc = Process.Start(start))
            {
                retval = proc?.StandardOutput.ReadToEnd() ?? "";
                proc!.WaitForExit();
            }
            return retval;
        }

    }
}
