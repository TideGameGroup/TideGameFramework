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
        public FView view;
        public int width;
        public GameWindow window;
    }

    public class TWindow : ISystem
    {
        private readonly GraphicsDeviceManager graphicsDeviceManager;
        public readonly FView view;
        public readonly GameWindow window;
        public int screenHeight;
        public int screenWidth;

        public TWindow(FWindowConstructorArgs args)
        {
            StaticValidation.NullCheck(args.settings);
            StaticValidation.TrySetDefault(args.graphicsDeviceManager, out graphicsDeviceManager);
            StaticValidation.TrySetDefault(args.view, out view);
            StaticValidation.TrySetDefault(args.window, out window);
            StaticValidation.TrySetDefault(args.width, out screenWidth);
            StaticValidation.TrySetDefault(args.height, out screenHeight);

            UserWidth = screenWidth;
            UserHeight = screenHeight;

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
        protected void RecalculateViewMatrix(int preferredWidth, int preferredHeight)
        {
            view.viewport.Width = preferredWidth;
            view.viewport.Height = preferredHeight;
            view.BuildMatrices();
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