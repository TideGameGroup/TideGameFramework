using System;

namespace Tide.Core
{
    public struct FActionHandle
    {
        public string action;
        public ButtonDelegate actionCallback;
        public Axis2DDelegate axis2DCallback;
        public AxisDelegate axisCallback;

        public FActionHandle(string action, ButtonDelegate callback)
        {
            this.action = action ?? throw new ArgumentNullException(nameof(action));
            actionCallback = callback;
            axisCallback = null;
            axis2DCallback = null;
        }

        public FActionHandle(string action, AxisDelegate callback)
        {
            this.action = action ?? throw new ArgumentNullException(nameof(action));
            actionCallback = null;
            axisCallback = callback;
            axis2DCallback = null;
        }

        public FActionHandle(string action, Axis2DDelegate callback)
        {
            this.action = action ?? throw new ArgumentNullException(nameof(action));
            actionCallback = null;
            axisCallback = null;
            axis2DCallback = callback;
        }
    }
}