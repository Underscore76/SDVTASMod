-- TO USE THIS FILE: copy into `StardewTAS/Scripts/init.lua` file (Scripts folder may not exist, make it)
-- when you load the lua interface (type `lua`) in the in game console, it will do a bunch 
-- of environment setup and will check to see if in the `StardewTAS/Scripts/` folder there is 
-- a file called `init.lua`. if there is, it will load the file and run it. this allows you 
-- to define a custom boot script for your TAS that can do things like pull in additional libraries, 
-- rename some convenience functions, load your input scripts, etc.
-- everything is programmatic so in theory your `init.lua` could define the entire TAS input sequence
-- as a series of advance calls... that sounds miserable, don't do that. but you *could*!

-- this file has a number of examples of how to launch your local configuration
-- here we do the following:
--  1) bind some custom keybinds
--  2) check if our game file exists, 
--      if exists:
--          a) fast load the game file
--      else:
--          a) generate a new game file
--          b) launch into the start of day 1 using a custom character configuration
--          c) save the game at this point
--  3) run some additional configuration


local keybinds = require("core.keybinds")

-- example for adding a custom keybind, 
-- here we bind the key 'j' to an inlined function definition that animation cancels 10 times
keybinds.add(Keys.J,
    function()
        for i=1,10 do
            advance({keyboard={Keys.C, Keys.RightShift, Keys.R, Keys.Delete}})
        end
    end
)

-- we can also take an existing function and bind it to a key, and we can also add a description for the keybind
local function z()
    for i=1,10 do
        advance({keyboard={Keys.C, Keys.RightShift, Keys.R, Keys.Delete}})
    end
end
keybinds.add(Keys.Z, z, "press z")

-- basic usage of the startup script
local function launch()
    local startup = require("core.scripts.startup")

    -- farm startup params
    startup.set_skip(true)
    startup.set_sex('female')
    startup.set_farm('Forest')
    -- NOTE: these numbers can be wonky due to issues with how SDV actually numbers them
    -- there may be gaps in the numbers, some numbers might be negative indexed
    -- its confusing... Generally it's the number shown in the menu - 1.
    -- if you try and number that doesn't work, it will just attempt to spin for a while before giving up and moving on
    startup.set_hair(8) -- there are missing hair styles
    startup.set_shirt(105)
    startup.set_acc(0) -- 0 is the beard in this case, -1 is no accessory...
    startup.set_skin(2) -- 0 indexed and as expected indexing
    startup.set_pant(0) -- 0 indexed and as expected indexing

    startup.set_eye_color(nil,nil,89) -- these are the hsv sliders, 0-100% on each
    startup.set_hair_color(nil,nil,24) -- these are the hsv sliders, 0-100% on each
    startup.set_pants_color(nil,nil,21) -- these are the hsv sliders, 0-100% on each
    
    -- this is an example startup script that will launch you into the start of day 1
    startup.run()
    startup.click_ok()
    
    advance() -- get past the initial launch
    halt() -- waits until past the save (generally shouldn't need to manually halt outside this case of overnight load)
    
    advance() -- push into the day1 load
    -- the game will be paused at this point, so you can do whatever you want
end

-- shows an example of how to generate a new game file
local function gen_file()
    local newgame = require("core.scripts.newgame")
    local farmerName = "abc"
    local farmName = "abc"
    local favThing = "abc"
    local filePrefix = "example"
    local seed = 0 -- 0-2^31-1
    local language = "en" -- en, fr, es, de, pt, ru, ja, zh
    newgame.set_params(farmerName, farmName, favThing, filePrefix, seed, language)
    newgame.run()
end


-- this a direct reference to the TAS controller object, and you can interact with it as if it was C#
if Controller.State.Prefix ~= "example" then
    if save_state_exists("example") then
        fload("example")
    else
        gen_file() -- create a new game
        launch() -- launch into a specific state
        save() -- save the game at this point
    end
end

-- run some additional configuration to make things how you want on boot
exec("loadengine default") -- can swap to whatever engine state you like
exec("overlay off Layers") -- run standard commands for toggling on/off certain features

-- I hate typing so I'm going to make a functions that prints something I want to see
-- I make these aliases pretty regularly for things I use often. even better 
function rt()
    print(real_time())
end