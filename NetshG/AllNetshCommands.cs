using Newtonsoft.Json;
using System.Collections.Generic;

namespace NetshG
{
    class AllNetshCommands
    {
        public static List<CommandInfo> GetCommands()
        {
            var retval = JsonConvert.DeserializeObject<List<CommandInfo>>(AllCommands);
            if (retval == null)
            {
                return new List<CommandInfo>() { };
            }
			foreach (var item in retval)
			{
				item.UpdateRequiresFromList(); // As appropriate
			}
            return retval;
        }

        public static string AllCommands = """
[
   {
		"Cmd":"netsh",
		"Args":"advfirewall show allprofiles",
		"note":"missing parameters"
	},
    {
		"Cmd":"netsh",
		"Args":"advfirewall show currentprofile"
	},
    {
		"Cmd":"netsh",
		"Args":"advfirewall show domainprofile"
	},
    {
		"Cmd":"netsh",
		"Args":"advfirewall show global"
	},
    {
		"Cmd":"netsh",
		"Args":"advfirewall show privateprofile"
	},
    {
		"Cmd":"netsh",
		"Args":"advfirewall show publicprofile"
	},
    {
		"Cmd":"netsh",
		"Args":"advfirewall show store"
	},
    {
		"Cmd":"netsh",
		"Args":"bridge show adapter"
	},
    {
		"Cmd":"netsh",
		"Args":"dnsclient show encryption"
	},
    {
		"Cmd":"netsh",
		"Args":"dnsclient show global"
	},
    {
		"Cmd":"netsh",
		"Args":"dnsclient show state"
	},
    {
		"Cmd":"netsh",
		"Args":"http show cacheparam"
	},
    {
		"Cmd":"netsh",
		"Args":"http show cachestate"
	},
    {
		"Cmd":"netsh",
		"Args":"http show iplisten"
	},
    {
		"Cmd":"netsh",
		"Args":"http show servicestate",
		"notes":"has verbose=yes|no instead of level"
	},
    {
		"Cmd":"netsh",
		"Args":"http show setting"
	},
    {
		"Cmd":"netsh",
		"Args":"http show sslcert"
	},
    {
		"Cmd":"netsh",
		"Args":"http show timeout"
	},
    {
		"Cmd":"netsh",
		"Args":"http show urlacl"
	},
    {
		"Cmd":"netsh",
		"Args":"interface 6to4 show interface"
	},
    {
		"Cmd":"netsh",
		"Args":"interface 6to4 show relay"
	},
    {
		"Cmd":"netsh",
		"Args":"interface 6to4 show routing"
	},
    {
		"Cmd":"netsh",
		"Args":"interface 6to4 show state"
	},
    {
		"Cmd":"netsh",
		"Args":"interface httpstunnel show interfaces"
	},
    {
		"Cmd":"netsh",
		"Args":"interface httpstunnel show statistics",
		"MoreArgs":"store=Store",
		"RequireList":"Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show addresses"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show compartments",
		"MoreArgs":"level=Level store=Store",
		"RequireList":"Level,Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show config"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show config InterfaceIndex",
		"Requires":[
			{
				"Name": "InterfaceIndex"
			}
		]
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show destinationcache",
		"MoreArgs":"level=Level",
		"RequireList":"Level"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show dnsservers"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show dynamicportrange",
		"ArgsExtra": "tcp",
		"MoreArgs":"store=Store",
		"RequireList":"Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show dynamicportrange",
		"ArgsExtra": "udp",
		"MoreArgs":"store=Store",
		"RequireList":"Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show excludedportrange",
		"ArgsExtra": "tcp",
		"MoreArgs":"store=Store",
		"RequireList":"Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show excludedportrange",
		"ArgsExtra": "udp",
		"MoreArgs":"store=Store",
		"RequireList":"Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show global",
		"MoreArgs":"store=Store",
		"RequireList":"Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show icmpstats"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show interfaces",
		"Sets": "InterfaceIndex",
		"SetParser": "Interfaces",
		"MoreArgs":"level=Level store=Store",
		"RequireList":"Level,Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show ipaddresses",
		"Tags":"#common",
		"MoreArgs":"level=Level store=Store",
		"RequireList":"Level,Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show ipnettomedia"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show ipstats",
		"Tags":"#common"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show joins",
		"MoreArgs":"level=Level",
		"RequireList":"Level"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show neighbors",
		"MoreArgs":"level=Level store=Store",
		"RequireList":"Level,Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show offload"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show route",
		"MoreArgs":"level=Level store=Store",
		"RequireList":"Level,Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show subinterfaces",
		"MoreArgs":"level=Level store=Store",
		"RequireList":"Level,Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show tcpconnections"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show tcpstats",
		"Tags":"#common"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show udpconnections"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show udpstats",
		"Tags":"#common"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show winsservers"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 6to4 show interface"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 6to4 show relay"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 6to4 show routing"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 6to4 show state"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 isatap show router"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 isatap show state"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show addresses",
		"Tags":"#common",
		"MoreArgs":"level=Level store=Store",
		"RequireList":"Level,Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show compartments",
		"MoreArgs":"level=Level store=Store",
		"RequireList":"Level,Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show destinationcache",
		"MoreArgs":"level=Level",
		"RequireList":"Level"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show dnsservers"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show dynamicportrange",
		"ArgsExtra":"tcp",
		"MoreArgs":"store=Store",
		"RequireList":"Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show dynamicportrange",
		"ArgsExtra":"udp",
		"MoreArgs":"store=Store",
		"RequireList":"Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show excludedportrange",
		"ArgsExtra":"tcp",
		"MoreArgs":"store=Store",
		"RequireList":"Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show excludedportrange",
		"ArgsExtra":"udp",
		"MoreArgs":"store=Store",
		"RequireList":"Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show global",
		"MoreArgs":"store=Store",
		"RequireList":"Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show interfaces",
		"MoreArgs":"level=Level store=Store",
		"RequireList":"Level,Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show ipstats"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show joins",
		"MoreArgs":"level=Level",
		"RequireList":"Level"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show neighbors",
		"MoreArgs":"level=Level store=Store",
		"RequireList":"Level,Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show offload"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show potentialrouters"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show prefixpolicies",
		"MoreArgs":"store=Store",
		"RequireList":"Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show privacy",
		"MoreArgs":"store=Store",
		"RequireList":"Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show route",
		"MoreArgs":"level=Level store=Store",
		"RequireList":"Level,Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show siteprefixes"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show subinterfaces",
		"MoreArgs":"level=Level store=Store",
		"RequireList":"Level,Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show tcpstats"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show teredo"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show tfofallback"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show udpstats"
	},
    {
		"Cmd":"netsh",
		"Args":"interface isatap show router"
	},
    {
		"Cmd":"netsh",
		"Args":"interface isatap show state"
	},
    {
		"Cmd":"netsh",
		"Args":"interface portproxy show all"
	},
    {
		"Cmd":"netsh",
		"Args":"interface portproxy show v4tov4"
	},
    {
		"Cmd":"netsh",
		"Args":"interface portproxy show v4tov6"
	},
    {
		"Cmd":"netsh",
		"Args":"interface portproxy show v6tov4"
	},
    {
		"Cmd":"netsh",
		"Args":"interface portproxy show v6tov6"
	},
    {
		"Cmd":"netsh",
		"Args":"interface show interface"
	},
    {
		"Cmd":"netsh",
		"Args":"interface tcp show global",
		"MoreArgs":"store=Store",
		"RequireList":"Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface tcp show heuristics"
	},
    {
		"Cmd":"netsh",
		"Args":"interface tcp show rscstats"
	},
    {
		"Cmd":"netsh",
		"Args":"interface tcp show rscstats InterfaceIndex",
		"RequireList":"InterfaceIndex"
	},
    {
		"Cmd":"netsh",
		"Args":"interface tcp show security",
		"MoreArgs":"store=Store",
		"RequireList":"Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface tcp show supplemental",
		"note":"missing template"
	},
    {
		"Cmd":"netsh",
		"Args":"interface tcp show supplementalports",
		"MoreArgs":"level=Level",
		"RequireList":"Level"
	},
    {
		"Cmd":"netsh",
		"Args":"interface tcp show supplementalsubnets",
		"MoreArgs":"level=Level",
		"RequireList":"Level"
	},
    {
		"Cmd":"netsh",
		"Args":"interface teredo show state"
	},
    {
		"Cmd":"netsh",
		"Args":"interface udp show global",
		"MoreArgs":"store=Store",
		"RequireList":"Store"
	},
    {
		"Cmd":"netsh",
		"Args":"lan show interfaces",
		"Tags":"#common"
	},
    {
		"Cmd":"netsh",
		"Args":"lan show profiles"
	},
    {
		"Cmd":"netsh",
		"Args":"lan show settings"
	},
    {
		"Cmd":"netsh",
		"Args":"lan show tracing"
	},
    {
		"Cmd":"netsh",
		"Args":"mbn show acstate"
	},
    {
		"Cmd":"netsh",
		"Args":"mbn show capability"
	},
    {
		"Cmd":"netsh",
		"Args":"mbn show connection"
	},
    {
		"Cmd":"netsh",
		"Args":"mbn show d3cold"
	},
    {
		"Cmd":"netsh",
		"Args":"mbn show dataenablement"
	},
    {
		"Cmd":"netsh",
		"Args":"mbn show dataroamcontrol"
	},
    {
		"Cmd":"netsh",
		"Args":"mbn show dmprofiles"
	},
    {
		"Cmd":"netsh",
		"Args":"mbn show enterpriseapnparams"
	},
    {
		"Cmd":"netsh",
		"Args":"mbn show highestconncategory"
	},
    {
		"Cmd":"netsh",
		"Args":"mbn show homeprovider"
	},
    {
		"Cmd":"netsh",
		"Args":"mbn show interfaces"
	},
    {
		"Cmd":"netsh",
		"Args":"mbn show netlteattachinfo"
	},
    {
		"Cmd":"netsh",
		"Args":"mbn show pin"
	},
    {
		"Cmd":"netsh",
		"Args":"mbn show pinlist"
	},
    {
		"Cmd":"netsh",
		"Args":"mbn show preferredproviders"
	},
    {
		"Cmd":"netsh",
		"Args":"mbn show profiles"
	},
    {
		"Cmd":"netsh",
		"Args":"mbn show profilestate"
	},
    {
		"Cmd":"netsh",
		"Args":"mbn show provisionedcontexts"
	},
    {
		"Cmd":"netsh",
		"Args":"mbn show purpose"
	},
    {
		"Cmd":"netsh",
		"Args":"mbn show radio"
	},
    {
		"Cmd":"netsh",
		"Args":"mbn show readyinfo"
	},
    {
		"Cmd":"netsh",
		"Args":"mbn show signal"
	},
    {
		"Cmd":"netsh",
		"Args":"mbn show slotmapping"
	},
    {
		"Cmd":"netsh",
		"Args":"mbn show slotstatus"
	},
    {
		"Cmd":"netsh",
		"Args":"mbn show smsconfig"
	},
    {
		"Cmd":"netsh",
		"Args":"mbn show tracing"
	},
    {
		"Cmd":"netsh",
		"Args":"mbn show visibleproviders"
	},
    {
		"Cmd":"netsh",
		"Args":"namespace show effectivepolicy"
	},
    {
		"Cmd":"netsh",
		"Args":"namespace show policy"
	},
    {
		"Cmd":"netsh",
		"Args":"netio show bindingfilters",
		"MoreArgs":"store=Store",
		"RequireList":"Store"
	},
    {
		"Cmd":"netsh",
		"Args":"nlm show connectivity",
		"Tags":"#common"
	},
    {
		"Cmd":"netsh",
		"Args":"nlm show cost",
		"Tags":"#common"
	},
    {
		"Cmd":"netsh",
		"Args":"p2p idmgr show groups",
		"ArgsExtra":"ALL"
	},
    {
		"Cmd":"netsh",
		"Args":"p2p idmgr show groups",
		"ArgsExtra":"ALL EXPIRED"
	},
    {
		"Cmd":"netsh",
		"Args":"p2p pnrp cloud show list"
	},
    {
		"Cmd":"netsh",
		"Args":"p2p pnrp cloud show names"
	},
    {
		"Cmd":"netsh",
		"Args":"p2p pnrp cloud show statistics"
	},
    {
		"Cmd":"netsh",
		"Args":"ras show wanports"
	},
    {
		"Cmd":"netsh",
		"Args":"rpc filter show filter"
	},
    {
		"Cmd":"netsh",
		"Args":"rpc show"
	},
    {
		"Cmd":"netsh",
		"Args":"show alias"
	},
    {
		"Cmd":"netsh",
		"Args":"show helper"
	},
    {
		"Cmd":"netsh",
		"Args":"trace show CaptureFilterHelp"
	},
    {
		"Cmd":"netsh",
		"Args":"trace show globalKeywordsAndLevels"
	},
    {
		"Cmd":"netsh",
		"Args":"trace show helperclass"
	},
    {
		"Cmd":"netsh",
		"Args":"trace show interfaces"
	},
    {
		"Cmd":"netsh",
		"Args":"trace show provider"
	},
    {
		"Cmd":"netsh",
		"Args":"trace show ProviderFilterHelp"
	},
    {
		"Cmd":"netsh",
		"Args":"trace show providers",
		"note":"takes up to 5 seconds while the UX is frozen"
	},
    {
		"Cmd":"netsh",
		"Args":"trace show scenario name=Scenario",
		"RequireList":"Scenario"
	},
    {
		"Cmd":"netsh",
		"Args":"trace show scenarios",
		"Sets":"Scenario",
		"SetParser":"Scenario"
	},
    {
		"Cmd":"netsh",
		"Args":"trace show status"
	},
    {
		"Cmd":"netsh",
		"Args":"winhttp show advproxy"
	},
    {
		"Cmd":"netsh",
		"Args":"winhttp show proxy"
	},
    {
		"Cmd":"netsh",
		"Args":"winsock audit trail"
	},
    {
		"Cmd":"netsh",
		"Args":"winsock show autotuning"
	},
    {
		"Cmd":"netsh",
		"Args":"winsock show catalog"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show all",
		"Tags":"#wifi"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show allowexplicitcreds",
		"Tags":"#wifi"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show autoconfig",
		"Tags":"#wifi"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show blockednetworks",
		"Tags":"#wifi"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show createalluserprofile",
		"Tags":"#wifi"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show drivers",
		"Tags":"#common #wifi"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show filters",
		"Tags":"#wifi"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show hostednetwork",
		"Tags":"#wifi"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show hostednetwork setting=security",
		"Tags":"#wifi"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show interfaces",
		"Tags":"#common #wifi"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show networks",
		"ArgsExtra":"mode=ssid",
		"Tags":"#common #wifi"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show networks",
		"ArgsExtra":"mode=bssid",
		"Tags":"#common #wifi"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show onlyUseGPProfilesforAllowedNetworks",
		"Tags":"#wifi"
	},
	{
		"Cmd":"netsh",
		"Args":"wlan show profiles",
		"Tags":"#common #wifi",
		"Sets": "Profile",
		"SetParser": "Profile"
	},
	{
		"Cmd":"netsh",
		"Args":"wlan show profiles",
		"ArgsExtra": "name=\"Profile\" key=clear",
		"Tags":"#common #wifi",
		"Requires": [
			{ 
				"Name": "Profile",
			}
		]
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show randomization",
		"Tags":"#wifi"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show settings",
		"Tags":"#wifi"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show tracing",
		"Tags":"#wifi"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show wirelesscapabilities",
		"Tags":"#wifi"
	}

]
""";

    }
}

