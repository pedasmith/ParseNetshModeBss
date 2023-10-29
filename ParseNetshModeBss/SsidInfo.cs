using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ParseNetshModeBss
{
    public enum NetworkType { Other, Infrastructure };
    public enum Authentication { Other, Open, Owe, WPA_Personal, WPA2_Personal, WPA2_Enterprise, WPA3_Personal, WPA3_Enterprise_192 };
    public enum Encryption { Other, None, WEP, CCMP, GCMP_256 };
    /// <summary>
    /// Wi-Fi type like 802.11be for Wi-Fi 7
    /// </summary>
    public enum RadioType { Other, W802_11, W802_11b, W802_11a, W802_11g, W802_11n, W802_11ac, W802_11ax, W802_11be, };

    public static class Convert
    {
        public static Dictionary<Authentication, string> AuthenticationStrings = new Dictionary<Authentication, string>
        {
            { Authentication.Other, "Other" },
            { Authentication.Open, "Open" },
            { Authentication.Owe, "OWE" },
            { Authentication.WPA_Personal, "WPA-Personal" },
            { Authentication.WPA2_Personal, "WPA2-Personal" },
            { Authentication.WPA2_Enterprise, "WPA2-Enterprise" },
            { Authentication.WPA3_Personal, "WPA3-Personal" },
            { Authentication.WPA3_Enterprise_192, "WPA3-Enterprise 192 Bits" },
        };

        public static Dictionary<Encryption, string> EncryptionStrings = new Dictionary<Encryption, string>
        {
            { Encryption.Other, "Other" },
            { Encryption.None, "None" },
            { Encryption.CCMP, "CCMP" },
            { Encryption.GCMP_256, "GCMP-256" },
            { Encryption.WEP, "WEP" },
        };


        public static Dictionary<NetworkType, string> NetworkTypeStrings = new Dictionary<NetworkType, string>
        {
            { NetworkType.Other, "Other" },
            { NetworkType.Infrastructure, "Infrastructure" },
        };

        public static Dictionary<RadioType, string> RadioTypeStrings = new Dictionary<RadioType, string>()
        {
            { RadioType.Other, "Other" },
            { RadioType.W802_11, "802.11" },
            { RadioType.W802_11b, "802.11b" },
            { RadioType.W802_11a, "802.11a" },
            { RadioType.W802_11g, "802.11g" },
            { RadioType.W802_11n, "802.11n" },
            { RadioType.W802_11ac, "802.11ac" },
            { RadioType.W802_11ax, "802.11ax" },
            { RadioType.W802_11be, "802.11be" },
        };

    }

    internal class SsidInfo
    {
        // These can't be properties because properties can't be used the way
        // I want when I do the parsing in ParseBssidMode.cs
        public int Index = -1;
        public string Name = "";
        public NetworkType NetworkType = NetworkType.Other;
        public Authentication Authentication = Authentication.Other;
        public Encryption Encryption = Encryption.Other;
        public IList<BssidInfo> Bssids { get; } = new List<BssidInfo>();

        public static string ToCsv(IList<SsidInfo> list)
        {
            var stable = "SsidIndex,Ssid,Authentication,Encryption,BssidIndex,Mac,Radio,Band,Channel";
            var perf = "SignalStrengthPercent,ConnectedStations,LoadUtilization,LoadUtilitizationPercent";
            var retval = stable + "," + perf + "\n";
            foreach (var ssid in list)
            {
                retval += ssid.ToCsv();
            }
            return retval;
        }
        private string ToCsv()
        {
            var retval = "";
            foreach (var bssid in Bssids)
            {
                retval += bssid.ToCsv(this);
            }
            return retval;
        }
        public override string ToString()
        {
            return $"SSID {Name} w/{Bssids.Count} BSSIDs";
        }
    }

    internal class BssidInfo
    {
        // Fixed data
        public int Index = -1;
        public string Mac = "";
        public RadioType RadioType = RadioType.Other;
        public double BandInGHz = 0.0;
        public string Channel = "";

        // Performance data
        public double SignalStrengthPercent = -1.0;
        public int BssLoadConnectedStations = -1;
        public double BssLoadUtilization = -1;
        public double BssLoadUtilizationPercent = -1.0;
        public double BssLoadMediumAvailableCapacity = -1.0;
        public double BssLoadMediumAvailableCapacityUSS = -1.0;

        public string BssLoadConnectedStationsCsv { get { return BssLoadConnectedStations >= 0 ? BssLoadConnectedStations.ToString() : ""; } }
        public string BssLoadUtilizationCsv { get { return BssLoadUtilization >= 0 ? BssLoadUtilization.ToString() : ""; } }
        public string BssLoadUtilizationPercentCsv { get { return BssLoadUtilizationPercent >= 0 ? BssLoadUtilizationPercent.ToString() : ""; } }


        // Not actually useful :-)
        public string Rates = "";
        public string OtherRates = "";

        public string ToCsv(SsidInfo ssid)
        {
            //"SsidIndex,Ssid,Authentication,Encryption,BssidIndex,Mac,Radio,Band,Channel";
            //"SignalStrengthPercent,ConnectedStations,LoadUtilization,LoadUtilitizationPercent";

            var stable = $"{ssid.Index},{ssid.Name},{Convert.AuthenticationStrings[ssid.Authentication]},{Convert.EncryptionStrings[ssid.Encryption]},{Index},{Mac},{Convert.RadioTypeStrings[RadioType]},{BandInGHz},{Channel}";
            var perf = $"{SignalStrengthPercent},{BssLoadConnectedStationsCsv},{BssLoadUtilizationCsv},{BssLoadUtilizationPercentCsv}";
            return stable+","+perf+"\n";
        }
        public override string ToString()
        {
            return $"{Index} {Mac} {SignalStrengthPercent}% {Convert.RadioTypeStrings[RadioType]} {BandInGHz}GHz {Channel}";
        }
    }


}
