using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TASMod.Helpers;

namespace TASMod.Console.Commands
{
    public class GetTrashCans : IConsoleCommand
    {
        private Dictionary<string, Tuple<Vector2, int>> TrashCans;
        public GetTrashCans()
        {
            TrashCans = new Dictionary<string, Tuple<Vector2, int>>();
            TrashCans.Add("Jodi  ", new Tuple<Vector2, int>(new Vector2(13, 86), 0));
            TrashCans.Add("Emily ", new Tuple<Vector2, int>(new Vector2(19, 89), 1));
            TrashCans.Add("Lewis ", new Tuple<Vector2, int>(new Vector2(56, 85), 2));
            TrashCans.Add("Museum", new Tuple<Vector2, int>(new Vector2(108, 91), 3));
            TrashCans.Add("Clint ", new Tuple<Vector2, int>(new Vector2(97, 80), 4));
            TrashCans.Add("Saloon", new Tuple<Vector2, int>(new Vector2(47, 70), 5));
            TrashCans.Add("Alex  ", new Tuple<Vector2, int>(new Vector2(52, 63), 6));
            TrashCans.Add("Joja  ", new Tuple<Vector2, int>(new Vector2(110, 56), 7));
        }

        public override string Name => "trashcans";

        public override string Description => "get trash can drops";

        public override void Run(string[] tokens)
        {
            if (!CurrentLocation.Active)
                return;

            foreach (var can in TrashCans)
            {
                string trashItem = CurrentLocation.CheckTrash(can.Value.Item1, can.Value.Item2);
                if (trashItem != "")
                    Write("\t{0}: {1}", can.Key, trashItem.Split("/")[0]);
            }
        }
    }
}
