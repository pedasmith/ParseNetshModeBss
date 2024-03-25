# About NetshG

![NETSHG Gui shell for NETSH](Assets/HelpImages/Netshg_interface_example.png)

## Quickstart
NetshG version {VERSION} is a graphical interface for Netsh. It turns different Netsh commands, and displays the results in a scrolling text area. Some commands can be parsed as a table; for those commands the table will be displayed.

The starting set of commands are the most commonly used. You can also choose to see **all** commands or the Wi-Fi** commands.

## Menus
### File Exit
Exits the program. Your current preferences (like the set of commands
to display) will be saved.

### Show Common, All, All (Verbose), Wi-Fi
Sets the set of commands to display. All these commands are intended to show the current configuration and will not change your settings. Common shows the most commonly used commands, All shows all commands, the All (Verbose) shows all commands include obsolete ones, and Wi-Fi shows the commands used to help diagnose Wi-Fi issues.

### Show Help
When a command is run, will also show the help for the command. The help is taken straight from the NETSH command.

### Clear and Reset Common, All

Displays the commands which will reset or clear different parts of hte network stack. For example, the **netsh ipconfig /flushdns** command will flush your DNS cache and is in this set of commands and not the "show" set of commands.

### Repeat / Repeat Once
Repeats a command once. If the auto-repeat is on, will stop auto-repeat

## Repeat time interval

Repeats a command at the given interval (e.g., every second, every 10 seconds, and so on)

## Repeat / Repeat Stop

If the auto-repeat is on, will stop auto-repeat

## Parameters
Lets you set some of the command parameters like the verbosity level. Not all commands use all parameters.

## Help Help
Shows this help text; this is also where the version information is shown.

## Help Keyboard Versions
Shows a history of the program.

### Help Keyboard Shortcuts
Shows a list of keyboard shortcuts.

# Technical Details
## Where is the data from?
The NetshG program gets its information by running the Netsh and similar commands. It's not a re-implementation of NETSH at all.

## MAKING A RELEASE: 
Bump the version, build as release, update Github. 

To bump the version, you must set the VersionPrefix in the NetshG project file AND the Version in the NetshG-Setup project properties (this also changes the ProductCode). 

The final "standalone" .MSI file that's generated in the NetshG\bin\Publish-Setup should then be added as a new release in the Github directory. The two "Publish" items in the NETSHG project are useless, BTW.

