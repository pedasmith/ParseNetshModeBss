# NETSHG Version History

## Version 1.16 2025-05-23

Added menu items for **perfmon**. The perfmon /rel (reliability) command is marked as a common command.

## Version 1.15

Added menu item to start the Get Help troubleshooter at the Network page.
Added menu items (under Macros) to try multiple vendor "am I online" sites.


## Version 1.14

Help diagnose NCSI (msftconnectest.com and msftncsi.com) probes. Includes an HTTPS: probe and a /redirect probe. Curl is run with -vs so that the exact HTTP results are visible. The output of the command is printed first, then the logging.

## Version 1.13

Some commands are so common (e.g., SystemInfo) that they deserve to be at the top of the menu areas. 

Added a new Macro menu with common macros for tracing.

## Version 1.12

From feedback: netshg is now a complete replacement for netsh. For any netsh command, you can replace the netsh with netshg and the command will be run. Note that it doesn't do any quotes or anything: whatever you type will just be converted to a string and run. You can run other comands by starting them with an exclamation point: ```netshg !ping microsoft.com``` will run the **ping microsoft.com** command.


## Version 1.11

Added favorites: your preferred commands can be shown in **boldface**. Right-click a command and select toggle favorite, or after you run a command, select the menu Settings > Toggle Favorite

Supports automation with the **netshg** protocol (scheme). When a URL with the scheme "netshg" is seen, NetshG will be run and the command embedded in the URL will be run.

## Version 1.10 2024-03-26 

Minor bugs fixed, and preferences remembers whether the user last picked text output or a table. Default is text output, mostly because the table output isn't quite reliable enough.


## Version 1.9 2024-03-25

The app now tracks the history of command outputs and can switch between them. In the example, the history bar is highlighted. Every command is added to the history. Click on a history dot to switch to that command output or use the arrow keys. After too many commands are run (e.g., when you're repeating a lot of commands), the history kept uses a statistically valid sampling algorithm (expoentially weighted moving average, or EWMA) to keep a summary of commands.

![Highlighted area shows history bar](Assets/HelpImages/Netshg_show_history.png)

The set of common commands has been trimmed down a bit to make the list more manageable. The installer is now an MSIX bundle, not an MSI file, so the app can theoretically be placed in the store.


## Version 1.8 2024-01-16

Update to the parameter user interface: parameters like "InterfaceIndex" are comboboxes now (and the output handles this more gracefully)
Added support for more commands including PING and NETSTAT and the "dump" commands from Netstat. The output can flow in over time now.
There's a search for commands now; you can enter e.g., "dns" and get all commands that have "dns" in them (or their tags)


## Version 1.7 2024-01-10

- Include Reset commands like "netsh interface ip reset"
- User can automatically repeat a command for all variables (for example, to get information on all adapters)
- The help sytem is nicer

- 
## Version 1.6 2024-01-03
Will automatically call the right command when a previously-unseen argument is needed. For example,
if an InterfaceIndex is needed, will so a show interfaces to get a list of interfaces, and will then pick one to show.


## Version 1.4 2023-12-31

First good version. Most commands work, will auto repeat, 
there's help and keyboard shortcuts.

## Version 1.3 2023-12-30

First version with a working installer :-)
