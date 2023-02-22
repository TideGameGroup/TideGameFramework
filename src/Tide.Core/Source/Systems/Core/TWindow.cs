using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Tide.Core
{
    public struct FWindowConstructorArgs
    {
        public bool bAllowUserResizing;
        public bool bFullscreen;
        public GraphicsDeviceManager graphicsDeviceManager;
        public int height;
        public USettings settings;
        public int width;
        public GameWindow window;
    }

    public class TWindow : ISystem
    {
        private readonly GraphicsDeviceManager graphicsDeviceManager;
        public readonly GameWindow window;
        public int screenHeight;
        public int screenWidth;

        public TWindow(FWindowConstructorArgs args)
        {
            FStaticValidation.NullCheck(args.settings);
            FStaticValidation.TrySetDefault(args.graphicsDeviceManager, out graphicsDeviceManager);
            FStaticValidation.TrySetDefault(args.window, out window);
            FStaticValidation.TrySetDefault(args.width, out screenWidth);
            FStaticValidation.TrySetDefault(args.height, out screenHeight);

            UserWidth = screenWidth;
            UserHeight = screenHeight;

            View = new FView(args.graphicsDeviceManager.GraphicsDevice.Viewport);

            RenderTargetConstructorArgs renderTargetConstructorArgs =
                new RenderTargetConstructorArgs
                {
                    graphicsDevice = graphicsDeviceManager.GraphicsDevice
                };
            RenderTarget = new FRenderTarget(renderTargetConstructorArgs);

            SetFullscreen(args.bFullscreen);
            window.ClientSizeChanged += new EventHandler<EventArgs>(RecreateWindow);
            window.AllowUserResizing = true;

            args.settings.SetOnChangedCallback("fullscreen", () =>
                {
                    SetFullscreen(args.settings["fullscreen"].b);
                }
            );
        }

        public FRenderTarget RenderTarget { get; set; }
        public int UserHeight { get; set; }
        public int UserWidth { get; set; }
        public FView View { get; private set; }

        protected void RecalculateViewMatrix(int preferredWidth, int preferredHeight)
        {
            View.viewport.Width = preferredWidth;
            View.viewport.Height = preferredHeight;
            View.BuildMatrices();
        }

        protected virtual void RecreateWindow(object _, EventArgs e)
        {
            RecreateWindow(window.ClientBounds.Width, window.ClientBounds.Height);
        }
        protected void RecreateWindow(int preferredWidth, int preferredHeight)
        {
            RecalculateViewMatrix(preferredWidth, preferredHeight);
            RenderTarget.Recreate();
        }

        public void SetFullscreen()
        {
            graphicsDeviceManager.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphicsDeviceManager.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphicsDeviceManager.HardwareModeSwitch = true;
            graphicsDeviceManager.IsFullScreen = true;
            graphicsDeviceManager.ApplyChanges();
            RenderTarget.Recreate();

            RecalculateViewMatrix(
                graphicsDeviceManager.PreferredBackBufferWidth,
                graphicsDeviceManager.PreferredBackBufferHeight);
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

        public void UnsetFullscreen()
        {
            graphicsDeviceManager.PreferredBackBufferWidth = UserWidth;
            graphicsDeviceManager.PreferredBackBufferHeight = UserHeight;
            graphicsDeviceManager.IsFullScreen = false;
            graphicsDeviceManager.ApplyChanges();
            RenderTarget.Recreate(); // post process too

            RecalculateViewMatrix(UserWidth, UserHeight);
        }

        #region ISystem
        public void Draw(TComponentGraph graph, GameTime gameTime)
        {
        }

        public void Update(TComponentGraph graph, GameTime gameTime)
        {
        }
        #endregion
    }
}