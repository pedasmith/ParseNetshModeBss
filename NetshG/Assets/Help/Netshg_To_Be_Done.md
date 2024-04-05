# Items to be done

## For version 1.11

UX Updates:
- favorite commands. Have favorites in userpref. Added right-click menu for NetshCommandControl

Bugs fixed:
- seperate left and right mouse button clicks now

In progress right now:



Still to be done
- URL to run the app + commands
- Add auto-run commands?
- Clear the history? Which deletes all but the most recent item?

UX improvements that would be awesome
- Support more teams' commands.

Other improvements

Bugs

## MAKING A RELEASE: 
Bump the version, build as release, update Github. 

To bump the version, you must set the VersionPrefix in the NetshG project file.

### *Obsolete*
As of 2024-03-35, the old NetshG-Setup and the Click to run settings are obsolete. Instead, the project is 
published with the MSIX packaging project. Old instructions included "Also set the Version in the NetshG-Setup project properties 
(this also changes the ProductCode).""

Also obsolete: the final "standalone" .MSI file that's generated in the NetshG\bin\Publish-Setup should then be added as a new release in the Github directory. The two "Publish" items in the NETSHG project are useless, BTW.
