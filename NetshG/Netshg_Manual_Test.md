# Testing NETSH for each release

## Smoke test

1. Menu Show > Common
2. Menu Show > ensure Show Help is checked
3. Select netsh wlan show drivers

Results: 
- Help should be displayed within 1 second. Help should be longer than will fit. Scroll bar should be visible.
- Output should show up within 2 seconds. Output should be longer than will fit on screen; scroll bar should be visible.

1. Select netsh interface show ipstats

Results:
- Output should be display

2. Wait 5 seconds and type ``alt-R R``. 

Results:
- Display should update; numbers should change. Display should flash.
