using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Tide.Core
{
    public struct TPostProcessStackConstructorArgs
    {
        public List<FDrawPass> drawPasses;
    }

    public class TPostProcessStack : ISystem
    {
        private readonly Dictionary<string, FDrawPass> processStack = new Dictionary<string, FDrawPass>();

        public TPostProcessStack(TPostProcessStackConstructorArgs args)
        {
            foreach (var process in args.drawPasses)
            {
                if (process.bIsPostProcess)
                {
                    string name = Path.GetFileNameWithoutExtension(process.effect.Name);
                    processStack[name] = process;
                }
            }
        }

        public void Draw(TComponentGraph graph, GameTime gameTime)
        {
        }

        public EffectParameter GetParameter(string pass, string parameter)
        {
            return GetPostProcessPass(pass).effect.Parameters[parameter];
        }

        public FDrawPass GetPostProcessPass(string name)
        {
            return processStack[name];
        }

        public void Update(TComponentGraph graph, GameTime gameTime)
        {
        }
    }
}