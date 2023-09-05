---@diagnostic disable: undefined-global, undefined-field
local newgame = {
    farmer_name = "abc",
    farm_name = "abc",
    favorite_thing = "abc",
    prefix = nil,
    seed = 0,
    language = "en",
}

---set the params for the new game
---@param farmer_name string @farmer name
---@param farm_name string @farm name
---@param favorite_thing string @favorite thing
---@param prefix string @save file prefix
---@param seed number @game seed value: 0-2^31-1
---@param language string @game language: en, fr, es, de, pt, ru, ja, zh
function newgame.set_params(farmer_name, farm_name, favorite_thing, prefix, seed, language)
    if farmer_name ~= nil then
        newgame.farmer_name = farmer_name
    end
    if farm_name ~= nil then
        newgame.farm_name = farm_name
    end
    if favorite_thing ~= nil then
        newgame.favorite_thing = favorite_thing
    end
    if prefix ~= nil then
        newgame.prefix = prefix
    end
    if seed ~= nil then
        newgame.seed = seed
    end

    if language ~= nil then 
        if language == "en" or language == "fr" or language == "es" or language == "de" or language == "pt" or language == "ru" or language == "ja" or language == "zh" then
            newgame.language = language
        else
            print("invalid language code, defaulting to en")
        end
    end
end

---set the farmer name
---@param farmer_name string @farmer name
function newgame.set_farmer_name(farmer_name)
    if farmer_name ~= nil then
        newgame.farmer_name = farmer_name
    end
end

---set the farm name
---@param farm_name string @farm name
function newgame.set_farm_name(farm_name)
    if farm_name ~= nil then
        newgame.farm_name = farm_name
    end
end

---set the favorite thing
---@param favorite_thing string @favorite thing
function newgame.set_favorite_thing(favorite_thing)
    if favorite_thing ~= nil then
        newgame.favorite_thing = favorite_thing
    end
end

---set the save file prefix
---@param prefix string @save file prefix
function newgame.set_prefix(prefix)
    if prefix ~= nil then
        newgame.prefix = prefix
    end
end

---set the game seed
---@param seed number @game seed value: 0-2^31-1
function newgame.set_seed(seed)
    if seed ~= nil then
        newgame.seed = seed
    end
end

---set the game language
---@param language string @game language: en, fr, es, de, pt, ru, ja, zh
function newgame.set_language(language)
    local options = {"en", "fr", "es", "de", "pt", "ru", "ja", "zh"}
    if indexof(options, language) ~= nil then
        newgame.language = language
    else
        print("invalid language code, defaulting to en")
    end
end


---generate a new game
function newgame.run()
    if newgame.prefix == nil then
        newgame.prefix = string.random(4)
        print("no prefix specified, defaulting to "..newgame.prefix)
    else
        print("prefix: "..newgame.prefix)
    end
    Controller.State.Prefix = newgame.prefix

    if newgame.seed == nil then
        newgame.seed = math.random(0, 2147483647)
        print("no seed specified, defaulting to "..newgame.seed)
    else
        print("seed: "..newgame.seed)
    end
    Controller.State.Seed = newgame.seed

    if newgame.language == nil then
        newgame.language = "en"
        print("no language specified, defaulting to "..newgame.language)
    else
        print("language: "..newgame.language)
    end
    Controller.State.Language = newgame.language

    if newgame.farmer_name == nil then
        newgame.farmer_name = "abc"
        print("no farmer name specified, defaulting to "..newgame.farmer_name)
    else
        print("farmer name: "..newgame.farmer_name)
    end
    Controller.State.FarmerName = newgame.farmer_name

    if newgame.farm_name == nil then
        newgame.farm_name = "abc"
        print("no farm name specified, defaulting to "..newgame.farm_name)
    else
        print("farm name: "..newgame.farm_name)
    end
    Controller.State.FarmName = newgame.farm_name

    if newgame.favorite_thing == nil then
        newgame.favorite_thing = "abc"
        print("no favorite thing specified, defaulting to "..newgame.favorite_thing)
    else
        print("favorite thing: "..newgame.favorite_thing)
    end
    Controller.State.FavoriteThing = newgame.favorite_thing
    advance()
    advance()
    freset(0)
end

return newgame