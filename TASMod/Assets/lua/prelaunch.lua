-- file is run at generation of the lua state to setup core library overrides

__old_print__ = print
__old_require__ = require

---
---dump a lua object to the in-game console
---
---@param dump object
function dump(obj)
    function isobj(v)
        return type(v) == "table"
    end

    function offset(depth)
        local s = ""
        for i = 1, depth do
            s = s .. "\t"
        end
        return s
    end

    function anon(obj, depth)
        local s = offset(depth)
        for k, v in pairs(obj) do
            if isobj(v) then
                interface:Print(s .. " " .. k)
                anon(v, depth + 1)
            else
                interface:Print(s .. " " .. k .. " " .. tostring(v))
            end
        end
    end

    if isobj(obj) then
        anon(obj, 0)
    else
        interface:Print(tostring(obj))
    end
end

---
---variadic function to dump a set of items to the console
---
---@param ... set of objects to dump
print = function(...)
    dump(...)
end

---
---print contents based on a format string specification
---
---@param format_str format string to input values into
---@param ... set of objects to dump
printf = function(format_str, ...)
    print(string.format(format_str, ...))
end

---
---load additional module by name, overridden to allow reloading
---
---@param label string
require = function(label)
    if label == 'prelaunch' then
        print('ERROR: cannot reload prelaunch')
        return nil
    end
    if package.loaded[label] ~= nil then
        package.loaded[label] = nil
    end

    return __old_require__(label)
end

require('core.utils')
require('core.aliases')