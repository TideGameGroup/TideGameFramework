using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tide.Core
{
    public class AFiniteStateMachine : UComponent, IUpdateComponent
    {
        private List<UComponent> TrueChildren = new List<UComponent>();
        
        public int ActiveChild { get; private set; }

        private int deferredActiveChild = 0;

        public AFiniteStateMachine()
        {
            OnRegisterChildComponent += OnRegisterChild;
        }

        public UComponent GetActiveScript()
        {
            return TrueChildren[ActiveChild];
        }

        public bool IsValidIndex(int index)
        {
            return index >= 0 && index < TrueChildren.Count;
        }

        private void SwitchStateDeferred(int index)
        {
            if (IsValidIndex(index))
            {
                RemoveChildComponent(TrueChildren[ActiveChild]);
                AddChildComponent(TrueChildren[index]);
                ActiveChild = index;
            }
        }

        public void SwitchState(int index)
        {
            deferredActiveChild = index;
        }

        public void OnRegisterChild(UComponent child)
        {
            if (!TrueChildren.Contains(child))
            {
                TrueChildren.Add(child);
                RemoveChildComponent(child);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (deferredActiveChild != -1)
            {
                SwitchStateDeferred(deferredActiveChild);
            }
            deferredActiveChild = -1;
        }
    }
}