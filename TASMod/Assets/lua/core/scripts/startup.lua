-- script to startup a new game based on your character customization

local clickables = require("core.clickables")
local input = require("core.input")

--- load defaults
local startup = {
    pet={true, 0},
    skin=0,
    hair=0,
    shirt=0,
    pant=0,
    acc=0,
    farm="Standard",
    sex="male",
    skip=true,
    eye_color={h=nil,s=nil,v=nil},
    hair_color={h=nil,s=nil,v=nil},
    pants_color={h=nil,s=nil,v=nil},
}

local function select(func, desire, name)
    local f = desire
    local curr = func()
    local itr = 0
    local component = clickables.get_object_by_name(name)
    if component == nil then
        error("Component not found: " .. name)
        return
    end
    while curr ~= f do
        input.click_point(component.Rect.Center, true, false)
        curr = func()
        itr = itr + 1
        if itr > 300 then
            print('Breaking...')
            break
        end
    end
end

---setup your pet selection
---@param pick_cat boolean @true if you want to pick a cat, false if you want to pick a dog
---@param variant number @the variant of the pet you want to pick
function startup.set_pet(pick_cat, variant)
    startup.pet = {pick_cat, variant}
end

---setup your hair selection
---@param variant number @the variant of the hair you want to pick
function startup.set_hair(variant)
    startup.hair = variant
end

---setup your accessory selection
---@param variant number @the variant of the accessory you want to pick
function startup.set_acc(variant)
    startup.acc = variant
end

---setup your shirt selection
---@param variant number @the variant of the shirt you want to pick
function startup.set_shirt(variant)
    startup.shirt = variant
end

---setup your skin selection
---@param variant number @the variant of the skin you want to pick
function startup.set_skin(variant)
    startup.skin = variant
end

---setup your pants selection
---@param variant number @the variant of the pants you want to pick
function startup.set_pant(variant)
    startup.pant = variant
end


---setup your eye color
---@param h number|nil @first value of the hsv color
---@param s number|nil @second value of the hsv color
---@param v number|nil @third value of the hsv color
function startup.set_eye_color(h,s,v)
    startup.eye_color = {h=h,s=s,v=v}
end
---setup your hair color
---@param h number|nil @first value of the hsv color
---@param s number|nil @second value of the hsv color
---@param v number|nil @third value of the hsv color
function startup.set_hair_color(h,s,v)
    startup.hair_color = {h=h,s=s,v=v}
end
---setup your pants color
---@param h number|nil @first value of the hsv color
---@param s number|nil @second value of the hsv color
---@param v number|nil @third value of the hsv color
function startup.set_pants_color(h,s,v)
    startup.pants_color = {h=h,s=s,v=v}
end

---setup your farm selection
---@param variant string @the name of the farm you want to pick
function startup.set_farm(variant)
    local options = {"Standard", "Riverland", "Forest", "Hills", "Wilderness", "FourCorners", "Beach"}
    if indexof(options, variant) == nil then
        print("Invalid farm type: " .. variant)
        return
    end
    startup.farm = variant
end

---setup character sex
---@param sex string @male or female
function startup.set_sex(sex)
    startup.sex = sex
end

---setup whether to skip the intro
---@param skip boolean @true if you want to skip the intro, false if you want to watch it
function startup.set_skip(skip)
    startup.skip = skip
end

--- wrappers for getting player values
local player = {}
function player.get_skin()
    return RunCS("Game1.player.skin.Value")
end
function player.get_hair()
    return RunCS("Game1.player.hair.Value")
end
function player.get_shirt()
    return RunCS("Game1.player.shirt.Value")
end
function player.get_pant()
    return RunCS("Game1.player.pants.Value")
end
function player.get_acc()
    return RunCS("Game1.player.accessory.Value")
end


---run the startup script
function startup.run()
    -- safety advance for MonoGame window initialization
    advance()
    advance()
    -- boot game
    bfreset(0)
    advance()
    advance({ keyboard = { Keys.Escape } })
    
    -- click to new
    if not clickables.has_object("New") then
        print("unable to find New button")
        return
    end
    input.click_component("New")
    
    -- wait for the menu to load
    local x = 0
    while not clickables.has_object("NameBox") and x < 400 do
        advance()
        x = x+1
    end
    if not clickables.has_object("NameBox") then
        print("Failed to reach character customization screen")
        return
    end

    -- click a component by name
    input.click_component("NameBox")
    input.click_component("FarmBox")
    input.click_component("FavBox")

    if startup.skip then
        input.click_component("SkipIntro")
    end

    if startup.farm ~= "Standard" then
        input.click_component(startup.farm)
    end 

    if startup.sex == "female" then
        input.click_component("Female")
    end


    local function click_dir(func, desired, n, left, right)
        local diff = n - ((func() - desired) % n)
        if diff > n/2 then
            return left
        else
            return right
        end
    end

    local skin_comp_name = click_dir(player.get_skin, startup.skin, 24, "Skin_Left", "Skin_Right")
    select(player.get_skin, startup.skin, skin_comp_name)
    local hair_comp_name = click_dir(player.get_hair, startup.hair, 74, "Hair_Left", "Hair_Right")
    select(player.get_hair, startup.hair, hair_comp_name)
    local shirt_comp_name = click_dir(player.get_shirt, startup.shirt, 111, "Shirt_Left", "Shirt_Right")
    select(player.get_shirt, startup.shirt, shirt_comp_name)
    local pant_comp_name = click_dir(player.get_pant, startup.pant, 3, "Pants Style_629", "Pants Style_517")
    select(player.get_pant, startup.pant, pant_comp_name)
    local acc_comp_name = click_dir(player.get_acc, startup.acc, 18, "Acc_Left", "Acc_Right")
    select(player.get_acc, startup.acc, acc_comp_name)

    local function change_colors(hsv, comp_name)
        if hsv.h ~= nil then
            input.click_slider_component(comp_name .. "_H", hsv.h)
        end
        if hsv.s ~= nil then
            input.click_slider_component(comp_name .. "_S", hsv.s)
        end
        if hsv.v ~= nil then
            input.click_slider_component(comp_name .. "_V", hsv.v)
        end
    end

    change_colors(startup.eye_color, "Eye")
    change_colors(startup.hair_color, "Hair")
    change_colors(startup.pants_color, "Pants")
end

---click the OK button
function startup.click_ok()
    input.click_component("OK")
end

return startup
