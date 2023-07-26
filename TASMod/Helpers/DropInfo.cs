using Netcode;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TASMod.Helpers
{
    public class DropInfo
    {
        public static DropInfo _instance;
        static DropInfo()
        {
            _instance = new DropInfo();
        }

        public static string ObjectName(int index)
        {
            if (!Game1.objectInformation.ContainsKey(index))
            {
                index = Math.Abs(index);
                switch (index)
                {
                    case 0:
                        index = 378;
                        break;
                    case 2:
                        index = 380;
                        break;
                    case 4:
                        index = 382;
                        break;
                    case 6:
                        index = 384;
                        break;
                    case 10:
                        index = 386;
                        break;
                    case 12:
                        index = 388;
                        break;
                    case 14:
                        index = 390;
                        break;
                    default:
                        return "unknown";
                }
            }
            return Game1.objectInformation[index].Split('/')[0];
        }
        public static string MultiDrop(int index, int num = 1)
        {
            string drop = ObjectName(index);
            if (num == 1)
                return drop;
            return string.Format("{0} x{1}", drop, num);
        }

        public static IEnumerable<string> OnStoneDestroyed(int indexOfStone, int x, int y)
        {
            List<string> drops = new List<string>();
            if (Game1.currentLocation is MineShaft mines)
            {
                drops.AddRange(CheckStoneForItems(mines, indexOfStone, x, y));
            }
            else
            {
                if (indexOfStone == 343 || indexOfStone == 450)
                {
                    Random random = new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2 + x * 2000 + y);
                    if (random.NextDouble() < 0.035 && Game1.stats.DaysPlayed > 1)
                    {
                        drops.Add(ObjectName(535 + (
                                    (Game1.stats.DaysPlayed > 60 && random.NextDouble() < 0.2) ? 1 :
                                    ((Game1.stats.DaysPlayed > 120 && random.NextDouble() < 0.2) ? 2 : 0))));
                    }
                    if (random.NextDouble() < 0.035 * (double)((!Game1.player.professions.Contains(21)) ? 1 : 2) && Game1.stats.DaysPlayed > 1)
                    {
                        drops.Add(ObjectName(382));
                    }
                    if (random.NextDouble() < 0.01 && Game1.stats.DaysPlayed > 1)
                    {
                        drops.Add(ObjectName(390));
                    }
                }
                drops.AddRange(BreakStone(indexOfStone, x, y, Game1.player, new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2 + x * 4000 + y)));
            }
            return drops;
        }

        // MineShaft::checkStoneForItems, not fixing any weirdness
        public static IEnumerable<string> CheckStoneForItems(MineShaft mine, int tileIndexOfStone, int x, int y)
        {
            double num = Game1.player.DailyLuck / 2.0 + (double)Game1.player.MiningLevel * 0.005 + (double)Game1.player.LuckLevel * 0.001;
            Random random = new Random(x * 1000 + y + mine.mineLevel + (int)Game1.uniqueIDForThisGame / 2);
            random.NextDouble();
            double num2 = (tileIndexOfStone == 40 || tileIndexOfStone == 42) ? 1.2 : 0.8;
            if (tileIndexOfStone != 34 && tileIndexOfStone != 36 && tileIndexOfStone != 50)
            {
                // WHY DOES THIS HAPPEN!?
                _ = 52;
            }

            int stonesLeftOnThisLevel = CurrentLocation.StonesLeftOnThisLevel() - 1;
            double num3 = 0.02 + 1.0 / (double)Math.Max(1, stonesLeftOnThisLevel) + (double)Game1.player.LuckLevel / 100.0 + Game1.player.DailyLuck / 5.0;
            if (mine.EnemyCount == 0)
            {
                num3 += 0.04;
            }
            if (!CurrentLocation.LadderHasSpawned() && !mine.mustKillAllMonstersToAdvance() && (stonesLeftOnThisLevel == 0 || random.NextDouble() < num3) && mine.shouldCreateLadderOnThisLevel())
            {
                // create the ladder
            }
            List<string> breakStone = new List<string>(BreakStone(tileIndexOfStone, x, y, Game1.player, random));
            if (breakStone.Count != 0)
            {
                return breakStone;
            }

            if (tileIndexOfStone == 44)
            {
                int num4 = random.Next(59, 70);
                num4 += num4 % 2;
                if (Game1.player.timesReachedMineBottom == 0)
                {
                    if (mine.mineLevel < 40 && num4 != 66 && num4 != 68)
                    {
                        num4 = ((random.NextDouble() < 0.5) ? 66 : 68);
                    }
                    else if (mine.mineLevel < 80 && (num4 == 64 || num4 == 60))
                    {
                        num4 = ((!(random.NextDouble() < 0.5)) ? ((random.NextDouble() < 0.5) ? 68 : 62) : ((random.NextDouble() < 0.5) ? 66 : 70));
                    }
                }
                breakStone.Add(ObjectName(num4));
                return breakStone.ToArray();
            }

            if (random.NextDouble() < 0.022 * (1.0 + num) * (double)((!Game1.player.professions.Contains(22)) ? 1 : 2))
            {
                int objectIndex = 535 + ((mine.getMineArea() == 40) ? 1 : ((mine.getMineArea() == 80) ? 2 : 0));
                if (mine.getMineArea() == 121)
                {
                    objectIndex = 749;
                }
                string drop = ObjectName(objectIndex);
                if (Game1.player.professions.Contains(19) && random.NextDouble() < 0.5)
                {
                    drop += " x2";
                }
                breakStone.Add(drop);
            }
            if (mine.mineLevel > 20 && random.NextDouble() < 0.005 * (1.0 + num) * (double)((!Game1.player.professions.Contains(22)) ? 1 : 2))
            {
                string drop = ObjectName(749);
                if (Game1.player.professions.Contains(19) && random.NextDouble() < 0.5)
                {
                    drop += " x2";
                }
                breakStone.Add(drop);
            }
            if (random.NextDouble() < 0.05 * (1.0 + num) * num2)
            {
                random.Next(1, 3);
                random.NextDouble();
                // WHY DOES THIS HAPPEN!?
                _ = 0.1 * (1.0 + num);
                if (random.NextDouble() < 0.25 * (double)((!Game1.player.professions.Contains(21)) ? 1 : 2))
                {
                    breakStone.Add(ObjectName(382));
                }
                breakStone.Add(ObjectName(mine.getOreIndexForLevel(mine.mineLevel, random)));
            }
            else if (random.NextDouble() < 0.5)
            {
                breakStone.Add(ObjectName(390));
            }
            return breakStone;
        }

        // GameLocation::BreakStone
        public static IEnumerable<string> BreakStone(int indexOfStone, int x, int y, Farmer who, Random r)
        {
            List<string> drops = new List<string>();
            int num = 0;
            int num2 = who.professions.Contains(18) ? 1 : 0;
            switch (indexOfStone)
            {
                case 75:
                    drops.Add(ObjectName(535));
                    break;
                case 76:
                    drops.Add(ObjectName(536));
                    break;
                case 77:
                    drops.Add(ObjectName(537));
                    break;
                case 8:
                    drops.Add(ObjectName(66));
                    break;
                case 10:
                    drops.Add(ObjectName(68));
                    break;
                case 12:
                    drops.Add(ObjectName(60));
                    break;
                case 14:
                    drops.Add(ObjectName(62));
                    break;
                case 6:
                    drops.Add(ObjectName(70));
                    break;
                case 4:
                    drops.Add(ObjectName(64));
                    break;
                case 2:
                    drops.Add(ObjectName(72));
                    break;
                case 668:
                case 670:
                    drops.Add(MultiDrop(390, num2 + r.Next(1, 3) + ((r.NextDouble() < (double)((float)who.LuckLevel / 100f)) ? 1 : 0) + ((r.NextDouble() < (double)((float)who.MiningLevel / 100f)) ? 1 : 0)));
                    num = 3;
                    if (r.NextDouble() < 0.08)
                    {
                        drops.Add(MultiDrop(382, 1 + num2));
                        num = 4;
                    }
                    break;
                case 751:
                    drops.Add(MultiDrop(378, num2 + r.Next(1, 4) + ((r.NextDouble() < (double)((float)who.LuckLevel / 100f)) ? 1 : 0) + ((r.NextDouble() < (double)((float)who.MiningLevel / 100f)) ? 1 : 0)));
                    break;
                case 290:
                    drops.Add(MultiDrop(380, num2 + r.Next(1, 4) + ((r.NextDouble() < (double)((float)who.LuckLevel / 100f)) ? 1 : 0) + ((r.NextDouble() < (double)((float)who.MiningLevel / 100f)) ? 1 : 0)));
                    break;
                case 764:
                    drops.Add(MultiDrop(384, num2 + r.Next(1, 4) + ((r.NextDouble() < (double)((float)who.LuckLevel / 100f)) ? 1 : 0) + ((r.NextDouble() < (double)((float)who.MiningLevel / 100f)) ? 1 : 0)));
                    break;
                case 765:
                    drops.Add(MultiDrop(386, num2 + r.Next(1, 4) + ((r.NextDouble() < (double)((float)who.LuckLevel / 100f)) ? 1 : 0) + ((r.NextDouble() < (double)((float)who.MiningLevel / 100f)) ? 1 : 0)));
                    if (r.NextDouble() < 0.04)
                    {
                        drops.Add(ObjectName(74));
                    }
                    break;
            }
            if (who.professions.Contains(19) && r.NextDouble() < 0.5)
            {
                switch (indexOfStone)
                {
                    case 8:
                        drops.Add(ObjectName(66));
                        break;
                    case 10:
                        drops.Add(ObjectName(68));
                        break;
                    case 12:
                        drops.Add(ObjectName(60));
                        break;
                    case 14:
                        drops.Add(ObjectName(62));
                        break;
                    case 6:
                        drops.Add(ObjectName(70));
                        break;
                    case 4:
                        drops.Add(ObjectName(64));
                        break;
                    case 2:
                        drops.Add(ObjectName(72));
                        break;
                }
            }
            if (indexOfStone == 46)
            {
                drops.Add(MultiDrop(386, r.Next(1, 4)));
                drops.Add(MultiDrop(384, r.Next(1, 5)));
                if (r.NextDouble() < 0.25)
                {
                    drops.Add(ObjectName(74));
                }
            }
            if (((bool)Game1.currentLocation.IsOutdoors || (bool)Game1.currentLocation.treatAsOutdoors.Value) && num == 0)
            {
                double num3 = who.DailyLuck / 2.0 + (double)who.MiningLevel * 0.005 + (double)who.LuckLevel * 0.001;
                Random random = new Random(x * 1000 + y + (int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2);
                drops.Add(ObjectName(390));
                if (who.professions.Contains(21) && random.NextDouble() < 0.05 * (1.0 + num3))
                {
                    drops.Add(ObjectName(382));
                }
                if (random.NextDouble() < 0.05 * (1.0 + num3))
                {
                    random.Next(1, 3);
                    random.NextDouble();
                    drops.Add(ObjectName(382));
                }
            }
            return drops;
        }


        public static IEnumerable<string> BreakContainer(Vector2 location, BreakableContainer container)
        {
            return BreakContainer(location, ContainerType(container));
        }
        public static IEnumerable<string> BreakContainer(Vector2 location, int containerType)
        {
            if (Game1.currentLocation is MineShaft mine)
            {
                int X = (int)location.X;
                int Y = (int)location.Y;
                int type = containerType;
                int level = mine.mineLevel;
                Random random = new Random(X + Y * 10000 + (int)Game1.stats.DaysPlayed);

                if (random.NextDouble() < 0.2)
                {
                    return new string[] { };
                }
                switch (type)
                {
                    case 118:
                        if (random.NextDouble() < 0.65)
                        {
                            if (random.NextDouble() < 0.8)
                            {
                                switch (random.Next(9))
                                {
                                    case 0:
                                        return new string[] { ObjectName(382) };
                                    case 1:
                                        return new string[] { ObjectName(378) };
                                    case 5:
                                        return new string[] { ObjectName(92) };
                                    case 4:
                                    case 6:
                                        return new string[] { ObjectName(388) };
                                    case 3:
                                    case 7:
                                        return new string[] { ObjectName(390) };
                                    case 8:
                                        return new string[] { ObjectName(770) };
                                }
                            }
                            else
                            {
                                switch (random.Next(4))
                                {
                                    case 0:
                                    case 1:
                                    case 2:
                                        return new string[] { ObjectName(78) };
                                    case 3:
                                        return new string[] { ObjectName(535) };
                                }
                            }
                        }
                        else if (random.NextDouble() < 0.4)
                        {
                            switch (random.Next(5))
                            {
                                case 0:
                                    return new string[] { ObjectName(66) };
                                case 1:
                                    return new string[] { ObjectName(68) };
                                case 2:
                                    return new string[] { ObjectName(709) };
                                case 3:
                                    return new string[] { ObjectName(535) };
                                case 4:
                                    return GetSpecialItemForThisMineLevel(level, X, Y);
                            }
                        }
                        break;
                    case 120:
                        if (random.NextDouble() < 0.65)
                        {
                            if (random.NextDouble() < 0.8)
                            {
                                switch (random.Next(9))
                                {
                                    case 0:
                                        return new string[] { ObjectName(382) };
                                    case 1:
                                        return new string[] { ObjectName(380) };
                                    case 3:
                                        return new string[] { ObjectName(378) };
                                    case 4:
                                        return new string[] { ObjectName(388) };
                                    case 5:
                                        return new string[] { ObjectName(92) };
                                    case 6:
                                    case 7:
                                        return new string[] { ObjectName(390) };
                                    case 8:
                                        return new string[] { ObjectName(770) };
                                }
                            }
                            else
                            {
                                switch (random.Next(4))
                                {
                                    case 0:
                                    case 2:
                                    case 3:
                                        return new string[] { ObjectName(78) };
                                    case 1:
                                        return new string[] { ObjectName(536) };
                                }
                            }
                        }
                        else if (random.NextDouble() < 0.4)
                        {
                            switch (random.Next(5))
                            {
                                case 0:
                                    return new string[] { ObjectName(62) };
                                case 1:
                                    return new string[] { ObjectName(70) };
                                case 2:
                                    return new string[] { string.Format("{0}x {1}", random.Next(1, 4), ObjectName(709)) };
                                case 3:
                                    return new string[] { ObjectName(536) };
                                case 4:
                                    return GetSpecialItemForThisMineLevel(level, X, Y);
                            }
                        }
                        break;
                    case 122:
                    case 124:
                        if (random.NextDouble() < 0.65)
                        {
                            if (random.NextDouble() < 0.8)
                            {
                                switch (random.Next(8))
                                {
                                    case 0:
                                        return new string[] { ObjectName(382) };
                                    case 1:
                                        return new string[] { ObjectName(384) };
                                    case 3:
                                        return new string[] { ObjectName(380) };
                                    case 4:
                                        return new string[] { ObjectName(378) };
                                    case 5:
                                        return new string[] { ObjectName(390) };
                                    case 6:
                                        return new string[] { ObjectName(388) };
                                    case 7:
                                        return new string[] { ObjectName(92) };
                                }
                            }
                            else
                            {
                                switch (random.Next(4))
                                {
                                    case 0:
                                        return new string[] { ObjectName(78) };
                                    case 1:
                                        return new string[] { ObjectName(537) };
                                    case 2:
                                        return new string[] { ObjectName(78) };
                                    case 3:
                                        return new string[] { ObjectName(78) };
                                }
                            }
                        }
                        else if (random.NextDouble() < 0.4)
                        {
                            switch (random.Next(6))
                            {
                                case 0:
                                    return new string[] { ObjectName(60) };
                                case 1:
                                    return new string[] { ObjectName(64) };
                                case 2:
                                    return new string[] { string.Format("{0}x {1}", random.Next(1, 4), ObjectName(709)) };
                                case 3:
                                    return new string[] { ObjectName(749) };
                                case 4:
                                    return GetSpecialItemForThisMineLevel(level, X, Y);
                                case 5:
                                    return new string[] { ObjectName(688) };
                            }
                        }
                        break;
                }
            }
            return null;
        }

        public static IEnumerable<string> GetSpecialItemForThisMineLevel(int level, int x, int y)
        {
            Random random = new Random(level + (int)Game1.stats.DaysPlayed + x + y * 10000);
            if (level < 20)
            {
                switch (random.Next(6))
                {
                    case 0:
                        return SpecialDrop("MeleeWeapon", 16);
                    case 1:
                        return SpecialDrop("MeleeWeapon", 24);
                    case 2:
                        return SpecialDrop("Boots", 504);
                    case 3:
                        return SpecialDrop("Boots", 505);
                    case 4:
                        return SpecialDrop("Ring", 516);
                    case 5:
                        return SpecialDrop("Ring", 518);
                }
            }
            else if (level < 40)
            {
                switch (random.Next(7))
                {
                    case 0:
                        return SpecialDrop("MeleeWeapon", 22);
                    case 1:
                        return SpecialDrop("MeleeWeapon", 24);
                    case 2:
                        return SpecialDrop("Boots", 504);
                    case 3:
                        return SpecialDrop("Boots", 505);
                    case 4:
                        return SpecialDrop("Ring", 516);
                    case 5:
                        return SpecialDrop("Ring", 518);
                    case 6:
                        return SpecialDrop("MeleeWeapon", 15);
                }
            }
            else if (level < 60)
            {
                switch (random.Next(7))
                {
                    case 0:
                        return SpecialDrop("MeleeWeapon", 6);
                    case 1:
                        return SpecialDrop("MeleeWeapon", 26);
                    case 2:
                        return SpecialDrop("MeleeWeapon", 15);
                    case 3:
                        return SpecialDrop("Boots", 510);
                    case 4:
                        return SpecialDrop("Ring", 517);
                    case 5:
                        return SpecialDrop("Ring", 519);
                    case 6:
                        return SpecialDrop("MeleeWeapon", 27);
                }
            }
            else if (level < 80)
            {
                switch (random.Next(7))
                {
                    case 0:
                        return SpecialDrop("MeleeWeapon", 26);
                    case 1:
                        return SpecialDrop("MeleeWeapon", 27);
                    case 2:
                        return SpecialDrop("Boots", 508);
                    case 3:
                        return SpecialDrop("Boots", 510);
                    case 4:
                        return SpecialDrop("Ring", 517);
                    case 5:
                        return SpecialDrop("Ring", 519);
                    case 6:
                        return SpecialDrop("MeleeWeapon", 19);
                }
            }
            else if (level < 100)
            {
                switch (random.Next(7))
                {
                    case 0:
                        return SpecialDrop("MeleeWeapon", 48);
                    case 1:
                        return SpecialDrop("MeleeWeapon", 48);
                    case 2:
                        return SpecialDrop("Boots", 511);
                    case 3:
                        return SpecialDrop("Boots", 513);
                    case 4:
                        return SpecialDrop("MeleeWeapon", 18);
                    case 5:
                        return SpecialDrop("MeleeWeapon", 28);
                    case 6:
                        return SpecialDrop("MeleeWeapon", 52);
                }
            }
            else if (level < 120)
            {
                switch (random.Next(7))
                {
                    case 0:
                        return SpecialDrop("MeleeWeapon", 19);
                    case 1:
                        return SpecialDrop("MeleeWeapon", 50);
                    case 2:
                        return SpecialDrop("Boots", 511);
                    case 3:
                        return SpecialDrop("Boots", 513);
                    case 4:
                        return SpecialDrop("MeleeWeapon", 18);
                    case 5:
                        return SpecialDrop("MeleeWeapon", 46);
                    case 6:
                        return SpecialDrop("Ring", 887);
                }
            }
            else
            {
                switch (random.Next(12))
                {
                    case 0:
                        return SpecialDrop("MeleeWeapon", 45);
                    case 1:
                        return SpecialDrop("MeleeWeapon", 50);
                    case 2:
                        return SpecialDrop("Boots", 511);
                    case 3:
                        return SpecialDrop("Boots", 513);
                    case 4:
                        return SpecialDrop("MeleeWeapon", 18);
                    case 5:
                        return SpecialDrop("MeleeWeapon", 28);
                    case 6:
                        return SpecialDrop("MeleeWeapon", 52);
                    case 7:
                        return new string[] { "Battery Pack" };
                    case 8:
                        return SpecialDrop("Boots", 878);
                    case 9:
                        return new string[] { "Curiosity Lure" };
                    case 10:
                        return SpecialDrop("Ring", 859);
                    case 11:
                        return SpecialDrop("Ring", 887);

                }
            }
            return new string[] { "Cave Carrot" };
        }


        public static int ContainerType(BreakableContainer container)
        {
            return Reflector.GetValue<BreakableContainer, NetInt>(container, "containerType").Value;
        }

        public static IEnumerable<string> SpecialDrop(string type, int index)
        {
            Dictionary<int, string> data;
            switch (type)
            {
                case "MeleeWeapon":
                    data = Game1.content.Load<Dictionary<int, string>>("Data\\weapons");
                    break;
                case "Boots":
                    data = Game1.content.Load<Dictionary<int, string>>("Data\\Boots");
                    break;
                case "Ring":
                    return new string[] { ObjectName(index) };
                default:
                    return new string[] { string.Format("{0}({1})", type, index) };
            }
            return new string[] { data[index].Split('/')[0] };
        }
    }
}
