using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using StardewValley;
using TASMod.Helpers;

namespace TASMod.Console.Commands
{
    public class GetFriendship : IConsoleCommand
    {
        public override string Name => "friendship";
        public static GetFriendship _instance;
        public GetFriendship()
        {
            _instance = this;
        }
        public override string Description => "Get friendship info for npcs";
        public override string[] Usage => new string[]
            {
                string.Format("{0}: get friendship info", Name),
                string.Format("all friendships: \"{0}\"", Name),
                string.Format("single friendship: \"{0} <name>\"", Name)
            };


        public override void Run(string[] tokens)
        {
            if (tokens.Length > 1)
            {
                Write(HelpText());
            }
            else if (tokens.Length == 1)
            {
                if (PlayerInfo.Friendships.ContainsKey(tokens[0]))
                {
                    Write("\t{0}: {1}/{2}", tokens[0], PlayerInfo.Friendships[tokens[0]].Points, PlayerInfo.Friendships[tokens[0]].Points / 250);
                }
                else
                {
                    Write("invalid name: {0} is not a npc with friendship", tokens[0]);
                }
            }
            else
            {
                foreach (string npc in PlayerInfo.Friendships.Keys)
                {
                    Write("\t{0}: {1}/{2}", npc, PlayerInfo.Friendships[npc].Points, PlayerInfo.Friendships[npc].Points / 250);
                }
            }
        }
    }

    public class GetPlayer : IConsoleCommand
    {
        public override string Name => "player";
        public override string Description => "get player data info";
        public override string[] Usage => new string[]
        {
            $"\"{Name}\" - all player data",
            $"\"{Name} pos\" - current position",
            $"\"{Name} luck\" - current luck",
            $"\"{Name} steps\" - current step count",
            $"\"{Name} hp\" - current hp",
            $"\"{Name} energy\" - current energy",
            $"\"{Name} xp\" - all skill xp",
            $"\"{Name} friendship\" - all friendships data",
        };

        public override void Run(string[] tokens)
        {
            if (tokens.Length > 1)
            {
                Write(HelpText());
            }
            if (tokens.Length == 0)
            {
                tokens = new string[] { "pos", "luck", "steps", "hp", "energy", "xp", "friendship" };
            }
            foreach (var token in tokens)
            {
                switch (token)
                {
                    case "pos":
                    case "position":
                        Write("\tPosition: {0}, {1}", Game1.player.getStandingX() / Game1.tileSize, Game1.player.getStandingY() / Game1.tileSize);
                        break;
                    case "luck":
                        Write("\tDaily Luck: {0}", Game1.player.DailyLuck);
                        break;
                    case "steps":
                        Write("\tSteps Taken: {0}", Game1.stats.StepsTaken);
                        break;
                    case "energy":
                        Write("\tEnergy: {0}", Game1.player.Stamina);
                        break;
                    case "hp":
                        Write("\tHealth: {0}", Game1.player.health);
                        break;
                    case "xp":
                        Write("\tExperience:");
                        Write("\t\tFarming: {0}", Game1.player.experiencePoints[0]);
                        Write("\t\tFishing: {0}", Game1.player.experiencePoints[1]);
                        Write("\t\tForage : {0}", Game1.player.experiencePoints[2]);
                        Write("\t\tMining : {0}", Game1.player.experiencePoints[3]);
                        Write("\t\tCombat : {0}", Game1.player.experiencePoints[4]);
                        break;
                    case "friend":
                    case "friendship":
                    case "friendships":
                        Write("\tFriendships:");
                        GetFriendship._instance.Run();
                        break;
                    default:
                        Write("invalid token: {0} not in list of options", token);
                        return;
                }
            }
        }
    }

}
