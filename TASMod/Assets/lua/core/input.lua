-- `input`: defines some basic click functions for interacting with the game

local clickables = require("core.clickables")

local input = {}

---click a point on the screen
---@param point table @a table with X and Y fields
---@param left boolean @true if you want to left click
---@param right boolean @true if you want to right click
function input.click_point(point, left, right)
    local mouse = {
        X=point.X,
        Y=point.Y,
        left=false,
        right=false
    }
    local last_mouse = Controller.LastFrameMouse()
    if last_mouse.X ~= mouse.X or last_mouse.Y ~= mouse.Y  or (last_mouse.LeftMouseClicked and left) or (last_mouse.RightMouseClicked and right) then
        advance({mouse=mouse})
    end
    mouse.left=left
    mouse.right=right
    advance({mouse=mouse})
end

---click the center of a rectangle
---@param rect table|Microsoft.Xna.Framework.Rectangle @a table with Center field or a Rectangle object
function input.click_rect(rect)
    input.click_point(rect.Center, true, false)
end

---click a component by name
---@param name string @the name of the component to click
function input.click_component(name)
    local component = clickables.get_object_by_name(name)
    if component == nil then
        print("Component not found: " .. name)
    else
        input.click_rect(component.Rect)
    end
end

---click a slider component by name
---@param name string @the name of the component to click
---@param value number @the value to set the slider to (0-100)
function input.click_slider_component(name, value)
    local obj = clickables.get_object_by_name(name)
    if obj ~= nil then
        input.click_horizontal_slider(obj.Rect, value)
    end
end

---click a horizontal slider
---@param rect table|Microsoft.Xna.Framework.Rectangle @a table with X, Y, Width, and Center fields or a Rectangle object
---@param value number @the value to set the slider to (0-100)
function input.click_horizontal_slider(rect, value)
    local point = {
        X = rect.X + rect.Width * value / 100,
        Y = rect.Center.Y
    }
    input.click_point(point, true, false)
end

return input