# Items to be done


## For version 1.15

Added GET HELP as a command
Updated the URL Launch code to actually work (it was throwing exceptions)
Added CURL commands for non-Microsoft web beacons

## For version 1.14

UX Updates: added CURL for NCSI
Bugs fixed:
The repeat button doesn't work for macro
Can't press return and run a command
Trace Start vanished



## For version 1.11

UX Updates:

Bugs fixed:

In progress right now:



Still to be done
- Add auto-run commands?
- Clear the history? Which deletes all but the most recent item?

UX improvements that would be awesome
- Support more teams' commands.

Other improvements

Bugs

## netshg: URI scheme

When installed, Netshg will be called when the user selects a netshg: URL. The URL might be embedded in an email, or via a command line.

The syntax is similar to the MECARD format used by the wifi: protocol. Unlike that protocol, the commands are more explicit, and you can have multiple commands.

Example: ```netsh:action:run;cmd:netshinterfaceipv4showaddressesInterfaceIndex;;action:run;cmd:netshwlanshowdrivers;;```

List of actions
Action|Meaning
-----|-----
run|Runs one of the "show" commands. No other command lists can be run (this helps make the feature less prone to misuse). Requires the "cmd" value to specify the command to be run.

Field|Used In|Meaning
-----|-----|-----
cmd|run|Selects a command to run. Only commands in the "show" list can be run. The cmd field is a shortened version of the command: it does not include spaces. For example, the command 



## MAKING A RELEASE: 
Bump the version, build as release, package, update the store. 

- To bump the version, you must set the VersionPrefix in the NetshG project file.
- To build as release, set type to release and arch as x86 and do build clean build rebuild
- To package, right-click MSIX_Packaging and "Publish" > "Create app packages"


### *Obsolete*
As of 2024-03-35, the old NetshG-Setup and the Click to run settings are obsolete. Instead, the project is 
published with the MSIX packaging project. Old instructions included "Also set the Version in the NetshG-Setup project properties 
(this also changes the ProductCode).""

Also obsolete: the final "standalone" .MSI file that's generated in the NetshG\bin\Publish-Setup should then be added as a new release in the Github directory. The two "Publish" items in the NETSHG project are useless, BTW.
