# Stardew Valley TAS Mod

## Basic controls (for this alpha)
Basic Framework for a TAS Mod, includes
* frame advance (press `q` or 'down' for 1 frame, hold 'space' for real time)
* reset (press '|' for normal speed, ']' for fast speed)
* save (press '.', writes to tmp.json)
* load (press ',', reads from tmp.json)
* AC keybind (hold `r` to simulate ac keys)
* Console (Ctrl+`) or (⌘+`) to open/close the console

Hold the keys you want and then advance a frame to store those inputs (simulate AC with `r`)

## Console commands
Slowly copying over/making sure things work in the new system

Helper Functions:
* `help` - allows you to check command description/usage
* `list` - lists available overlays/logics/commands, to find available options
* `clr` - clear the current console screen
* `exit` - exit the game

File System functions:
* `ls` - list save states (allows for wildcard search)
* `cp` - copy save state to new file
* `rm` - delete a save state file
* `mv` - rename a save state file

Save State functions:
* `frame` - print the current frame
* `stateinfo` - print some details about the current save state
* `reset` - reset to the desired frame
* `freset` - fast reset to the desired frame
* `load` - load a save state and advance to final frame
* `fload` -  load a save state and fast advance to final frame
* `save` - save the current state to file
* `saveas` - save the current state to a new file


## Automation

Need for a system (console, clickable menu, etc) to toggle these but some basic automated frame advance for common actions

* `AcceptSleep` - auto advance when the frame for the sleep menu is popping up and approve the sleep question
* `AnimationCancel` - advance through frames after swing up to the first cancel frame
* `DialoguBoxTransition` - advance through frames where a dialogue box is transitioning on/off the screen
* `ScreenFade` - advance through frames where the screen is fading in and out of black

To implement a new Automation, create a new class that inherits from `IAutomatedLogic`. All classes that inherit from `IAutomatedLogic` will be automatically loaded and available in `Controller.Logics`.

## Overlays

* `DebugMouse` - draw your actual mouse onto the screen

To implement a new Overlay, create a new class that inherits from `IOverlay`. All classes that inherit from `IOverlay` will be automatically loaded and available in `Controller.Overlays`.

## How does this work?

A ton of harmony patches (including patching SMAPI inputs and base System.DateTime and System.Random) to rewrap functionality and control all parts of the update/draw loop. Added some additional fields to System.Random to trace random calls for debugging purposes.

