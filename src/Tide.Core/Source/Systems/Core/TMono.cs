using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Tide.Core
{
    public struct TMonoConstructorArgs
    {
        public FStaticGameSettings staticSettings;
        public USettings settings;
        public TComponentGraph componentGraph;
        public TSystemGraph systems;
    }

    public class TMono : Game
    {
        private readonly TComponentGraph graph;
        private readonly TSystemGraph systems;

        public TMono(TMonoConstructorArgs args)
        {
            StaticValidation.TrySetDefault(args.componentGraph, out graph);

            Content.RootDirectory = "Content";
            IsMouseVisible = args.settings["ShowCursor"].b;

            GraphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                SynchronizeWithVerticalRetrace = args.settings["VSync"].b
            };

            IsFixedTimeStep = args.staticSettings.bUseFixedTimestep;
        }

        protected GraphicsDeviceManager GraphicsDeviceManager { get; private set; }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            foreach (var system in systems)
            {
                system.Draw(graph, gameTime);
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (var system in systems)
            {
                system.Update(graph, gameTime);
            }
        }
    }
}
