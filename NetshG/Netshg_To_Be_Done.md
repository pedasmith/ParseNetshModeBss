# Items to be done

## For version 1.9

The history control is the first big new item for 1.9. Currently implemented
- there's a history control
- new output get placed in it
- the user can pick a different item 
- Must be able to use arrows to go back and forth and home and end
- The current selected item must be visible. In the middle, but with no blank space on the left or right.

Still to be done
- right-click to delete, copy, and show information (title and time).
- Handle a huge amount of data.
- "Fix" in case the user picked "output" and now wants everything to be "table" or vice-versa.
- Update UX of history control to use icons instead of words; include tooltips for accessability.

UX improvements that would be awesome
- Support more teams' commands.
- Shorter list of "common" netshg commands.

Other improvements
- MSIX installer and put into store

Bugs
- UX text focus? I shifted away from the app and then back on, and now keys don't work?
- There's a flicker when the user prefers output, but the output prefers table. It shows up as table briefly, then switches to output.