using System;
using System.Collections.Generic;

namespace Tide.Core
{
    public struct FDeferredRegistrationInputs
    {
        public int at;
        public UComponent child;
        public UComponent parent;
    }

    public class UComponent
    {
        private readonly List<UComponent> children = new List<UComponent>();
        internal bool bCanDraw = true;
        internal bool bCanUpdate = true;
        internal bool bIsActive = true;
        internal bool bIsVisible = true;
        public readonly List<FDeferredRegistrationInputs> deferredRegistrations = new List<FDeferredRegistrationInputs>();
        public readonly List<FDeferredRegistrationInputs> deferredUnregistrations = new List<FDeferredRegistrationInputs>();
        public List<UComponent> Children => children;
        public virtual TComponentGraph ComponentGraph => Parent?.ComponentGraph;

        public bool IsActive
        {
            get
            {
                if (Parent != null)
                {
                    return bIsActive && Parent.IsActive;
                }
                else
                {
                    return bIsActive;
                }
            }
            set
            {
                if (value != bIsActive)
                {
                    bIsActive = value;
                    OnSetActive?.Invoke(bIsActive);

                    foreach (var child in children)
                    {
                        child.OnSetActive?.Invoke(bIsActive);
                    }
                }
            }
        }

        public bool IsDirty => deferredUnregistrations.Count > 0 || deferredRegistrations.Count > 0;

        public bool IsVisible
        {
            get
            {
                if (Parent != null)
                {
                    return bIsVisible && Parent.IsVisible;
                }
                else
                {
                    return bIsVisible;
                }
            }
            set
            {
                if (value != bIsVisible)
                {
                    if (value == false && bIsActive) bIsActive = false; //  IsActive = false; ?

                    bIsVisible = value;
                    OnSetVisibility?.Invoke(bIsVisible);

                    foreach (var child in children)
                    {
                        child.OnSetVisibility?.Invoke(bIsVisible);
                    }
                }
            }
        }

        public OnGraphEvent OnRegisterChildComponent { get; set; }
        public OnEvent OnRegisterComponent { get; set; }
        public OnPropertyEvent OnSetActive { get; set; }
        public OnPropertyEvent OnSetVisibility { get; set; }
        public OnGraphEvent OnUnregisterChildComponent { get; set; }
        public OnEvent OnUnregisterComponent { get; set; }
        public UComponent Parent { get; set; }
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

        public UComponent AddChildComponent(UComponent child, int at = -1)
        {
            if (child == null) { return null; }

            deferredRegistrations.Add(new FDeferredRegistrationInputs
            {
                parent = this,
                child = child,
                at = at
            });
            return child;
        }

        public bool DeferredAddChildComponent(UComponent child, int at = -1)
        {
            if (child == null) { return false; }

            child.Parent = this;

            at = (at == -1) ? Children.Count : at;
            at = Math.Min(Children.Count, at);
            Children.Insert(at, child);

            return true;
        }

        public bool DeferredRemoveChildComponent(UComponent child)
        {
            if (Children == null || child == null) { return false; }

            while (child.Children.Count > 0)
            {
                child.DeferredRemoveChildComponent(child.Children[0]);
            }

            child.IsActive = false;
            child.Parent = null;
            Children.Remove(child);

            return true;
        }

        public T GetChildComponent<T>(bool includePending = false) where T : UComponent
        {
            UComponent child = Children.Find((item) => item is T);

            if (child == null && includePending)
            {
                child = deferredRegistrations.Find((item) => item.child is T).child;
            }
            return (T)child;
        }

        public UComponent RemoveChildComponent(UComponent child)
        {
            if (child == null) { return null; }

            deferredUnregistrations.Add(new FDeferredRegistrationInputs
            {
                parent = this,
                child = child,
                at = 0
            });
            return child;
        }
    }
}