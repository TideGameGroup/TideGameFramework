using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace Tide.Core
{
    public struct TMonoConstructorArgs
    {
        public FStaticGameSettings staticSettings;
        public USettings settings;
        public TGame game;
        public TComponentGraph componentGraph;
        public TSystemGraph systemGraph;
    }

    public class TMono : Game
    {
        private readonly TComponentGraph componentGraph;
        private readonly TSystemGraph systemGraph;
        private readonly TGame game;

        private readonly Stopwatch stopwatchDraw = null;
        private readonly Stopwatch stopwatchUpdate = null;

        public TMono(TMonoConstructorArgs args)
        {
            FStaticValidation.TrySetDefault(args.componentGraph, out componentGraph);
            FStaticValidation.TrySetDefault(args.systemGraph, out systemGraph);
            FStaticValidation.TrySetDefault(args.game, out game);

            Content.RootDirectory = "Content";

            GraphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                SynchronizeWithVerticalRetrace = args.settings["vsync"].b
            };

            IsFixedTimeStep = args.staticSettings.bUseFixedTimestep;
            IsMouseVisible = args.staticSettings.bUseHardwareCursor;

            stopwatchDraw = new Stopwatch();
            stopwatchUpdate = new Stopwatch();
        }

        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }
        public UContentManager ContentManager { get; private set; }

        protected override void Draw(GameTime gameTime)
        {
            stopwatchDraw.Restart();

            base.Draw(gameTime);

            foreach (var system in systemGraph)
            {
                system.Draw(componentGraph, gameTime);
            }

            stopwatchDraw.Stop();

            DrawStats(gameTime);
        }

        [Conditional("DEBUG")]
        private void DrawStats(GameTime gameTime)
        {
            SpriteBatch batch = new SpriteBatch(GraphicsDeviceManager.GraphicsDevice);
            batch.Begin(SpriteSortMode.Immediate, null, null, null, new RasterizerState());

            SpriteFont font = ContentManager.Load<SpriteFont>("Arial");
            batch.DrawString(font, "FPS:   " + (1.0 / gameTime.ElapsedGameTime.TotalSeconds).ToString("0"), new Vector2(10, 10), Color.Black);
            batch.DrawString(font, "update:" + stopwatchUpdate.Elapsed.TotalMilliseconds.ToString(), new Vector2(10, 25), Color.Black);
            batch.DrawString(font, "draw:  " + stopwatchDraw.Elapsed.TotalMilliseconds.ToString(), new Vector2(10, 40), Color.Black);

            int y = 55;
            foreach (var stat in game.Statistics.stats)
            {
                batch.DrawString(font, stat.Key + ":" + stat.Value, new Vector2(10, y), Color.Black);
                y += 15;
            }
            
            batch.End();
        }

        protected override void Initialize()
        {
            base.Initialize();

            UContentManagerConstructorArgs args =
                new UContentManagerConstructorArgs
                {
                    contentManager = Content,
                    graphicsDevice = GraphicsDevice
                };

            ContentManager = new UContentManager(args);

            game.InitialiseSystems(ContentManager, systemGraph);
            game.InitialiseComponents(ContentManager, componentGraph, systemGraph);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            stopwatchUpdate.Restart();

            base.Update(gameTime);

            foreach (var system in systemGraph)
            {
                system.Update(componentGraph, gameTime);
            }

            stopwatchUpdate.Stop();
        }
    }
}
