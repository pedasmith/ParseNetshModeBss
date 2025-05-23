# About NetshG

![NETSHG Gui shell for NETSH](Assets/HelpImages/Netshg_interface_example.png)

## Quickstart
NetshG version {VERSION} is a graphical interface for Netsh and other common commands. It turns different Netsh commands, and displays the results in a scrolling text area. Some commands can be parsed as a table; for those commands the table will be displayed.

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

# Automation with the **netshg** protocol

Example: in a command window, type ```start netshg:action:run;cmd:netshwlanshowdriver;;```. The Netshg program will start and the netsh wlan show driver command will be run.

You can run multiple commands, seperated with double-semicolons.

Only commands listed in the "Show" menu can be run; no other command is permitted. The commands are abbreviated from what is listed in the command list: spaces are removed.

# Replacement for NETSH

You can replace any command-line "netsh" command with "netshg". The netshg program will take all your arguments and will run that command as a netsh command. Want to run a different command? Just start it with an exclamation point: ```netshg !ping microsoft.com``` will run the **ping microsoft.com** command using the NETSHG shell.

# Technical Details
## Where is the data from?
The NetshG program gets its information by running the Netsh and similar commands. It's not a re-implementation of NETSH at all.


