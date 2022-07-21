using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace Tide.Core
{
    public class TGame2 : Game
    {
        private Stopwatch drawStopwatch = null;
        private SpriteFont font;
        public int height = 720;
        public int width = 1280;
        private RasterizerState rasterizerState;
        private RasterizerState rasterizerUIState;
        private Stopwatch updateStopwatch = null;

        protected UStatistics statistics = null;
        public bool bDrawStats = true;
        public bool bFixedTimestep = true;
        public bool bAllowUserResizing = true;
        public bool bVsync = false;
        public Color clearColor = Color.AntiqueWhite;
        public int numPhysicsSubsteps = 2;

        public Texture2D backgroundTexture;
        public Texture2D overlayTexture;

        public TGame()
        {
            Content.RootDirectory = "Content";
            IsMouseVisible = false;

            GraphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                SynchronizeWithVerticalRetrace = bVsync
            };
            IsFixedTimeStep = bFixedTimestep;
        }

        protected UContentManager ContentManager { get; private set; }
        protected GraphicsDeviceManager GraphicsDeviceManager { get; private set; }
        protected UPostProcessStack PostProcessStack { get; private set; }
        protected TComponentGraph ComponentGraph { get; private set; }
        protected USettings Settings { get; private set; }
        protected SpriteBatch SpriteBatch { get; private set; }
        protected TAudio Audio { get; private set; }
        protected TInput Input { get; private set; }
        public FView View { get; set; }
        public TWindow WindowManager { get; set; }

        [Conditional("DEBUG")]
        private void DrawStats(GameTime gameTime)
        {
            SpriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, rasterizerUIState);
            if (bDrawStats)
            {
                SpriteBatch.DrawString(font, "FPS:   " + (1.0 / gameTime.ElapsedGameTime.TotalSeconds).ToString("0"), new Vector2(10, 10), Color.Black);
                SpriteBatch.DrawString(font, "update:" + updateStopwatch.Elapsed.TotalMilliseconds.ToString(), new Vector2(10, 25), Color.Black);
                SpriteBatch.DrawString(font, "draw:  " + drawStopwatch.Elapsed.TotalMilliseconds.ToString(), new Vector2(10, 40), Color.Black);

                int y = 55;
                foreach (var stat in statistics.stats)
                {
                    SpriteBatch.DrawString(font, stat.Key + ":" + stat.Value, new Vector2(10, y), Color.Black);
                    y += 15;
                }
            }
            SpriteBatch.End();
        }

        private void DrawToBackBuffer(RenderTarget2D renderTarget)
        {
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(clearColor);

            SpriteBatch.Begin(0, BlendState.Opaque, SamplerState.PointClamp);
            PresentationParameters parameters = GraphicsDevice.PresentationParameters;
            SpriteBatch.Draw(renderTarget,
                new Rectangle(
                    0,
                    0,
                    parameters.BackBufferWidth,
                    parameters.BackBufferHeight
                    ),
                Color.White);
            SpriteBatch.End();
        }

        protected override void Draw(GameTime gameTime)
        {
            drawStopwatch.Restart();
            GraphicsDevice.SetRenderTarget(WindowManager.RenderTarget.RenderTarget);
            GraphicsDevice.Clear(clearColor);

            // background
            
            SpriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                null, null, null, null);

            if (backgroundTexture != null)
            {
                SpriteBatch.Draw(backgroundTexture, GraphicsDevice.Viewport.Bounds, Color.White);
            }

            SpriteBatch.End();

            // 2d pass
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
                null, null, null, View.ViewProjectionMatrix);

            foreach (UComponent script in ComponentGraph)
            {
                if (script is IDrawableComponent && script.IsVisible)
                {
                    ((IDrawableComponent)script).Draw2D(View, SpriteBatch, gameTime);
                }
            }

            OnDraw2D(View, SpriteBatch, gameTime);

            SpriteBatch.End();

            // ui pass
            SpriteBatch.Begin( SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
                null, null, null, null);

            foreach (UComponent script in ComponentGraph)
            {
                if (script is IDrawableCanvasComponent && script.IsVisible)
                {
                    ((IDrawableCanvasComponent)script).DrawUI(View, SpriteBatch, gameTime);
                }
            }

            OnDrawUI(View, SpriteBatch, gameTime);

            if (overlayTexture != null)
            {
                SpriteBatch.Draw(overlayTexture, GraphicsDevice.Viewport.Bounds, Color.White);
            }

            SpriteBatch.End();

            base.Draw(gameTime);

            //PostProcessStack.DrawPostProcess(RenderTarget, RenderTarget, SpriteBatch, gameTime);
            DrawToBackBuffer(WindowManager.RenderTarget.RenderTarget);

            // stats
            drawStopwatch.Stop();
            DrawStats(gameTime);
        }

        protected override void Initialize()
        {
            base.Initialize();

            SpriteBatch = new SpriteBatch(GraphicsDevice);
            updateStopwatch = new Stopwatch();
            drawStopwatch = new Stopwatch();

            rasterizerState = new RasterizerState() { ScissorTestEnable = false };
            rasterizerUIState = new RasterizerState() { ScissorTestEnable = true };


            statistics = new UStatistics();
            Audio = new TAudio(ContentManager, Settings);
            Input = new TInput(statistics);

            View = new FView(GraphicsDevice.Viewport);

            ComponentGraph = new TComponentGraph();
            PostProcessStack = new UPostProcessStack(ContentManager, GraphicsDevice);
            Settings = new USettings();

            FWindowConstructorArgs windowConstructorArgs =
                new FWindowConstructorArgs
                {
                    bFullscreen = Settings["fullscreen"].b,
                    graphicsDeviceManager = GraphicsDeviceManager,
                    view2D = View,
                    window = Window
                };

            WindowManager = new TWindow(windowConstructorArgs);

            ComponentGraph.Add(PostProcessStack);
            ComponentGraph.Add(Settings);
            ComponentGraph.Add(statistics);
            ComponentGraph.Add(WindowManager);
            ComponentGraph.Add(Audio);
            ComponentGraph.Add(Input);

            Settings.SetOnChangedCallback("fullscreen", () =>
                {
                    WindowManager.SetFullscreen(Settings["fullscreen"].b);
                }
            );
        }

        protected override void LoadContent()
        {
            ContentManager = new UContentManager(Content, GraphicsDevice);
            font = ContentManager.Load<SpriteFont>("Arial");
        }

        protected virtual void OnDraw2D(FView view2D, SpriteBatch spriteBatch, GameTime gameTime)
        { }

        protected virtual void OnDrawUI(FView view2D, SpriteBatch spriteBatch, GameTime gameTime)
        { }

        protected virtual void OnUpdate(GameTime gameTime)
        { }

        protected override void Update(GameTime gameTime)
        {
            updateStopwatch.Restart();
            
            PhysicsUpdate(gameTime);

            foreach (UComponent script in ComponentGraph)
            {
                if (script is IUpdateComponent && script.IsActive)
                {
                    ((IUpdateComponent)script).Update(gameTime);
                }
            }
            OnUpdate(gameTime);
            updateStopwatch.Stop();
        }

        private void PhysicsUpdate(GameTime gameTime)
        {
            TimeSpan elapsedStepTime = gameTime.ElapsedGameTime / numPhysicsSubsteps;
            GameTime stepTime = new GameTime(
                gameTime.TotalGameTime - gameTime.ElapsedGameTime,
                gameTime.ElapsedGameTime / numPhysicsSubsteps
                );

            foreach (UComponent script in ComponentGraph)
            {
                if (script is IPhysicsComponent && script.IsActive)
                {
                    ((IPhysicsComponent)script).PrePhysics(gameTime);
                }
            }

            for (int n = 0; n < numPhysicsSubsteps; n++)
            {
                stepTime.TotalGameTime += elapsedStepTime * (n + 1);

                foreach (UComponent script in ComponentGraph)
                {
                    if (script is IPhysicsComponent && script.IsActive)
                    {
                        ((IPhysicsComponent)script).CollisionUpdate(stepTime);
                    }
                }

                foreach (UComponent script in ComponentGraph)
                {
                    if (script is IPhysicsComponent && script.IsActive)
                    {
                        ((IPhysicsComponent)script).PhysicsUpdate(stepTime);
                    }
                }
            }

            foreach (UComponent script in ComponentGraph)
            {
                if (script is IPhysicsComponent && script.IsActive)
                {
                    ((IPhysicsComponent)script).PostPhysics(gameTime);
                }
            }
        }
    }
}