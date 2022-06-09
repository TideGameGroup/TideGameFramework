using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace Tide.Core
{
    public class TGame : Game
    {
        private Stopwatch drawStopwatch = null;
        private SpriteFont font;
        private int height = 900;

        private RasterizerState rasterizerState;
        private RasterizerState rasterizerUIState;

        private UStatistics statistics = null;

        private Stopwatch updateStopwatch = null;
        private int width = 900;
        protected RenderTarget2D RenderTarget;
        protected RenderTarget2D PostProcessTarget;
        public bool bDrawStats = true;

        public TGame()
        {
            GraphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                SynchronizeWithVerticalRetrace = false
            };
            Window.ClientSizeChanged += new EventHandler<EventArgs>(OnResize);
            IsFixedTimeStep = false;
        }

        protected UContentManager ContentManager { get; private set; }
        protected GraphicsDeviceManager GraphicsDeviceManager { get; private set; }
        protected UPostProcessStack PostProcessStack { get; private set; }
        protected UComponentGraph ScriptGraph { get; private set; }
        protected USettings Settings { get; private set; }
        protected SpriteBatch SpriteBatch { get; private set; }

        public UView3D View3D { get; set; }

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

        private void RecreateRenderTarget()
        {
            PresentationParameters parameters = GraphicsDevice.PresentationParameters;
            SurfaceFormat format = parameters.BackBufferFormat;
            RenderTarget = new RenderTarget2D(GraphicsDevice,
                parameters.BackBufferWidth,
                parameters.BackBufferHeight,
                false,
                format,
                parameters.DepthStencilFormat,
                parameters.MultiSampleCount,
                RenderTargetUsage.DiscardContents
                ); 
            
            PostProcessTarget = new RenderTarget2D(GraphicsDevice,
                 parameters.BackBufferWidth,
                 parameters.BackBufferHeight,
                 false,
                 format,
                 parameters.DepthStencilFormat,
                 parameters.MultiSampleCount,
                 RenderTargetUsage.DiscardContents
                 );
        }

        private void SetFullscreen()
        {
            GraphicsDeviceManager.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            GraphicsDeviceManager.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            GraphicsDeviceManager.HardwareModeSwitch = true;
            GraphicsDeviceManager.IsFullScreen = true;
            GraphicsDeviceManager.ApplyChanges();
            RecreateRenderTarget();
        }

        private void UnsetFullscreen()
        {
            GraphicsDeviceManager.PreferredBackBufferWidth = width;
            GraphicsDeviceManager.PreferredBackBufferHeight = height;
            GraphicsDeviceManager.IsFullScreen = false;
            GraphicsDeviceManager.ApplyChanges();
            RecreateRenderTarget();
        }

        protected override void Draw(GameTime gameTime)
        {
            drawStopwatch.Restart();

            GraphicsDevice.SetRenderTarget(RenderTarget);
            GraphicsDevice.Clear(new Color(45, 45, 45, 1));

            SpriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.LinearClamp,
                null, null, null, null);

            foreach (UComponent script in ScriptGraph)
            {
                if (script is IDrawableComponent2D && script.bIsVisible)
                {
                    ((IDrawableComponent2D)script).Draw2D(View3D, SpriteBatch, gameTime);
                }
            }

            OnDraw2D(View3D, SpriteBatch, gameTime);

            foreach (UComponent script in ScriptGraph)
            {
                if (script is IDrawableCanvasScript && script.bIsVisible)
                {
                    ((IDrawableCanvasScript)script).DrawUI(View3D, SpriteBatch, gameTime);
                }
            }

            SpriteBatch.End();

            base.Draw(gameTime);

            PostProcessStack.DrawPostProcess(RenderTarget, RenderTarget, SpriteBatch, gameTime);
            DrawToBackBuffer(RenderTarget);

            // stats
            drawStopwatch.Stop();
            DrawStats(gameTime);
        }

        private void DrawToBackBuffer(RenderTarget2D renderTarget)
        {
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(new Color(45, 45, 45, 1));

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

        protected override void Initialize()
        {
            base.Initialize();

            SpriteBatch = new SpriteBatch(GraphicsDevice);
            updateStopwatch = new Stopwatch();
            drawStopwatch = new Stopwatch();
            rasterizerState = new RasterizerState() { ScissorTestEnable = false };
            rasterizerUIState = new RasterizerState() { ScissorTestEnable = true };
            statistics = new UStatistics();

            View3D = new UView3D(GraphicsDevice.Viewport);
            ScriptGraph = new UComponentGraph();
            PostProcessStack = new UPostProcessStack(ContentManager, GraphicsDevice);
            Settings = new USettings();

            ScriptGraph.RegisterScript(PostProcessStack);
            ScriptGraph.RegisterScript(Settings);
            ScriptGraph.RegisterScript(statistics);

            settingChangedEvent fullscreenEvent = new settingChangedEvent(() =>
            {
                SetFullscreen(Settings["fullscreen"].b);
            });
            Settings.SetOnChangedCallback("fullscreen", fullscreenEvent);

            if (!Settings["fullscreen"].b)
            {
                GraphicsDeviceManager.PreferredBackBufferWidth = width;
                GraphicsDeviceManager.PreferredBackBufferHeight = height;
                GraphicsDeviceManager.ApplyChanges();
                RecalculateViewMatrix(width, height);
                RecreateRenderTarget();
            }
            else
            {
                SetFullscreen(true);
            }
        }

        protected override void LoadContent()
        {
            ContentManager = new UContentManager(Content, GraphicsDevice);
            font = ContentManager.Load<SpriteFont>("RoentgenNbp");
        }

        protected virtual void OnDraw2D(UView3D view3D, SpriteBatch spriteBatch, GameTime gameTime)
        { }

        protected virtual void OnResize(object _, EventArgs e)
        {
            OnResize(Window.ClientBounds.Width, Window.ClientBounds.Height);
        }

        protected virtual void OnResize(int preferredWidth, int preferredHeight)
        {
            RecalculateViewMatrix(preferredWidth, preferredHeight);
            RecreateRenderTarget();
        }

        protected virtual void OnUpdate(GameTime gameTime)
        { }

        protected virtual void RecalculateViewMatrix(int preferredWidth, int preferredHeight)
        {
            View3D.viewport.Width = preferredWidth;
            View3D.viewport.Height = preferredHeight;
            View3D.BuildMatrices();
        }

        protected override void Update(GameTime gameTime)
        {
            updateStopwatch.Restart();
            foreach (UComponent script in ScriptGraph)
            {
                if (script is IUpdateComponent && script.bIsActive)
                {
                    ((IUpdateComponent)script).Update(gameTime);
                }
            }
            OnUpdate(gameTime);
            updateStopwatch.Stop();
        }

        public void SetFullscreen(bool set)
        {
            if (set)
            {
                SetFullscreen();
            }
            else
            {
                UnsetFullscreen();
            }
        }
    }
}