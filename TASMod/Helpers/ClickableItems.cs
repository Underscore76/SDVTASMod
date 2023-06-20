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
    public static class ClickableItems
    {
        public static Dictionary<int, string> ID_TO_NAME_MAP;
        static ClickableItems()
        {
            ID_TO_NAME_MAP = new Dictionary<int, string>();
            ID_TO_NAME_MAP[101] = "QuestLog_ForwardButton";
            ID_TO_NAME_MAP[102] = "QuestLog_BackButton";
            ID_TO_NAME_MAP[103] = "QuestLog_RewardBox";
            ID_TO_NAME_MAP[104] = "QuestLog_CancelQuestButton";
            ID_TO_NAME_MAP[105] = "QuestLog_UpArrow";
            ID_TO_NAME_MAP[106] = "QuestLog_DownArrow";

            ID_TO_NAME_MAP[536] = "CharacterCustomization_nameBoxCC";
            ID_TO_NAME_MAP[537] = "CharacterCustomization_farmnameBoxCC";
            ID_TO_NAME_MAP[538] = "CharacterCustomization_favThingBoxCC";
        }

        public struct ClickObject
        {
            public string Name;
            public int ID;
            public Rectangle Rect;

            public ClickObject(ClickableComponent comp)
            {
                ID = comp.myID;
                Rect = comp.bounds;
                if (comp.name == "")
                {
                    if (ID_TO_NAME_MAP.ContainsKey(ID))
                        Name = ID_TO_NAME_MAP[ID];
                    else
                        Name = ID.ToString();
                }
                else
                {
                    Name = string.Format("{0}_{1}", comp.name, ID);

                }
            }
        }
        public static List<ClickObject> GetClickObjects()
        {
            List<ClickObject> items = new List<ClickObject>();
            if (CurrentMenu.Active)
            {
                IClickableMenu current = Game1.activeClickableMenu;
                if (current is TitleMenu menu && TitleMenu.subMenu != null)
                {
                    current = TitleMenu.subMenu;
                }
                foreach (var field in Reflector.GetFields<ClickableComponent>(current.GetType()))
                {
                    ClickableComponent c = (ClickableComponent)Reflector.GetValue(current, field);
                    if (c == null) continue;
                    ClickObject clickObject = new ClickObject(c);
                    if (clickObject.Name == "-500")
                        continue;
                    items.Add(clickObject);
                }
                foreach (var field in Reflector.GetFields<ClickableTextureComponent>(current.GetType()))
                {
                    ClickableTextureComponent c = (ClickableTextureComponent)Reflector.GetValue(current, field);
                    if (c == null) continue;
                    ClickObject clickObject = new ClickObject(c);
                    if (clickObject.Name == "-500")
                        continue;
                    items.Add(clickObject);
                }
                foreach (var field in Reflector.GetListFields<ClickableComponent>(current.GetType()))
                {
                    var l = (List<ClickableComponent>)Reflector.GetValue(current, field);
                    if (l == null) continue;
                    foreach (ClickableComponent c in l)
                    {
                        if (c == null) continue;
                        ClickObject clickObject = new ClickObject(c);
                        if (clickObject.Name == "-500")
                            continue;
                        items.Add(clickObject);
                    }
                }
                foreach (string field in Reflector.GetListFields<ClickableTextureComponent>(current.GetType()))
                {
                    List<ClickableTextureComponent> l;
                    try
                    {
                        l = (List<ClickableTextureComponent>)Reflector.GetValue(current, field);
                    }
                    catch
                    {
                        l = null;
                    }
                    if (l == null) continue;
                    foreach (ClickableTextureComponent c in l)
                    {
                        if (c == null) continue;
                        ClickObject clickObject = new ClickObject(c);
                        if (clickObject.Name == "-500")
                            continue;
                        items.Add(clickObject);
                    }
                }
            }
            return items;
        }
    }
}
