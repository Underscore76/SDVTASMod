using System;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using TASMod.System;

namespace TASMod.Patches
{
    public class DateTime_Now : IPatch
    {
        public override string Name => "DateTime.Now";

        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Property(typeof(DateTime), nameof(DateTime.Now)).GetMethod,
                transpiler: new HarmonyMethod(this.GetType(), nameof(this.Transpiler))
                );
        }
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            return new CodeInstruction[] {
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DateTime_Now), nameof(NowOverride))),
                new CodeInstruction(OpCodes.Ret)
                };
        }
        public static DateTime NowOverride()
        {
            return TASDateTime.Now;
        }
    }

    public class DateTime_UtcNow : IPatch
    {
        public override string Name => "DateTime.UtcNow";

        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Property(typeof(DateTime), nameof(DateTime.UtcNow)).GetMethod,
                transpiler: new HarmonyMethod(this.GetType(), nameof(this.Transpiler))
                );
        }
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            return new CodeInstruction[] {
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DateTime_UtcNow), nameof(UtcNowOverride))),
                new CodeInstruction(OpCodes.Ret)
                };
        }
        public static DateTime UtcNowOverride()
        {
            return TASDateTime.UtcNow;
        }
    }
}

