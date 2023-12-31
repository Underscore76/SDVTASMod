using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TASMod.System;
using StardewValley;
using xTile.Layers;
using Microsoft.Xna.Framework.Input;
using TASMod.Inputs;
using TASMod.Extensions;
using StardewValley.Buildings;
using StardewValley.TerrainFeatures;
using TASMod.Monogame.Framework;
using System.Reflection;

namespace TASMod.Views
{
	public class MapView
	{
        public xTile.Dimensions.Rectangle OldViewport;
        public xTile.Dimensions.Rectangle CurrentViewport;
        public Rectangle ScreenRect;
        public bool NeedsReset;
        public bool ScaleUp;
        public float Scale;
        public float MaxScale;
        public float OldZoomLevel;
        public int scrollSpeed = 16;
        public ulong lastFrame;
        public RenderTarget2D target;
        public Overlays.TileGrid GridOverlay;
        public Overlays.DebugMouse Mouse;
        public Overlays.TileHighlight Highlights;
        public Overlays.InfoPanel Info;
        public Overlays.ObjectDrop Drops;
        public Overlays.MinesLadder MinesLadder;
        public Overlays.DrawPath Paths;

        public MapView()
        {
            GridOverlay = new Overlays.TileGrid();
            Mouse = new Overlays.DebugMouse();
            Highlights = new Overlays.TileHighlight();
            Drops = new Overlays.ObjectDrop();
            MinesLadder = new Overlays.MinesLadder();
            Info = new Overlays.InfoPanel();
            Info.SetObjectDrops(Drops);
            Paths = new Overlays.DrawPath();

            // HARD FORCE THE RESOLUTION
            // TODO: move this to pull from the same config as in Game1_SetWindowSize
            ScreenRect = new Rectangle(0, 0, ModEntry.Config.ScreenWidth, ModEntry.Config.ScreenHeight);
            ModEntry.Console.Log($"initializing MapView: {ScreenRect}", StardewModdingAPI.LogLevel.Error);
            Scale = 1;
        }

