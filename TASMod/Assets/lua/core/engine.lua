local console = require('core.console')
local engine = {}
function engine.halt(max_frames)
    if max_frames == nil then
        max_frames = 100
    end
    local i = 0
    while interface.HasStep and i < max_frames do
        i = i + 1
        interface:StepLogic()
    end
end

function engine.advance(input)
    console.close()
    interface:AdvanceFrame(input)
    engine.halt()
    console.open()
end

function engine.step_logic()
    console.close()
    interface:StepLogic()
    console.open()
end

function engine.reset(frame)
    console.close()
    if frame == nil then
        frame = -1
    end
    interface:ResetGame(frame)
end

function engine.fast_reset(frame)
    console.close()
    if frame == nil then
        frame = -1
    end
    interface:FastResetGame(frame)
end

function engine.save()
    console.exec("save")
end

function engine.blocking_reset(frame)
    if frame == nil then
        frame = -1
    end
    interface:BlockResetGame(frame)
end

function engine.blocking_fast_reset(frame)
    if frame == nil then
        frame = -1
    end
    interface:BlockFastResetGame(frame)
end

return engine
