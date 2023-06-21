using System;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData;
using StardewValley.Locations;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System.Collections.Generic;

namespace TASMod.Helpers
{
    public class SleepInfo
    {
        private static Random random;
        private static int dishOfTheDay;
        private static double dailyLuck;
        private static string whichFriend;
        private static bool receiveGift;
        private static bool lightningTriggered;
        private static int weatherForTomorrow;

        private static NetVector2Dictionary<TerrainFeature, Netcode.NetRef<TerrainFeature>> Farm_terrainFeatures;

        private static int last_dayOfMonth;
        private static uint last_stats_DaysPlayed;
        private static uint last_stats_StepsTaken;
        private static int last_timeOfDay;
        private static int last_Farm_terrainFeatures_Count;

        public static int DishOfTheDay
        {
            get
            {
                if (NeedsUpdate())
                    RecomputeStats();
                return dishOfTheDay;
            }
        }

        public static double DailyLuck
        {
            get
            {
                if (NeedsUpdate())
                    RecomputeStats();
                return dailyLuck;
            }
        }

        public static string WhichFriend
        {
            get
            {
                if (NeedsUpdate())
                    RecomputeStats();
                return whichFriend;
            }
        }

        public static bool ReceiveGift
        {
            get
            {
                if (NeedsUpdate())
                    RecomputeStats();
                return receiveGift;
            }
        }

        public static bool LightningTriggered
        {
            get
            {
                if (NeedsUpdate())
                    RecomputeStats();
                return lightningTriggered;
            }
        }

        public static int WeatherForTomorrow
        {
            get
            {
                if (NeedsUpdate())
                    RecomputeStats();
                return weatherForTomorrow;
            }
        }


        public static bool NeedsUpdate()
        {
            if (last_dayOfMonth == Game1.dayOfMonth &&
                last_stats_DaysPlayed == Game1.stats.DaysPlayed &&
                last_stats_StepsTaken == Game1.stats.StepsTaken &&
                last_timeOfDay == Game1.timeOfDay &&
                Game1.getLocationFromName("Farm") != null && Game1.getLocationFromName("Farm").terrainFeatures != null &&
                last_Farm_terrainFeatures_Count == Game1.getLocationFromName("Farm").terrainFeatures.Count())
            {
                return false;
            }
            return true;
        }

