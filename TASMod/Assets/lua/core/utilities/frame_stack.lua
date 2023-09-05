-- Desc: frame stack utility
-- defines a utility stack class for working with frames
-- allows pushing and popping frames from the stack
-- and fast resetting the game to the frame on top of the stack

local fs = {
    _X = {}
}

---get the last frame on the stack
---@return number last frame number
function fs.last()
    return fs._X[#fs._X]
end

---print the frame stack
function fs.print()
    print(fs._X)
end

---push a frame to the stack
---@param f number|nil frame to push (defaults current frame)
---@return number pushed frame number added to the stack
function fs.push(f)
    if f == nil then
        f = interface:GetCurrentFrame()
    end
    if fs.last() ~= f then
        table.insert(fs._X, f)
    end
    return f
end

---pop a frame from the stack
---@return number|nil popped frame number removed from the stack
function fs.pop()
    if fs.last() == nil then
        return nil
    end
    return table.remove(fs._X)
end

---clear the frame stack
function fs.clear()
    fs._X = {}
end

---fast reset the game to the last frame on the stack (NON-BLOCKING)
function fs.rw()
    if fs.last() ~= nil then
        freset(fs.last())
    end
end

---fast reset the game to the last frame on the stack (BLOCKING)
function fs.brw()
    if fs.last() ~= nil then
        bfreset(fs.last())
    end
end

return fs