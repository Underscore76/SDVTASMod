---@diagnostic disable: lowercase-global, undefined-field
--- NOTE: this file pollutes the global namespace with utility functions


---computes the length of an object.
---  for tables this is via direct lookup,
---  all other objects are via iteration.
---@param obj any
---@returns int length
function len(obj)
    if type(obj) == "table" then
        return #obj
    end
    local count = 0
    for _ in pairs(obj) do count = count + 1 end
    return count
end

--- generator for key/value pairs from a dictionary
---@param dict any @dictionary to iterate over
---@return function,table,nil data key/value pairs to iterate over
function dict_items(dict)
    local x = dict.Keys:GetEnumerator()
    return function(dict)
        while x:MoveNext() do
            return x.Current, dict[x.Current]
        end
    end, dict, nil
end

---print key/value pairs from a dictionary
---@param dict any @dictionary to iterate over
---@param itemfunc function|nil @modifier function to apply to each item (defaults to nil)
---@param keyfunc function|nil @modifier function to apply to each key (defaults to nil)
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

---print a dictionary with vector keys
---@param dict any @dictionary to iterate over
---@param itemfunc function|nil @modifier function to apply to each item (defaults to nil)
function print_vec_dict(dict, itemfunc)
    print_dict(dict, itemfunc, function(k) return concat(k.X, k.Y) end)
end

---generator for items from a list
---@param obj any @list to iterate over
---@return function,table,nil data items to iterate over
function list_items(obj)
    local x = obj:GetEnumerator()
    local i = 0
    return function(dict)
        while x:MoveNext() do
            local v = i
            i = i+1
            return v, x.Current
        end
    end, obj, nil
end

---print items from a list
---@param obj any @list to iterate over
---@param itemfunc function|nil @modifier function to apply to each item (defaults to nil)
function print_list(obj, itemfunc)
    if itemfunc == nil then
        itemfunc = function(item) return item end
    end
    for i, v in list_items(obj) do
        print("[" .. i .. "]: " .. tostring(itemfunc(v)))
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


--- computes element index of item in table
---@param list table @table to search
---@param item any @item to search for
function indexof(list, item)
    if item == nil then
        return nil
    end
    for k, v in pairs(list) do
        if v == item then
            return k
        end
    end
    return nil
end

---duplicates table
---@param list table @table to copy
---@return table @copy of table
function copytable(list)
    local n = len(list)
    local v = {}
    for i = 1, n do
        v[i] = list[i]
    end
    return v
end

---concatenates a list of strings together with tabs
---@vararg string @list of strings to concatenate
---@return string @concatenated string
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

---split a string into a table based on a separator
---@param inputstr string @string to split
---@param sep string @separator to split on
---@return table @split string
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

---join a bunch of items together with a delimiter
---@param delim string @delimiter to join with
---@param items table|string @items to join
---@return string @joined string
function string.join(delim, items)
    if type(items) == 'table' then
        return table.concat(items, delim)
    end
    local text = ""
    for _,item in list_items(items) do
        if text == "" then
            text = item
        else
            text = text .. delim .. item
        end
    end
    return text
end

---filter a list of items based on if they exist in a table
---@param items table|string @items to filter
---@param filters table @filters to apply
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
        for _,item in list_items(items) do
            if filters[item] == nil then
                table.insert(t, item)
            end
        end
    end
    return t
end

---print a c# array
---@param x any @array to print
---@return string @string representation of array
function print_arr(x)
    local n = x.Length
    local res = {}

    for i = 0, n - 1 do
        table.insert(res, x[i])
    end
    return table.concat(res, ',')
end


---generates a random string of a given length
---taken from https://gist.github.com/haggen/2fd643ea9a261fea2094
---@param length number @length of string to generate
---@return string|unknown @random string
function string.random(length)
    local charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"
    if length > 0 then
        return string.random(length - 1) .. charset:sub(math.random(1, #charset), 1)
    else
        return ""
    end
end