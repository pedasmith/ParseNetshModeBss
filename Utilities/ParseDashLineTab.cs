using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    internal class ParseDashLineTab: ITableParse
    {
        public const string Example = """
Domain Profile Settings: 
----------------------------------------------------------------------
State                                 ON
Firewall Policy                       BlockInbound,AllowOutbound
LocalFirewallRules                    N/A (GPO-store only)
LocalConSecRules                      N/A (GPO-store only)
InboundUserNotification               Enable
RemoteManagement                      Disable
UnicastResponseToMulticast            Enable

Logging:
LogAllowedConnections                 Disable
LogDroppedConnections                 Disable
FileName                              %systemroot%\system32\LogFiles\Firewall\pfirewall.log
MaxFileSize                           4096

""";

        public List<ArgumentSettingValue> ParseForValues(string value)
        {
            var retval = new List<ArgumentSettingValue>();
            return retval;
        }
    }
}
