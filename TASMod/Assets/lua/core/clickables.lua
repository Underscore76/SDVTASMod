-- clickables: a module for finding clickable objects in menus

local intro_click_map = require('core.data.click_maps')
local clickables = {}

---get a clickable object by name
---@param name string @the name of the object to get
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

---check if a clickable object exists by name
---@param name string @the name of the object to check
function clickables.has_object(name)
    return clickables.get_object_by_name(name) ~= nil
end

---list all clickable objects on screen
function clickables.list()
    print_list(interface:GetClickableObjects(), (
        function(x)
            return x.Name
        end
    ))
end

return clickables