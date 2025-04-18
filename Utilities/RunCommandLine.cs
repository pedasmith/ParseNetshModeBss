﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Text;
using System.Threading.Tasks;


namespace ParseNetshModeBss
{
    interface AddToText
    {
        void DoAddToText(string text);
    }
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

        public static async Task<string> RunOpenUrl(string url, AddToText? tb = null)
        {
            //Uri? uri;
            //bool created = Uri.TryCreate(url, UriKind.Absolute, out uri);
            string retval = "NOTE: Launching URLs doesn't redirect";
            var start = new ProcessStartInfo (url) {  
                UseShellExecute = true,
            };


            using (Process? proc = Process.Start(start))
            {
                if (proc != null)
                {
                    retval = $"Launched {url}";
                    await Task.Delay(0); // just so the compiler is OK with an async method that doesn't await.
                    // No need to wait for exit: await proc.WaitForExitAsync();
                }
                else
                {
                    retval = $"Unable to launch {url}";
                }
            }
            return retval;
        }

        /// <summary>
        /// Program is e.g., "Netsh" and args is e.g., "wlan show networks mode-bssid"
        /// </summary>
        /// <returns></returns>
        public static async Task<string> RunNetshGAsync(string program, string args, AddToText? tb = null, bool commandHasOutput=true)
        {
            string retval = "";
            var start = new ProcessStartInfo()
            {
                FileName = program,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                StandardErrorEncoding = Encoding.UTF8,
                StandardOutputEncoding = Encoding.UTF8,
            };

            // Changes for G
            //start.UseShellExecute = true;
            //start.WindowStyle = ProcessWindowStyle.Minimized;
            //start.RedirectStandardOutput = false;
            start.CreateNoWindow = true;

            int mod = 1;
            int nline = 0;
            using (Process? proc = Process.Start(start))
            {
                if (proc != null && commandHasOutput) // both are almost always true. commandHasOutput is false for #nooutput which is USBVIEW only
                {
                    string? line;
                    do
                    {
                        line = await proc.StandardOutput.ReadLineAsync();
                        retval += line + "\n";

                        // Have to batch the lines. Otherwise commands with lots of output like
                        // netsh advfirewall firewall show rule name=all
                        // will cause the UX to stall.
                        if (nline % mod == 0)
                        {
                            if (tb != null) tb.DoAddToText(line + "\n");
                        }
                        switch (nline)
                        {
                            case 50: mod = 2; break;
                            case 100: mod = 10; break;
                            case 500: mod = 100; break;
                        }
                        nline++;
                    }
                    while (line != null);


                    do
                    {
                        line = await proc.StandardError.ReadLineAsync();
                        if (line != null && line != "") // comes out with extra blank lines for no reason?
                        {
                            retval += line + "\n";

                            // Have to batch the lines. Otherwise commands with lots of output like
                            // netsh advfirewall firewall show rule name=all
                            // will cause the UX to stall.
                            if (nline % mod == 0)
                            {
                                if (tb != null) tb.DoAddToText(line + "\n");
                            }
                            switch (nline)
                            {
                                case 50: mod = 2; break;
                                case 100: mod = 10; break;
                                case 500: mod = 100; break;
                            }
                            nline++;
                        }
                    }
                    while (line != null);



                    // retval = await proc.StandardOutput.ReadToEndAsync();
                    // FYI: it seems like there's no standard error to read. It's in the object,
                    // but just looking at it throws an exception.

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
