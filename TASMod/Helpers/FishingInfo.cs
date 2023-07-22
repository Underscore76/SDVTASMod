using StardewValley.Menus;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TASMod.Helpers
{
    public class FishingInfo
    {
        public struct FishData
        {
            public int WhichFish;
            public string Name;
            public int Difficulty;
            public string MotionType;
            public int MinSize;
            public int MaxSize;
            public int StartTime;
            public int EndTime;
            public string Season;
            public string Weather;
            public int DepthModifier;
            public double BaseChance;
            public double DropOffAmount;
            public int MinLevel;
        }

        public class FishingGameState
        {
            public float bobberPosition;
            public float bobberSpeed;
            public float bobberAcceleration;
            public float bobberTargetPosition;
            public float scale;
            public float everythingShakeTimer;
            public float floaterSinkerAcceleration;
            public bool bobberInBar;
            public bool buttonPressed;
            public int bobberBarHeight;
            public float bobberBarPos;
            public float bobberBarSpeed;
            public float distanceFromCatching;
            public int whichBobber;

            public bool perfect;
            public bool beginnersRod;
            public bool treasure;
            public bool treasureCaught;
            public Vector2 barShake;
            public Vector2 fishShake;
            public Vector2 treasureShake;
            public int fishSize;
            public int fishSizeReductionTimer;
            public int minFishSize;
            public int maxFishSize;
            public float reelRotation;

            public float treasurePosition;
            public float treasureCatchLevel;
            public float treasureAppearTimer;
            public float treasureScale;
            public bool flipBubble;

            public FishingGameState() { }
            public FishingGameState(FishingGameState o)
            {
                bobberPosition = o.bobberPosition;
                bobberSpeed = o.bobberSpeed;
                bobberAcceleration = o.bobberAcceleration;
                bobberTargetPosition = o.bobberTargetPosition;
                scale = o.scale;
                everythingShakeTimer = o.everythingShakeTimer;
                floaterSinkerAcceleration = o.floaterSinkerAcceleration;
                bobberInBar = o.bobberInBar;
                buttonPressed = o.buttonPressed;
                bobberBarHeight = o.bobberBarHeight;
                bobberBarPos = o.bobberBarPos;
                bobberBarSpeed = o.bobberBarSpeed;
                distanceFromCatching = o.distanceFromCatching;
                whichBobber = o.whichBobber;

                perfect = o.perfect;
                beginnersRod = o.beginnersRod;
                treasure = o.treasure;
                treasureCaught = o.treasureCaught;
                barShake = o.barShake;
                fishShake = o.fishShake;
                treasureShake = o.treasureShake;
                fishSize = o.fishSize;
                fishSizeReductionTimer = o.fishSizeReductionTimer;
                minFishSize = o.minFishSize;
                maxFishSize = o.maxFishSize;
                reelRotation = o.reelRotation;

                treasurePosition = o.treasurePosition;
                treasureCatchLevel = o.treasureCatchLevel;
                treasureAppearTimer = o.treasureAppearTimer;
                treasureScale = o.treasureScale;
                flipBubble = o.flipBubble;
            }
        }

        public static FishingInfo _instance;

        static FishingInfo()
        {
            _instance = new FishingInfo();
        }

        public static bool IsInitialized = false;
        public static Dictionary<int, FishData> data;

        public static void Init()
        {
            if (!IsInitialized)
            {
                try
                {
                    Dictionary<int, string> fishData = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
                    data = new Dictionary<int, FishData>();
                    foreach (KeyValuePair<int, string> p in fishData)
                    {
                        string[] array = p.Value.Split('/');
                        if (array[1] == "trap")
                            continue;
                        try
                        {
                            data[p.Key] = new FishData()
                            {
                                WhichFish = p.Key,
                                Name = array[0],
                                Difficulty = Convert.ToInt32(array[1]),
                                MotionType = array[2],
                                MinSize = Convert.ToInt32(array[3]),
                                MaxSize = Convert.ToInt32(array[4]),
                                StartTime = Convert.ToInt32(array[5].Split(' ')[0]),
                                EndTime = Convert.ToInt32(array[5].Split(' ')[1]),
                                Season = array[6],
                                Weather = array[7],
                                DepthModifier = Convert.ToInt32(array[9]),
                                BaseChance = Convert.ToDouble(array[10]),
                                DropOffAmount = Convert.ToDouble(array[11]),
                                MinLevel = Convert.ToInt32(array[12]),
                            };
                        }
                        catch
                        { }
                    }
                    IsInitialized = true;
                }
                catch
                {
                    // do nothing
                }
            }
        }
        public static int WhichFish
        {
            get
            {
                if (!Active)
                    return -1;

                return (int)Reflector.GetValue((Game1.activeClickableMenu as BobberBar), "whichFish");
            }
        }

        public static bool Active
        {
            get
            {
                Init();
                if (Game1.activeClickableMenu is BobberBar)
                {
                    return true;
                }
                return false;
            }
        }

        public FishingGameState GameState()
        {
            if (!Active)
                return null;
            BobberBar bar = Game1.activeClickableMenu as BobberBar;
            return new FishingGameState()
            {
                bobberPosition = (float)Reflector.GetValue(bar, "bobberPosition"),
                bobberSpeed = (float)Reflector.GetValue(bar, "bobberSpeed"),
                bobberAcceleration = (float)Reflector.GetValue(bar, "bobberAcceleration"),
                bobberTargetPosition = (float)Reflector.GetValue(bar, "bobberTargetPosition"),
                scale = (float)Reflector.GetValue(bar, "scale"),
                everythingShakeTimer = (float)Reflector.GetValue(bar, "everythingShakeTimer"),
                floaterSinkerAcceleration = (float)Reflector.GetValue(bar, "floaterSinkerAcceleration"),
                bobberInBar = (bool)Reflector.GetValue(bar, "bobberInBar"),
                buttonPressed = (bool)Reflector.GetValue(bar, "buttonPressed"),
                bobberBarHeight = (int)Reflector.GetValue(bar, "bobberBarHeight"),
                bobberBarPos = (float)Reflector.GetValue(bar, "bobberBarPos"),
                bobberBarSpeed = (float)Reflector.GetValue(bar, "bobberBarSpeed"),
                distanceFromCatching = (float)Reflector.GetValue(bar, "distanceFromCatching"),
                whichBobber = (int)Reflector.GetValue(bar, "whichBobber"),

                perfect = (bool)Reflector.GetValue(bar, "perfect"),
                beginnersRod = (bool)Reflector.GetValue(bar, "beginnersRod"),
                treasure = (bool)Reflector.GetValue(bar, "treasure"),
                treasureCaught = (bool)Reflector.GetValue(bar, "treasureCaught"),
                barShake = (Vector2)Reflector.GetValue(bar, "barShake"),
                fishShake = (Vector2)Reflector.GetValue(bar, "fishShake"),
                treasureShake = (Vector2)Reflector.GetValue(bar, "treasureShake"),
                fishSize = (int)Reflector.GetValue(bar, "fishSize"),
                fishSizeReductionTimer = (int)Reflector.GetValue(bar, "fishSizeReductionTimer"),
                minFishSize = (int)Reflector.GetValue(bar, "minFishSize"),
                maxFishSize = (int)Reflector.GetValue(bar, "maxFishSize"),
                reelRotation = (float)Reflector.GetValue(bar, "reelRotation"),

                treasurePosition = (float)Reflector.GetValue(bar, "treasurePosition"),
                treasureCatchLevel = (float)Reflector.GetValue(bar, "treasureCatchLevel"),
                treasureAppearTimer = (float)Reflector.GetValue(bar, "treasureAppearTimer"),
                treasureScale = (float)Reflector.GetValue(bar, "treasureScale"),
                flipBubble = (bool)Reflector.GetValue(bar, "flipBubble"),
            };
        }

        public FishingGameState NextGameState(bool click, int numCalls)
        {
            FishingGameState current = GameState();
            if (current == null)
                return null;

            Random random = Game1.random.Copy();
            for (int i = 0; i < numCalls; ++i)
                random.NextDouble();
            return AdvanceFishing(click, current, random);
        }

        public static FishingGameState AdvanceFishing(bool click, FishingGameState state, Random random)
        {
            FishingGameState next = new FishingGameState(state);
            next.buttonPressed = click;
            FishData fish = data[WhichFish];
            int motionType = 0;
            switch (fish.MotionType.ToLower())
            {
                case "mixed":
                    motionType = 0;
                    break;
                case "dart":
                    motionType = 1;
                    break;
                case "smooth":
                    motionType = 2;
                    break;
                case "floater":
                    motionType = 4;
                    break;
                case "sinker":
                    motionType = 3;
                    break;
            }

            if (state.everythingShakeTimer > 0f)
            {
                random.Next(-10, 11);
                random.Next(-10, 11);
            }

            if (random.NextDouble() < (double)(fish.Difficulty * (float)((motionType != 2) ? 1 : 20) / 4000f) && (motionType != 2 || next.bobberTargetPosition == -1f))
            {
                float spaceBelow = 548f - next.bobberPosition;
                float spaceAbove = next.bobberPosition;
                float percent = Math.Min(99f, fish.Difficulty + (float)random.Next(10, 45)) / 100f;
                next.bobberTargetPosition = next.bobberPosition + (float)random.Next(-(int)spaceAbove, (int)spaceBelow) * percent;
            }
            if (motionType == 4)
            {
                next.floaterSinkerAcceleration = Math.Max(next.floaterSinkerAcceleration - 0.01f, -1.5f);
            }
            else if (motionType == 3)
            {
                next.floaterSinkerAcceleration = Math.Min(next.floaterSinkerAcceleration + 0.01f, 1.5f);
            }
            if (Math.Abs(next.bobberPosition - next.bobberTargetPosition) > 3f && next.bobberTargetPosition != -1f)
            {
                next.bobberAcceleration = (next.bobberTargetPosition - next.bobberPosition) / ((float)random.Next(10, 30) + (100f - Math.Min(100f, fish.Difficulty)));
                next.bobberSpeed += (next.bobberAcceleration - next.bobberSpeed) / 5f;
            }
            else if (motionType != 2 && random.NextDouble() < (double)(fish.Difficulty / 2000f))
            {
                next.bobberTargetPosition = next.bobberPosition + (float)((random.NextDouble() < 0.5) ? random.Next(-100, -51) : random.Next(50, 101));
            }
            else
            {
                next.bobberTargetPosition = -1f;
            }
            if (motionType == 1 && random.NextDouble() < (double)(fish.Difficulty / 1000f))
            {
                next.bobberTargetPosition = next.bobberPosition + (float)((random.NextDouble() < 0.5) ? random.Next(-100 - (int)fish.Difficulty * 2, -51) : random.Next(50, 101 + (int)fish.Difficulty * 2));
            }
            next.bobberTargetPosition = Math.Max(-1f, Math.Min(next.bobberTargetPosition, 548f));
            next.bobberPosition += next.bobberSpeed + next.floaterSinkerAcceleration;
            if (next.bobberPosition > 532f)
            {
                next.bobberPosition = 532f;
            }
            else if (next.bobberPosition < 0f)
            {
                next.bobberPosition = 0f;
            }
            next.bobberInBar = (next.bobberPosition + 12f <= next.bobberBarPos - 32f + (float)next.bobberBarHeight && next.bobberPosition - 16f >= next.bobberBarPos - 32f);
            if (next.bobberPosition >= (float)(548 - next.bobberBarHeight) && next.bobberBarPos >= (float)(568 - next.bobberBarHeight - 4))
            {
                next.bobberInBar = true;
            }

            float gravity = next.buttonPressed ? (-0.25f) : 0.25f;
            if (next.buttonPressed && gravity < 0f && (next.bobberBarPos == 0f || next.bobberBarPos == (float)(568 - next.bobberBarHeight)))
            {
                next.bobberBarSpeed = 0f;
            }
            if (next.bobberInBar)
            {
                gravity *= ((next.whichBobber == 691) ? 0.3f : 0.6f);
                if (next.whichBobber == 691)
                {
                    if (next.bobberPosition + 16f < next.bobberBarPos + (float)(next.bobberBarHeight / 2))
                    {
                        next.bobberBarSpeed -= 0.2f;
                    }
                    else
                    {
                        next.bobberBarSpeed += 0.2f;
                    }
                }
            }
            next.bobberBarSpeed += gravity;
            next.bobberBarPos += next.bobberBarSpeed;
            if (next.bobberBarPos + (float)next.bobberBarHeight > 568f)
            {
                next.bobberBarPos = 568 - next.bobberBarHeight;
                next.bobberBarSpeed = (0f - next.bobberBarSpeed) * 2f / 3f * ((next.whichBobber == 692) ? 0.1f : 1f);
            }
            else if (next.bobberBarPos < 0f)
            {
                next.bobberBarPos = 0f;
                next.bobberBarSpeed = (0f - next.bobberBarSpeed) * 2f / 3f;
            }
            bool treasureInBar = false;
            if (next.treasure)
            {
                float oldTreasureAppearTimer = next.treasureAppearTimer;
                next.treasureAppearTimer -= 16;
                if (next.treasureAppearTimer <= 0f)
                {
                    if (next.treasureScale < 1f && !next.treasureCaught)
                    {
                        if (oldTreasureAppearTimer > 0f)
                        {
                            next.treasurePosition = ((next.bobberBarPos > 274f) ? random.Next(8, (int)next.bobberBarPos - 20) : random.Next(Math.Min(528, (int)next.bobberBarPos + next.bobberBarHeight), 500));
                        }
                        next.treasureScale = Math.Min(1f, next.treasureScale + 0.1f);
                    }
                    treasureInBar = (next.treasurePosition + 12f <= next.bobberBarPos - 32f + (float)next.bobberBarHeight && next.treasurePosition - 16f >= next.bobberBarPos - 32f);
                    if (treasureInBar && !next.treasureCaught)
                    {
                        next.treasureCatchLevel += 0.0135f;
                        next.treasureShake = new Vector2(random.Next(-2, 3), random.Next(-2, 3));
                        if (next.treasureCatchLevel >= 1f)
                        {
                            next.treasureCaught = true;
                        }
                    }
                    else if (next.treasureCaught)
                    {
                        next.treasureScale = Math.Max(0f, next.treasureScale - 0.1f);
                    }
                    else
                    {
                        next.treasureShake = Vector2.Zero;
                        next.treasureCatchLevel = Math.Max(0f, next.treasureCatchLevel - 0.01f);
                    }
                }
            }
            if (next.bobberInBar)
            {
                next.distanceFromCatching += 0.002f;
                next.reelRotation += (float)Math.PI / 8f;
                next.fishShake.X = (float)random.Next(-10, 11) / 10f;
                next.fishShake.Y = (float)random.Next(-10, 11) / 10f;
                next.barShake = Vector2.Zero;
            }
            else if (!treasureInBar || next.treasureCaught || next.whichBobber != 693)
            {
                if (!next.fishShake.Equals(Vector2.Zero))
                {
                    next.perfect = false;
                }
                next.fishSizeReductionTimer -= 16;
                if (next.fishSizeReductionTimer <= 0)
                {
                    next.fishSize = Math.Max(next.minFishSize, next.fishSize - 1);
                    next.fishSizeReductionTimer = 800;
                }
                if ((Game1.player.fishCaught != null && Game1.player.fishCaught.Count() != 0) || Game1.currentMinigame != null)
                {
                    next.distanceFromCatching -= ((next.whichBobber == 694 || next.beginnersRod) ? 0.002f : 0.003f);
                }
                float distanceAway = Math.Abs(next.bobberPosition - (next.bobberBarPos + (float)(next.bobberBarHeight / 2)));
                next.reelRotation -= (float)Math.PI / Math.Max(10f, 200f - distanceAway);
                next.barShake.X = (float)random.Next(-10, 11) / 10f;
                next.barShake.Y = (float)random.Next(-10, 11) / 10f;
                next.fishShake = Vector2.Zero;
            }
            next.distanceFromCatching = Math.Max(0f, Math.Min(1f, next.distanceFromCatching));
            // where the decision is made to tick random an extra time
            //if (Game1.player.CurrentTool != null)
            //{
            //    Game1.player.CurrentTool.tickUpdate(time, Game1.player);
            //}
            if (next.distanceFromCatching <= 0f)
            {
            }
            else if (next.distanceFromCatching >= 1f)
            {

                if (next.perfect)
                {
                }
                else if (next.fishSize == next.maxFishSize)
                {
                    next.fishSize--;
                }
            }

            if (next.bobberPosition < 0f)
            {
                next.bobberPosition = 0f;
            }
            if (next.bobberPosition > 548f)
            {
                next.bobberPosition = 548f;
            }

            return next;
        }
    }
}
