# NETSH Version History

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
