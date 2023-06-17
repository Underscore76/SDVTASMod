using System;
using StardewValley;
using StardewValley.Menus;

namespace TASMod.Helpers
{
    public static class CurrentMenu
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
    }
}

