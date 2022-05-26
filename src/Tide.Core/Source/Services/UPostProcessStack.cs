using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Tide.Core
{
    public class UPostProcessStack : UComponent
    {
        private readonly Dictionary<string, string> processes = new Dictionary<string, string>();
        private readonly GraphicsDevice graphicsDevice;

        UPostProcessStack (GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
        }

        public void AddProcess(string name, string effectPath)
        {
            
        }

        public void RemoveProcess(string name)
        {

        }

        public void DrawPostProcess(RenderTarget2D source, RenderTarget2D target, SpriteBatch spriteBatch, GameTime gameTime)
        {
            graphicsDevice.SetRenderTarget(target);
            graphicsDevice.Clear(new Color(45, 45, 45, 1));

            spriteBatch.Begin(0, BlendState.Opaque, SamplerState.PointClamp);
            PresentationParameters parameters = graphicsDevice.PresentationParameters;
            spriteBatch.Draw(source,
                new Rectangle(
                    0,
                    0,
                    parameters.BackBufferWidth,
                    parameters.BackBufferHeight
                    ),
                Color.White);
            spriteBatch.End();
        }
    }
}
