# NETSH Version History


## Version 1.9 2024-03-09

The app now tracks the history of command outputs and can switch between them.

## Version 1.8 2024-01-16

Update to the parameter user interface: parameters like "InterfaceIndex" are comboboxes now (and the output handles this more gracefully)
Added support for more commands including PING and NETSTAT and the "dump" commands from Netstat. The output can flow in over time now.
There's a search for commands now; you can enter e.g., "dns" and get all commands that have "dns" in them (or their tags)



## Version 1.7 2024-01-10

- Include Reset commands like "netsh interface ip reset"
- User can automatically repeat a command for all variables (for example, to get information on all adapters)
- The help sytem is nicer

## Version 1.6 2024-01-03
Will automatically call the right command when a previously-unseen argument is needed. For example,
if an InterfaceIndex is needed, will so a show interfaces to get a list of interfaces, and will then pick one to show.


## Version 1.4 2023-12-31

First good version. Most commands work, will auto repeat, 
there's help and keyboard shortcuts.

## Version 1.3 2023-12-30

First version with a working installer :-)
