---@diagnostic disable: undefined-global, lowercase-global

local keybinds = {}
---add a key to the keybind map
---@param key Microsoft.Xna.Framework.Input.Keys @key to bind
---@param func function @function to call when key is pressed
---@param name string|nil @name of the keybind
function keybinds.add(key, func, name)
    if name == nil then
        name = "user-defined function"
    end

    interface:AddKeyBind(key, name, func)
end

---remove a key from the keybind map
---@param key Microsoft.Xna.Framework.Input.Keys @key to remove
function keybinds.remove(key)
    interface:RemoveKeyBind(key)
end

---print the keybind map
function keybinds.print()
    interface:PrintKeyBinds()
end

---clear the keybind map
function keybinds.clear()
    interface:ClearKeyBinds()
end

return keybinds