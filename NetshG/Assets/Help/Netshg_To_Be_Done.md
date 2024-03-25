# Items to be done

## For version 1.10

Bugs fixed:
- the menus get the right checkmarks now when clicked + Reset menu only has things that make sense.
- Reduced the 

In progress right now:

Still to be done
- Create a user preferences
- Add favorite commands and auto-run commands?
- "Fix" in case the user picked "output" and now wants everything to be "table" or vice-versa.
- Clear the history? Which deletes all but the most recent item?

UX improvements that would be awesome
- Support more teams' commands.
- Shorter list of "common" netshg commands.

Other improvements
- MSIX installer and put into store

Bugs
- UX text focus? I shifted away from the app and then back on, and now keys don't work?
- There's a flicker when the user prefers output, but the output prefers table. It shows up as table briefly, then switches to output.

## MAKING A RELEASE: 
Bump the version, build as release, update Github. 

To bump the version, you must set the VersionPrefix in the NetshG project file.

### *Obsolete*
As of 2024-03-35, the old NetshG-Setup and the Click to run settings are obsolete. Instead, the project is 
published with the MSIX packaging project. Old instructions included "Also set the Version in the NetshG-Setup project properties 
(this also changes the ProductCode).""

Also obsolete: the final "standalone" .MSI file that's generated in the NetshG\bin\Publish-Setup should then be added as a new release in the Github directory. The two "Publish" items in the NETSHG project are useless, BTW.
