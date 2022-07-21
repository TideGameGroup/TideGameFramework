using System;
using System.Collections.Generic;

namespace Tide.Core
{
    public class UComponent
    {
        private readonly List<Func<bool>> deferredRegistrations = new List<Func<bool>>();
        private readonly List<Func<bool>> deferredUnregistrations = new List<Func<bool>>();

        public UComponent()
        {
            Children = new List<UComponent>();
            Parent = null;
        }

        public List<UComponent> Children { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsVisible { get; private set; }
        public OnGraphEvent OnRegisterChildComponent { get; set; }
        public OnEvent OnRegisterComponent { get; set; }
        public OnPropertyEvent OnSetActive { get; set; }
        public OnPropertyEvent OnSetVisibility { get; set; }
        public OnGraphEvent OnUnregisterChildComponent { get; set; }
        public OnEvent OnUnregisterComponent { get; set; }
        public UComponent Parent { get; set; }
        public virtual TComponentGraph ScriptGraph => Parent.ScriptGraph;

        //public USerialisationComponent SerialisationComponent { get; set; }

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

        public bool DeferredAddChildComponent(UComponent child, int at = -1)
        {
            if (child == null) { return false; }

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

            return true;
        }

        public bool DeferredRemoveChildComponent(UComponent child)
        {
            if (Children == null || child == null) { return false; }

            while (child.Children.Count > 0)
            {
                child.DeferredRemoveChildComponent(child.Children[0]);
            }

            child.Parent = null;
            child.OnUnregisterComponent?.Invoke();

            Children.Remove(child);
            OnUnregisterChildComponent?.Invoke(child);

            return true;
        }

        public T GetChildComponent<T>() where T : UComponent
        {
            return (T)Children.Find((item) => item is T);
        }

        public UComponent AddChildComponent(UComponent child, int at = -1)
        {
            deferredRegistrations.Add(() => DeferredAddChildComponent(child, at));
            return child;
        }

        public void SetActive(bool isActive)
        {
            IsActive = isActive;
            OnSetActive?.Invoke(isActive);
        }

        public void SetVisibility(bool isVisible)
        {
            IsVisible = isVisible;
            OnSetVisibility?.Invoke(isVisible);
        }

        public UComponent RemoveChildComponent(UComponent child)
        {
            deferredUnregistrations.Add(() => DeferredRemoveChildComponent(child));
            return child;
        }

        public void Update()
        {
            foreach(var f in deferredRegistrations)
            {
                f.Invoke();
            }
            deferredRegistrations.Clear();

            foreach (var f in deferredUnregistrations)
            {
                f.Invoke();
            }
            deferredUnregistrations.Clear();
        }
    }
}