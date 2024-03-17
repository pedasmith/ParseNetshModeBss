# Items to be done

## For version 1.9

The history control is the first big new item for 1.9. Currently implemented
- there's a history control
- new output get placed in it
- the user can pick a different item 
- Must be able to use arrows to go back and forth and home and end
- The current selected item must be visible. In the middle, but with no blank space on the left or right.

Did in this version :-)
- Selection works better: on mouse up, do the command. This lets a command be run multiple times.
- Update UX of history control to use icons instead of words; include tooltips for accessability.
- right-click to delete and show information (title and time).

Still to be done
- When moving in the history, must update the title! 
- When deleting in the history, when the last item is deleted, clear the screen.
- Handle a huge amount of data.
- "Fix" in case the user picked "output" and now wants everything to be "table" or vice-versa.

UX improvements that would be awesome
- Support more teams' commands.
- Shorter list of "common" netshg commands.

Other improvements
- MSIX installer and put into store

Bugs
- UX text focus? I shifted away from the app and then back on, and now keys don't work?
- There's a flicker when the user prefers output, but the output prefers table. It shows up as table briefly, then switches to output.