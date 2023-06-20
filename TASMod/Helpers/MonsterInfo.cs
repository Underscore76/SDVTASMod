using Netcode;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Tools;
using StardewValley;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TASMod.Extensions;
using TASMod.System;
using Microsoft.Xna.Framework;

namespace TASMod.Helpers
{
    public class MonsterInfo
    {
        public class MonsterHit
        {
            public List<string> Drops = new();
            public float Damage = 0f;
            public bool SpawnsLadder = false;
        }

        private static Random random;
        private static float damage = 0;
        private static ulong CurrentFrame;

        public static float Damage
        {
            get
            {
                if (NeedsUpdate())
                    RecomputeStats();
                return damage;
            }
        }

        public static bool NeedsUpdate()
        {
            return CurrentFrame != TASDateTime.CurrentFrame;
        }

        public static List<MonsterHit> Hits
        {
            get
            {
                return CheckHits();
            }
        }

        public static void RecomputeStats()
        {
            random = Game1.random.Copy();
            CurrentFrame = TASDateTime.CurrentFrame;
        }

        public static Rectangle GetAreaOfEffect(Random random)
        {
            return GetAreaOfEffect(random, 0);
        }
        public static Rectangle GetAreaOfEffect(Random random, int index)
        {
            Vector2 toolLoc = PlayerInfo.GetToolLocation(Game1.player.facingDirection);
            Vector2 tileLoc = Vector2.Zero;
            Vector2 tileLoc2 = Vector2.Zero;
            return WeaponInfo.GetAreaOfEffect(Game1.player.CurrentTool as MeleeWeapon, (int)toolLoc.X, (int)toolLoc.Y, Game1.player.facingDirection, ref tileLoc, ref tileLoc2, PlayerInfo.BoundingBox, index, random);
        }

        public static List<MonsterHit> CheckHits()
        {
            Random random = Game1.random.Copy();
            List<MonsterHit> hits = new List<MonsterHit>();
            // DoDamage
            if (!(Game1.player.CurrentTool is MeleeWeapon))
                return hits;
            if (Game1.player.UsingTool)
            {
                Vector2 position = ((!Game1.wasMouseVisibleThisFrame) ? Game1.player.GetToolLocation() : new Vector2(Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y));
                return Tool_DoFunction(Game1.player.CurrentTool, Game1.currentLocation, (int)position.X, (int)position.Y, 1, Game1.player, random);
            }
            MeleeWeapon weapon = Game1.player.CurrentTool as MeleeWeapon;
            Farmer who = Game1.player;
            Vector2 activeTile = who.GetToolLocation(ignoreClick: true);

            if (weapon.type.Value != 2)
            {
                hits.AddRange(Tool_DoFunction(weapon, Game1.currentLocation, (int)activeTile.X, (int)activeTile.Y, 1, who, random));
            }
            Rectangle areaOfEffect = GetAreaOfEffect(random);
            float critChance = weapon.critChance.Value;
            float critMultiplier = weapon.critMultiplier.Value;
            if ((int)weapon.type.Value == 1)
            {
                critChance += 0.005f;
                critChance *= 1.12f;
            }
            int minDamage = (int)(weapon.minDamage.Value * (1f + who.attackIncreaseModifier));
            int addedPrecision = (int)(weapon.addedPrecision.Value * (1f + who.weaponPrecisionModifier));
            int maxDamage = (int)(weapon.maxDamage.Value * (1f + who.attackIncreaseModifier));
            float knockback = (float)weapon.knockback.Value * (1f + who.knockbackModifier);
            // GameLocation.damageMonster
            hits.AddRange(Location_damageMonster(Game1.currentLocation, areaOfEffect, minDamage, maxDamage, false, knockback, addedPrecision, critChance, critMultiplier, who, random));
            return hits;
        }

