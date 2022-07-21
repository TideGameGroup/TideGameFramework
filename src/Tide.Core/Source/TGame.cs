using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace Tide.Core
{
    public struct FStaticGameSettings
    {
        public bool bUseFixedTimestep;
    }

    public class TGame
    {
        public TGame()
        {
            TMonoConstructorArgs monoargs =
                new TMonoConstructorArgs
                {
                    settings = Settings
                };

            MonoGame = new TMono(monoargs);

            SystemGraph = new TSystemGraph();

            ComponentGraph = new TComponentGraph();
        }

        public TMono MonoGame { get; set; }
        public TComponentGraph ComponentGraph { get; private set; }
        public TSystemGraph SystemGraph { get; private set; }
        public USettings Settings { get; private set; }

        public void Run()
        {
            MonoGame.Run();
        }
    }
}