using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Tide.XMLSchema;

namespace Tide.Core
{

    // https://docs.monogame.net/api/Microsoft.Xna.Framework.GameWindow.html

    public class FTextField
    {
        public static void SetCursorPosition(FCanvas canvas, int i, Vector2 position, Rectangle rect)
        {

        }

        public static bool HandleTextInput(FCanvas canvas, int i, TextInputEventArgs args)
        {
            switch (args.Key)
            {
                case Keys.Escape:
                    return true;

                case Keys.Enter:
                    canvas.texts[i] += '\n';
                    break;

                case Keys.Back:
                    if (canvas.texts[i].Length > 0)
                    {
                        canvas.texts[i] = canvas.texts[i].Remove(canvas.texts[i].Length - 1);
                    }
                    break;

                default:
                    canvas.texts[i] += args.Character;
                    break;
            }

            return false;
        }
    }
}
