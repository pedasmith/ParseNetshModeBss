using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;


namespace ParseNetshModeBss
{
    public interface AddToText
    {
        void DoAddToText(string text);
    }
    public static class RunCommandLine
    {
        /// <summary>
        /// Specifies how the command has to be run. RunProgram is good for command-line apps like NETSH. OpenUrl is needed for
        /// e.g., ms-contact-support://?ActivationType=NetworkDiagnostics&invoker=NetshG. ShellExecute is used for e.g., perfmon
        /// </summary>
        public enum CmdType { RunProgram, OpenUrl, ShellExecute }

#if NEVER_EVER_DEFINED
        // This is seemingly never used? 2025-05-23
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
#endif

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
        public static async Task<string> RunNetshGAsync(string program, string args, AddToText? tb = null, CmdType cmdType = CmdType.RunProgram)
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
                CreateNoWindow = true,

            };

            string url = args; // only used for OpenUrl types of commands.

            switch (cmdType)
            {
                case CmdType.OpenUrl:
                    // Note: launching with OpenUrl is in progress.
                    retval = "NOTE: Launching URLs doesn't redirect";
                    start = new ProcessStartInfo(url)
                    {
                        UseShellExecute = true,
                    };
                    break;
                case CmdType.ShellExecute:
                    start = new ProcessStartInfo()
                    {
                        UseShellExecute = true,
                        FileName = program,
                        Arguments = args,
                        RedirectStandardOutput = false,
                        RedirectStandardError = false,
                        WindowStyle = ProcessWindowStyle.Normal,
                        CreateNoWindow = false,
                        ErrorDialog = true, // 2025-05-23: Not sure if this makes a difference
                    };
                    break;
            }

            string? line;
            int mod = 1;
            int nline = 0;
            try
            {
                using (Process? proc = Process.Start(start))
                {
                    if (proc != null)
                    {
                        switch (cmdType)
                        {
                            case CmdType.OpenUrl:
                                retval = $"Launched {url}";
                                break;
                            case CmdType.ShellExecute:
                                retval = $"Launched {program} {args}";
                                break;
                            default:
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
                                break;
                        }

                    }
                    else
                    {
                        switch (cmdType)
                        {
                            case CmdType.OpenUrl:
                                retval = $"Unable to launch {url}";
                                break;
                            case CmdType.ShellExecute:
                                retval = $"Unable to launch {program} {args}";
                                break;
                            default:
                                retval = "";
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Fake the results
                line = "Error: Exception: " + ex.Message;
                retval += line + "\n";
            }

            return retval;
        }

    }
}
