using System;
using System.Collections.Generic;
using StardewValley;
using StardewValley.Menus;

namespace TASMod.Helpers
{
    public class CurrentMenu
    {
        public static bool Active { get { return Game1.activeClickableMenu != null; } }

        public static IClickableMenu Menu { get { return Game1.activeClickableMenu; } }
        public static IClickableMenu SubMenu
        {
            get
            {
                if (Game1.activeClickableMenu is TitleMenu)
                    return TitleMenu.subMenu;
                return null;
            }
        }
        // Save Game functions
        public static bool IsSaveGame { get { return Game1.activeClickableMenu is SaveGameMenu; } }
        public static bool CanQuit { get { return (Game1.activeClickableMenu as SaveGameMenu).quit; } }

        // Dialogue Box functions
        public static bool IsDialogue { get { return Game1.activeClickableMenu is DialogueBox; } }
        public static bool Transitioning
        {
            get
            {
                return (Game1.activeClickableMenu as DialogueBox).transitioning;
            }
        }
        public static bool IsQuestion
        {
            get
            {
                return (Game1.activeClickableMenu as DialogueBox).isQuestion;
            }
        }
        public static int CharacterIndexInDialogue
        {
            get
            {
                return (Game1.activeClickableMenu as DialogueBox).characterIndexInDialogue;
            }
        }
        public static int SafetyTimer
        {
            get
            {
                return (Game1.activeClickableMenu as DialogueBox).safetyTimer;
            }
        }
        public static string CurrentString
        {
            get
            {
                return (Game1.activeClickableMenu as DialogueBox).getCurrentString();
            }
        }

        public static Dictionary<string, bool> GetPageRecipes(CraftingPage page)
        {
            int currentCraftingPage = (int)Reflector.GetValue(page, "currentCraftingPage");
            List<Item> containerContents = Reflector.InvokeMethod<CraftingPage,List<Item>>(page, "getContainerContents");
            Dictionary<string, bool> recipes = new Dictionary<string, bool>();
            foreach (ClickableTextureComponent key in page.pagesOfCraftingRecipes[currentCraftingPage].Keys)
            {
                string name = page.pagesOfCraftingRecipes[currentCraftingPage][key].name;
                recipes.Add(name,
                    page.pagesOfCraftingRecipes[currentCraftingPage][key].doesFarmerHaveIngredientsInInventory(containerContents)
                );
            }
            return recipes;
        }
        public static Dictionary<string, bool> CraftingPageRecipes
        {
            get
            {
                if (Game1.activeClickableMenu is GameMenu menu)
                {
                    if (menu.pages[menu.currentTab] is CraftingPage page)
                    {
                        return GetPageRecipes(page);
                    }
                }
                if (Game1.activeClickableMenu is CraftingPage page1)
                {
                    return GetPageRecipes(page1);
                }
                return null;
            }
        }
        public static int HeldItemStack
        {
            get
            {
                CraftingPage page = null;
                if (Game1.activeClickableMenu is GameMenu menu)
                {
                    if (menu.pages[menu.currentTab] is CraftingPage p)
                    {
                        page = p;
                    }
                }
                if (Game1.activeClickableMenu is CraftingPage p2)
                {
                    page = p2;
                }
                if (page != null)
                {
                        Item item = (Item)Reflector.GetValue(page, "heldItem");
                        if (item == null)
                        {
                            return 0;
                        }
                        return item.Stack;
                }
                return 0;
            }
        }
    }
}

