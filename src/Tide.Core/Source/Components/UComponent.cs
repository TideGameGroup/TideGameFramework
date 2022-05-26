using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Tide.XMLSchema;

namespace Tide.Core
{
    public class UComponent
    {
        public List<UComponent> Children { get; private set; }
        public UComponent Parent { get; set; }
        public virtual UComponentGraph ScriptGraph => Parent.ScriptGraph;

        public bool bIsActive  = true;

        public bool bIsVisible = true;

        public UComponent()
        {
            Children    = new List<UComponent>();
            Parent      = null;
        }

        public OnGraphEvent OnRegisterChildComponent { get; set; }
        public OnGraphEvent OnUnregisterChildComponent { get; set; }
        public OnEvent OnRegisterComponent { get; set; }
        public OnEvent OnUnregisterComponent { get; set; }

        public UComponent RegisterChildComponent(UComponent child, int at = -1)
        {
            if (child == null) { return null; }

            if (Children == null)
            {
                Children = new List<UComponent>();
            }
            child.Parent = this;

            at = (at == -1) ? Children.Count : at;
            Children.Insert(at, child);

            OnRegisterChildComponent?.Invoke(child);
            child.OnRegisterComponent?.Invoke();

            return child;
        }

        public T GetChildComponent<T>() where T : UComponent
        {
            return (T)Children.Find((item) => item is T);
        }

        public void UnregisterChildComponent(UComponent child)
        {
            if (Children == null || child == null) { return; }

            while (child.Children.Count > 0)
            {
                child.UnregisterChildComponent(child.Children[0]);
            }

            child.Parent = null;
            child.OnUnregisterComponent?.Invoke();

            Children.Remove(child);
            OnUnregisterChildComponent?.Invoke(child);
        }
    }
}
