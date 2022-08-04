using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Tide.Core;
using Tide.Editor;
using Tide.XMLSchema;

namespace Tide.Editor
{
    public class Game1 : TGame
    {
        public Game1()
        {
        }

        public UEditorInterface EditorInterfaceComponent { get; private set; }

        public override void InitialiseComponents(UContentManager content, TComponentGraph components, TSystemGraph systems)
        {
            FEditorInterfaceConstructorArgs interfaceArgs =
                new FEditorInterfaceConstructorArgs
                {
                    input = systems.Find<TInput>(),
                    content = content,
                    window = systems.Find<TWindow>(),
                };

            EditorInterfaceComponent = new UEditorInterface(interfaceArgs);

            ComponentGraph.Add(EditorInterfaceComponent);
        }

        public override void InitialiseSystems(UContentManager content, TSystemGraph systems)
        {
            FView view = new FView(MonoGame.GraphicsDevice.Viewport);

            FWindowConstructorArgs windowArgs =
                new FWindowConstructorArgs
                {
                    settings = Settings,
                    bFullscreen = false,
                    bAllowUserResizing = true,
                    graphicsDeviceManager = MonoGame.GraphicsDeviceManager,
                    window = MonoGame.Window,
                    width = 1280,
                    height = 720
                };

            TWindow window = new TWindow(windowArgs);
            systems.Add(window);

            FInputConstructorArgs inputArgs =
                new FInputConstructorArgs
                {
                    statistics = Statistics,
                    bindings = new FDefaultBindings
                    {
                        bindings = TInput.GetDefaultKeybindings()
                    }
                };

            systems.Add(new TInput(inputArgs));

            FDrawPass UIPass = new FDrawPass
            {
                bClearRenderTarget = true,
                clearColor = Color.DarkGray,
                sortMode = SpriteSortMode.Deferred,
                blendState = BlendState.AlphaBlend,
                samplerState = SamplerState.LinearClamp,
                bUseMatrix = false,
                postPassDelegate = null,
                renderTarget = null,
                rasterizerState = new RasterizerState
                {
                    ScissorTestEnable = false
                }
            };

            TDrawConstructorArgs drawConstructorArgs =
                new TDrawConstructorArgs
                {
                    graphicsDevice = MonoGame.GraphicsDevice,
                    window = window,
                    drawPasses = new List<FDrawPass>
                    {
                        UIPass
                    }
                };

            systems.Add(new TDraw<IDrawableCanvasComponent>(drawConstructorArgs));
        }
    }
}