        public static void RecomputeStats()
        {
            if (Game1.getLocationFromName("Farm") == null || Game1.getLocationFromName("Farm").terrainFeatures == null)
            {
                Farm_terrainFeatures = new NetVector2Dictionary<TerrainFeature, Netcode.NetRef<TerrainFeature>>();
            }
            else
            {
                Farm_terrainFeatures = new NetVector2Dictionary<TerrainFeature, Netcode.NetRef<TerrainFeature>>(Game1.getLocationFromName("Farm").terrainFeatures.Pairs);
            }

            last_dayOfMonth = Game1.dayOfMonth;
            last_stats_DaysPlayed = Game1.stats.DaysPlayed;
            last_stats_StepsTaken = Game1.stats.StepsTaken;
            last_timeOfDay = Game1.timeOfDay;
            last_Farm_terrainFeatures_Count = Farm_terrainFeatures.Count();

            int dayOfMonth = Game1.dayOfMonth;
            uint stats_DaysPlayed = Game1.stats.DaysPlayed;
            int seed = (int)Game1.uniqueIDForThisGame / 100 + (int)(stats_DaysPlayed * 10) + 1 + (int)Game1.stats.StepsTaken;

            random = new Random(seed);
            for (int index = 0; index < dayOfMonth; ++index)
                random.Next();

            /*** Dish Of the Day ***/
            dishOfTheDay = random.Next(194, 240);
            while (Utility.getForbiddenDishesOfTheDay().Contains(dishOfTheDay))
                dishOfTheDay = random.Next(194, 240);

            int dishOfTheDayQty = random.Next(1, 4 + ((random.NextDouble() < 0.08) ? 10 : 0));
            random.NextDouble(); // Object::Constructor() flipped.Value

            // Minutes Elapsed Block
            foreach (GameLocation location in Game1.locations)
            {
                for (int n = location.objects.Count() - 1; n >= 0; n--)
                {
                }
            }
            if (Game1.getFarm() != null)
            {
                foreach (Building building in Game1.getFarm().buildings)
                {
                    if (building.indoors.Value != null)
                    {
                    }
                }
            }
            if (Game1.isLightning)
                lightningTriggered = OvernightLightning();
            else
                lightningTriggered = false;

            /*** Friendship Gift ***/
            if (Game1.player.friendshipData.Count() > 0)
            {
                whichFriend = Game1.player.friendshipData.Keys.ElementAt(random.Next(Game1.player.friendshipData.Keys.Count()));
                receiveGift = random.NextDouble() < (double)(Game1.player.friendshipData[whichFriend].Points / 250) * 0.1 &&
                    (Game1.player.spouse == null || !Game1.player.spouse.Equals(whichFriend)) &&
                    Game1.content.Load<Dictionary<string, string>>("Data\\mail").ContainsKey(whichFriend);
            }

            random.Next(); // scarecrow society in Farmer.dayupdate();

            /*** Daily Luck ***/
            dailyLuck = Math.Min(0.10000000149011612, (double)random.Next(-100, 101) / 1000.0);

            dayOfMonth++;
            stats_DaysPlayed++;
            string currentSeason = Game1.currentSeason;
            int year = Game1.year;

            if (dayOfMonth == 29)
            {
                // Game1.newSeason()
                switch (Game1.currentSeason)
                {
                    case "spring":
                        currentSeason = "summer";
                        break;
                    case "summer":
                        currentSeason = "fall";
                        break;
                    case "fall":
                        currentSeason = "winter";
                        break;
                    case "winter":
                        currentSeason = "spring";
                        year++;
                        break;
                }
                dayOfMonth = 1;
            }
            /*** Special Orders ***/
            if (dayOfMonth == 1 || dayOfMonth == 8 || dayOfMonth == 15 || dayOfMonth == 22)
            {
                SpecialOrder_UpdateAvailableSpecialOrders(dayOfMonth, stats_DaysPlayed);
            }
            /*** Weather Block ***/
            WorldDate date = new WorldDate(year, currentSeason, dayOfMonth);
            weatherForTomorrow = Game1.getWeatherModificationsForDate(date, Game1.weatherForTomorrow);
#pragma warning disable CS0219 // Variable is assigned but its value is never used
            bool isRaining = false;
            bool isLightning = false;
#pragma warning restore CS0219 // Variable is assigned but its value is never used
            bool isSnowing = false;
            bool wasRainingYesterday = (Game1.isRaining || Game1.isLightning);
            if (weatherForTomorrow == 1 || weatherForTomorrow == 3)
                isRaining = true;
            if (weatherForTomorrow == 3)
                isLightning = true;
            if (weatherForTomorrow == 0 || weatherForTomorrow == 2 || weatherForTomorrow == 4 || weatherForTomorrow == 5 || weatherForTomorrow == 6)
            {
                isRaining = false;
                isLightning = false;
                isSnowing = weatherForTomorrow == 5;
            }
            // NOTE: v1.5 change, setting island weather
            // Game1::SetOtherLocationWeatherForTomorrow()
            random.NextDouble();

            if (weatherForTomorrow == 2)
                PopulateDebrisWeatherArray();

            double chanceToRainTomorrow;
            if (currentSeason.Equals("summer"))
            {
                chanceToRainTomorrow = ((dayOfMonth > 1) ? (0.12 + (double)((float)dayOfMonth * 0.003f)) : 0.0);
            }
            else if (currentSeason.Equals("winter"))
            {
                chanceToRainTomorrow = 0.63;
            }
            else
            {
                chanceToRainTomorrow = 0.183;
            }

            weatherForTomorrow = 0;
            if (random.NextDouble() < chanceToRainTomorrow)
            {
                weatherForTomorrow = 1;
                if ((currentSeason.Equals("summer") && random.NextDouble() < 0.85) || (!currentSeason.Equals("winter") && random.NextDouble() < 0.25 && dayOfMonth > 2 && stats_DaysPlayed > 27))
                {
                    weatherForTomorrow = 3;
                }
                if (currentSeason.Equals("winter"))
                {
                    weatherForTomorrow = 5;
                }
            }
            else if (stats_DaysPlayed > 2 && ((currentSeason.Equals("spring") && random.NextDouble() < 0.2) || (currentSeason.Equals("fall") && random.NextDouble() < 0.6)))
            {
                weatherForTomorrow = 2;
            }
            else
            {
                weatherForTomorrow = 0;
            }
            if (Utility.isFestivalDay(dayOfMonth + 1, currentSeason))
            {
                weatherForTomorrow = 4;
            }
            if (stats_DaysPlayed == 2)
            {
                weatherForTomorrow = 1;
            }
        }