        public static List<string> monsterDrops(Monster monster, Random random, out bool spawnsLadder)
        {
            spawnsLadder = false;
            int x = monster.GetBoundingBox().Center.X;
            int y = monster.GetBoundingBox().Center.Y;
            MineShaft location = Game1.currentLocation as MineShaft;
            int mineLevel = CurrentLocation.MineLevel;
            Farmer who = Game1.player;

            List<string> items = new List<string>();
            // MineShaft::monsterDrop
            if (monster.hasSpecialItem.Value)
            {
                items.AddRange(DropInfo.GetSpecialItemForThisMineLevel(mineLevel, x / 64, y / 64));
                random.Next(4);
            }
            else if (mineLevel > 121 && who != null && who.getFriendshipHeartLevelForNPC("Krobus") >= 10 && (int)who.HouseUpgradeLevel >= 1 && !who.isMarried() && !who.isEngaged() && random.NextDouble() < 0.001)
            {
                items.Add(DropInfo.ObjectName(808));
                random.Next(4);
            }
            else
            {
                items.AddRange(baseMonsterDrops(monster, x, y, who, random));
            }

            if ((CurrentLocation.MustKillAllMonstersToAdvance() || !(random.NextDouble() < 0.15)) && (!CurrentLocation.MustKillAllMonstersToAdvance() || CurrentLocation.EnemyCount > 1))
            {
                return items;
            }

            // Why...
            Vector2 p = new Vector2(x, y) / 64f;
            p.X = (int)p.X;
            p.Y = (int)p.Y;
            Microsoft.Xna.Framework.Rectangle tileRect = new Microsoft.Xna.Framework.Rectangle((int)p.X * 64, (int)p.Y * 64, 64, 64);
            string tmp = monster.Name;
            monster.Name = "ignoreMe";
            bool flag = !location.isTileOccupied(p, "ignoreMe") && location.isTileOnClearAndSolidGround(p) && !Game1.player.GetBoundingBox().Intersects(tileRect) && location.doesTileHaveProperty((int)p.X, (int)p.Y, "Type", "Back") != null && location.doesTileHaveProperty((int)p.X, (int)p.Y, "Type", "Back").Equals("Stone");
            monster.Name = tmp;
            if (flag)
            {
                spawnsLadder = true;
            }
            else if (CurrentLocation.MustKillAllMonstersToAdvance() && CurrentLocation.EnemyCount <= 1)
            {
                spawnsLadder = true;
            }
            return items;
        }

        public static List<string> baseMonsterDrops(Monster monster, int x, int y, Farmer who, Random random)
        {
            List<string> items = new List<string>();
            List<string> extraDrops = new List<string>();
            if (monster is GreenSlime slime)
                extraDrops.AddRange(GreenSlime_ExtraDropItems(slime, random));
            else if (monster is Bat bat)
                extraDrops.AddRange(Bat_ExtraDropItems(bat, random));

            // TODO: wearing ring stuff
            // actual items
            foreach (var obj in monster.objectsToDrop)
            {
                items.Add(DropInfo.ObjectName(obj));
                if (obj < 0)
                    random.Next(1, 4);
            }
            foreach (string item in extraDrops)
            {
                items.Add(item);
            }
            // TODO: Wearing ring stuff
            // TODO: Magnifying glass
            return items;
        }

        public static List<string> GreenSlime_ExtraDropItems(GreenSlime slime, Random gameRandom)
        {
            Color color = slime.color.Value;
            List<string> extra = new List<string>();
            if (color.R < 80 && color.G < 80 && color.B < 80)
            {
                extra.Add(DropInfo.ObjectName(382)); gameRandom.Next();
                Random random = new Random((int)slime.Position.X * 777 + (int)slime.Position.Y * 77 + (int)Game1.stats.DaysPlayed);
                if (random.NextDouble() < 0.05)
                {
                    extra.Add(DropInfo.ObjectName(553)); gameRandom.Next();
                }
                if (random.NextDouble() < 0.05)
                {
                    extra.Add(DropInfo.ObjectName(539)); gameRandom.Next();
                }
            }
            else if (color.R > 200 && color.G > 180 && color.B < 50)
            {
                extra.Add(DropInfo.ObjectName(384) + "x2"); gameRandom.Next();
            }
            else if (color.R > 220 && color.G > 90 && color.G < 150 && color.B < 50)
            {
                extra.Add(DropInfo.ObjectName(378) + "x2"); gameRandom.Next();
            }
            else if (color.R > 230 && color.G > 230 && color.B > 230)
            {
                extra.Add(DropInfo.ObjectName(380)); gameRandom.Next();
                if ((int)color.R % 2 == 0 && (int)color.G % 2 == 0 && (int)color.B % 2 == 0)
                {
                    extra.Add(DropInfo.ObjectName(72)); gameRandom.Next();
                }
            }
            else if (color.R > 150 && color.G > 150 && color.B > 150)
            {
                extra.Add(DropInfo.ObjectName(390) + "x2"); gameRandom.Next();
            }
            else if (color.R > 150 && color.B > 180 && color.G < 50 && (int)slime.specialNumber.Value % (slime.firstGeneration.Value ? 4 : 2) == 0)
            {
                extra.Add(DropInfo.ObjectName(386) + "x2"); gameRandom.Next();
            }
            if (Game1.MasterPlayer.mailReceived.Contains("slimeHutchBuilt") && (int)slime.specialNumber.Value == 1)
            {
                string name = slime.Name;
                if (!(name == "Green Slime"))
                {
                    if (name == "Frost Jelly")
                    {
                        extra.Add(DropInfo.ObjectName(413)); gameRandom.Next();
                    }
                }
                else
                {
                    extra.Add(DropInfo.ObjectName(680)); gameRandom.Next();
                }
            }
            return extra;
        }

