---@diagnostic disable: lowercase-global, undefined-global
--- NOTE: this file pollutes the global namespace with common named elements
local console = require('core.console')

---swaps between different view states (base, map)
function view()
    console.exec("view")
end

---save the current TAS inputs to the current file
function save()
    console.exec("save")
end

---run the specified command on the top level console
---@param command string command to execute
function exec(command)
    console.exec(command)
end

