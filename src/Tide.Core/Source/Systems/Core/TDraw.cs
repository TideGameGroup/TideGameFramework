using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tide.Core
{
    public delegate void DrawDelegate(FView view, SpriteBatch spriteBatch, GameTime gameTime);

    public struct TDrawConstructorArgs
    {
        public GraphicsDevice graphicsDevice;
        public TWindow window;
    }

    public struct FDrawPass<T> where T : IDrawableComponent
    {
        public SpriteSortMode sortMode;
        public BlendState blendState;
        public SamplerState samplerState;
        public bool bUseMatrix;
        public DrawDelegate postPassDelegate;
        public RenderTarget2D renderTarget;
        public T _type;
    }

    public class TDraw
    {
        private readonly Stopwatch drawStopwatch = null;
        private readonly SpriteFont font;
        private readonly GraphicsDevice graphicsDevice;
        private readonly RasterizerState rasterizerState;
        public Color clearColor;

        private List<FDrawPass<IDrawableComponent>> drawPasses;

        public TDraw(TDrawConstructorArgs args)
        {
            StaticValidation.TrySetDefault(args.graphicsDevice, out graphicsDevice);
            SpriteBatch = new SpriteBatch(args.graphicsDevice);
        }

        public SpriteBatch SpriteBatch { get; private set; }
        public FView View { get; set; }
        public TWindow WindowManager { get; set; }

        private void DrawToBackBuffer(RenderTarget2D renderTarget)
        {
            graphicsDevice.SetRenderTarget(null);
            graphicsDevice.Clear(clearColor);

            SpriteBatch.Begin(0, BlendState.Opaque, SamplerState.PointClamp);
            PresentationParameters parameters = graphicsDevice.PresentationParameters;
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

        public void AddDrawPass<T>(FDrawPass<T> drawpass, int at = -1) where T : IDrawableComponent
        {
            if (at == -1)
            drawPasses.Add(drawpass);
        }

        public void Draw(TComponentGraph graph, GameTime gameTime)
        {
            drawStopwatch.Restart();
            graphicsDevice.SetRenderTarget(WindowManager.RenderTarget.RenderTarget);
            graphicsDevice.Clear(clearColor);

            foreach (var pass in drawPasses)
            {
                DrawPass(pass, graph, gameTime);
            }
        }

        private void DrawPass(FDrawPass<IDrawableComponent> drawPass, TComponentGraph graph, GameTime gameTime)
        {
            Matrix? matrix = null;
            if (drawPass.bUseMatrix)
            {
                matrix = View.ViewProjectionMatrix;
            }

            SpriteBatch.Begin(
                drawPass.sortMode,
                drawPass.blendState,
                drawPass.samplerState,
                null,
                null,
                null,
                matrix);

            foreach (UComponent component in graph)
            {
                if (component is drawPass._type && component.IsVisible)
                {
                    ((IDrawableComponent)component).Draw(View, SpriteBatch, gameTime);
                }
            }

            drawPass.postPassDelegate?.Invoke(View, SpriteBatch, gameTime);

            SpriteBatch.End();
        }
    }
}