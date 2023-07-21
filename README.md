# Stardew Valley TAS Mod
[Build Issues?](#build-issues)

## Basic controls (for this alpha)
Basic Framework for a TAS Mod, includes
* frame advance (press `q` or `downarrow` for 1 frame, hold `space` for real time)
* reset (press `|` for normal speed, ']' for fast speed)
* save (press `.`, writes to tmp.json)
* load (press `,`, reads from tmp.json)
* AC keybind (hold `r` to simulate ac keys)
* Console (Ctrl+\`) or (⌘+\`) to open/close the console (\` is above tab on US keyboards, may function as `~` if using another keyboard layout...)
    * you cannot manually input while the console is up

Hold the keys you want and then advance a frame to store those inputs (simulate AC with `r`)

## Console commands
Slowly copying over/making sure things work in the new system.

Console supports scrolling, selection, normal copy and paste (or it should, LET ME KNOW because it works on mac fine). Console font has no support for non-ascii, so will print `?`.

#### Helper Functions:
Any function that is callable through this top level console has some help text associated with its use, and you can discover different tools through the list command.
* `help` - allows you to check command description/usage
* `list` - lists available overlays/logics/commands, to find available options
* `overlay` - print or modify the status of TAS overlays
* `logic` - print or modify the status of TAS automation logic
* `clr` - clear the current console screen
* `exit` - exit the game

#### File System functions:
Tries to mimic basic linux command line methods
* `ls` - list save states (allows for wildcard search)
* `cp` - copy save state to new file
* `rm` - delete a save state file
* `mv` - rename a save state file

#### Save State functions:
Basic functions for manipulating the save state including loading, saving, and resetting.
* `frame` - print the current frame
* `stateinfo` - print some details about the current save state
* `reset` - reset to the desired frame
* `freset` - fast reset to the desired frame
* `load` - load a save state and advance to final frame
* `fload` -  load a save state and fast advance to final frame
* `save` - save the current state to file
* `saveas` - save the current state to a new file
* `newgame` - setup a new save state file and fill in details
* `swapseed` - swap the seed of the current save state

#### Engine State functions:
Engine state is the current state of overlays/logics. You can save different configurations if you find that you often want to switch between different setups.
* `saveengine` - save the current engine state to file
* `loadengine` - load engine state from file

## Automation logic
Slowly copying over/making sure things work in the new system.

Some basic automated frame advance for common actions

* `AcceptSleep` - auto advance when the frame for the sleep menu is popping up and approve the sleep question
* `AnimationCancel` - advance through frames after swing up to the first cancel frame
* `DialoguBoxTransition` - advance through frames where a dialogue box is transitioning on/off the screen
* `ScreenFade` - advance through frames where the screen is fading in and out of black

To implement a new Automation, create a new class that inherits from `IAutomatedLogic`. All classes that inherit from `IAutomatedLogic` will be automatically loaded and available in `Controller.Logics`.

## Overlays
Slowly copying over/making sure things work in the new system.

* `Mouse` - draw your actual mouse onto the screen
* `MinesLadder` - draw rock break counts and path to nearest ladder
* `Grid` - draw the tile grid outlines
* `TimerPanel` - draw a timer panel in the top left corner
* `Hitbox` - draw hitboxes for all characters
* `WeaponGuide` - draw a guide for weapon hitboxes

To implement a new Overlay, create a new class that inherits from `IOverlay`. All classes that inherit from `IOverlay` will be automatically loaded and available in `Controller.Overlays`.

## How does this work?

A ton of harmony patches (including patching SMAPI inputs and base System.DateTime and System.Random) to rewrap functionality and control all parts of the update/draw loop. Added some additional fields to System.Random to trace random calls for debugging purposes.


## Build Issues
If you have Stardew installed at a non-standard location, you'll need to modify the StardewModConfig build config which can be found at:

`C:\Users\<User>\.nuget\packages\pathoschild.stardew.modbuildconfig\4.1.0\build\find-game-folder.targets`

**NOTE**: you may need to try building and failing once before this file will exist.

At the bottom of the file you can add
```xml
<GamePath Condition="!Exists('$(GamePath)')">Path-To-Stardew-Directory</GamePath>
```

(Thanks to @PianoAddict for finding these details!)
