using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Tide.XMLSchema;

namespace Tide.Core
{
    public struct UContentManagerConstructorArgs
    {
        public ContentManager contentManager;
        public GraphicsDevice graphicsDevice;
    }

    public class UContentManager
    {
        private readonly Dictionary<Type, object> defaults = new Dictionary<Type, object>();
        private readonly Dictionary<string, string> pathMappings = new Dictionary<string, string>();

        public UContentManager(UContentManagerConstructorArgs args)
        {
            StaticValidation.NullCheck(args.contentManager);
            StaticValidation.NullCheck(args.graphicsDevice);

            Content = args.contentManager;
            GraphicsDevice = args.graphicsDevice;
            DynamicLibrary = new Dictionary<string, object>();

            FContentMappings mappings = Content.Load<FContentMappings>("mappings");

            for (int i = 0; i < mappings.names.Length; i++)
            {
                string name = mappings.names[i];
                pathMappings[name] = mappings.paths[i];
            }

            defaults.Add(typeof(Texture2D), GenerateNullTexture(Color.DeepPink));
            defaults.Add(typeof(SpriteFont), Content.Load<SpriteFont>("Arial"));
            defaults.Add(typeof(SoundEffect), Content.Load<SoundEffect>("Peep"));
        }

        public ContentManager Content { get; private set; }
        public Dictionary<string, object> DynamicLibrary { get; private set; }
        public GraphicsDevice GraphicsDevice { get; private set; }

        public Texture2D GenerateNullTexture(Color color)
        {
            Texture2D nullTex = new Texture2D(GraphicsDevice, 1, 1);
            nullTex.SetData(new Color[] { color });
            return nullTex;
        }

        public T Load<T>(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return (T)defaults[typeof(T)];
            }

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