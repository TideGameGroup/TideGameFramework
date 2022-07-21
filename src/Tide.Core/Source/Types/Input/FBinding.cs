using Microsoft.Xna.Framework.Input;

namespace Tide.Core
{
    public struct FBinding
    {
        public string bound;
        public Buttons button;
        public Keys key;
        public EMouseButtons mouseButton;
        public IVirtualInputEvent custom;

        public FBinding(Keys key, string bound)
        {
            this.key = key;
            this.bound = bound;

            button = Buttons.A;
            mouseButton = EMouseButtons.None;
            custom = null;
        }

        public FBinding(Buttons button, string bound)
        {
            this.button = button;
            this.bound = bound;

            key = Keys.None;
            mouseButton = EMouseButtons.None;
            custom = null;
        }

        public FBinding(EMouseButtons mouseButton, string bound)
        {
            this.mouseButton = mouseButton;
            this.bound = bound;

            key = Keys.None;
            button = Buttons.A;
            custom = null;
        }

        public FBinding(IVirtualInputEvent custom, string bound)
        {
            this.custom = custom;
            this.bound = bound;

            key = Keys.None;
            button = Buttons.A;
            mouseButton = EMouseButtons.None;
        }
    }
}