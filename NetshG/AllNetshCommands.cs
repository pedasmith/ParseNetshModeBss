using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetshG
{
    class AllNetshCommands
    {
        public static List<CommandInfo> GetCommands()
        {
            var retval = JsonConvert.DeserializeObject<List<CommandInfo>>(AllCommands);
            //var retval = JsonConvert.DeserializeObject<List<CommandInfo>>(MicroList);
            if (retval == null)
            {
                return new List<CommandInfo>() { };
            }
            return retval;
        }

		public static string MicroList = """
[
	{
		"Cmd":"netsh",
		"Args":"wlan show profiles",
		"Sets": "Profile",
		"SetParser": "Profile",
	},
	{
		"Cmd":"netsh",
		"Args":"wlan show profiles name=\"Profile\" key=clear",
		"Requires": [
			{ 
				"Name": "Profile",
			}
		]
	}
	]
""";

        public static string AllCommands = """
[
   {
		"Cmd":"netsh",
		"Args":"advfirewall show allprofiles"
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
		"Args":"http show servicestate"
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
		"Args":"interface httpstunnel show statistics"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show addresses"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show compartments"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show config"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show destinationcache"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show dnsservers"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show dynamicportrange tcp store=active"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show dynamicportrange udp store=active"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show dynamicportrange tcp store=persistent"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show dynamicportrange udp store=persistent"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show excludedportrange tcp store=active"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show excludedportrange udp store=active"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show excludedportrange tcp store=persistent"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show excludedportrange udp store=persistent"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show global"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show icmpstats"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show interfaces"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show ipaddresses"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show ipnettomedia"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show ipstats"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show joins"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show neighbors"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show offload"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show route"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show subinterfaces"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show tcpconnections"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show tcpstats"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show udpconnections"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show udpstats"
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
		"Args":"interface ipv6 show addresses"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show compartments"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show destinationcache"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show dnsservers"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show dynamicportrange tcp store=active"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show dynamicportrange udp store=active"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show dynamicportrange tcp store=persistent"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show dynamicportrange udp store=persistent"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show excludedportrange tcp store=active"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show excludedportrange udp store=active"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show excludedportrange tcp store=persistent"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show excludedportrange udp store=persistent"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show global"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show interfaces"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show ipstats"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show joins"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show neighbors"
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
		"Args":"interface ipv6 show prefixpolicies store=active"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show prefixpolicies store=persistent"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show privacy store=active"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show privacy store=persistent"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show route"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show siteprefixes"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show subinterfaces"
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
		"Args":"interface tcp show global"
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
		"Args":"interface tcp show security"
	},
    {
		"Cmd":"netsh",
		"Args":"interface tcp show supplemental"
	},
    {
		"Cmd":"netsh",
		"Args":"interface tcp show supplementalports"
	},
    {
		"Cmd":"netsh",
		"Args":"interface tcp show supplementalsubnets"
	},
    {
		"Cmd":"netsh",
		"Args":"interface teredo show state"
	},
    {
		"Cmd":"netsh",
		"Args":"interface udp show global store=active"
	},
    {
		"Cmd":"netsh",
		"Args":"interface udp show global store=persistent"
	},
    {
		"Cmd":"netsh",
		"Args":"lan show interfaces"
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
		"Args":"netio show bindingfilters store=active"
	},
    {
		"Cmd":"netsh",
		"Args":"netio show bindingfilters store=persistent"
	},
    {
		"Cmd":"netsh",
		"Args":"nlm show connectivity"
	},
    {
		"Cmd":"netsh",
		"Args":"nlm show cost"
	},
    {
		"Cmd":"netsh",
		"Args":"p2p idmgr show groups ALL"
	},
    {
		"Cmd":"netsh",
		"Args":"p2p idmgr show groups ALL EXPIRED"
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
		"Args":"trace show providers"
	},
    {
		"Cmd":"netsh",
		"Args":"trace show scenario"
	},
    {
		"Cmd":"netsh",
		"Args":"trace show scenarios"
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
		"Args":"wlan show all"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show allowexplicitcreds"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show autoconfig"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show blockednetworks"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show createalluserprofiles"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show drivers"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show filters"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show hostednetwork"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show hostednetwork setting=security"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show interfaces"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show networks mode=ssid"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show networks mode=bssid"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show onlyUseGPProfilesforAllowedNetworks"
	},
	{
		"Cmd":"netsh",
		"Args":"wlan show profiles",
		"Sets": "Profile",
		"SetParser": "Profile",
	},
	{
		"Cmd":"netsh",
		"Args":"wlan show profiles name=\"Profile\" key=clear",
		"Requires": [
			{ 
				"Name": "Profile",
			}
		]
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show randomization"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show settings"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show tracing"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show wirelesscapabilities"
	}

]
""";

    }
}

