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
            SystemGraph = new TSystemGraph();
            InitialiseSystems(SystemGraph);

            ComponentGraph = new TComponentGraph();
            InitialiseComponents(ComponentGraph, SystemGraph);

            SystemGraph.Add(ComponentGraph);

            Settings = new USettings();

            TMonoConstructorArgs monoargs =
                new TMonoConstructorArgs
                {
                    staticSettings = InitialiseStaticGameSettings(),
                    settings = Settings,
                    systems = SystemGraph,
                    componentGraph = ComponentGraph,
                };

            MonoGame = new TMono(monoargs);
        }

        public TMono MonoGame { get; set; }
        public TComponentGraph ComponentGraph { get; private set; }
        public TSystemGraph SystemGraph { get; private set; }
        public USettings Settings { get; private set; }

        public virtual FStaticGameSettings InitialiseStaticGameSettings()
        {
            return new FStaticGameSettings
            {
                bUseFixedTimestep = true
            };
        }

        public virtual void InitialiseSystems(TSystemGraph systems)
        {

        }

        public virtual void InitialiseComponents(TComponentGraph components, TSystemGraph systems)
        {

        }

        public void Run()
        {
            MonoGame.Run();
        }
    }
}