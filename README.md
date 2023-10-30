# ParseNetshModeBss: turn the output of netsh wlan show networks mode=bss to CSV

IT admins use the ```netsh wlan show network mode=bss``` command to get detailed information about each of the wireless access
points that are visible to a PC. However, the output can be verbose to read, and cannot be directly read into spreadsheet
programs like **Excel**. That's where the **ParseNetshModeBss** program comes in. It's a "single exe" type of .NET program that's
designed to read in the output of the command and present the data in the RFC 4180 standard CSV files.



## Example of the input and output
The ```netsh wlan show network mode=bss``` creates output like this:

```
> netsh wlan show networks mode=bssid

Interface name : Wi-Fi
There are 3 networks currently visible.

SSID 1 : MyOfficeWifi
    Network type            : Infrastructure
    Authentication          : WPA2-Personal
    Encryption              : CCMP
    BSSID 1                 : 4c:22:8e:48:ba:5f
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

```

this generates the much more compact CSV output:

```CSV
SsidIndex,Ssid,Authentication,Encryption,BssidIndex,Mac,Radio,Band,Channel,SignalStrengthPercent,ConnectedStations,LoadUtilization,LoadUtilitizationPercent
1,MyOfficeWifi,WPA2-Personal,CCMP,1,2c:12:8b:49:b3:1e,802.11be,5,40,91,,,
1,MyOfficeWifi,WPA2-Personal,CCMP,2,2c:12:8b:49:b3:1d,802.11be,2.4,10,96,,,
```
