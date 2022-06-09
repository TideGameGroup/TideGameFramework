using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Tide.Core
{
    public class UPostProcessStack : UComponent
    {
        private readonly UContentManager content;
        private readonly GraphicsDevice graphicsDevice;
        private readonly Dictionary<string, Effect> processes = new Dictionary<string, Effect>();

        public UPostProcessStack(UContentManager content, GraphicsDevice graphicsDevice)
        {
            this.content = content ?? throw new ArgumentNullException(nameof(content));
            this.graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
        }

        public void AddProcess(string effectPath)
        {
            Effect effect = content.Load<Effect>(effectPath);
            processes.TryAdd(effectPath, effect);
        }

        public EffectParameter GetParameter(string effect, string parameter)
        {
            if (processes.ContainsKey(effect))
            {
                return processes[effect].Parameters[parameter];
            }
            return null;
        }

        public void DrawPostProcess(RenderTarget2D source, RenderTarget2D target, SpriteBatch spriteBatch, GameTime gameTime)
        {
            graphicsDevice.SetRenderTarget(target);

            foreach (var effect in processes)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque,
                    SamplerState.PointClamp, DepthStencilState.Default,
                    RasterizerState.CullNone, effect.Value);
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

        public void RemoveProcess(string name)
        {
            processes.Remove(name);
        }
    }
}