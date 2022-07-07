using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Tide.XMLSchema;

namespace Tide.Core
{
    public struct FCanvasCacheConstructorArgs
    {
        public FCanvas canvas;
        public UContentManager content;
    }

    public class FCanvasCache
    {
        private readonly UContentManager content;

        public readonly FCanvas canvas;
        public readonly Dictionary<string, SpriteFont> fontCache = new Dictionary<string, SpriteFont>();
        public readonly Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();
        public readonly Dictionary<string, FCanvas> tooltipCache = new Dictionary<string, FCanvas>();

        public FCanvasCache(FCanvasCacheConstructorArgs args)
        {
            canvas = args.canvas.DeepCopy();
            content = args.content;

            CacheWidgetContent(content, canvas);
        }

        private void CacheWidgetContent(UContentManager content, FCanvas widget)
        {
            foreach (string texture in widget.textures)
            {
                if (texture == "") { continue; }
                textureCache.TryAdd(texture, content.Load<Texture2D>(texture));
            }

            CreateDefaultTexture(content);

            foreach (string tooltip in widget.tooltips)
            {
                if (tooltip == "") { continue; }

                FCanvas canvas = content.Load<FCanvas>(tooltip).DeepCopy();
                tooltipCache.TryAdd(tooltip, canvas);
                CacheWidgetContent(content, canvas);
            }

            foreach (string font in widget.fonts)
            {
                if (font == "") { continue; }
                fontCache.TryAdd(font, content.Load<SpriteFont>(font));
            }
        }

        private void CreateDefaultTexture(UContentManager content)
        {
            Texture2D texture = new Texture2D(content.GraphicsDevice, 8, 8);

            Color[] data = new Color[texture.Width * texture.Height];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = Color.White;
            }
            texture.SetData(data);
            textureCache.TryAdd("", texture);
        }
    }
}