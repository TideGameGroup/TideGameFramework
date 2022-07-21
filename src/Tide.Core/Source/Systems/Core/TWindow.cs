using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Tide.Core
{
    public struct FWindowConstructorArgs
    {
        public bool bFullscreen;
        public GraphicsDeviceManager graphicsDeviceManager;
        public FView view2D;
        public GameWindow window;
        public int width;
        public int height;
    }

    public class TWindow : UComponent
    {
        private readonly GraphicsDeviceManager graphicsDeviceManager;
        private readonly FView view2D;
        private readonly GameWindow window;
        public int screenHeight;
        public int screenWidth;

        public TWindow(FWindowConstructorArgs args)
        {
            TrySetDefault(args.graphicsDeviceManager, out graphicsDeviceManager);
            TrySetDefault(args.view2D, out view2D);
            TrySetDefault(args.window, out window);
            TrySetDefault(args.width, out screenWidth);
            TrySetDefault(args.height, out screenHeight);

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
        }

        public int UserHeight { get; set; }
        public int UserWidth { get; set; }
        public FRenderTarget RenderTarget { get; set; }

        protected virtual void RecreateWindow(object _, EventArgs e)
        {
            RecreateWindow(window.ClientBounds.Width, window.ClientBounds.Height);
        }

        protected void RecalculateViewMatrix(int preferredWidth, int preferredHeight)
        {
            view2D.viewport.Width = preferredWidth;
            view2D.viewport.Height = preferredHeight;
            view2D.BuildMatrices();
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
    }
}