using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseNetshModeBss
{

    internal class ParseBssidMode
    {

        public ParseBssidMode()
        {

        }
        static Dictionary<string, Authentication> Authentications = new Dictionary<string, Authentication>()
        {
            { "Open", Authentication.Open },
            { "OWE", Authentication.Owe },
            { "WPA-Personal", Authentication.WPA_Personal },
            { "WPA2-Personal", Authentication.WPA2_Personal },
            { "WPA2-Enterprise", Authentication.WPA2_Enterprise },
            { "WPA3-Personal", Authentication.WPA3_Personal },
            { "WPA3-Enterprise", Authentication.WPA3_Enterprise },
            { "WPA3-Enterprise 192 Bits", Authentication.WPA3_Enterprise_192 },
        };

        static Dictionary<string, Encryption> Encryptions= new Dictionary<string, Encryption>()
        {
            { "CCMP", Encryption.CCMP },
            { "GCMP-256", Encryption.GCMP_256 },
            { "WEP", Encryption.WEP },
            { "None", Encryption.None },
        };

        static Dictionary<string, NetworkType> NetworkTypes = new Dictionary<string, NetworkType>()
        {
            { "Infrastructure", NetworkType.Infrastructure },
        };

        static Dictionary<string, RadioType> RadioTypes = new Dictionary<string, RadioType>()
        {
            { "802.11", RadioType.W802_11n}, // 
            { "802.11b", RadioType.W802_11n}, // 1
            { "802.11a", RadioType.W802_11n}, // 2
            { "802.11g", RadioType.W802_11n}, // 3
            { "802.11n", RadioType.W802_11n}, // 4
            { "802.11ac", RadioType.W802_11ac}, // 5
            { "802.11ax", RadioType.W802_11ax}, // 6 and 6E
            { "802.11be", RadioType.W802_11be}, // 7
        };

        enum ParseState {  Start, Ssid, Bssid};
        ParseState CurrParseState = ParseState.Start;
        SsidInfo CurrSsid = new SsidInfo(); // useless blank version
        BssidInfo CurrBssid = new BssidInfo(); // useless blank version
        public IList<SsidInfo> ParsedData = new List<SsidInfo>();
        public enum ParseStatus {  Ok, OtherError};

        public IList<SsidInfo> Parse(string value)  // is multiple lines
        {
            var lines = value.Split('\n');
            foreach (var rawline in lines)
            {
                ParseLine(rawline);
            }
            return ParsedData;
        }
        public ParseStatus ParseLine(string rawline)
        {
            var line = rawline.TrimEnd();
            var retval = ParseStatus.Ok;

            // Most lines can be split this way
            var split  = line.Split(':', 2);
            var leftraw = split[0];
            var left = leftraw.Trim();
            var right = split.Length >= 2 ? split[1].Trim() : "";
            var leftsplit = left.Trim().Split(' ', 2);
            int index = 0; // is the '2' in SSID 2 : or '1' in BSSID 1 :34:345:34:32
            if (leftsplit.Length == 2)
            {
                Int32.TryParse(leftsplit[1], out index);
            }

            // Now do the parsing
            // Lines starting SSID and BSSID can always be parsed regardless of mode.
            if (left.StartsWith("SSID "))
            {
                CurrSsid = new SsidInfo();
                CurrSsid.Name = right;
                CurrSsid.Index = index;
                ParsedData.Add(CurrSsid);
                CurrParseState = ParseState.Ssid;
            }
            else if (left.StartsWith("BSSID "))
            {
                CurrBssid = new BssidInfo();
                CurrBssid.Index = index;
                CurrBssid.Mac = right;

                CurrSsid.Bssids.Add(CurrBssid);
                CurrParseState = ParseState.Bssid;
            }
            else if (rawline == "\r")
            {
                ; // ignore here so the rest of the code only gets "real" lines
            }
            else
            {
                switch (CurrParseState)
                {
                    case ParseState.Start:
                        break;
                    case ParseState.Ssid:
                        switch (left)
                        {
                            case "Authentication":
                                if (!Authentications.TryGetValue(right, out CurrSsid.Authentication))
                                {
                                    Log($"ERROR: unknown {left} '{right}'");
                                }
                                break;
                            case "Encryption":
                                if (!Encryptions.TryGetValue(right, out CurrSsid.Encryption))
                                {
                                    Log($"ERROR: unknown {left} '{right}'");
                                }
                                break;
                            case "Network type":
                                if (!NetworkTypes.TryGetValue(right, out CurrSsid.NetworkType))
                                {
                                    Log($"ERROR: unknown {left} '{right}'");
                                }
                                break;
                        }


                        break;
                    case ParseState.Bssid:
                        switch (left)
                        {
                            case "Band": // Example: Band : 2.4 GHz
                                if (!Double.TryParse(right.Replace(" GHz", ""), out CurrBssid.BandInGHz))
                                {
                                    Log($"ERROR: unknown BSSID {left} '{right}'");
                                }
                                break;
                            case "Channel":
                                CurrBssid.Channel = right; // just a string
                                break;
                            case "Channel Utilization": // Example: Channel Utilization:        73 (28 %)
                                var rsplit = right.Split(' ');
                                if (!Double.TryParse(rsplit[0], out CurrBssid.BssLoadUtilization)) // the '73'
                                {
                                    Log($"ERROR: unknown BSSID {left} '{right}' ; expected e.g. 73 (28 %)");
                                }
                                if (rsplit.Length >= 2 && rsplit[1][0] == '(')
                                {
                                    var pct = rsplit[1].Substring(1);
                                    if (!Double.TryParse(pct, out CurrBssid.BssLoadUtilizationPercent)) // the '28'
                                    {
                                        Log($"ERROR: unknown BSSID {left} '{right}' ; expected util. e.g. 73 (28 %)");
                                    }
                                }
                                else
                                {
                                    Log($"ERROR: unknown BSSID {left} '{right}' ; expected utilization e.g. 73 (28 %)");
                                }
                                break;

                            case "Connected Stations": // Example:  Connected Stations:         0
                                if (!Int32.TryParse(right, out CurrBssid.BssLoadConnectedStations))
                                {
                                    Log($"ERROR: unknown BSSID {left} '{right}'");
                                }
                                break;

                            case "Radio type":
                                if (!RadioTypes.TryGetValue(right, out CurrBssid.RadioType))
                                {
                                    Log($"ERROR: unknown BSSID {left} '{right}'");
                                }
                                break;

                            case "Signal":
                                if (!Double.TryParse(right.Replace("%", ""), out CurrBssid.SignalStrengthPercent))
                                {
                                    Log($"ERROR: unknown BSSID {left} '{right}'");
                                }
                                break;

                        }
                        if (left.StartsWith("Signal"))
                        {

                        }


                        break;
                }
            }
            return retval;
        }

        public void Log(string text)
        {
            Console.WriteLine(text);
        }


        public static IList<string> Examples = new List<string>()
        {
""" 
> netsh wlan show networks mode=bssid

Interface name : Wi-Fi
There are 3 networks currently visible.

SSID 1 : MyOfficeWifi
    Network type            : Infrastructure
    Authentication          : WPA2-Personal
    Encryption              : CCMP
    BSSID 1                 : 2c:12:8b:49:b3:1e
         Signal             : 91%
         Radio type         : 802.11be
         Band               : 5 GHz
         Channel            : 40
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54
    BSSID 2                 : 2c:12:8b:49:b3:1d
         Signal             : 96%
         Radio type         : 802.11be
         Band               : 2.4 GHz
         Channel            : 10
         Basic rates (Mbps) : 1 2 5.5 11
         Other rates (Mbps) : 6 9 12 18 24 36 48 54

SSID 2 :
    Network type            : Infrastructure
    Authentication          : WPA2-Personal
    Encryption              : CCMP
    BSSID 1                 : 10:12:8b:49:b3:30
         Signal             : 20%
         Radio type         : 802.11ax
         Band               : 2.4 GHz
         Channel            : 7
         Bss Load:
             Connected Stations:         0
             Channel Utilization:        73 (28 %)
             Medium Available Capacity:  0 (0 us/s)
         Basic rates (Mbps) : 1 2 5.5 11
         Other rates (Mbps) : 6 9 12 18 24 36 48 54
    BSSID 2                 : 10:12:8b:49:b3:31
         Signal             : 94%
         Radio type         : 802.11be
         Band               : 2.4 GHz
         Channel            : 10
         Basic rates (Mbps) : 1 2 5.5 11
         Other rates (Mbps) : 6 9 12 18 24 36 48 54

SSID 3 : HouseWiFi
    Network type            : Infrastructure
    Authentication          : WPA2-Personal
    Encryption              : CCMP
    BSSID 1                 : 22:12:8b:49:b3:92
         Signal             : 85%
         Radio type         : 802.11ac
         Band               : 5 GHz
         Channel            : 149
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54
    BSSID 2                 : 22:12:8b:49:b3:93
         Signal             : 53%
         Radio type         : 802.11ac
         Band               : 5 GHz
         Channel            : 149
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54
    BSSID 3                 : 22:12:8b:49:b3:94
         Signal             : 72%
         Radio type         : 802.11n
         Band               : 2.4 GHz
         Channel            : 6
         Basic rates (Mbps) : 1 2 5.5 11
         Other rates (Mbps) : 6 9 12 18 24 36 48 54
    BSSID 4                 : 22:12:8b:49:b3:95
         Signal             : 82%
         Radio type         : 802.11n
         Band               : 2.4 GHz
         Channel            : 6
         Basic rates (Mbps) : 1 2 5.5 11
         Other rates (Mbps) : 6 9 12 18 24 36 48 54
    BSSID 5                 : 22:12:8b:49:b3:97
         Signal             : 88%
         Radio type         : 802.11n
         Band               : 2.4 GHz
         Channel            : 1
         Basic rates (Mbps) : 1 2 5.5 11
         Other rates (Mbps) : 6 9 12 18 24 36 48 54
    BSSID 6                 : 22:12:8b:49:b3:98
         Signal             : 78%
         Radio type         : 802.11ac
         Band               : 5 GHz
         Channel            : 149
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54

"""
        };
    }
}
