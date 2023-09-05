--defines wrapper object around the TAS console

local console = {}

---closes the console
function console.close()
    Controller.Console.openHeightTarget = 0
end

---opens the console
function console.open()
    Controller.Console.openHeightTarget = 0.5
end

---plays back a function with the console closed
function console.playback(func)
    Controller.Console.Close()
    func()
    Controller.Console.Open()
end

---executes a TAS console command. this is not the same as the SDV debug commands
---see docs for a list of commands or try `help` or `list`
function console.exec(command)
    Controller.Console:RunCommand(command)
end

return console