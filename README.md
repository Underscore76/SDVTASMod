# Stardew Valley TAS Mod
[Build Issues?](#build-issues)

NOTE: Use of this mod will create a folder `StardewTAS` in your `Documents` folder. This folder will contain your save states, scripts, screenshots, etc.

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

### Lua Support

Scripting in this TAS is done with Lua! By typing `lua` onto the console you'll enter into a Lua REPL (read-eval-print-loop), which will allow you to run arbitrary lua code. Documentation for the core lua engine functions can be found by navigating locally to docs/ldoc/index.html or you can browse the `TASMod/Assets/lua` folder where the lua files are stored.

There is an example `init.lua` file in the `lua-examples` folder. The TASMod will look for a file called `init.lua` in the `StardewTAS/Scripts/` folder and will run that file when first launching into the lua console. This allows you to define custom functions and aliases that you can use in the console. For example, you can define a function called `myfunc` in `init.lua` and then call it from the lua console with `myfunc()`. You can also do things like auto-load into a specific save state or configure the engine state by toggling overlays/logic etc (or loading a particular engine state).

#### Visual Studio Code Support
If you are using Visual Studio Code for editing files in your local `StardewTAS/Scripts` folder, I recommend installing the [Lua language server](https://marketplace.visualstudio.com/items?itemName=sumneko.lua) extension to get type annotations and autocomplete for Lua. To include all the base engine lua files, open the workspace settings (Shift + Command + P (Mac) / Ctrl + Shift + P (Windows/Linux) to open the command palette) and search for `lua.workspace.library`, click Add Item, and then paste the path to the `lua` folder from the mod install.

Default Steam install paths (if you are on linux let me know the path!):
* Mac: `~/Library/Application Support/Steam/steamapps/common/Stardew Valley/Contents/MacOS/Mods/TASMod/assets/lua/`
* Windows: `C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\TASMod\assets\lua\`

You don't need to do this (the mod will correctly link the files), but it will give you intellisense for the base engine code.

#### Key Lua Helper
* `advance({keyboard={keys to press}, mouse={X=int, Y=int, left=bool, right=bool}})` - advance a frame with the given keyboard/mouse inputs. This should then pause while any existing automation is running. Example for holding down and right and clicking the mouse:
```lua
advance({keyboard={Keys.S, Keys.D}, mouse={X=100, Y=100, left=true}})
```

* Global frame stack
    * `current_frame()` - returns the current frame actual frame
    * `push(f)` - pushes a frame onto the frame stack
    * `push()` or `gcf()` - pushes the current frame onto the frame stack
    * `f = pop()` - pops a frame off the frame stack
    * `rw()` - rewinds to the frame on the top of the frame stack
    * `brw()` - blocking rewind to the frame on the top of the frame stack
    * `frame_stack_clear()` - clears the frame stack
    * `pgcf()` - print the global frame stack
* reset functions (blocking forces lua to wait until the reset is complete, others can occur async)
    * `reset()` - reset to the current frame
    * `freset()` - fast reset to the current frame
    * `reset(f)` - reset to the given frame
    * `freset(f)` - fast reset to the given frame
    * `breset()` - blocking reset to the current frame
    * `breset(f)` - blocking reset to the given frame
    * `bfreset()` - blocking fast reset to the current frame
    * `bfreset(f)` - blocking fast reset to the given frame
* save state functions
    * `save()` - save the current state to file
    * `saveas(f)` or `save(f)` - save the current state to a new file
    * `load(f)` - load a save state and advance to final frame
    * `fload(f)` - load a save state and fast advance to final frame
    * `view()` - swap between view modes
    * `exec("...")` - execute a command on the base console (e.g. `exec("overlay off Layers")`)

There's a ton more but there's a whole generated set of docs for the lua engine that you can find in the docs folder.

### Core Console Functions

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
* `alias` - allows you to define custom commands based on other commands `alias set ss=screenshot` will set `ss` to call the `screenshot` function

#### Path Finding
* `genpath` - generates a nav path to a tile
* `walkpath` - walk the current nav path (activates the `WalkPath` logic)

#### Info functions:
* `forage` - print current details about forage (defaults to current location, define specific `forage loc` or `forage all`)
* `friendship` - prints current friendship details
* `player` - prints details on player including xp/friendship/pos/etc
* `trashcans` - prints current trash can drops

#### Misc functions:
* `sethayday` - sets the date for the `WheatHay` overlay
* `blankscreen` - turns the screen black
* `screenshot` - screenshot the current game screen (stored in the StardewTAS folder)
* `takestep` - take N steps in a direction (activates the `TakeStep` logic)

## Automation logic
Tools for automated frame advance through common actions

* `AcceptSleep` - auto advance when the frame for the sleep menu is popping up and approve the sleep question
* `AdvanceFrozen` - advance when character is frozen/picking up an artifact/sleeping emoting
* `AnimationCancel` - advance through frames after swing up to the first cancel frame
* `DialogueBox` - advance to the frame where you can click a dialogue box
* `DialogueBoxTransition` - advance through frames where a dialogue box is transitioning on/off the screen
* `GhostCancel` - walk like a ghost (without taking a step)
* `LevelUpMenu` - advance through level up menu (does not advance profession select)
* `SaveGame` - advance through the save game menu
* `ScreenFade` - advance through frames where the screen is fading in and out of black
* `ShippingMenu` - holds the mouse and advances through shipping menu
* `SkipEvent` - skips events or advances through pause frames up during an event/requires dialogue interaction
* `SwipePickup` - swing cancel pickup an item if melee weapon in inventory (will swap tools suboptimally)
* `TakeStep` - walk N steps in a direction
* `WalkPath` - attempt to walk the current nav path automatically (will be suboptimal vs manual, may panic out)

To implement a new Automation, create a new class that inherits from `IAutomatedLogic`. All classes that inherit from `IAutomatedLogic` will be automatically loaded and available in `Controller.Logics`.

## Overlays
Additional info layers to draw on top of the game

* `Clay` - draw the clay tilling map
* `ClickableMenuRects` - show outlines of clickable screen elements in menus
* `CropQuality` - draw first gold quality day for a tile
* `DrawPath` - draw the current nav path
* `Fishing` - draw a simplified view of the fishing minigame including where the fish wants to go
* `Grid` - draw the tile grid outlines
* `Hitbox` - draw hitboxes for all characters
* `InfoPanel` - draw a bunch of random game state details
* `Layers` - highlight all of the unwalkable tiles
* `MinesLadder` - draw rock break counts and path to nearest ladder
* `MixedSeed` - overlay the seed you'll get if you plant a mixed seed on the next frame
* `MonsterDrop` - displays items a monster will drop
* `Mouse` - draw your actual mouse onto the screen
* `MouseData` - draws tile, steps, ticks at mouse position
* `ObjectDrop` - draw what items an object will drop
* `ObjectTiles` - draw boundaries around objects
* `Sleep` - draw sleep details given current step count
* `TileHighlight` - draw highlighted tiles
* `TimerPanel` - draw a timer panel in the top left corner
* `WeaponGuide` - draw a guide for weapon hitboxes
* `WheatHay` - draw tiles where wheat will give hay if harvested on a specific day

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


## Generating Lua Docs
This project uses [LDoc](https://github.com/lunarmodules/ldoc) to generate documentation for the lua files. To generate the docs, run the following command from the root directory of the project:

```bash
ldoc .
```

You'll need Lua/LuaRocks installed to run this command, which will vary by system. On Mac you can install Lua/LuaRocks with the following command:

```bash
brew install lua luarocks
luarocks install penlight
luarocks install ldoc
```

Looking for someone on windows to give guidance on how to install Lua/LuaRocks :).