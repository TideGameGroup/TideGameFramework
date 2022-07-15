using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Tide.XMLSchema;

namespace Tide.Core
{
    public class UContentManager
    {
        private readonly Dictionary<Type, object> defaults = new Dictionary<Type, object>();
        private readonly Dictionary<string, string> pathMappings = new Dictionary<string, string>();

        public UContentManager(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            Content = contentManager ?? throw new ArgumentNullException(nameof(contentManager));
            GraphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
            DynamicLibrary = new Dictionary<string, object>();

            FContentMappings mappings = Content.Load<FContentMappings>("mappings");

            for (int i = 0; i < mappings.names.Length; i++)
            {
                string name = mappings.names[i];
                pathMappings[name] = mappings.paths[i];
            }

            defaults.Add(typeof(Texture2D), GenerateNullTexture());
            defaults.Add(typeof(SpriteFont), Content.Load<SpriteFont>("Arial"));
        }

        public ContentManager Content { get; private set; }
        public Dictionary<string, object> DynamicLibrary { get; private set; }
        public GraphicsDevice GraphicsDevice { get; private set; }

        private Texture2D GenerateNullTexture()
        {
            Texture2D nullTex = new Texture2D(GraphicsDevice, 1, 1);
            nullTex.SetData(new Color[] { Color.DeepPink });
            return nullTex;
        }

        public T Load<T>(string path)
        {
            if (DynamicLibrary.ContainsKey(path))
            {
                return (T)DynamicLibrary[path];
            }

            if (pathMappings.ContainsKey(path))
            {
                return Content.Load<T>(pathMappings[path]);
            }

            try
            {
                return Content.Load<T>(path);
            }
            catch (ContentLoadException e)
            {
                Debug.Write(e);
                return (T)defaults[typeof(T)];
            }
        }
    }
}