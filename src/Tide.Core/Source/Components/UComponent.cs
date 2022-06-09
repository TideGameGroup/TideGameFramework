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

        /// <summary>
        /// This function is syntactic sugar for checking passed values are non-null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="o"></param>
        protected static void NullCheck<T>(T t)
        {
            if (t == null)
            { 
                throw new ArgumentNullException(nameof(t));
            }
        }

        /// <summary>
        /// This function is syntactic sugar for checking passed values are non-null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="o"></param>
        protected static void TrySetDefault<T>(T t, out T o)
        {
            o = t ?? throw new ArgumentNullException(nameof(t));
        }

        protected static void TrySetOptional<T>(T t, out T o)
        {
            o = t;
        }

        public UComponent RegisterChildComponent(UComponent child, int at = -1)
        {
            if (child == null) { return null; }

            if (Children == null)
            {
                Children = new List<UComponent>();
            }
            child.Parent = this;

            at = (at == -1) ? Children.Count : at;
            at = Math.Min(Children.Count, at);
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
