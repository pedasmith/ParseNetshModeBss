using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

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

        public static async Task<string> RunNetshGAsync(string program = "netsh", string args = "wlan show networks mode=bssid")
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
                if (proc != null)
                {
                    retval = await proc.StandardOutput.ReadToEndAsync();
                    await proc.WaitForExitAsync();
                }
                else
                {
                    retval = "";
                }
            }
            return retval;
        }

    }
}
