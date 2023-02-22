using System;

namespace Tide.Core
{
    public struct FStaticGameSettings
    {
        public bool bAllowUserResizing;
        public bool bUseFixedTimestep;
        public bool bUseHardwareCursor;
        public bool bUseVerticalSync;
    }

    public class TGame : IDisposable
    {
        public TGame()
        {
            Settings = new USettings();
            Statistics = new UStatistics();
            SystemGraph = new TSystemGraph();
            ComponentGraph = new TComponentGraph();

            UStatistics.Get = Statistics;

            TMonoConstructorArgs monoargs =
                new TMonoConstructorArgs
                {
                    staticSettings = InitialiseStaticGameSettings(),
                    settings = Settings,
                    systemGraph = SystemGraph,
                    componentGraph = ComponentGraph,
                    game = this
                };

            MonoGame = new TMono(monoargs);
            SystemGraph.Add(ComponentGraph);
        }

        public TComponentGraph ComponentGraph { get; private set; }
        public TMono MonoGame { get; set; }
        public USettings Settings { get; private set; }
        public UStatistics Statistics { get; private set; }
        public TSystemGraph SystemGraph { get; private set; }

        public void Dispose() => MonoGame.Dispose();

        public virtual void InitialiseComponents(UContentManager content, TComponentGraph components, TSystemGraph systems)
        {
        }

        public virtual FStaticGameSettings InitialiseStaticGameSettings()
        {
            return new FStaticGameSettings
            {
                bAllowUserResizing = true,
                bUseFixedTimestep = true,
                bUseHardwareCursor = true,
                bUseVerticalSync = true
            };
        }

        public virtual void InitialiseSystems(UContentManager content, TSystemGraph systems)
        {
        }

        public void Run()
        {
            MonoGame.Run();
        }
    }
}