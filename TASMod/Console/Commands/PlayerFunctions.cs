using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
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
}
