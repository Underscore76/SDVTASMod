using System;
using System.Collections.Generic;

namespace TASMod.Recording
{
    public class EngineState
    {
        public Dictionary<string, string> Aliases;
        public Dictionary<string, bool> OverlayState;
        public Dictionary<string, bool> LogicState;

        public EngineState()
        {
            Aliases = new Dictionary<string, string>(Controller.Console.Aliases);
            OverlayState = new Dictionary<string, bool>();
            foreach (var overlay in Controller.Overlays)
            {
                OverlayState.Add(overlay.Key, overlay.Value.Active);
            }
            LogicState = new Dictionary<string, bool>();
            foreach (var logic in Controller.Logics)
            {
                LogicState.Add(logic.Key, logic.Value.Active);
            }
        }

        public void UpdateGame()
        {
            Controller.Console.Aliases = new Dictionary<string, string>(Aliases);
            foreach (var overlay in OverlayState)
            {
                if (Controller.Overlays.ContainsKey(overlay.Key))
                    Controller.Overlays[overlay.Key].Active = overlay.Value;
            }
            foreach (var logic in LogicState)
            {
                if (Controller.Logics.ContainsKey(logic.Key))
                    Controller.Logics[logic.Key].Active = logic.Value;
            }
        }
    }
}

