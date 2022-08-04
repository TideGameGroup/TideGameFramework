using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tide.Core
{
    public delegate void Axis2DDelegate(float x, float y, GameTime gameTime);

    public delegate void AxisDelegate(float x, GameTime gameTime);

    public delegate void ButtonDelegate(GameTime gameTime);

    public struct FDefaultBindings
    {
        public FBinding[] bindings;
    };

    public struct FInputConstructorArgs
    {
        public UStatistics statistics;
        public FDefaultBindings bindings;
    };

    public class TInput : ISystem
    {
        private readonly Dictionary<string, List<ButtonDelegate>> keyEvents = new Dictionary<string, List<ButtonDelegate>>();
        private readonly Dictionary<string, List<Axis2DDelegate>> axis2DEvents = new Dictionary<string, List<Axis2DDelegate>>();
        private readonly Dictionary<string, List<AxisDelegate>> axisEvents = new Dictionary<string, List<AxisDelegate>>();

        // per frame binding events
        public readonly List<FBinding> keyBindings = new List<FBinding>();
        private readonly HashSet<string> keyStatus = new HashSet<string>();

        private List<IVirtualInputEvent> virtualInputState = new List<IVirtualInputEvent>();
        private List<IVirtualInputEvent> priorVirtualInputState = new List<IVirtualInputEvent>();
        private KeyboardState priorKeyboardState;
        private MouseState priorMouseState;

        // stats
        private readonly UStatistics statistics;

        public TInput(FInputConstructorArgs args)
        {
            StaticValidation.TrySetDefault(args.statistics, out statistics);

            priorMouseState = Mouse.GetState();
            priorKeyboardState = Keyboard.GetState();

            foreach(var bindings in args.bindings.bindings)
            {
                keyBindings.Add(bindings);
            }
        }

        public static FBinding[] GetDefaultKeybindings()
        {
            List<FBinding> defaultBindings = new List<FBinding>
            {
                new FBinding(EMouseButtons.LeftButton, "primary"),
                new FBinding(EMouseButtons.RightButton, "secondary"),
                new FBinding(EMouseButtons.MiddleButton, "tertiary"),
                new FBinding(EMouseButtons.ScrollDown, "scrollup"),
                new FBinding(EMouseButtons.ScrollUp, "scrolldown"),

                new FBinding(Keys.Up, "uparrow"),
                new FBinding(Keys.Down, "downarrow"),
                new FBinding(Keys.Left, "leftarrow"),
                new FBinding(Keys.Right, "rightarrow"),

                new FBinding(Keys.D0, "0"),
                new FBinding(Keys.D1, "1"),
                new FBinding(Keys.D2, "2"),
                new FBinding(Keys.D3, "3"),
                new FBinding(Keys.D4, "4"),
                new FBinding(Keys.D5, "5"),
                new FBinding(Keys.D6, "6"),
                new FBinding(Keys.D7, "7"),
                new FBinding(Keys.D8, "8"),
                new FBinding(Keys.D9, "9"),

                new FBinding(Keys.Escape, "escape"),
                new FBinding(Keys.Back, "back"),
                new FBinding(Keys.LeftShift, "lshift"),
                new FBinding(Keys.LeftControl, "lctrl")
            };

            return defaultBindings.ToArray();
        }

        public Vector2 MousePosition { get; private set; } = Vector2.Zero;

        public FActionHandle BindAction(string action, ButtonDelegate callback)
        {
            if (!keyEvents.ContainsKey(action))
            {
                keyEvents[action] = new List<ButtonDelegate>();
            }
            keyEvents[action].Add(callback);
            return new FActionHandle(action, callback);
        }

        public FActionHandle BindAxis(string action, AxisDelegate callback)
        {
            if (!axisEvents.ContainsKey(action))
            {
                axisEvents[action] = new List<AxisDelegate>();
            }
            axisEvents[action].Add(callback);
            return new FActionHandle(action, callback);
        }

        public FActionHandle BindAxis2D(string action, Axis2DDelegate callback)
        {
            if (!axis2DEvents.ContainsKey(action))
            {
                axis2DEvents[action] = new List<Axis2DDelegate>();
            }
            axis2DEvents[action].Add(callback);
            return new FActionHandle(action, callback);
        }

        public bool PollActionBinding(string action)
        {
            return keyStatus.Contains(action);
        }

        public void PressVirtualInput(IVirtualInputEvent evt)
        {
            virtualInputState.Add(evt);
        }

        public void RemoveAction(FActionHandle handle)
        {
            if (handle.action != null)
            {
                RemoveKeyAction(handle.action, handle.actionCallback);
            }
            else if (handle.axisCallback != null)
            {
                RemoveAxisAction(handle.action, handle.axisCallback);
            }
            else if (handle.axis2DCallback != null)
            {
                RemoveAxis2DAction(handle.action, handle.axis2DCallback);
            }
        }

        public void RemoveAxis2DAction(string action, Axis2DDelegate callback)
        {
            Debug.Assert(axis2DEvents[action].Remove(callback));
        }

        public void RemoveAxisAction(string action, AxisDelegate callback)
        {
            Debug.Assert(axisEvents[action].Remove(callback));
        }

        public void RemoveKeyAction(string action, ButtonDelegate callback)
        {
            Debug.Assert(keyEvents[action].Remove(callback));
        }

        // interface implementation
        public void Update(TComponentGraph graph, GameTime gameTime)
        {
            keyStatus.Clear(); 
            
            string GetAxisInputSuffix(int A, int B)
            {
                return (A - B > 0) ? ".Pressed" : ".Released";
            }

            string GetKeyInputSuffix(bool current, bool prior)
            {
                return current ? prior ? ".Pressed" : ".OnPressed" : prior ? ".OnReleased" : ".Released";
            }

            void InvokeKey(string action)
            {
                if (keyEvents.ContainsKey(action))
                {
                    keyStatus.Add(action);
                    foreach (var evt in keyEvents[action])
                    {
                        evt.Invoke(gameTime);
                    }
                }
            }

            MouseState currentMouseState = Mouse.GetState();
            KeyboardState currentKeyboardState = Keyboard.GetState();

            //todo extend to other input methods
            MousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);

            statistics.Set("MousePosition", MousePosition.ToString());

            // handle key events
            foreach (var binding in keyBindings)
            {
                // keyboard bindings //
                if (binding.key != Keys.None)
                {
                    Keys key = binding.key;
                    InvokeKey(binding.bound + GetKeyInputSuffix(currentKeyboardState.IsKeyDown(key), priorKeyboardState.IsKeyDown(key)));
                }

                // mouse bindings //
                else if (binding.mouseButton != EMouseButtons.None)
                {
                    switch (binding.mouseButton)
                    {
                        case EMouseButtons.LeftButton:
                            InvokeKey(binding.bound + GetKeyInputSuffix(currentMouseState.LeftButton == ButtonState.Pressed, priorMouseState.LeftButton == ButtonState.Pressed));
                            break;

                        case EMouseButtons.MiddleButton:
                            InvokeKey(binding.bound + GetKeyInputSuffix(currentMouseState.MiddleButton == ButtonState.Pressed, priorMouseState.MiddleButton == ButtonState.Pressed));
                            break;

                        case EMouseButtons.RightButton:
                            InvokeKey(binding.bound + GetKeyInputSuffix(currentMouseState.RightButton == ButtonState.Pressed, priorMouseState.RightButton == ButtonState.Pressed));
                            break;

                        case EMouseButtons.Mouse4:
                            InvokeKey(binding.bound + GetKeyInputSuffix(currentMouseState.XButton1 == ButtonState.Pressed, priorMouseState.XButton1 == ButtonState.Pressed));
                            break;

                        case EMouseButtons.Mouse5:
                            InvokeKey(binding.bound + GetKeyInputSuffix(currentMouseState.XButton2 == ButtonState.Pressed, priorMouseState.XButton2 == ButtonState.Pressed));
                            break;

                        case EMouseButtons.ScrollUp:
                            InvokeKey(binding.bound + GetAxisInputSuffix(currentMouseState.ScrollWheelValue, priorMouseState.ScrollWheelValue));
                            break;

                        case EMouseButtons.ScrollDown:
                            InvokeKey(binding.bound + GetAxisInputSuffix(priorMouseState.ScrollWheelValue, currentMouseState.ScrollWheelValue));
                            break;

                        default:
                            break;
                    }
                }

                //todo handle other input peripherals here
                else
                {
                }
            }

            // custom bindings //
            foreach (var binding in keyBindings)
            {
                if (binding.custom != null)
                {
                    bool isPressed = virtualInputState.Exists((item) => item.GetType() == binding.custom.GetType());
                    bool wasPressed = priorVirtualInputState.Exists((item) => item.GetType() == binding.custom.GetType());
                    string key = binding.bound + GetKeyInputSuffix(isPressed, wasPressed);
                    InvokeKey(key);
                }
            }

            priorMouseState = currentMouseState;
            priorKeyboardState = currentKeyboardState;
            priorVirtualInputState.Clear();
            priorVirtualInputState.AddRange(virtualInputState);
            virtualInputState.Clear();
        }

        public void Draw(TComponentGraph graph, GameTime gameTime){}
    }
}