        public static List<string> Bat_ExtraDropItems(Bat bat, Random gameRandom)
        {
            List<string> extraDrops = new List<string>();
            if (bat.cursedDoll.Value && gameRandom.NextDouble() < 0.1429 && bat.hauntedSkull.Value)
            {
                switch (gameRandom.Next(11))
                {
                    case 0:
                        //switch (Game1.random.Next(6))
                        //{
                        //    case 0:
                        //        {
                        //            Clothing v = new Clothing(10);
                        //            v.clothesColor.Value = Color.DimGray;
                        //            extraDrops.Add(v);
                        //            break;
                        //        }
                        //    case 1:
                        //        extraDrops.Add(new Clothing(1004));
                        //        break;
                        //    case 2:
                        //        extraDrops.Add(new Clothing(1014));
                        //        break;
                        //    case 3:
                        //        extraDrops.Add(new Clothing(1263));
                        //        break;
                        //    case 4:
                        //        extraDrops.Add(new Clothing(1262));
                        //        break;
                        //    case 5:
                        //        {
                        //            Clothing v = new Clothing(12);
                        //            v.clothesColor.Value = Color.DimGray;
                        //            extraDrops.Add(v);
                        //            break;
                        //        }
                        //}
                        extraDrops.Add("clothing");
                        break;
                    case 1:
                        {
                            extraDrops.Add("Dark Sword");
                            //MeleeWeapon weapon = new MeleeWeapon(2);
                            //weapon.AddEnchantment(new VampiricEnchantment());
                            //extraDrops.Add(weapon);
                            break;
                        }
                    case 2:
                        extraDrops.Add(DropInfo.ObjectName(288));
                        break;
                    case 3:
                        extraDrops.Add("Ruby Ring");
                        break;
                    case 4:
                        extraDrops.Add("Aq Ring");
                        break;
                    case 5:
                        do
                        {
                            extraDrops.Add(DropInfo.ObjectName(768));
                            extraDrops.Add(DropInfo.ObjectName(769));
                        }
                        while (gameRandom.NextDouble() < 0.33);
                        break;
                    case 6:
                        extraDrops.Add(DropInfo.ObjectName(581));
                        break;
                    case 7:
                        extraDrops.Add(DropInfo.ObjectName(582));
                        break;
                    case 8:
                        extraDrops.Add(DropInfo.ObjectName(725));
                        break;
                    case 9:
                        extraDrops.Add(DropInfo.ObjectName(86));
                        break;
                    case 10:
                        if (Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccVault"))
                        {
                            extraDrops.Add(DropInfo.ObjectName(275));
                        }
                        else
                        {
                            extraDrops.Add(DropInfo.ObjectName(749));
                        }
                        break;
                }
                return extraDrops;
            }
            if (bat.hauntedSkull.Value && gameRandom.NextDouble() < 0.25 && Game1.currentSeason == "winter")
            {
                do
                {
                    extraDrops.Add(DropInfo.ObjectName(273));
                }
                while (gameRandom.NextDouble() < 0.4);
            }
            if (bat.hauntedSkull.Value && gameRandom.NextDouble() < 0.001502)
            {
                extraDrops.Add(DropInfo.ObjectName(279));
            }
            return new List<string>();
        }

        public static List<MonsterHit> Tool_DoFunction(Tool tool, GameLocation location, int x, int y, int power, Farmer who, Random random)
        {
            List<MonsterHit> hits = new List<MonsterHit>();
            random.Next(); // recentMultiplayerRandom
            if (tool.isHeavyHitter() && !(tool is MeleeWeapon))
            {
                random.NextDouble();
                random.Next();
                hits.AddRange(Location_damageMonster(location, new Rectangle(x - 32, y - 32, 64, 64), (int)tool.UpgradeLevel + 1, (int)(tool.UpgradeLevel + 1) * 3, who, random));
            }
            if (tool is MeleeWeapon weapon)
            {
                if (weapon.type.Value == 2 && weapon.isOnSpecial)
                {
                    hits.AddRange(MeleeWeapon_triggerClubFunction(weapon, who, random));
                }
            }
            return hits;
        }


        public static List<MonsterHit> MeleeWeapon_triggerClubFunction(MeleeWeapon weapon, Farmer who, Random random)
        {
            Rectangle areaOfEffect = new Rectangle((int)who.Position.X - 192, who.GetBoundingBox().Y - 192, 384, 384);
            List<MonsterHit> hits = Location_damageMonster(Game1.currentLocation, areaOfEffect, weapon.minDamage.Value, weapon.maxDamage.Value, false, 1.5f, 100, 0f, 1f, who, random);
            random.Next(); // viewport wobble
            return hits;
        }
        public static List<MonsterHit> Location_damageMonster(GameLocation location, Rectangle areaOfEffect, int minDamage, int maxDamage, Farmer who, Random random)
        {
            return Location_damageMonster(location, areaOfEffect, minDamage, maxDamage, false, 1f, 0, 0f, 1f, who, random);
        }
        public static List<MonsterHit> Location_damageMonster(GameLocation location, Rectangle areaOfEffect, int minDamage, int maxDamage, bool isBomb, float knockback, int addedPrecision, float critChance, float critMultiplier, Farmer who, Random random)
        {
            List<MonsterHit> monsterHits = new List<MonsterHit>();
            foreach (Monster monster in CurrentLocation.Monsters.Reverse())
            {
                if (monster != null && monster.IsMonster && monster.Health > 0 && monster.TakesDamageFromHitbox(areaOfEffect))
                {
                    MonsterHit hit = new MonsterHit();
                    bool crit = false;
                    int damageAmount = 0;
                    // rumble the controller
                    random.NextDouble();
                    random.Next();
                    // trajectory
                    random.Next();
                    random.Next();
                    //
                    if (monster.hitWithTool(who.CurrentTool))
                    {
                        return monsterHits;
                    }
                    if (who.professions.Contains(25))
                    {
                        critChance += critChance * 0.5f;
                    }
                    if (maxDamage >= 0)
                    {
                        damageAmount = random.Next(minDamage, maxDamage + 1);
                        if (who != null && random.NextDouble() < (double)(critChance + (float)who.LuckLevel * (critChance / 40f)))
                        {
                            crit = true;
                        }
                        damageAmount = (crit ? ((int)((float)damageAmount * critMultiplier)) : damageAmount);
                        damageAmount = Math.Max(1, damageAmount + ((who != null) ? (who.attack * 3) : 0));
                        if (who != null && who.professions.Contains(24))
                        {
                            damageAmount = (int)Math.Ceiling((float)damageAmount * 1.1f);
                        }
                        if (who != null && who.professions.Contains(26))
                        {
                            damageAmount = (int)Math.Ceiling((float)damageAmount * 1.15f);
                        }
                        if (who != null && crit && who.professions.Contains(29))
                        {
                            damageAmount = (int)((float)damageAmount * 2f);
                        }
                        //if (who != null)
                        //{
                        //    foreach (BaseEnchantment enchantment in who.enchantments)
                        //    {
                        //        enchantment.OnCalculateDamage(monster, this, who, ref damageAmount);
                        //    }
                        //}
                        damageAmount = Monster_takeDamage(monster, damageAmount, addedPrecision / 10.0, who, random);
                        //damageAmount = monster.takeDamage(damageAmount, (int)trajectory.X, (int)trajectory.Y, isBomb, (double)addedPrecision / 10.0, who);

                        if (damageAmount == -1)
                        {
                        }
                        else
                        {
                            // create the debris
                            random.Next();

                            //if (who != null)
                            //{
                            //    foreach (BaseEnchantment enchantment2 in who.enchantments)
                            //    {
                            //        enchantment2.OnDealDamage(monster, this, who, ref damageAmount);
                            //    }
                            //}
                        }
                    }
                    else
                    {
                        damageAmount = -2;
                    }
                    hit.Damage = damageAmount;
                    if (who != null && who.CurrentTool != null && who.CurrentTool.Name.Equals("Galaxy Sword"))
                    {
                        // random calls that probably cascade
                    }
                    if (monster.Health - hit.Damage <= 0)
                    {
                        // enchant slaying
                        // ring slaying
                        hit.Drops = monsterDrops(monster, random, out hit.SpawnsLadder);
                    }
                    else if (damageAmount > 0)
                    {
                        // generate a TON of random calls
                        shedChunks(random.Next(1, 3), random);
                        if (crit)
                        {
                            for (int i = 0; i < 14 ; i++) { random.Next(); }
                        }
                    }
                    monsterHits.Add(hit);

                }
            }
            return monsterHits;
        }

        public static int Monster_takeDamage(Monster monster, int damage, double addedPrecision, Farmer who, Random random)
        {
            int resilience = monster.resilience.Value;
            double missChance = monster.missChance.Value;
            int health = monster.Health;
            int actualDamage = damage;
            if (monster is GreenSlime greenSlime)
            {
                actualDamage = Math.Max(1, damage - resilience);
                if (random.NextDouble() < missChance - missChance * addedPrecision)
                {
                    actualDamage = -1;
                }
                else
                {
                    if (random.NextDouble() < 0.025 && (bool)greenSlime.cute.Value)
                    {
                        // set farmer focus
                    }
                    if (health - actualDamage <= 0)
                    {
                        if (Game1.gameMode == 3 && greenSlime.Scale > 1.8f)
                        {
                            // create new slimes
                        }
                        else
                        {
                            // add temp sprites
                        }
                    }
                }
                return actualDamage;
            }
            else if (monster is Bug bug)
            {
                actualDamage = Math.Max(1, damage - resilience);
                if (bug.isArmoredBug.Value) return 0;
                if (random.NextDouble() < missChance - missChance * addedPrecision)
                {
                    actualDamage = -1;
                }
                else
                {
                    if (bug.isHardModeMonster.Value)
                    {
                        random.Next(); // set facing direction
                    }
                    if (health - actualDamage <= 0)
                    {
                        // death animation
                        shedChunks(random.Next(4, 9), random);
                        for (int i = 0; i < 8; i++) { random.Next(); } // SpriteJuicer
                    }
                }
            }
            else if (monster is Grub grub)
            {
                actualDamage = Math.Max(1, damage - resilience);
                if (random.NextDouble() < missChance - missChance * addedPrecision)
                {
                    actualDamage = -1;
                }
                else
                {
                    if ((NetBool)Reflector.GetValue(grub, "pupating"))
                    {
                        return 0;
                    }
                    if (health - actualDamage <= 0)
                    {
                        for (int i = 0; i < 8; i++) { random.Next(); } // SpriteJuicer from default death animation
                    }
                }
            }
            else if (monster is Bat bat)
            {
                actualDamage = Math.Max(1, damage - resilience);
                if (random.NextDouble() < missChance - missChance * addedPrecision)
                {
                    actualDamage = -1;
                }
                else
                {
                    if (health - actualDamage <= 0)
                    {
                        for (int i = 0; i < 8; i++) { random.Next(); } // SpriteJuicer from default death animation
                    }
                }
                random.Next(); // change speed
            }
            else if (monster is Fly fly)
            {
                actualDamage = Math.Max(1, damage - resilience);
                if (random.NextDouble() < missChance - missChance * addedPrecision)
                {
                    actualDamage = -1;
                }
                else
                {
                    if (health - actualDamage <= 0)
                    {
                        for (int i = 0; i < 8; i++) { random.Next(); } // SpriteJuicer from default death animation
                    }
                }
                random.Next(); // change speed
            }
            else
            {
                Debug.WriteLine("monster type not found in take damage handler");
            }
            return actualDamage;
        }

        public static void shedChunks(int numChunks, Random random)
        {
            for (int i = 0; i < numChunks; i++)
            {
                // compute debris offsets
                if (random.Next(4) >= 2)
                {
                    random.Next();
                }
                // debris constructor
                random.Next();
                random.Next();
                random.Next();
                random.Next();
            }
        }
    }
}
