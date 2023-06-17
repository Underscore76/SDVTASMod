using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using System.IO;

namespace TASMod
{
	internal static class ContentManager
	{
        /// <summary>
        /// All assets that have been modified.
        /// The dictionary key is the name of the asset as the game requests it,
        /// the value is the filepath of the asset to replace it with.
        /// </summary>
        private static readonly Dictionary<string, string> ModifiedAssets = new()
        {
            { "Fonts/ConsoleFont", "assets/ConsoleFont.xnb" }
        };

        /// <summary>
        /// Event handler for when a game asset is requested to be loaded.
        /// This handles loading all custom assets as defined in <see cref="ModifiedAssets"/>.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="AssetRequestedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="NotImplementedException">If a file attempting to be loaded has no implemented way to be loaded.</exception>
        public static void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (ModifiedAssets.TryGetValue(e.Name.BaseName, out string? fromPath))
            {
                string extension = Path.GetExtension(fromPath);
                switch (extension)
                {
                    case ".xnb":
                        e.LoadFromModFile<SpriteFont>(fromPath, AssetLoadPriority.High);
                        return;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}

