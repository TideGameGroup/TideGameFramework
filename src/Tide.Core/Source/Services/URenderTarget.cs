using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tide.Core
{
    public struct RenderTargetConstructorArgs
    {
        public GraphicsDevice graphicsDevice;
    }

    public class URenderTarget : UComponent
    {
        private readonly GraphicsDevice graphicsDevice;

        public URenderTarget(RenderTargetConstructorArgs args)
        {
            TrySetDefault(args.graphicsDevice, out graphicsDevice);
        }

        public RenderTarget2D RenderTarget { get; private set; }

        public void Recreate()
        {
            PresentationParameters parameters = graphicsDevice.PresentationParameters;
            SurfaceFormat format = parameters.BackBufferFormat;
            RenderTarget = new RenderTarget2D(graphicsDevice,
                parameters.BackBufferWidth,
                parameters.BackBufferHeight,
                false,
                format,
                parameters.DepthStencilFormat,
                parameters.MultiSampleCount,
                RenderTargetUsage.DiscardContents
                );
        }
    }
}
