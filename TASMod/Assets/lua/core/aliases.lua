--- this file pollutes the global namespace with common named elements
local engine = require('core.engine')
local console = require('core.console')

function advance(input)
    engine.advance(input)
end

function reset(frame)
    engine.reset(frame)
end

function halt(max_frames)
    engine.halt(max_frames)
end

function freset(frame)
    engine.fast_reset(frame)
end

function breset(frame)
    engine.blocking_reset(frame)
end

function bfreset(frame)
    engine.blocking_fast_reset(frame)
end

function save()
    engine.save()
end

function exec(command)
    console.exec(command)
end