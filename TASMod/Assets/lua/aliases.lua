--- NOTE: this file pollutes the global namespace with common named elements
local console = require('core.console')

---swaps between different view states (base, map)
function view()
    console.exec("view")
end

---save the current TAS inputs to the current file
---@param name string|nil @name of file to save to (default: nil)
function save(name)
    if name ~= nil then
        saveas(name)
        return
    end
    console.exec("save")
end

---save the current TAS inputs to the specified file
---@param name string @name of file to save to
function saveas(name)
    if name == nil then
        print("ERROR: saveas requires a filename")
        return
    end
    console.exec("saveas " .. name)
end

---load a TAS input file
---@param name string @name of file to load
function load(name)
    if name == nil then
        print("ERROR: load requires a filename")
        return
    end
    if not save_state_exists(name) then
        print("ERROR: save state does not exist")
        return
    end
    console.exec("load " .. name)
end

---fast load a TAS input file
---@param name string @name of file to load
function fload(name)
    if name == nil then
        print("ERROR: fload requires a filename")
        return
    end
    if not save_state_exists(name) then
        print("ERROR: save state does not exist")
        return
    end
    console.exec("fload " .. name)
end

---run the specified command on the top level console
---@param command string command to execute
function exec(command)
    console.exec(command)
end

---check if a save state exists
---@param name string @name of the save state
---@return boolean @whether the save state exists
function save_state_exists(name)
    if name == nil then
        name = Controller.State.Prefix
    end
    local items = Controller.Console.Commands['ls']:GetFilePaths(RunCS(string.format('new string[]{"%s.json"}',name)))
    return items.Count == 1
end