        private static bool OvernightLightning()
        {
            int numberOfLoops = (2400 - Game1.timeOfDay) / 100;
            bool triggered = false;
            for (int i = 1; i <= numberOfLoops; i++)
            {
                triggered |= performLightningUpdate(Game1.timeOfDay + 100 * i);
            }
            return triggered;
        }
        private static bool performLightningUpdate(int timeOfDay)
        {
            bool triggered = false;
            Random currRandom = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed + timeOfDay);
            if (currRandom.NextDouble() < 0.125 + Game1.player.DailyLuck + Game1.player.luckLevel.Value / 100f)
            {
                Farm farm = Game1.getLocationFromName("Farm") as Farm;
                List<Vector2> list = new List<Vector2>();
                foreach (KeyValuePair<Vector2, StardewValley.Object> pair in farm.objects.Pairs)
                {
                    if (pair.Value.bigCraftable.Value && pair.Value.ParentSheetIndex == 9)
                    {
                        list.Add(pair.Key);
                    }
                }
                if (list.Count > 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 vector = list.ElementAt(currRandom.Next(list.Count));
                        if (farm.objects[vector].heldObject.Value == null)
                        {
                            random.NextDouble(); // Object::ctor() flipped.Value set
                            return true;
                        }
                    }
                }
                if (currRandom.NextDouble() < 0.25 - Game1.player.team.AverageDailyLuck() - Game1.player.team.AverageLuckLevel() / 100.0)
                {
                    triggered = true;
                    try
                    {
                        KeyValuePair<Vector2, TerrainFeature> keyValuePair = Farm_terrainFeatures.Pairs.ElementAt(currRandom.Next(Farm_terrainFeatures.Count()));
                        if (!(keyValuePair.Value is FruitTree))
                        {
                            if (TerrainFeature_PerformToolAction(keyValuePair.Value, null, 50, keyValuePair.Key, farm))
                            {
                                Farm_terrainFeatures.Remove(keyValuePair.Key);
                            }
                        }
                        else if (keyValuePair.Value is FruitTree fruitTree)
                        {
                            FruitTree_Shake(fruitTree, keyValuePair.Key, true, farm);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return triggered;
        }

        private static bool TerrainFeature_PerformToolAction(TerrainFeature terrainFeature, Tool t, int damage, Vector2 tileLocation, GameLocation location)
        {
            if (terrainFeature is Grass grass)
            {
                random.NextDouble(); // Grass Shake (Grass.cs:228)
                for (int i = 0; i < 4; i++) // multiplayer.BroadcastSprites (Grass.cs:261)
                    random.NextDouble();
            }
            else if (terrainFeature is HoeDirt hoeDirt)
            {

            }
            else if (terrainFeature is Tree tree)
            {
                return Tree_PerformToolAction(tree, t, damage, tileLocation, location);
            }
            return false;
        }

        private static bool Tree_PerformToolAction(Tree tree, Tool t, int explosion, Vector2 tileLocation, GameLocation location)
        {
            if (location == null)
            {
                location = Game1.currentLocation;
            }
            if (explosion > 0)
            {
                tree.tapped.Value = false;
            }
            if ((bool)tree.tapped.Value)
            {
                return false;
            }
            if ((float)tree.health.Value <= -99f)
            {
                return false;
            }
            if (tree.growthStage.Value >= 5)
            {
                Tree_Shake(tree, tileLocation, true, location);
                float health = tree.health.Value - explosion;
                return health < 0f && Tree_PerformTreeFall(tree, t, explosion, tileLocation, location);
            }
            if (tree.growthStage.Value >= 3)
            {
                Tree_Shake(tree, tileLocation, true, location);
                float health = tree.health.Value - explosion;
                return health < 0f && Tree_PerformBushDestroy(tree, tileLocation, location);
            }
            return false;
        }

        private static void Tree_Shake(Tree tree, Vector2 tileLocation, bool doEvenIfStillShaking, GameLocation location)
        {
            float maxShake = (float)Reflector.GetValue(tree, "maxShake");
            if (((maxShake == 0f) | doEvenIfStillShaking) && (int)tree.growthStage.Value >= 3 && !tree.stump.Value)
            {
                bool shakeLeft = ((float)Game1.player.getStandingX() > (tileLocation.X + 0.5f) * 64f || ((Game1.player.getTileLocation().X == tileLocation.X && random.NextDouble() < 0.5) ? true : false));
                if (tree.growthStage.Value >= 5)
                {
                    if (random.NextDouble() < 0.66)
                    {
                        int numberOfLeaves = random.Next(1, 6);
                        for (int j = 0; j < numberOfLeaves; j++)
                        {
                            //leaves.Add(new Leaf(...))
                            random.Next(); // position.X
                            random.Next(); // position.Y
                            random.Next(); // rotationRate
                            random.Next(); // type
                            random.Next(); // yVelocity
                        }
                    }
                    if (random.NextDouble() < 0.01 && (Game1.GetSeasonForLocation(tree.currentLocation).Equals("spring") || Game1.GetSeasonForLocation(tree.currentLocation).Equals("summer") || tree.currentLocation.GetLocationContext() == GameLocation.LocationContext.Island))
                    {
                        while (random.NextDouble() < 0.8)
                        {
                            // location.addCrititer(new Butterfly(...))
                            random.Next(); // position.X
                            random.Next(); // position.Y
                        }
                    }
                    if (tree.hasSeed.Value && (Game1.IsMultiplayer || Game1.player.ForagingLevel >= 1))
                    {
                        int seedIndex = -1;
                        switch (tree.treeType.Value)
                        {
                            case 3:
                                seedIndex = 311;
                                break;
                            case 1:
                                seedIndex = 309;
                                break;
                            case 8:
                                seedIndex = 292;
                                break;
                            case 2:
                                seedIndex = 310;
                                break;
                            case 6:
                            case 9:
                                seedIndex = 88;
                                break;
                        }
                        if (Game1.GetSeasonForLocation(tree.currentLocation).Equals("fall") && (int)tree.treeType.Value == 2 && last_dayOfMonth + 1 >= 14)
                        {
                            seedIndex = 408;
                        }
                        if (seedIndex != -1)
                        {
                            //Game1.createObjectDebris(seedIndex, (int)tileLocation.X, (int)tileLocation.Y - 3, ((int)tileLocation.Y + 1) * 64, 0, 1f, location);
                        }
                        if (seedIndex == 88 && new Random((int)Game1.uniqueIDForThisGame + (int)last_stats_DaysPlayed + (int)tileLocation.X * 13 + (int)tileLocation.Y * 54).NextDouble() < 0.1 && location != null && location is IslandLocation)
                        {
                            //Game1.createObjectDebris(791, (int)tileLocation.X, (int)tileLocation.Y - 3, ((int)tileLocation.Y + 1) * 64, 0, 1f, location);
                        }
                        if (random.NextDouble() <= 0.5 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
                        {
                            //Game1.createObjectDebris(890, (int)tileLocation.X, (int)tileLocation.Y - 3, ((int)tileLocation.Y + 1) * 64, 0, 1f, location);
                        }
                    }
                }
                else if (random.NextDouble() < 0.66)
                {
                    int numberOfLeaves = random.Next(1, 3);
                    for (int i = 0; i < numberOfLeaves; i++)
                    {
                        //leaves.Add(new Leaf(...))
                        random.Next(); // position.X
                        random.Next(); // rotationRate
                        random.Next(); // type
                        random.Next(); // yVelocity
                    }
                }
            }
        }
        private static bool Tree_PerformTreeFall(Tree tree, Tool t, int explosion, Vector2 tileLocation, GameLocation location)
        {
            if (!tree.stump.Value)
            {
                return false;
            }
            else
            {
                random.Next(); // Game1.createRadialDebris for drops
                if (t == null || t.getLastFarmerToUse() == null)
                {
                    if (location.Equals(Game1.currentLocation))
                    {
                        //Game1.createMultipleObjectDebris(92, (int)tileLocation.X, (int)tileLocation.Y, 2, location);
                    }
                    else
                    {
                        //Game1.createItemDebris(new Object(92, 1), tileLocation * 64f, 2, location);
                        //Game1.createItemDebris(new Object(92, 1), tileLocation * 64f, 2, location);
                        random.Next();
                        random.Next();
                    }
                }
                //if (Game1.random.NextDouble() <= 0.25 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
                random.NextDouble();
                return !(bool)Reflector.GetValue(tree, "falling");
            }
        }

        private static bool Tree_PerformBushDestroy(Tree tree, Vector2 tileLocation, GameLocation location)
        {
            if ((int)tree.treeType.Value == 7)
            {
                //Game1.createMultipleObjectDebris(420, (int)tileLocation.X, (int)tileLocation.Y, 1, location);
                //Game1.createRadialDebris(location, 12, (int)tileLocation.X, (int)tileLocation.Y, random.Next(20, 30), resource: false, -1, item: false, 10000);
                random.Next();
            }
            else
            {
                //Game1.createDebris(12, (int)tileLocation.X, (int)tileLocation.Y, (int)((Game1.getFarmer((Netcode.NetLong)Reflector.GetValue(tree,"lastPlayerToHit")).professions.Contains(12) ? 1.25 : 1.0) * 4.0), location);
                //Game1.createRadialDebris(location, 12, (int)tileLocation.X, (int)tileLocation.Y, random.Next(20, 30), resource: false);
                random.Next();
            }
            return true;
        }
        private static void FruitTree_Shake(FruitTree tree, Vector2 tileLocation, bool doEvenIfStillShaking, Farm farm)
        {
            // TODO
        }

        private static void PopulateDebrisWeatherArray()
        {
            int debrisToMake = random.Next(16, 64);
            for (int i = 0; i < debrisToMake; i++)
            {
                // Game1::populateDebrisWeatherArray initialize the variables for constructor
                float randomVector_X = (float)random.Next(0, Game1.viewport.Width);
                float randomVector_Y = (float)random.Next(0, Game1.viewport.Height);
                float rotationVelocity = random.Next(15) / 500f;
                float dx = random.Next(-10, 0) / 50f;
                float dy = random.Next(10) / 50f;
                // WeatherDebris::constructor() animationIntervalOffset
                int tmp = random.Next();
            }
        }

        private static void SpecialOrder_UpdateAvailableSpecialOrders(int dayOfMonth, uint daysPlayed)
        {
            Dictionary<string, SpecialOrderData> order_data = Game1.content.Load<Dictionary<string, SpecialOrderData>>("Data\\SpecialOrders");
            List<string> keys = new List<string>(order_data.Keys);
            for (int k = 0; k < keys.Count; k++)
            {
                string key = keys[k];
                bool invalid = false;
                if (!invalid && order_data[key].Repeatable != "True" && Game1.MasterPlayer.team.completedSpecialOrders.ContainsKey(key))
                {
                    invalid = true;
                }
                if (dayOfMonth >= 16 && order_data[key].Duration == "Month")
                {
                    invalid = true;
                }
                if (!invalid && !SpecialOrder.CheckTags(order_data[key].RequiredTags))
                {
                    invalid = true;
                }
                if (!invalid)
                {
                    foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
                    {
                        if ((string)specialOrder.questKey.Value == key)
                        {
                            invalid = true;
                            break;
                        }
                    }
                }
                if (invalid)
                {
                    keys.RemoveAt(k);
                    k--;
                }
            }
            Random r = new Random((int)Game1.uniqueIDForThisGame + (int)((float)(double)daysPlayed * 1.3f));
            List<string> typed_keys = new List<string>();
            foreach (string key3 in keys)
            {
                if (order_data[key3].OrderType == "")
                {
                    typed_keys.Add(key3);
                }
            }
            // no qi check
            List<string> all_keys = new List<string>(typed_keys);
            for (int j = 0; j < typed_keys.Count; j++)
            {
                if (Game1.player.team.completedSpecialOrders.ContainsKey(typed_keys[j]))
                {
                    typed_keys.RemoveAt(j);
                    j--;
                }
            }

            for (int i = 0; i < 2; i++)
            {
                if (typed_keys.Count == 0)
                {
                    if (all_keys.Count == 0)
                    {
                        break;
                    }
                    typed_keys = new List<string>(all_keys);
                }
                int index = r.Next(typed_keys.Count);
                string key2 = typed_keys[index];
                SpecialOrder_GetSpecialOrder(key2, r.Next());
                typed_keys.Remove(key2);
                all_keys.Remove(key2);
            }
        }

        public static void SpecialOrder_GetSpecialOrder(string key, int generation_seed)
        {
            Dictionary<string, SpecialOrderData> order_data = Game1.content.Load<Dictionary<string, SpecialOrderData>>("Data\\SpecialOrders");
            if (order_data.ContainsKey(key))
            {
                Random r = new Random(generation_seed);
                SpecialOrderData data = order_data[key];
                SpecialOrder order = new SpecialOrder();
                order.generationSeed.Value = generation_seed;
                Reflector.SetValue(order, "_orderData", data);
                order.questKey.Value = key;
                order.questName.Value = data.Name;
                order.requester.Value = data.Requester;
                order.orderType.Value = data.OrderType.Trim();
                order.specialRule.Value = data.SpecialRule.Trim();
                if (data.ItemToRemoveOnEnd != null)
                {
                    int item_to_remove = -1;
                    if (int.TryParse(data.ItemToRemoveOnEnd, out item_to_remove))
                    {
                        order.itemToRemoveOnEnd.Value = item_to_remove;
                    }
                }
                if (data.MailToRemoveOnEnd != null)
                {
                    order.mailToRemoveOnEnd.Value = data.MailToRemoveOnEnd;
                }
                order.selectedRandomElements.Clear();
                if (data.RandomizedElements != null)
                {
                    foreach (RandomizedElement randomized_element in data.RandomizedElements)
                    {
                        List<int> valid_indices = new List<int>();
                        for (int i = 0; i < randomized_element.Values.Count; i++)
                        {
                            if (SpecialOrder.CheckTags(randomized_element.Values[i].RequiredTags))
                            {
                                valid_indices.Add(i);
                            }
                        }
                        int selected_index = Utility.GetRandom(valid_indices, r);
                        order.selectedRandomElements[randomized_element.Name] = selected_index;
                        string value2 = randomized_element.Values[selected_index].Value;
                        if (value2.StartsWith("PICK_ITEM"))
                        {
                            value2 = value2.Substring("PICK_ITEM".Length);
                            string[] array = value2.Split(',');
                            List<int> valid_item_ids = new List<int>();
                            string[] array2 = array;
                            for (int j = 0; j < array2.Length; j++)
                            {
                                string valid_item_name = array2[j].Trim();
                                if (valid_item_name.Length != 0)
                                {
                                    if (char.IsDigit(valid_item_name[0]))
                                    {
                                        int item_id = -1;
                                        if (int.TryParse(valid_item_name, out item_id))
                                        {
                                            valid_item_ids.Add(item_id);
                                        }
                                    }
                                    else
                                    {
                                        // calls 1 if the item is an object
                                        Item item = Utility_fuzzyItemSearch(valid_item_name);
                                        //if (Utility.IsNormalObjectAtParentSheetIndex(item, item.ParentSheetIndex))
                                        //{
                                        //    valid_item_ids.Add(item.ParentSheetIndex);
                                        //}
                                    }
                                }
                            }
                            order.preSelectedItems[randomized_element.Name] = Utility.GetRandom(valid_item_ids, r);
                        }
                    }
                }
                //if (data.Duration == "Month")
                //{
                //    order.SetDuration(QuestDuration.Month);
                //}
                //else if (data.Duration == "TwoWeeks")
                //{
                //    order.SetDuration(QuestDuration.TwoWeeks);
                //}
                //else if (data.Duration == "TwoDays")
                //{
                //    order.SetDuration(QuestDuration.TwoDays);
                //}
                //else if (data.Duration == "ThreeDays")
                //{
                //    order.SetDuration(QuestDuration.ThreeDays);
                //}
                //else
                //{
                //    order.SetDuration(QuestDuration.Week);
                //}
                order.questDescription.Value = data.Text;
                foreach (SpecialOrderObjectiveData objective_data in data.Objectives)
                {
                    OrderObjective objective2 = null;
                    Type objective_type = Type.GetType("StardewValley." + objective_data.Type.Trim() + "Objective");
                    if (!(objective_type == null) && objective_type.IsSubclassOf(typeof(OrderObjective)))
                    {
                        objective2 = (OrderObjective)Activator.CreateInstance(objective_type);
                        if (objective2 != null)
                        {
                            SpecialOrder_Parse(order, objective_data.Text);
                            SpecialOrder_Parse(order, objective_data.RequiredCount);
                            OrderObjective_Load(objective2, order, objective_data.Data);
                        }
                    }
                }
            }
        }
        private static void SpecialOrder_Parse(SpecialOrder order, string data)
        {
            data = data.Trim();
            order.GetData();
            data = order.MakeLocalizationReplacements(data);
            int open_index = 0;
            do
            {
                open_index = data.LastIndexOf('{');
                if (open_index < 0)
                {
                    continue;
                }
                int close_index = data.IndexOf('}', open_index);
                if (close_index == -1)
                {
                    return;
                }
                string inner = data.Substring(open_index + 1, close_index - open_index - 1);
                string value = inner;
                string key = inner;
                string subkey = null;
                if (inner.Contains(":"))
                {
                    string[] split2 = inner.Split(':');
                    key = split2[0];
                    if (split2.Length > 1)
                    {
                        subkey = split2[1];
                    }
                }
                if (((SpecialOrderData)Reflector.GetValue(order, "_orderData")).RandomizedElements != null)
                {
                    if (order.preSelectedItems.ContainsKey(key))
                    {
                        //Object requested_item = new Object(Vector2.Zero, order.preSelectedItems[key], 0);
                        random.Next(); // ^ object.flipped_val
                                       //if (subkey == "Text")
                                       //{
                                       //    value = requested_item.DisplayName;
                                       //}
                                       //else if (subkey == "TextPlural")
                                       //{
                                       //    value = Lexicon.makePlural(requested_item.DisplayName);
                                       //}
                                       //else if (subkey == "TextPluralCapitalized")
                                       //{
                                       //    value = Utility.capitalizeFirstLetter(Lexicon.makePlural(requested_item.DisplayName));
                                       //}
                                       //else if (subkey == "Tags")
                                       //{
                                       //    string alternate_id2 = "id_" + Utility.getStandardDescriptionFromItem(requested_item, 0, '_');
                                       //    alternate_id2 = alternate_id2.Substring(0, alternate_id2.Length - 2).ToLower();
                                       //    value = alternate_id2;
                                       //}
                                       //else if (subkey == "Price")
                                       //{
                                       //    value = string.Concat(requested_item.sellToStorePrice(-1L));
                                       //}
                    }
                    else if (order.selectedRandomElements.ContainsKey(key))
                    {
                        foreach (RandomizedElement randomized_element in ((SpecialOrderData)Reflector.GetValue(order, "_orderData")).RandomizedElements)
                        {
                            if (randomized_element.Name == key)
                            {
                                value = order.MakeLocalizationReplacements(randomized_element.Values[order.selectedRandomElements[key]].Value);
                                break;
                            }
                        }
                    }
                }
                if (subkey != null)
                {
                    string[] split = value.Split('|');
                    for (int i = 0; i < split.Length; i += 2)
                    {
                        if (i + 1 <= split.Length && split[i] == subkey)
                        {
                            value = split[i + 1];
                            break;
                        }
                    }
                }
                data = data.Remove(open_index, close_index - open_index + 1);
                data = data.Insert(open_index, value);
            }
            while (open_index >= 0);
            return;
        }

        private static void OrderObjective_Load(OrderObjective objective, SpecialOrder order, Dictionary<string, string> data)
        {
            if (objective is CollectObjective || objective is FishObjective)
            {
                if (data.ContainsKey("AcceptedContextTags"))
                {
                    SpecialOrder_Parse(order, data["AcceptedContextTags"]);
                }
            }
            if (objective is DeliverObjective)
            {
                if (data.ContainsKey("AcceptedContextTags"))
                {
                    SpecialOrder_Parse(order, data["AcceptedContextTags"]);
                }
                if (data.ContainsKey("TargetName"))
                {
                    SpecialOrder_Parse(order, data["TargetName"]);
                }
                if (data.ContainsKey("Message"))
                {
                    SpecialOrder_Parse(order, data["Message"]);
                }
            }
        }

        private static Item Utility_fuzzyItemSearch(string query, int stack_count = 1)
        {
            Dictionary<string, string> items = new Dictionary<string, string>();
            foreach (int key7 in Game1.objectInformation.Keys)
            {
                string item_data7 = Game1.objectInformation[key7];
                if (item_data7 != null)
                {
                    string[] data = item_data7.Split('/');
                    string item_name7 = data[0];
                    if (!(item_name7 == "Stone") || key7 == 390)
                    {
                        if (data[3] == "Ring")
                        {
                            items[item_name7] = "R " + key7 + " " + stack_count;
                        }
                        else
                        {
                            items[item_name7] = "O " + key7 + " " + stack_count;
                        }
                    }
                }
            }
            foreach (int key6 in Game1.bigCraftablesInformation.Keys)
            {
                string item_data6 = Game1.bigCraftablesInformation[key6];
                if (item_data6 != null)
                {
                    string item_name6 = item_data6.Substring(0, item_data6.IndexOf('/'));
                    items[item_name6] = "BO " + key6 + " " + stack_count;
                }
            }
            Dictionary<int, string> furniture_data = Game1.content.Load<Dictionary<int, string>>("Data\\Furniture");
            foreach (int key5 in furniture_data.Keys)
            {
                string item_data5 = furniture_data[key5];
                if (item_data5 != null)
                {
                    string item_name5 = item_data5.Substring(0, item_data5.IndexOf('/'));
                    items[item_name5] = "F " + key5 + " " + stack_count;
                }
            }
            Dictionary<int, string> weapon_data = Game1.content.Load<Dictionary<int, string>>("Data\\weapons");
            foreach (int key4 in weapon_data.Keys)
            {
                string item_data4 = weapon_data[key4];
                if (item_data4 != null)
                {
                    string item_name4 = item_data4.Substring(0, item_data4.IndexOf('/'));
                    items[item_name4] = "W " + key4 + " " + stack_count;
                }
            }
            Dictionary<int, string> boot_data = Game1.content.Load<Dictionary<int, string>>("Data\\Boots");
            foreach (int key3 in boot_data.Keys)
            {
                string item_data3 = boot_data[key3];
                if (item_data3 != null)
                {
                    string item_name3 = item_data3.Substring(0, item_data3.IndexOf('/'));
                    items[item_name3] = "B " + key3 + " " + stack_count;
                }
            }
            Dictionary<int, string> hat_data = Game1.content.Load<Dictionary<int, string>>("Data\\hats");
            foreach (int key2 in hat_data.Keys)
            {
                string item_data2 = hat_data[key2];
                if (item_data2 != null)
                {
                    string item_name2 = item_data2.Substring(0, item_data2.IndexOf('/'));
                    items[item_name2] = "H " + key2 + " " + stack_count;
                }
            }
            foreach (int key in Game1.clothingInformation.Keys)
            {
                string item_data = Game1.clothingInformation[key];
                if (item_data != null)
                {
                    string item_name = item_data.Substring(0, item_data.IndexOf('/'));
                    items[item_name] = "C " + key + " " + stack_count;
                }
            }
            string result = Utility.fuzzySearch(query, items.Keys.ToList());
            if (result != null)
            {
                Utility_getItemFromStandardTextDescription(items[result], null);
            }
            return null;
        }

        private static void Utility_getItemFromStandardTextDescription(string description, Farmer who, char delimiter = ' ')
        {
            string[] array = description.Split(delimiter);
            string type = array[0];
            int index = Convert.ToInt32(array[1]);
            int stock = Convert.ToInt32(array[2]);
            Item item = null;
            switch (type)
            {
                case "Furniture":
                case "F":
                    item = Furniture.GetFurnitureInstance(index, Vector2.Zero);
                    break;
                case "Object":
                case "O":
                    //item = new Object(index, 1);
                    random.Next();
                    break;
                case "BigObject":
                case "BO":
                    //item = new Object(Vector2.Zero, index);
                    random.Next();
                    break;
                case "Ring":
                case "R":
                    item = new Ring(index);
                    break;
                case "Boot":
                case "B":
                    item = new Boots(index);
                    break;
                case "Weapon":
                case "W":
                    item = new MeleeWeapon(index);
                    break;
                case "Blueprint":
                case "BL":
                    //item = new Object(index, 1, isRecipe: true);
                    random.Next();
                    break;
                case "Hat":
                case "H":
                    item = new Hat(index);
                    break;
                case "BigBlueprint":
                case "BBl":
                case "BBL":
                    //item = new Object(Vector2.Zero, index, isRecipe: true);
                    random.Next();
                    break;
                case "C":
                    item = new Clothing(index);
                    break;
            }
        }
    }
}

