--- this file pollutes the global namespace with utility functions

---
---computes the length of an object
---  tables is direct lookup,
---  all other objects are via iteration
---
---@param dict c# dictionary
---@returns int length
function len(obj)
    if type(obj) == "table" then
        return #obj
    end
    local count = 0
    for _ in pairs(obj) do count = count + 1 end
    return count
end

---
--- generator for key/value pairs from a dictionary
---
---@param dict c# dictionary
---@returns key,value
function dict_items(dict)
    local x = dict.Keys:GetEnumerator()
    return function(dict)
        while x:MoveNext() do
            return x.Current, dict[x.Current]
        end
    end, dict, nil
end

function print_dict(dict, itemfunc, keyfunc)
    if itemfunc == nil then
        itemfunc = function(item) return item end
    end
    if keyfunc == nil then
        keyfunc = function(key) return key end
    end
    for k, v in dict_items(dict) do
        print("[" .. tostring(keyfunc(k)) .. "]: " .. tostring(itemfunc(v)))
    end
end

function print_vec_dict(dict, itemfunc)
    print_dict(dict, itemfunc, function(k) return concat(k.X, k.Y) end)
end

function list_items(obj)
    local x = obj:GetEnumerator()
    return function(dict)
        while x:MoveNext() do
            return x.Current
        end
    end, obj, nil
end

function print_list(obj, itemfunc)
    local x = 0
    if itemfunc == nil then
        itemfunc = function(item) return item end
    end
    for v in list_items(obj) do
        print("[" .. x .. "]: " .. tostring(itemfunc(v)))
        x = x + 1
    end
end

---
--- allow inline definition of ternary operations
--- if a function is passed, it will be lazy evaluated (only if necessary)
---
---@param cond boolean
---@param T any
---@param F any
---@returns any
function where(cond, T, F)
    if type(T) == "function" then
        if type(F) == "function" then
            if cond then return T() else return F() end
        else
            if cond then return T() else return F end
        end
    end
    if type(F) == "function" then
        if cond then return T else return F() end
    end
    if cond then return T else return F end
end

---
--- clamp a value between min and max
---
---@params x number
---@params min number
---@params max number
---@returns number
function clamp(x, min, max)
    if x < min then
        return min
    elseif x > max then
        return max
    end
    return x
end


--- computes element index of item in list
function indexof(li, i)
    if i == nil then
        return nil
    end
    for k, v in pairs(li) do
        if v == i then
            return k
        end
    end
    return nil
end

--- duplicates table
function copytable(list)
    local n = len(list)
    local v = {}
    for i = 1, n do
        v[i] = list[i]
    end
    return v
end

function concat(...)
    local arg = { ... }
    local result = ""
    for i, v in ipairs(arg) do
        result = result .. v
        if i < len(arg) then
            result = result .. "\t"
        end
    end
    return result
end

function string.split(inputstr, sep)
    if sep == nil then
        sep = "%s"
    end
    local t = {}
    for str in string.gmatch(inputstr, "([^" .. sep .. "]+)") do
        table.insert(t, str)
    end
    return t
end

function string.join(delim, items)
    if type(items) == 'table' then
        return table.concat(items, delim)
    end
    local text = ""
    for item in list_items(items) do
        if text == "" then
            text = item
        else
            text = text .. delim .. item
        end
    end
    return text
end

function list_filter(items, filters)
    local t = {}
    if type(items) == 'table' then
        for i, item in ipairs(items) do
            if filters[item] == nil then
                table.insert(t, item)
            end
        end
    else
        -- handle List<string>
        for item in list_items(items) do
            if filters[item] == nil then
                table.insert(t, item)
            end
        end
    end
    return t
end

function print_arr(x)
    local n = x.Length
    local res = {}

    for i = 0, n - 1 do
        table.insert(res, x[i])
    end
    return table.concat(res, ',')
end