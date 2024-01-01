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
		"Cmd":"ipconfig",
		"Args2":"/all",
		"Tags":"#common"
	},
    {
		"Cmd":"ipconfig",
		"Args2":"/displaydns"
	},
    {
		"Cmd":"ipconfig",
		"Args2":"/showclassid *"
	},
    {
		"Cmd":"ipconfig",
		"Args2":"/showclassid6 *"
	},
    {
		"Cmd":"systeminfo",
		"Args2":"/fo table"
	},
    {
		"Cmd":"systeminfo",
		"Args2":"/fo list"
	},
    {
		"Cmd":"systeminfo",
		"Args2":"/fo csv"
	},

    {
		"Cmd":"netsh",
		"Args":"advfirewall firewall show rule",
		"Args2":"name=all",
		"Tags":"#common"
	},
    {
		"Cmd":"netsh",
		"Args":"advfirewall show allprofiles",
		"Issues":"The Ok. at the end is awkward to parse."
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
		"Args":"advfirewall show global",
		"Issues":"The StrongCRLCheck is awkward to parse, as is the timestampt for SAIdleTimeMin."
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
		"Args":"dnsclient show encryption",
		"TableParser":"DashLine",
		"Issues":"Dealing with IPv6 is awkward at best."
	},
    {
		"Cmd":"netsh",
		"Args":"dnsclient show global"
	},
    {
		"Cmd":"netsh",
		"Args":"dnsclient show state",
		"Issues":"The spacing between lines is awkward and looks like multiple sections."
	},
    {
		"Cmd":"netsh",
		"Args":"http show cacheparam"
	},
    {
		"Cmd":"netsh",
		"Args":"http show cachestate",
		"Issues":"The output is not designed to be parsed -- the value (0) is written in the middle of a sentence as 'no cache entries'"
	},
    {
		"Cmd":"netsh",
		"Args":"http show iplisten"
	},
    {
		"Cmd":"netsh",
		"Args":"http show servicestate",
		"Issues":"Has verbose=yes|no instead of level"
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
		"Args":"interface 6to4 show relay",
		"TableParser":"List"
	},
    {
		"Cmd":"netsh",
		"Args":"interface 6to4 show routing",
		"TableParser":"List"
	},
    {
		"Cmd":"netsh",
		"Args":"interface 6to4 show state",
		"TableParser":"List"
	},
    {
		"Cmd":"netsh",
		"Args":"interface httpstunnel show interfaces"
	},
    {
		"Cmd":"netsh",
		"Args":"interface httpstunnel show statistics",
		"Args5NoUX":"store=Store",
		"RequireList":"Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 dump",
		"Tags":"#common"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show addresses",
		"TableParser":"Indent",
		"Tags":""
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show addresses",
		"Args2": "InterfaceIndex",
		"TableParser":"Indent",
		"Tags":"",
		"RequireList":"InterfaceIndex"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show compartments",
		"Args5NoUX":"level=Level store=Store",
		"TableParser":"List",
		"Sets":"CompartmentIndex",
		"SetParser": "List",
		"Tags":"",
		"RequireList":"Level,Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show compartments",
		"Args2":"CompartmentIndex",
		"Args5NoUX":"level=Level store=Store",
		"TableParser":"List",
		"Tags":"",
		"RequireList":"Level,Store,CompartmentIndex"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show config",
		"TableParser":"Indent",
		"Tags":"",
		"Issues":"The Statically Configured DNS servers is difficult to parse because it's two lines of text. It's not clear why there isn't an ipv6 version of this command."
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show config InterfaceIndex",
		"TableParser":"Indent",
		"Tags":"",
		"Requires":[
			{
				"Name": "InterfaceIndex"
			}
		]
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show destinationcache",
		"Args5NoUX":"level=Level",
		"Tags":"",
		"RequireList":"Level",
		"TableParser":"List"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show destinationcache",
		"Args2":"InterfaceIndex",
		"Args5NoUX":"level=Level",
		"Tags":"",
		"RequireList":"Level,InterfaceIndex",
		"TableParser":"List",
		"Issues":"when set to verbose=normal, can't be parsed with a List parser."
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show dnsservers",
		"Tags":"",
		"TableParser":"Indent",
		"Issues":"The interface name for each header line is quoted, which makes it hard to parse."
	},
	{
		"Cmd":"netsh",
		"Args":"interface ipv4 show dnsservers",
		"Args2":"InterfaceIndex",
		"Tags":"",
		"TableParser":"Indent",
		"RequireList":"InterfaceIndex"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show dynamicportrange",
		"Args2": "Protocol",
		"Args5NoUX":"store=Store",
		"RequireList":"Protocol,Store",
		"Tags":"",
		"TableParser":"List"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show excludedportrange",
		"Args2": "Protocol",
		"Args5NoUX":"store=Store",
		"RequireList":"Protocol,Store",
		"Issues":"Nearly impossible to parse because of the table struction. The * for adminstered port exclusions does not have a column header name."
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show global",
		"Args5NoUX":"store=Store",
		"RequireList":"Store",
		"TableParser":"DashLine",
		"Issues":"This command outputs two tables which must be forced into a single table."
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show icmpstats",
		"Issues":"Difficult to parse because the title (MIB-II ICMP Statistics) looks like a section header, but it's not"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show interfaces",
		"Tags":"#common ",
		"TableParser":"List",
		"Sets": "InterfaceIndex",
		"SetParser": "Interfaces",
		"Args5NoUX":"level=Level store=Store",
		"RequireList":"Level,Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show ipaddresses",
		"Tags":"#common",
		"Args5NoUX":"level=Level store=Store",
		"RequireList":"Level,Store",
		"TableParser":"DashLine"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show ipnettomedia",
		"Issues":"Unable to parse because of the tabular format."
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show ipstats",
		"Tags":"#common",
		"TableParser":"DashLine"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show joins",
		"Args5NoUX":"level=Level",
		"RequireList":"Level",
		"Issues":"Unable to parse because of the multiple blank lines means that determining the headers 'Interface 1' is difficult."
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show neighbors",
		"Args5NoUX":"level=Level store=Store",
		"RequireList":"Level,Store",
		"Issues":"Unable to parse for similar reasons as the show joins"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show offload",
		"Issues":"Difficult to parse because the values are sentences instead of in item : value format"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show route",
		"Args5NoUX":"level=Level store=Store",
		"RequireList":"Level,Store",
		"Issues":"The SitePrefixLength, ValidLifeTime, and PreferredLifeTime colons are all missing :"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show subinterfaces",
		"Args5NoUX":"level=Level store=Store",
		"RequireList":"Level,Store",
		"TableParser":"List"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show tcpconnections",
		"Issues":"One of the few outputs that uses tabs. Column headers are tab seperated, but not the data columns."
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show tcpstats",
		"Tags":"#common",
		"TableParser":"List"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show udpconnections",
		"Issues":"Why does tcpconnections use tabs, but udpconnections does not? "
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv4 show udpstats",
		"Tags":"#common",
		"Issues":"Splitting the table into normal and extended statistics makes is harder to parse, not easier."
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
		"Args":"interface ipv6 dump",
		"Tags":"#common"
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
		"Args5NoUX":"level=Level store=Store",
		"RequireList":"Level,Store",
		"TableParser":"DashLine"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show compartments",
		"Args5NoUX":"level=Level store=Store",
		"RequireList":"Level,Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show destinationcache",
		"Args5NoUX":"level=Level",
		"RequireList":"Level"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show dnsservers",
		"Issues":"Multi-value items (like DNS Servers configured through DHCP) are difficult to parse."
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show dynamicportrange",
		"Args2":"tcp",
		"Args5NoUX":"store=Store",
		"RequireList":"Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show dynamicportrange",
		"Args2":"udp",
		"Args5NoUX":"store=Store",
		"RequireList":"Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show excludedportrange",
		"Args2":"tcp",
		"Args5NoUX":"store=Store",
		"RequireList":"Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show excludedportrange",
		"Args2":"udp",
		"Args5NoUX":"store=Store",
		"RequireList":"Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show global",
		"Args5NoUX":"store=Store",
		"RequireList":"Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show interfaces",
		"Args5NoUX":"level=Level store=Store",
		"RequireList":"Level,Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show ipstats"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show joins",
		"Args5NoUX":"level=Level",
		"RequireList":"Level"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show neighbors",
		"Args5NoUX":"level=Level store=Store",
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
		"Args5NoUX":"store=Store",
		"RequireList":"Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show privacy",
		"Args5NoUX":"store=Store",
		"RequireList":"Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show route",
		"Args5NoUX":"level=Level store=Store",
		"RequireList":"Level,Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show siteprefixes"
	},
    {
		"Cmd":"netsh",
		"Args":"interface ipv6 show subinterfaces",
		"Args5NoUX":"level=Level store=Store",
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
		"Args5NoUX":"store=Store",
		"RequireList":"Store",
		"Tags":"#common",
		"Issues":"The Query active state message is difficult to parse."
	},
    {
		"Cmd":"netsh",
		"Args":"interface tcp show heuristics",
		"Tags":"#common"
	},
    {
		"Cmd":"netsh",
		"Args":"interface tcp show rscstats"
	},
    {
		"Cmd":"netsh",
		"Args":"interface tcp show rscstats InterfaceIndex",
		"Tags":"#common ",
		"TableParser":"List",
		"RequireList":"InterfaceIndex"
	},
    {
		"Cmd":"netsh",
		"Args":"interface tcp show security",
		"Args5NoUX":"store=Store",
		"RequireList":"Store"
	},
    {
		"Cmd":"netsh",
		"Args":"interface tcp show supplemental"
	},
    {
		"Cmd":"netsh",
		"Args":"interface tcp show supplemental",
		"Args2":"template=ITSSTemplate",
		"RequireList":"ITSSTemplate",
		"Issues":"List of template options in the help is inconsistant. Automatic doesn't provide useful data."
	},
    {
		"Cmd":"netsh",
		"Args":"interface tcp show supplementalports",
		"Args5NoUX":"level=Level",
		"RequireList":"Level"
	},
    {
		"Cmd":"netsh",
		"Args":"interface tcp show supplementalsubnets",
		"Args5NoUX":"level=Level",
		"RequireList":"Level"
	},
    {
		"Cmd":"netsh",
		"Args":"interface teredo show state"
	},
    {
		"Cmd":"netsh",
		"Args":"interface udp show global",
		"Args5NoUX":"store=Store",
		"RequireList":"Store",
		"Tags":"#common"
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
		"Args5NoUX":"store=Store",
		"RequireList":"Store"
	},
    {
		"Cmd":"netsh",
		"Args":"nlm show connectivity",
		"Tags":"#common",
		"Issues":"The Connectivity is hard to parse because it's a list. Should be split into two lines."
	},
    {
		"Cmd":"netsh",
		"Args":"nlm show cost",
		"Tags":"#common",
		"TableParser":"DashLine"
	},
    {
		"Cmd":"netsh",
		"Args":"p2p idmgr show groups",
		"Args2":"ALL",
		"Tags":"#obsolete"
	},
    {
		"Cmd":"netsh",
		"Args":"p2p idmgr show groups",
		"Args2":"ALL EXPIRED",
		"Tags":"#obsolete"
	},
    {
		"Cmd":"netsh",
		"Args":"p2p pnrp cloud show list",
		"Tags":"#obsolete"
	},
    {
		"Cmd":"netsh",
		"Args":"p2p pnrp cloud show names",
		"Tags":"#obsolete"
	},
    {
		"Cmd":"netsh",
		"Args":"p2p pnrp cloud show statistics",
		"Tags":"#obsolete"
	},
    {
		"Cmd":"netsh",
		"Args":"ras show wanports",
		"TableParser":"DashLine"
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
		"Args":"show helper",
		"Issues":"The Command column is hard to parse because of the indentation levels."
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
		"SetParser":"Scenario",
		"Issues":"The DirectAccessServer help value is blank. The list is difficult to parse."
	},
    {
		"Cmd":"netsh",
		"Args":"trace show status"
	},
    {
		"Cmd":"netsh",
		"Args":"winhttp show advproxy",
		"Issues":"This is one of the few values in JSON format"
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
		"TableParser":"DashLine",
		"Tags":"#wifi"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show autoconfig",
		"Tags":"#wifi",
		"Issues":"This is hard to parse because the value is written like a sentence instead of with lines of name : value "
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show blockednetworks",
		"TableParser":"DashLine",
		"Tags":"#wifi"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show createalluserprofile",
		"Tags":"#wifi",
		"Issues":"Not parsable"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show drivers",
		"Tags":"#common #wifi",
		"Issues":"Similar to Indent but the Auth stuff and more is not parsable. List of radio types is not in any useful order."
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show filters",
		"Tags":"#wifi",
		"Issues":"Having the list be <None> for no entries is difficult to parse."
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show hostednetwork",
		"TableParser":"DashLine",
		"Tags":"#wifi",
		"Issues":"Splitting the results into two tables makes this harder to parse without having much values."
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show hostednetwork",
		"Args2":"setting=security",
		"TableParser":"DashLine",
		"Tags":"#wifi"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show interfaces",
		"TableParser":"List",
		"Tags":"#common #wifi",
		"Issues":"Nearly parseable with List except for the extra space before Hosted network status"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show networks",
		"Args2":"mode=ssid",
		"TableParser":"Indent",
		"Tags":"#common #wifi",
		"Issues":"Value is sometimes just one network even when multiple networks are visible."
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show networks",
		"Args2":"mode=bssid",
		"TableParser":"Indent",
		"Tags":"#common #wifi",
		"Issues":"Value is often just one BSSID even when multitple BSSIDs are visible."
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show onlyUseGPProfilesforAllowedNetworks",
		"TableParser":"DashLine",
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
		"Args2": "name=\"Profile\" key=clear",
		"Tags":"#common #wifi",
		"Requires": [
			{ 
				"Name": "Profile",
			}
		],
		"Issues":"Output is awkward to parse because the different sections are parsed as different rows."
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show randomization",
		"Tags":"#wifi",
		"Issues":"Unable to parse"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show settings",
		"Tags":"#wifi",
		"Issues":"Unable to parse with DashLine because of the extra spaces"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show tracing",
		"Tags":"#wifi"
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show wirelesscapabilities",
		"Tags":"#wifi",
		"TableParser":"DashLine",
		"Issues": "System capabilities are hard to parse because they are sentences and not name : value pairs."
	},
    {
		"Cmd":"netsh",
		"Args":"wlan show wlanreport",
		"Tags":"#wifi #common"
	},

]
""";

    }
}

