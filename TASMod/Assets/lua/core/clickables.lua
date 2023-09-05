---@diagnostic disable: undefined-global

local intro_click_map = require('core.data.click_maps')
local clickables = {}

function clickables.get_object_by_name(name)
    local items = interface:GetClickableObjects()
    if intro_click_map[name] ~= nil then
        name = intro_click_map[name]
    end
    for i=0,items.Count-1
    do
        if items[i].Name == name then
            return items[i]
        elseif items[i].ID == name then
            return items[i]
        end
    end
    return nil
end

function clickables.has_object(name)
    return clickables.get_object_by_name(name) ~= nil
end

function clickables.list()
    print_list(interface:GetClickableObjects(), (
        function(x)
            return x.Name
        end
    ))
end

return clickables