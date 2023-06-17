using System;
using System.Collections.Generic;
using HarmonyLib;
using StardewValley;
using Microsoft.Xna.Framework;

namespace TASMod.Patches
{
    // Speedup overnight computation by caching NPC path finding results
    public class NPC_pathfindToNextScheduleLocation : IPatch
    {
        public override string Name => "NPC.pathfindToNextScheduleLocation";

        public override void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), "pathfindToNextScheduleLocation"),
                prefix: new HarmonyMethod(this.GetType(), nameof(this.Prefix)),
                postfix: new HarmonyMethod(this.GetType(), nameof(this.Postfix))
                );
        }
       
        public static bool Prefix(string startingLocation, int startingX, int startingY, string endingLocation, int endingX, int endingY, out Stack<Point> __state)
        {
            __state = NPC_PathFindCache.CheckCache(startingLocation, startingX, startingY, endingLocation, endingX, endingY);
            return __state == null;
        }
        public static void Postfix(ref SchedulePathDescription __result, Stack<Point> __state, string startingLocation, int startingX, int startingY, string endingLocation, int endingX, int endingY, int finalFacingDirection, string endBehavior, string endMessage)
        {
            if (__state != null)
            {
                __result = new SchedulePathDescription(__state, finalFacingDirection, endBehavior, endMessage);
            } else
            {
                NPC_PathFindCache.AddToCache(startingLocation, startingX, startingY, endingLocation, endingX, endingY, __result.route);
            }
        }
    }

    public static class NPC_PathFindCache
    {
        public static Dictionary<string, Stack<Point>> PathCache;
        static NPC_PathFindCache()
        {
            PathCache = new Dictionary<string, Stack<Point>>();
        }

        // laziest hash key ever but it works
        public static string CacheKey(string startingLocation, int startingX, int startingY, string endingLocation, int endingX, int endingY)
        {
            return string.Format("{0}:{1}:{2}:{3}:{4}:{5}", startingLocation, startingX, startingY, endingLocation, endingX, endingY);
        }

        public static Stack<Point> CheckCache(string startingLocation, int startingX, int startingY, string endingLocation, int endingX, int endingY)
        {
            var key = CacheKey(startingLocation, startingX, startingY, endingLocation, endingX, endingY);
            if (PathCache.ContainsKey(key))
            {
                // will get reversed out of the cache into the correct order
                return new Stack<Point>(PathCache[key]);
            }
            return null;
        }

        public static bool ContainsKey(string startingLocation, int startingX, int startingY, string endingLocation, int endingX, int endingY)
        {
            var key = CacheKey(startingLocation, startingX, startingY, endingLocation, endingX, endingY);
            return PathCache.ContainsKey(key);
        }

        public static void AddToCache(string startingLocation, int startingX, int startingY, string endingLocation, int endingX, int endingY, Stack<Point> path)
        {
            var key = CacheKey(startingLocation, startingX, startingY, endingLocation, endingX, endingY);
            // store them in the cache backwards to simplify the pull later
            PathCache.Add(key, new Stack<Point>(path));
        }
    }
}

