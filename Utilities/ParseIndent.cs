using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    internal class ParseIndent : TableParse, IMacroParse
    {
        List<string>[] savedRows = new List<string>[5]; // 5 is kind of arbitrary...
        public bool AllowSpacesInName = false;

        public override void Parse(string file)
        {
            var lines = file.Replace("\r\n", "\n").Split(new char[] { '\n' });
            var indents = lines.CountIndents();
            var indentstr = string.Join(",", indents);

            int prevIndent = 0;
            var prevLine = "";
            string l0linename = "";
            string l0namevalue = "";
            string l0colname = "";
            string l0index = "";
            List<string> currRow = new List<string>();
            var currRowHasData = false;

            // Saved rows

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var nextLine = (i+1 < lines.Length) ? lines[i + 1] : "";
                var indent = line.IndentLevel(indents);
                var nextIndent = nextLine.IndentLevel(indents);
                switch (indent)
                {
                    case 0:
                        if (indent < prevIndent)
                        {
                            if (currRowHasData)
                            {
                                Rows.Add(currRow);
                                currRowHasData = false;
                                currRow = savedRows[indent] ?? new List<string>();
                            }

                        }
                        if (line.Contains(":")) //  e.g., SSID 1 : MyHouseWiFi
                        {
                            // colname="SSID" index="1" value="MyHouseWiFi"
                            (l0linename, l0namevalue) = line.SplitColon();
                            l0colname = l0linename;
                            if (!AllowSpacesInName) // this is the common case
                            {
                                (l0colname, l0index) = l0linename.SplitSpace();
                            }
                            l0colname = l0colname.Trim();
                            l0namevalue = l0namevalue.Trim();

                            RowUpsert(currRow, l0colname, l0namevalue);
                            currRowHasData = true;
                        }
                        else if (line.StartsWith("Configuration for")) //  e.g., Configuration for interface "Ethernet 2"
                        {
                            // This type of value happens with netsh interface ipv4 show config
                            l0namevalue = line.GetQuotedValue("(not set)");
                            RowUpsert(currRow, "Name", l0namevalue);
                            currRowHasData = true;
                        }
                        if (nextIndent > indent) // save it for later
                        {
                            savedRows[indent] = new List<string>(currRow);
                        }
                        break;
                    case 1:
                    case 2:
                    case 3:
                        // e.g.,     Network type            : Infrastructure
                        // or     BSSID 2                 : 70:3a:cb:19:26:68
                        // or          Bss Load:
                        //    ..          Connected Stations:         0

                        var (name, value) = line.Trim().SplitColon(); // name is e.g., Network type or BBSID 2
                        name = name.Trim(); // see if it's BSSID 2 style -- with a space where the second is integer
                        value = value.Trim();

                        int index = 0;
                        var (nameNoIndex, nameIndex) = name.SplitSpace(); // eg, for BSSID 2 : ___ split the "BSSID 2" into two parts
                        nameNoIndex = nameNoIndex.Trim();
                        var isInt = Int32.TryParse(nameIndex, out index);
                        if (isInt && nextIndent > indent)
                        {
                            // Is the BSSID 2 case. Save the current row
                            savedRows[indent] = new List<string>(currRow);
                            RowUpsert(currRow, nameNoIndex, value);
                            currRowHasData = true;
                        }
                        else if (string.IsNullOrEmpty(value) && nextIndent > indent)
                        {
                            // For Bss Load, there's no point in creating a column called Bss Load. There will be specific
                            // columns for the underlying values like "Connected Stations"
                            // or          Bss Load:
                            //    ..          Connected Stations:         0
                            ;
                        }
                        else
                        {
                            RowUpsert(currRow, name, value);
                            currRowHasData = true;
                        }

                        if (indent == 2 && nextIndent < indent)
                        {
                            // NOTE: not clear how robust this is for more types of input
                            // row is complete; add to table and restore the old row
                            Rows.Add(currRow);
                            currRowHasData = false; // it has an echo of data. But don't save it unless more is added.
                            var indentForRow = nextIndent >= 0 ? nextIndent : 0;
                            currRow = savedRows[indentForRow] ?? new List<string>();
                        }
                        break;
                }
                if (!string.IsNullOrEmpty(line))
                {
                    prevLine = line;
                    prevIndent = indent;
                }
            }
            if (currRowHasData)
            {
                Rows.Add(currRow);
                currRowHasData = false;
                currRow = new List<string>();
            }
        }


        public string ColumnToReturn = "Name";
        public List<ArgumentSettingValue> ParseForValues(string value)
        {
            Parse(value);
            var retval = GetColumn(ColumnToReturn, "Name");
            return retval;
        }

        /// <summary>
        /// Grabbed 2023-12-29
        /// </summary>
        public static string Example1 = """
Interface name : Wi-Fi 
There are 19 networks currently visible. 

SSID 1 : XFINITY
    HESSID                  : 11:22:33:44:55:66
    Network type            : Infrastructure
    Authentication          : WPA2-Enterprise
    Encryption              : CCMP 
    BSSID 1                 : ba:5e:71:0d:83:fd
         Signal             : 16%  
         Radio type         : 802.11ax
         Band               : 5 GHz  
         Channel            : 44 
         Bss Load:
             Connected Stations:         0
             Channel Utilization:        51 (20 %)
             Medium Available Capacity:  0 (0 us/s)
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54
    BSSID 2                 : fa:d2:ac:0d:cf:9a
         Signal             : 57%  
         Radio type         : 802.11ax
         Band               : 5 GHz  
         Channel            : 149 
         Bss Load:
             Connected Stations:         0
             Channel Utilization:        53 (20 %)
             Medium Available Capacity:  0 (0 us/s)
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54

SSID 2 : xfinitywifi
    Network type            : Infrastructure
    Authentication          : Open
    Encryption              : None 
    BSSID 1                 : ba:5e:71:0d:83:fb
         Signal             : 16%  
         Radio type         : 802.11ax
         Band               : 5 GHz  
         Channel            : 44 
         Bss Load:
             Connected Stations:         0
             Channel Utilization:        51 (20 %)
             Medium Available Capacity:  0 (0 us/s)
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54
    BSSID 2                 : fa:d2:ac:0d:cf:98
         Signal             : 50%  
         Radio type         : 802.11ax
         Band               : 5 GHz  
         Channel            : 149 
         Bss Load:
             Connected Stations:         0
             Channel Utilization:        53 (20 %)
             Medium Available Capacity:  0 (0 us/s)
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54

SSID 3 : Supersoon Google WiFi
    Network type            : Infrastructure
    Authentication          : WPA2-Personal
    Encryption              : CCMP 
    BSSID 1                 : 58:cb:52:a6:1c:92
         Signal             : 38%  
         Radio type         : 802.11ac
         Band               : 5 GHz  
         Channel            : 149 
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54
    BSSID 2                 : 58:cb:52:a6:1c:96
         Signal             : 18%  
         Radio type         : 802.11n
         Band               : 2.4 GHz
         Channel            : 11 
         Basic rates (Mbps) : 1 2 5.5 11
         Other rates (Mbps) : 6 9 12 18 24 36 48 54
    BSSID 3                 : 58:cb:52:a6:1b:8e
         Signal             : 62%  
         Radio type         : 802.11n
         Band               : 2.4 GHz
         Channel            : 6 
         Basic rates (Mbps) : 1 2 5.5 11
         Other rates (Mbps) : 6 9 12 18 24 36 48 54

SSID 4 : Supersoon Google WiFi-guest
    Network type            : Infrastructure
    Authentication          : WPA2-Personal
    Encryption              : CCMP 
    BSSID 1                 : 5a:cb:52:a6:1c:90
         Signal             : 35%  
         Radio type         : 802.11ac
         Band               : 5 GHz  
         Channel            : 149 
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54
    BSSID 2                 : 5a:cb:52:a6:1b:8c
         Signal             : 65%  
         Radio type         : 802.11n
         Band               : 2.4 GHz
         Channel            : 6 
         Basic rates (Mbps) : 1 2 5.5 11
         Other rates (Mbps) : 6 9 12 18 24 36 48 54

SSID 5 : Martin
    Network type            : Infrastructure
    Authentication          : WPA2-Personal
    Encryption              : CCMP 
    BSSID 1                 : 44:07:0b:11:63:1a
         Signal             : 35%  
         Radio type         : 802.11ac
         Band               : 5 GHz  
         Channel            : 149 
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54
    BSSID 2                 : 38:8b:59:d6:5a:6b
         Signal             : 33%  
         Radio type         : 802.11ac
         Band               : 5 GHz  
         Channel            : 149 
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54
    BSSID 3                 : 1c:f2:9a:e5:20:a4
         Signal             : 22%  
         Radio type         : 802.11ac
         Band               : 5 GHz  
         Channel            : 149 
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54
    BSSID 4                 : b0:2a:43:c1:9f:9b
         Signal             : 22%  
         Radio type         : 802.11ac
         Band               : 5 GHz  
         Channel            : 149 
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54
    BSSID 5                 : 38:8b:59:d7:84:47
         Signal             : 46%  
         Radio type         : 802.11ac
         Band               : 5 GHz  
         Channel            : 149 
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54
    BSSID 6                 : 38:8b:59:d7:84:4b
         Signal             : 62%  
         Radio type         : 802.11n
         Band               : 2.4 GHz
         Channel            : 11 
         Basic rates (Mbps) : 1 2 5.5 11
         Other rates (Mbps) : 6 9 12 18 24 36 48 54
    BSSID 7                 : 38:8b:59:d6:5a:6f
         Signal             : 40%  
         Radio type         : 802.11n
         Band               : 2.4 GHz
         Channel            : 11 
         Basic rates (Mbps) : 1 2 5.5 11
         Other rates (Mbps) : 6 9 12 18 24 36 48 54
    BSSID 8                 : 44:07:0b:11:63:1e
         Signal             : 60%  
         Radio type         : 802.11n
         Band               : 2.4 GHz
         Channel            : 6 
         Basic rates (Mbps) : 1 2 5.5 11
         Other rates (Mbps) : 6 9 12 18 24 36 48 54

SSID 6 : Mung's Mansion
    Network type            : Infrastructure
    Authentication          : WPA2-Personal
    Encryption              : CCMP 
    BSSID 1                 : 34:98:b5:b3:90:da
         Signal             : 29%  
         Radio type         : 802.11ax
         Band               : 5 GHz  
         Channel            : 40 
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54
    BSSID 2                 : 34:98:b5:b3:90:be
         Signal             : 81%  
         Radio type         : 802.11ax
         Band               : 5 GHz  
         Channel            : 40 
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54
    BSSID 3                 : 34:98:b5:b5:20:a3
         Signal             : 72%  
         Radio type         : 802.11ax
         Band               : 5 GHz  
         Channel            : 40 
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54
    BSSID 4                 : 3a:98:b5:b5:20:a2
         Signal             : 72%  
         Radio type         : 802.11ax
         Band               : 2.4 GHz
         Channel            : 9 
         Basic rates (Mbps) : 1 2 5.5 11
         Other rates (Mbps) : 6 9 12 18 24 36 48 54
    BSSID 5                 : 3a:98:b5:b3:90:d9
         Signal             : 43%  
         Radio type         : 802.11ax
         Band               : 2.4 GHz
         Channel            : 9 
         Basic rates (Mbps) : 1 2 5.5 11
         Other rates (Mbps) : 6 9 12 18 24 36 48 54
    BSSID 6                 : 3a:98:b5:b3:90:bd
         Signal             : 75%  
         Radio type         : 802.11ax
         Band               : 2.4 GHz
         Channel            : 9 
         Basic rates (Mbps) : 1 2 5.5 11
         Other rates (Mbps) : 6 9 12 18 24 36 48 54

SSID 7 : Raflyn
    Network type            : Infrastructure
    Authentication          : WPA2-Personal
    Encryption              : CCMP 
    BSSID 1                 : 8a:d5:9d:68:0e:19
         Signal             : 53%  
         Radio type         : 802.11n
         Band               : 2.4 GHz
         Channel            : 11 
         Basic rates (Mbps) : 1 2 5.5 11
         Other rates (Mbps) : 6 9 12 18 24 36 48 54

SSID 8 : 
    Network type            : Infrastructure
    Authentication          : WPA2-Enterprise
    Encryption              : CCMP 
    BSSID 1                 : d6:ab:82:c3:32:b2
         Signal             : 31%  
         Radio type         : 802.11n
         Band               : 2.4 GHz
         Channel            : 11 
         Basic rates (Mbps) : 6
         Other rates (Mbps) : 9 12 18 24 36 48 54
    BSSID 2                 : fa:d2:ac:06:cf:9e
         Signal             : 82%  
         Radio type         : 802.11ax
         Band               : 2.4 GHz
         Channel            : 6 
         Bss Load:
             Connected Stations:         0
             Channel Utilization:        108 (42 %)
             Medium Available Capacity:  0 (0 us/s)
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54
    BSSID 3                 : fa:d2:ac:06:cf:9a
         Signal             : 82%  
         Radio type         : 802.11ax
         Band               : 2.4 GHz
         Channel            : 6 
         Bss Load:
             Connected Stations:         0
             Channel Utilization:        108 (42 %)
             Medium Available Capacity:  0 (0 us/s)
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54
    BSSID 4                 : fa:d2:ac:06:cf:98
         Signal             : 82%  
         Radio type         : 802.11ax
         Band               : 2.4 GHz
         Channel            : 6 
         Bss Load:
             Connected Stations:         0
             Channel Utilization:        108 (42 %)
             Medium Available Capacity:  0 (0 us/s)
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54
    BSSID 5                 : fa:d2:ac:0d:cf:9b
         Signal             : 43%  
         Radio type         : 802.11ax
         Band               : 5 GHz  
         Channel            : 149 
         Bss Load:
             Connected Stations:         0
             Channel Utilization:        53 (20 %)
             Medium Available Capacity:  0 (0 us/s)
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54
    BSSID 6                 : fa:d2:ac:0d:cf:99
         Signal             : 46%  
         Radio type         : 802.11ax
         Band               : 5 GHz  
         Channel            : 149 
         Bss Load:
             Connected Stations:         0
             Channel Utilization:        53 (20 %)
             Medium Available Capacity:  0 (0 us/s)
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54
    BSSID 7                 : ba:70:5d:5f:53:ae
         Signal             : 33%  
         Radio type         : 802.11n
         Band               : 2.4 GHz
         Channel            : 6 
         Basic rates (Mbps) : 1 2 5.5 11
         Other rates (Mbps) : 6 9 12 18 24 36 48 54

SSID 9 : barncottage
    Network type            : Infrastructure
    Authentication          : WPA2-Personal
    Encryption              : CCMP 
    BSSID 1                 : c8:7f:54:40:67:48
         Signal             : 38%  
         Radio type         : 802.11ax
         Band               : 2.4 GHz
         Channel            : 8 
         Bss Load:
             Connected Stations:         3
             Channel Utilization:        49 (19 %)
             Medium Available Capacity:  0 (0 us/s)
         Basic rates (Mbps) : 1 2 5.5 11
         Other rates (Mbps) : 6 9 12 18 24 36 48 54

SSID 10 : AnotherDoors
    Network type            : Infrastructure
    Authentication          : WPA2-Personal
    Encryption              : CCMP 
    BSSID 1                 : ac:ec:85:de:1c:b4
         Signal             : 40%  
         Radio type         : 802.11ax
         Band               : 2.4 GHz
         Channel            : 6 
         Basic rates (Mbps) : 1 2 5.5 11
         Other rates (Mbps) : 6 9 12 18 24 36 48 54

SSID 11 : ARLO_VMB_7635979860
    Network type            : Infrastructure
    Authentication          : WPA2-Personal
    Encryption              : CCMP 
    BSSID 1                 : 3c:37:86:6a:ac:2b
         Signal             : 72%  
         Radio type         : 802.11ac
         Band               : 2.4 GHz
         Channel            : 2 
         Basic rates (Mbps) : 1 2 5.5 11
         Other rates (Mbps) : 6 9 12 18 24 36 48 54

SSID 12 : Songer
    Network type            : Infrastructure
    Authentication          : WPA2-Personal
    Encryption              : CCMP 
    BSSID 1                 : f4:92:bf:78:4b:03
         Signal             : 67%  
         Radio type         : 802.11ax
         Band               : 2.4 GHz
         Channel            : 1 
         Basic rates (Mbps) : 1 2 5.5 11
         Other rates (Mbps) : 6 9 12 18 24 36 48 54
    BSSID 2                 : f4:92:bf:78:4b:04
         Signal             : 16%  
         Radio type         : 802.11ac
         Band               : 5 GHz  
         Channel            : 36 
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54

SSID 13 : DIRECT-bO-FireTV_cbe8
    Network type            : Infrastructure
    Authentication          : WPA2-Personal
    Encryption              : CCMP 
    BSSID 1                 : 0a:a6:bc:9d:c8:ed
         Signal             : 20%  
         Radio type         : 802.11ac
         Band               : 5 GHz  
         Channel            : 149 
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54

SSID 14 : Kid-5G
    Network type            : Infrastructure
    Authentication          : WPA2-Personal
    Encryption              : CCMP 
    BSSID 1                 : 10:0c:6b:23:39:fb
         Signal             : 57%  
         Radio type         : 802.11ax
         Band               : 5 GHz  
         Channel            : 157 
         Bss Load:
             Connected Stations:         6
             Channel Utilization:        37 (14 %)
             Medium Available Capacity:  0 (0 us/s)
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54

SSID 15 : Raflyn1
    Network type            : Infrastructure
    Authentication          : WPA2-Personal
    Encryption              : CCMP 
    BSSID 1                 : b0:39:56:dc:ac:48
         Signal             : 24%  
         Radio type         : 802.11ac
         Band               : 5 GHz  
         Channel            : 48 
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54
    BSSID 2                 : b0:39:56:dc:ac:47
         Signal             : 60%  
         Radio type         : 802.11ac
         Band               : 2.4 GHz
         Channel            : 5 
         Basic rates (Mbps) : 1 2 5.5 11
         Other rates (Mbps) : 6 9 12 18 24 36 48 54

SSID 16 : Songer-guest
    Network type            : Infrastructure
    Authentication          : Open
    Encryption              : None 
    BSSID 1                 : fa:92:bf:78:4b:04
         Signal             : 16%  
         Radio type         : 802.11ac
         Band               : 5 GHz  
         Channel            : 36 
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54
    BSSID 2                 : fa:92:bf:78:4b:03
         Signal             : 65%  
         Radio type         : 802.11ax
         Band               : 2.4 GHz
         Channel            : 1 
         Basic rates (Mbps) : 1 2 5.5 11
         Other rates (Mbps) : 6 9 12 18 24 36 48 54

SSID 17 : Raflyn1.5
    Network type            : Infrastructure
    Authentication          : WPA2-Personal
    Encryption              : CCMP 
    BSSID 1                 : 90:72:40:0f:8f:0a
         Signal             : 26%  
         Radio type         : 802.11n
         Band               : 2.4 GHz
         Channel            : 11 
         Basic rates (Mbps) : 1 2 5.5 11
         Other rates (Mbps) : 6 9 12 18 24 36 48 54

SSID 18 : Kid-2.4G
    Network type            : Infrastructure
    Authentication          : WPA2-Personal
    Encryption              : CCMP 
    BSSID 1                 : 10:0c:6b:23:39:fc
         Signal             : 81%  
         Radio type         : 802.11ax
         Band               : 2.4 GHz
         Channel            : 2 
         Bss Load:
             Connected Stations:         25
             Channel Utilization:        68 (26 %)
             Medium Available Capacity:  0 (0 us/s)
         Basic rates (Mbps) : 1 2 5.5 11
         Other rates (Mbps) : 6 9 12 18 24 36 48 54

SSID 19 : Kirkland1
    Network type            : Infrastructure
    Authentication          : WPA2-Personal
    Encryption              : CCMP 
    BSSID 1                 : 5e:7d:7d:52:6c:98
         Signal             : 53%  
         Radio type         : 802.11ax
         Band               : 2.4 GHz
         Channel            : 1 
         Bss Load:
             Connected Stations:         5
             Channel Utilization:        40 (15 %)
             Medium Available Capacity:  0 (0 us/s)
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54


""";

        public static string Example2 = """
 
Interface name : Wi-Fi 
There are 19 networks currently visible. 

SSID 1 : XFINITY
    Encryption              : CCMP 
    BSSID 1                 : ba:5e:71:0d:83:fd
         Signal             : 16%  
         Bss Load:
             Connected Stations:         0
             Channel Utilization:        51 (20 %)
             Medium Available Capacity:  0 (0 us/s)
         Basic rates (Mbps) : 6 12 24

SSID 2 : xfinitywifi
    Network type            : Infrastructure
    Authentication          : Open
""";

        public static string ExampleMBN = """

There is 1 interface on the system:

Name                   : Cellular
Description            : SDX55 5G Modem NetAdapter
GUID                   : {3B78BA46-E595-4E2D-A1A4-7602330D97A4}
Physical Address       : 00:a0:c6:00:00:01
Additional PDP Context : No (Physical interface)
Parent Interface Guid  : No parent
State                  : Connected
Device type            : Mobile Broadband device is embedded in the system
Cellular class         : GSM
Device Id              : 004403150997916
Manufacturer           : Qualcomm
Model                  : SDX55 5G Modem NetAdapter
Firmware Version       : 230209-AR-2bde3fc-01136-1
Provider Name          : 822 722
Roaming                : Not roaming
Signal                 : 48%
RSSI / RSCP            : 15 (-83 dBm)
""";
    } // end internal class ParseIndent : TableParse
} // end namespace utilities
