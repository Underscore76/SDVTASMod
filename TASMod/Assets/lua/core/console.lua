local console = {}
function console.close()
    Controller.Console.openHeightTarget = 0
end

function console.open()
    Controller.Console.openHeightTarget = 0.5
end

function console.playback(func)
    Controller.Console.Close()
    func()
    Controller.Console.Open()
end

function console.exec(command)
    Controller.Console:RunCommand(command)
end

return console