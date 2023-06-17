using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TASMod.Extensions;

namespace TASMod.Patches
{
	public class Cue_Play : IPatch
	{
        public override string Name => "Cue.Play";

        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method("Microsoft.Xna.Framework.Audio.Cue:Play"),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix))
            );
        }

        public static bool Prefix()
        {
            FixXactRandom();
            return true;
        }

        // extract the XactHelper's Random so that we can pin it
        private static FieldInfo XactHelper_Random = null;
        public static Random GetXactRandom()
        {
            if (XactHelper_Random == null)
                _FindXactRandom();
            return (Random)XactHelper_Random.GetValue(null);
        }
        public static void FixXactRandom()
        {
            if (XactHelper_Random == null)
                _FindXactRandom();
            Random xactRandom = GetXactRandom();
            Random blankRandom = new Random(0);
            blankRandom.CloneOver(xactRandom);
        }

        private static void _FindXactRandom()
        {
            if (XactHelper_Random == null)
            {
                List<Type> types = new List<Type>();
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly assembly in assemblies)
                {
                    if (assembly.GetName().Name!.StartsWith("MonoGame.Framework"))
                    {

                        Type[] types2 = assembly.GetTypes();
                        foreach (Type type in types2)
                        {
                            if (type.FullName == "Microsoft.Xna.Framework.Audio.XactHelpers")
                            {

                                XactHelper_Random = type.GetField("Random", BindingFlags.Static | BindingFlags.NonPublic);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}

