# Bugs found in netsh output

## Command: netsh wlan show networks mode=bssid

### Hash-to-element has 2 colons, not 1

The BSSID Hash-to-element item has two colons. One is correctly indented. But the other, right after the words "Hash-to-element", is extra.

```
SSID 2 : 7-Deco-B71C-MLO
    Network type            : Infrastructure
    Authentication          : WPA3-Personal
    Encryption              : CCMP 
    BSSID 1                 : 66:62:8b:42:b7:1f
         Signal             : 90%  
         Radio type         : 802.11be
         Band               : 5 GHz  
         Channel            : 48 
         Hash-to-Element:   : Supported
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54
```

### 9 spaces of indent instead of 8

The BSSID output looks something like the below. The data for the SSID (e.g., the Signal) is indented by 9 spaces. For consistancy with the rest of Netsh, it should be indented 8 spaced.

```
SSID 1 : ToomreHouse
    Network type            : Infrastructure
    Authentication          : WPA2-Personal
    Encryption              : CCMP 
    BSSID 1                 : 70:3a:cb:19:26:68
         Signal             : 81%  
         Radio type         : 802.11ac
         Band               : 5 GHz  
         Channel            : 149 
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54
    BSSID 2                 : 1c:f2:9a:e0:99:b0
         Signal             : 85%  
         Radio type         : 802.11ac
         Band               : 5 GHz  
         Channel            : 149 
         Basic rates (Mbps) : 6 12 24
         Other rates (Mbps) : 9 18 36 48 54
```