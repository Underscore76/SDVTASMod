using System;
using System.Collections.Generic;
using System.Diagnostics;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using TASMod.LuaScripting;

namespace TASMod.Helpers
{
    public class Seeding
    {
        public static int TotalSize = 1 << 16;
        public static int BlockSize = 1 << 10;
        public static int EvalDay = 29;

        public static bool Eval(int seed, int day)
        {
            VolcanoDungeon dungeon = ScriptInterface._instance.SpawnDungeonMinimal(1, (uint)day, (uint)seed);
            int nuts = 0;
            bool validChest = false;
            foreach (var kvp in dungeon.Objects.Pairs)
            {
                if (kvp.Value is BreakableContainer)
                {
                    Random r = new Random((int)(kvp.Key.X + kvp.Key.Y * 10000 + day));
                    if (r.NextDouble() < 0.03)
                    {
                        nuts++;
                    }
                }
                else if (kvp.Value is Chest chest)
                {
                    foreach (var item in chest.items)
                    {
                        if (item.Name == "Dwarf Hammer" || item.Name == "Dragontooth Club")
                        {
                            validChest = true;
                        }
                    }
                }
                else if (kvp.Value.Name == "Stone")
                {
                    Random r = new Random(day + seed / 2 + (int)(kvp.Key.X * 4000 + kvp.Key.Y));
                    if (r.NextDouble() < 0.03)
                    {
                        nuts++;
                    }
                }
            }
            Controller.Console.PushResult($"{seed}\t{day}\t{(validChest ? 1 : 0)}\t{nuts}");
            LocalizedContentManager mapContent = (LocalizedContentManager)Reflector.GetValue(dungeon, "mapContent");
            dungeon.CleanUp();
            mapContent.Dispose();
            dungeon.temporarySprites.Clear();
            return validChest && nuts >= 3;
        }

        public static void Run(int block)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            Controller.Console.Clear();
            int first = block * TotalSize;
            int last = first + TotalSize;
            for(int seed=first; seed < last; seed += BlockSize)
            {
                var t0 = stopwatch.Elapsed;
                for(int offset=0; offset < BlockSize; offset+=2)
                {
                    Eval(seed+offset, EvalDay);
                }
                ScriptInterface._instance.AdvanceFrame(null);
                var t1 = stopwatch.Elapsed;
                ModEntry.Console.Log($"Finished {seed}->{seed+BlockSize} in {(t1-t0).TotalMilliseconds}");
                Controller.Console.WriteToFile($"{first}_{last}");
                GC.Collect();
            }
        }
    }
}
