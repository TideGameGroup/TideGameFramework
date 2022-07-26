using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Tide.Core
{
    [Flags]
    public enum EFocus
    {
        None = 0,
        Layer1 = 1,
        Console = 2,
        Layer2 = 2,
        Layer3 = 4,
        Layer4 = 8,
        UI = 16,
        Layer5 = 16,
        Layer6 = 32,
        Layer7 = 64,
        Layer8 = 128,
        Cinematic = 256,
        Layer9 = 256,
        Layer10 = 512,
        Layer11 = 1024,
        GameUI = 2048,
        Layer12 = 2048,
        Layer13 = 4096,
        Game = 8192,
        Layer14 = 8192,
        Layer15 = 16384
    }

    public class AInputComponent : UComponent
    {
        private readonly List<FActionHandle> actionHandles = new List<FActionHandle>();
        private List<EFocus> localFocusList = new List<EFocus>();
        public static List<EFocus> focusList = new List<EFocus>();

        public AInputComponent(TInput handler)
        {
            Handler = handler;

            if (focusList.Count == 0)
            {
                PushFocus(EFocus.Game);
            }

            base.OnUnregisterComponent += DoUnregisterComponent;
            OnSetActive += DoActivateDeactivateComponent;
        }

        public TInput Handler { get; private set; }

        public Vector2 MousePosition => Handler.MousePosition;

        private void DoActivateDeactivateComponent(bool val)
        {
            if (val == false)
            {
                foreach (var local in localFocusList)
                {
                    focusList.Remove(local);
                }
            }
            else
            {
                foreach (var local in localFocusList)
                {
                    focusList.Add(local);
                }
            }

            focusList.Sort();
        }

        private void DoUnregisterComponent()
        {
            ClearBindings();
            IsActive = false;
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

        public void BindAxis(string action, EFocus focus, AxisDelegate callback)
        {
            Handler.BindAxis(action, (float x, GameTime gt) =>
            {
                if (CheckValidToTrigger(focus)) { callback.Invoke(x, gt); }
            });
        }

        public FActionHandle BindRawAction(string action, ButtonDelegate callback)
        {
            FActionHandle handle = Handler.BindAction(action, callback);
            actionHandles.Add(handle);
            return handle;
        }

        public bool CheckValidToTrigger(EFocus focus)
        {
            return IsActive && focusList.Count > 0 && focus.HasFlag(focusList[0]);
        }

        public void ClearBindings()
        {
            foreach (FActionHandle handle in actionHandles)
            {
                UnbindAction(handle);
            }
        }

        public bool PollActionBinding(string action)
        {
            return Handler.PollActionBinding(action);
        }

        public void PopFocus(EFocus focus)
        {
            localFocusList.Remove(focus);
            focusList.Remove(focus);
            focusList.Sort();
        }

        public void PushFocus(EFocus focus)
        {
            localFocusList.Add(focus);
            focusList.Add(focus);
            focusList.Sort();
        }

        public void UnbindAction(FActionHandle handle)
        {
            Handler.RemoveAction(handle);
        }
    }
}