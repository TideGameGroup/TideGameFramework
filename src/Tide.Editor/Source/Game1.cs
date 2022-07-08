using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tide.Core;
using Tide.Editor;
using Tide.XMLSchema;

namespace Tide.Editor
{
    public class Game1 : TGame
    {
        public Game1()
        {
            IsMouseVisible = true;
            clearColor = Color.LightGray;
        }

        public UEditorInterface EditorInterfaceComponent { get; private set; }

        protected override void Initialize()
        {
            base.Initialize();
            GraphicsDeviceManager.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            GraphicsDeviceManager.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            Window.AllowUserResizing = true;

            FEditorInterfaceConstructorArgs interfaceArgs =
                new FEditorInterfaceConstructorArgs
                {
                    input = Input,
                    content = ContentManager,
                    window = Window
                };

            EditorInterfaceComponent = new UEditorInterface(interfaceArgs);

            ComponentGraph.Add(EditorInterfaceComponent);
            SetupDefaultKeybindings();
        }

        private void SetupDefaultKeybindings()
        {
            Input.keyBindings.Add(new FBinding(EMouseButtons.LeftButton, "primary"));
            Input.keyBindings.Add(new FBinding(EMouseButtons.RightButton, "secondary"));
            Input.keyBindings.Add(new FBinding(EMouseButtons.MiddleButton, "tertiary"));

            Input.keyBindings.Add(new FBinding(Keys.W, "up"));
            Input.keyBindings.Add(new FBinding(Keys.A, "left"));
            Input.keyBindings.Add(new FBinding(Keys.S, "down"));
            Input.keyBindings.Add(new FBinding(Keys.D, "right"));
            Input.keyBindings.Add(new FBinding(EMouseButtons.ScrollDown, "scrollup"));
            Input.keyBindings.Add(new FBinding(EMouseButtons.ScrollUp, "scrolldown"));

            Input.keyBindings.Add(new FBinding(Keys.Q, "Q"));
            Input.keyBindings.Add(new FBinding(Keys.E, "E"));
            Input.keyBindings.Add(new FBinding(Keys.F, "F"));
            Input.keyBindings.Add(new FBinding(Keys.Z, "Z"));

            Input.keyBindings.Add(new FBinding(Keys.Up, "uparrow"));
            Input.keyBindings.Add(new FBinding(Keys.Down, "downarrow"));
            Input.keyBindings.Add(new FBinding(Keys.Left, "leftarrow"));
            Input.keyBindings.Add(new FBinding(Keys.Right, "rightarrow"));

            Input.keyBindings.Add(new FBinding(Keys.D1, "1"));
            Input.keyBindings.Add(new FBinding(Keys.D2, "2"));
            Input.keyBindings.Add(new FBinding(Keys.D3, "3"));
            Input.keyBindings.Add(new FBinding(Keys.D4, "4"));
            Input.keyBindings.Add(new FBinding(Keys.D5, "5"));
            Input.keyBindings.Add(new FBinding(Keys.D6, "6"));
            Input.keyBindings.Add(new FBinding(Keys.D7, "7"));

            Input.keyBindings.Add(new FBinding(Keys.Escape, "escape"));
            Input.keyBindings.Add(new FBinding(Keys.Back, "back"));

            Input.keyBindings.Add(new FBinding(Keys.LeftShift, "mod1"));
            Input.keyBindings.Add(new FBinding(Keys.LeftControl, "mod2"));
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