        public void Enter()
        {
            OldZoomLevel = Game1.options.baseZoomLevel;
            Game1.game1.zoomModifier = Game1.options.baseZoomLevel;
            OldViewport = Game1.viewport;
            if (Game1.gameMode != 3) return;
            Game1.options.baseZoomLevel = 1;
            Reset();
        }
        public void Reset()
        {
            lastFrame = TASDateTime.CurrentFrame;

            int width = Game1.currentLocation.map.DisplayWidth;
            int height = Game1.currentLocation.map.DisplayHeight;
            CurrentViewport = new(0, 0, width, height);
            Scale = Math.Min(width / (float)OldViewport.Width, height / (float)OldViewport.Height);
            MaxScale = Scale;

            RenderTarget2D cached_lightmap = Game1.lightmap;
            SetLightMap(null); //Game1._lightmap = null;
            Game1.game1.takingMapScreenshot = true;
            Game1.viewport = new xTile.Dimensions.Rectangle(0, 0, width, height);

            target = new RenderTarget2D(Game1.graphics.GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
            DrawLocation(target);

            SetLightMap(cached_lightmap); //Game1._lightmap = cached_lightmap;
            Game1.game1.takingMapScreenshot = false;
            Game1.viewport = OldViewport;
        }

        private FieldInfo lightmapInfo = null;
        private void SetLightMap(RenderTarget2D target)
        {
            if (lightmapInfo == null)
            {
                lightmapInfo = typeof(Game1).GetField("_lightmap", BindingFlags.Static | BindingFlags.NonPublic);
            }
            if (lightmapInfo != null)
            {
                lightmapInfo.SetValue(null, target);
            }
            ModEntry.Console.Log($"{lightmapInfo}", StardewModdingAPI.LogLevel.Warn);

        }

        public void Exit()
        {
            Game1.viewport = OldViewport;
            Game1.options.baseZoomLevel = OldZoomLevel;
            Game1.game1.zoomModifier = 1;
        }

        public static Vector2 MouseTile
        {
            get
            {
                MouseState mouseState = RealInputState.mouseState;
                Vector2 coords = new Vector2(mouseState.X, mouseState.Y);
                Vector2 zoomedCoords = coords * (1f / Game1.options.zoomLevel);

                int mouseTileX = (int)(zoomedCoords.X + Game1.viewport.X) / Game1.tileSize;
                int mouseTileY = (int)(zoomedCoords.Y + Game1.viewport.Y) / Game1.tileSize;
                return new Vector2(mouseTileX, mouseTileY);
            }
        }
        public void Update()
        {
            if (!Controller.Console.IsOpen)
            {
                if (RealInputState.IsKeyDown(Keys.A))
                {
                    CurrentViewport.X = Math.Max(0, CurrentViewport.X - scrollSpeed);
                }
                if (RealInputState.IsKeyDown(Keys.D))
                {
                    CurrentViewport.X = Math.Min(target.Width, CurrentViewport.X + CurrentViewport.Width + scrollSpeed) - Math.Min(target.Width, CurrentViewport.Width);
                }
                if (RealInputState.IsKeyDown(Keys.W))
                {
                    CurrentViewport.Y = Math.Max(0, CurrentViewport.Y - scrollSpeed);
                }
                if (RealInputState.IsKeyDown(Keys.S))
                {
                    CurrentViewport.Y = Math.Min(target.Height, CurrentViewport.Y + CurrentViewport.Height + scrollSpeed) - Math.Min(target.Height, CurrentViewport.Height);
                }
                if (RealInputState.KeyTriggered(Keys.R))
                {
                    Scale = Math.Min(target.Width / (float)OldViewport.Width, target.Height / (float)OldViewport.Height);
                    CurrentViewport = new xTile.Dimensions.Rectangle(0, 0, target.Width, target.Height);
                }
                if (RealInputState.KeyTriggered(Keys.C))
                {
                    Overlays.TileHighlight.Clear();
                }
                if (RealInputState.KeyTriggered(Keys.O))
                {
                    Overlays.TileHighlight.DrawOrder = !Overlays.TileHighlight.DrawOrder;
                }
                if (RealInputState.KeyTriggered(Keys.Escape))
                {
                    Exit();
                    Controller.CurrentView = TASView.None;
                    return;
                }
                if (RealInputState.scrollWheelDiff > 0)
                {
                    Scale = Math.Min(MaxScale, Scale + 0.1f);
                }
                else if (RealInputState.scrollWheelDiff < 0)
                {
                    Scale -= 0.1f;
                }
            }
            CurrentViewport.Width = (int)(OldViewport.Width * Scale);
            CurrentViewport.Height = (int)(OldViewport.Height * Scale);
            Game1.viewport = CurrentViewport;
            Game1.options.baseZoomLevel = 1 / Scale;

            if (!Controller.Console.IsOpen && Game1.gameMode == 3)
            {

                if (RealInputState.LeftMouseClicked())
                {
                    Vector2 tile = MouseTile;
                    if (0 <= tile.X && tile.X < Game1.currentLocation.map.Layers[0].LayerWidth && 0 <= tile.Y && tile.Y < Game1.currentLocation.map.Layers[0].LayerHeight)
                    {
                        Overlays.TileHighlight.Add(tile);
                    }
                }
                else if (RealInputState.RightMouseClicked())
                {
                    Overlays.TileHighlight.Remove(MouseTile);
                }

            }
            Info.ActiveUpdate();
            MinesLadder.ActiveUpdate();
            if (lastFrame != TASDateTime.CurrentFrame) Reset();
        }

        public void Draw()
        {
            bool inBeginEndPair = Game1.spriteBatch.inBeginEndPair();
            if (!inBeginEndPair)
            {
                //Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
                Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
            }
            Game1.game1.GraphicsDevice.SetRenderTarget(null);
            Game1.game1.GraphicsDevice.Clear(Color.Black);
            if (Game1.gameMode == 3 && target != null)
            {
                Mouse.DrawViewport(Game1.spriteBatch, target, CurrentViewport, ScreenRect, Color.White);
                GridOverlay.Draw();
                MinesLadder.Draw();
                Highlights.Draw();
                Paths.Draw();
                Info.Draw();
                Mouse.Draw();

            }
            if (!inBeginEndPair)
            {
                Game1.spriteBatch.End();
            }
        }

        public static void DrawLocation(RenderTarget2D target)
        {
            var spriteBatch = Game1.spriteBatch;
            var currentLocation = Game1.currentLocation;
            var mapDisplayDevice = Game1.mapDisplayDevice;
            var GraphicsDevice = Game1.game1.GraphicsDevice;
            Game1.SetRenderTarget(target);
            GraphicsDevice.Clear(Color.Black);
            // draw background
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            currentLocation.drawBackground(spriteBatch);
            mapDisplayDevice.BeginScene(spriteBatch);
            currentLocation.Map.GetLayer("Back").Draw(mapDisplayDevice, Game1.viewport, xTile.Dimensions.Location.Origin, wrapAround: false, 4);
            currentLocation.drawWater(spriteBatch);
            spriteBatch.End();

            // flooring
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
            currentLocation.drawFloorDecorations(spriteBatch);
            spriteBatch.End();

            // shadows

            // building layer
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            Layer building_layer = currentLocation.Map.GetLayer("Buildings");
            if (TASSpriteBatch.Active)
            {
                building_layer.Draw(mapDisplayDevice, Game1.viewport, xTile.Dimensions.Location.Origin, wrapAround: false, 4);
                mapDisplayDevice.EndScene();
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
            // draw shadows again?
            // currentLocation.draw
            {
                foreach (ResourceClump r in currentLocation.resourceClumps)
                {
                    r.draw(spriteBatch, r.tile.Value);
                }
                Reflector.InvokeMethod(currentLocation, "drawCharacters", new object[] { spriteBatch });
                Reflector.InvokeMethod(currentLocation, "drawFarmers", new object[] { spriteBatch });
                Reflector.InvokeMethod(currentLocation, "drawDebris", new object[] { spriteBatch });
                foreach (var obj in currentLocation.objects.Pairs)
                {
                    obj.Value.draw(spriteBatch, (int)obj.Key.X, (int)obj.Key.Y);
                }

                currentLocation.interiorDoors.Draw(spriteBatch);
                if (currentLocation.largeTerrainFeatures.Count > 0)
                {
                    foreach (LargeTerrainFeature largeTerrainFeature in currentLocation.largeTerrainFeatures)
                    {
                        largeTerrainFeature.draw(spriteBatch);
                    }
                }
                if (currentLocation is StardewValley.Locations.BuildableGameLocation loc)
                {
                    int border_buffer = 1;
                    Microsoft.Xna.Framework.Rectangle viewport_rect = new Microsoft.Xna.Framework.Rectangle(Game1.viewport.X / 64 - border_buffer, Game1.viewport.Y / 64 - border_buffer, (int)Math.Ceiling((float)Game1.viewport.Width / 64f) + 2 * border_buffer, (int)Math.Ceiling((float)Game1.viewport.Height / 64f) + 3 + 2 * border_buffer);
                    Microsoft.Xna.Framework.Rectangle object_rectangle = default(Microsoft.Xna.Framework.Rectangle);
                    foreach (Building building in loc.buildings)
                    {
                        int additional_radius = building.GetAdditionalTilePropertyRadius();
                        object_rectangle.X = (int)building.tileX.Value - additional_radius;
                        object_rectangle.Width = (int)building.tilesWide.Value + additional_radius * 2;
                        int bottom_y = (int)building.tileY.Value + (int)building.tilesHigh.Value + additional_radius;
                        object_rectangle.Height = bottom_y - (object_rectangle.Y = bottom_y - (int)Math.Ceiling((float)building.getSourceRect().Height * 4f / 64f) - additional_radius);
                        if (object_rectangle.Intersects(viewport_rect))
                        {
                            building.draw(spriteBatch);
                        }
                    }
                }
                if (currentLocation is StardewValley.Locations.BusStop busStop)
                {
                    Vector2 busPosition = (Vector2)Reflector.GetValue(busStop, "busPosition");
                    Rectangle busSource = (Rectangle)Reflector.GetValue(busStop, "busSource");
                    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)busPosition.X, (int)busPosition.Y)), busSource, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (busPosition.Y + 192f) / 10000f);
                }
                if (currentLocation is Farm farm)
                {
                    Point entry_position_tile = farm.GetMainFarmHouseEntry();
                    Vector2 entry_position_world = Utility.PointToVector2(entry_position_tile) * 64f;
                    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(entry_position_tile.X - 5, entry_position_tile.Y + 2) * 64f), Building.leftShadow, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
                    for (int x = 1; x < 8; x++)
                    {
                        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(entry_position_tile.X - 5 + x, entry_position_tile.Y + 2) * 64f), Building.middleShadow, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
                    }
                    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(entry_position_tile.X + 3, entry_position_tile.Y + 2) * 64f), Building.rightShadow, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
                    Texture2D house_texture = Farm.houseTextures;
                    if (farm.paintedHouseTexture != null)
                    {
                        house_texture = farm.paintedHouseTexture;
                    }
                    Color house_draw_color = Color.White;
                    Vector2 house_draw_position = new Vector2(entry_position_world.X - 384f, entry_position_world.Y - 440f);
                    spriteBatch.Draw(house_texture, Game1.GlobalToLocal(Game1.viewport, house_draw_position), farm.houseSource.Value, house_draw_color, 0f, Vector2.Zero, 4f, SpriteEffects.None, (house_draw_position.Y + 230f) / 10000f);
                    if (Game1.mailbox.Count > 0)
                    {
                        float yOffset = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);
                        Point mailbox_position = Game1.player.getMailboxPosition();
                        float draw_layer = (float)((mailbox_position.X + 1) * 64) / 10000f + (float)(mailbox_position.Y * 64) / 10000f;
                        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(mailbox_position.X * 64, (float)(mailbox_position.Y * 64 - 96 - 48) + yOffset)), new Microsoft.Xna.Framework.Rectangle(141, 465, 20, 24), Color.White * 0.75f, 0f, Vector2.Zero, 4f, SpriteEffects.None, draw_layer + 1E-06f);
                        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(mailbox_position.X * 64 + 32 + 4, (float)(mailbox_position.Y * 64 - 64 - 24 - 8) + yOffset)), new Microsoft.Xna.Framework.Rectangle(189, 423, 15, 13), Color.White, 0f, new Vector2(7f, 6f), 4f, SpriteEffects.None, draw_layer + 1E-05f);
                    }
                    //if (farm.shippingBinLid != null)
                    //{
                    //    farm.shippingBinLid.draw(spriteBatch);
                    //}
                    if (!farm.hasSeenGrandpaNote)
                    {
                        Point grandpa_shrine = farm.GetGrandpaShrinePosition();
                        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((grandpa_shrine.X + 1) * 64, grandpa_shrine.Y * 64)), new Microsoft.Xna.Framework.Rectangle(575, 1972, 11, 8), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0448009968f);
                    }
                }
            }
            // crabpot tiles
            // draw tool
            // draw farm buildings
            if (currentLocation.Name.Equals("Farm"))
            {
                Reflector.InvokeMethod(Game1.game1, "drawFarmBuildings");
            }
            // front
            mapDisplayDevice.BeginScene(spriteBatch);
            currentLocation.Map.GetLayer("Front").Draw(mapDisplayDevice, Game1.viewport, xTile.Dimensions.Location.Origin, wrapAround: false, 4);
            mapDisplayDevice.EndScene();
            {
                //currentLocation.drawAboveFrontLayer(spriteBatch);
                foreach (var tf in currentLocation.terrainFeatures.Pairs)
                {
                    tf.Value.draw(spriteBatch, tf.Key);
                }
            }
            spriteBatch.End();
            // always front
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            if (currentLocation.Map.GetLayer("AlwaysFront") != null)
            {
                mapDisplayDevice.BeginScene(spriteBatch);
                currentLocation.Map.GetLayer("AlwaysFront").Draw(mapDisplayDevice, Game1.viewport, xTile.Dimensions.Location.Origin, wrapAround: false, 4);
                mapDisplayDevice.EndScene();
            }
            // random stuff
            // 
            //currentLocation.drawAboveAlwaysFrontLayer(spriteBatch);
            spriteBatch.End();
        }
    }
}

