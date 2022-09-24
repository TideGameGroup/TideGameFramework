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
            graphicsDevice.SetRenderTarget(drawPass.renderTarget);

            if (drawPass.bClearRenderTarget)
            {
                graphicsDevice.Clear(drawPass.clearColor);
            }

            Matrix? matrix = null;
            if (drawPass.bUseMatrix)
            {
                matrix = drawPass.view.ViewProjectionMatrix;
            }

            SpriteBatch.Begin(
                drawPass.sortMode,
                drawPass.blendState,
                drawPass.samplerState,
                null,
                drawPass.rasterizerState,
                drawPass.effect,
                matrix);

            if (drawPass.bIsPostProcess)
            {
                DrawPostProcess(drawPass);
            }
            else
            {
                DrawComponents(drawPass, graph, gameTime);
            }

            drawPass.postPassDelegate?.Invoke(drawPass.view, SpriteBatch, gameTime);

            SpriteBatch.End();
        }

        private void DrawComponents(FDrawPass drawPass, TComponentGraph graph, GameTime gameTime)
        {
            foreach (UComponent component in graph)
            {
                if (component is T drawable && component.IsVisible && component.bCanDraw)
                {
                    drawable.Draw(drawPass.view, SpriteBatch, gameTime);
                }
                component.bCanDraw = component.IsVisible;
            }
        }

        private void DrawPostProcess(FDrawPass drawPass)
        {
            PresentationParameters parameters = graphicsDevice.PresentationParameters;
            SpriteBatch.Draw(drawPass.renderSource,
                new Rectangle(
                    0,
                    0,
                    parameters.BackBufferWidth,
                    parameters.BackBufferHeight
                    ),
                Color.White);
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