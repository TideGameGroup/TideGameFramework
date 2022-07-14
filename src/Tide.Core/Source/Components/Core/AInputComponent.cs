using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Tide.Core
{
    [Flags]
    public enum EFocus
    {
        None = 0,
        Console = 1,
        UI = 2,
        Cinematic = 4,
        GameUI = 8,
        Game = 16
    }

    public class AInputComponent : UComponent
    {
        private List<FActionHandle> actionHandles = new List<FActionHandle>();
        public static List<EFocus> focusList = new List<EFocus>();

        public AInputComponent(UInput handler)
        {
            Handler = handler;
            Active = true;

            if (focusList.Count == 0)
            {
                PushFocus(EFocus.Game);
            }

            OnUnregisterComponent += OnUnregisterScript;
        }

        private bool Active { get; set; }
        public Vector2 MousePosition => Handler.MousePosition;
        public UInput Handler { get; private set; }

        public static void PopFocus(EFocus focus)
        {
            focusList.Remove(focus);
            focusList.Sort();
        }

        public static void PushFocus(EFocus focus)
        {
            if (focusList.Count == 0 || focusList[0] >= focus)
            {
                focusList.Add(focus);
                focusList.Sort();
            }
        }

        public FActionHandle BindAction(string action, EFocus focus, ButtonDelegate callback)
        {
            FActionHandle handle = Handler.BindAction(action, (GameTime gt) =>
            {
                if (CheckValidToTrigger(focus)) { callback.Invoke(gt); }
            });
            actionHandles.Add(handle);
            return handle;
        }

        public FActionHandle BindRawAction(string action, ButtonDelegate callback)
        {
            FActionHandle handle = Handler.BindAction(action, callback);
            actionHandles.Add(handle);
            return handle;
        }

        public bool CheckValidToTrigger(EFocus focus)
        {
            return Active && focusList.Count > 0 && focus.HasFlag(focusList[0]);
        }

        /*
        public void BindAction(string action, ButtonDelegate callback)
        {
            TActionHandle handle = Handler.BindAction(action, (GameTime gt) =>
            {
                if (CheckValidToTrigger(Focus)) { callback.Invoke(gt); }
            });
            actionHandles.Add(handle);
        }
        */

        
        public void BindAxis(string action, EFocus focus, AxisDelegate callback)
        {
            Handler.BindAxis(action, (float x, GameTime gt) =>
            {
                if (CheckValidToTrigger(focus)) { callback.Invoke(x, gt); }
            });
        }

        /*
        public void BindAxis2D(string action, Axis2DDelegate callback)
        {
            Handler.BindAxis2D(action, (float x, float y, GameTime gt) =>
            {
                if (CheckValidToTrigger(Focus)) { callback.Invoke(x, y, gt); }
            });
        }
        */

        public void OnUnregisterScript()
        {
            ClearBindings();
        }

        public bool PollActionBinding(string action)
        {
            return Handler.PollActionBinding(action);
        }

        public void UnbindAction(FActionHandle handle)
        {
            Handler.RemoveAction(handle);
        }

        public void ClearBindings()
        {
            foreach (FActionHandle handle in actionHandles)
            {
                UnbindAction(handle);
            }
        }
    }
}