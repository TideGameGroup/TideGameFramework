using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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

        private void DrawPass(FDrawPass drawPass, TComponentGraph graph, GameTime gameTime)
        {
            if (drawPass.bClearRenderTarget)
            {
                graphicsDevice.Clear(drawPass.clearColor);
            }

            Matrix? matrix = null;
            if (drawPass.bUseMatrix)
            {
                matrix = drawPass.view.ViewProjectionMatrix;
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
                if (component is T drawable && component.IsVisible && component.bCanDraw)
                {
                    drawable.Draw(drawPass.view, SpriteBatch, gameTime);
                }
                component.bCanDraw = component.IsVisible;
            }

            drawPass.postPassDelegate?.Invoke(drawPass.view, SpriteBatch, gameTime);

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

        public void SetRenderTarget(RenderTarget2D renderTarget)
        {
            for (int i = 0; i < drawPasses.Count; i++)
            {
                SetRenderTarget(i, renderTarget);
            }
        }

        public void SetRenderTarget(int i, RenderTarget2D renderTarget)
        {
            FDrawPass pass = drawPasses[i];
            pass.renderTarget = renderTarget;
            drawPasses[i] = pass;
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