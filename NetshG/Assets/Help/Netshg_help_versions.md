# NETSHG Version History

## Version 1.11

Added favorites: your preferred commands can be shown in **boldface**. Right-click a command and select toggle favorite, or after you run a command, select the menu Settings > Toggle Favorite

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
