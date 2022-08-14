using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tide.Core
{
    public struct FDrawPass
    {
        public bool bClearRenderTarget;
        public BlendState blendState;
        public bool bUseMatrix;
        public Color clearColor;
        public DrawDelegate postPassDelegate;
        public RasterizerState rasterizerState;
        public RenderTarget2D renderTarget;
        public SamplerState samplerState;
        public SpriteSortMode sortMode;
        public FView view;
    }
}