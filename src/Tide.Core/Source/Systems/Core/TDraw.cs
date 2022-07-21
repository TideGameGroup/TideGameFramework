using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tide.Core
{
    public delegate void DrawDelegate(FView view, SpriteBatch spriteBatch, GameTime gameTime);

    public struct TDrawConstructorArgs
    {
        public List<FDrawPass> drawPasses;
        public GraphicsDevice graphicsDevice;
    }

    public class TDraw<T> : ISystem where T : IDrawableComponentType
    {
        private readonly List<FDrawPass> drawPasses = new List<FDrawPass>();
        private readonly GraphicsDevice graphicsDevice;

        public TDraw(TDrawConstructorArgs args)
        {
            StaticValidation.TrySetDefault(args.graphicsDevice, out graphicsDevice);

            SpriteBatch = new SpriteBatch(args.graphicsDevice);

            if (args.drawPasses != null)
            {
                drawPasses.AddRange(args.drawPasses);
            }
        }

        public SpriteBatch SpriteBatch { get; private set; }
        public FView View { get; set; }
        public TWindow WindowManager { get; set; }

        private void DrawPass(FDrawPass drawPass, TComponentGraph graph, GameTime gameTime)
        {
            if (drawPass.bClearRenderTarget)
            {
                graphicsDevice.Clear(drawPass.clearColor);
            }

            Matrix? matrix = null;
            if (drawPass.bUseMatrix)
            {
                matrix = View.ViewProjectionMatrix;
            }

            graphicsDevice.SetRenderTarget(drawPass.renderTarget);

            SpriteBatch.Begin(
                drawPass.sortMode,
                drawPass.blendState,
                drawPass.samplerState,
                null,
                drawPass.rasterizerState,
                null,
                matrix);

            foreach (UComponent component in graph)
            {
                if (component is T drawable && component.IsVisible)
                {
                    drawable.Draw(View, SpriteBatch, gameTime);
                }
            }

            drawPass.postPassDelegate?.Invoke(View, SpriteBatch, gameTime);

            SpriteBatch.End();
        }

        private void DrawToBackBuffer(RenderTarget2D renderTarget)
        {
            graphicsDevice.SetRenderTarget(null);
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

        public void Draw(TComponentGraph graph, GameTime gameTime)
        {
            foreach (var pass in drawPasses)
            {
                DrawPass(pass, graph, gameTime);
            }
        }

        public void Update(TComponentGraph graph, GameTime gameTime)
        {
        }
    }
}