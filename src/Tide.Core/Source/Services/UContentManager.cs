using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Tide.XMLSchema;

namespace Tide.Core
{
    public class UContentManager
    {
        Dictionary<string, string> pathMappings = new Dictionary<string, string>();

        public UContentManager(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            Content = contentManager ?? throw new ArgumentNullException(nameof(contentManager));
            GraphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));

            FContentMappings mappings = Content.Load<FContentMappings>("mappings");

            for (int i = 0; i < mappings.names.Length; i++)
            {
                string name = mappings.names[i];
                pathMappings[name] = mappings.paths[i];
            }
        }

        public ContentManager Content { get; private set; }
        public GraphicsDevice GraphicsDevice { get; private set; }

        public T Load<T>(string path)
        {
            if (pathMappings.ContainsKey(path))
            {
                return Content.Load<T>(pathMappings[path]);
            }
            else
            {
                return Content.Load<T>(path);
            }
        }
    }
}
