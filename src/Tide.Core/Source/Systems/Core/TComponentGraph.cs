using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Tide.Core
{
    public delegate void OnEvent();

    public delegate void OnGraphEvent(UComponent child);

    public delegate void OnPropertyEvent(bool val);

    public class TComponentGraph : IEnumerable<UComponent> , ISystem
    {
        public TComponentGraph()
        {
            RootScript = new ARootScript
            {
                RootParent = this
            };
        }
        private ARootScript RootScript { get; }

        private static IEnumerable<UComponent> _GetScriptsRecursive(UComponent script)
        {
            yield return script;

            foreach (UComponent child in script.Children)
            {
                foreach (var x in _GetScriptsRecursive(child))
                {
                    yield return x;
                }
            }
        }

        public UComponent Add(UComponent script, UComponent parent = null, int at = -1)
        {
            if (parent == null)
            {
                parent = RootScript;
            }
            parent.AddChildComponent(script, at);
            return script;
        }

        /// <summary>
        /// Searches for an element of the templated type.
        /// </summary>
        /// <returns>The first element that matches the templated type.
        /// object of type T if found; otherwise, null. </returns>
        public T Find<T>() where T : UComponent
        {
            UComponent script = Find((item) => item is T);
            if (script != null)
            {
                return (T)script;
            }
            return null;
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate,
        /// and returns the first occurrence within the entire UScriptGraph.
        /// </summary>
        /// <param name="match">The Predicate<AScript> delegate that defines the conditions of the element to search for.</param>
        /// <returns>The first element that matches the conditions defined by the specified predicate, /// if found; otherwise, null. </returns>
        public UComponent Find(Predicate<UComponent> match)
        {
            foreach (var script in this)
            {
                if (match.Invoke(script))
                {
                    return script;
                }
            }
            return null;
        }

        /// <summary>
        /// Retrieves all the scripts that match the template type.
        /// </summary>
        /// <returns></returns>
        public List<T> FindAll<T>() where T : UComponent
        {
            List<T> found = new List<T>();
            foreach (var script in this)
            {
                if (script is T)
                {
                    found.Add((T)script);
                }
            }
            return found;
        }

        /// <summary>
        /// Retrieves all the scripts that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public List<UComponent> FindAll(Predicate<UComponent> match)
        {
            List<UComponent> found = new List<UComponent>();
            foreach (var script in this)
            {
                if (match.Invoke(script))
                {
                    found.Add(script);
                }
            }
            return found;
        }

        public IEnumerator<UComponent> GetEnumerator()
        {
            return _GetScriptsRecursive(RootScript).GetEnumerator();
        }

        public void Remove(UComponent script)
        {
            RootScript.RemoveChildComponent(script);
        }

        // Remove the first script matching the predicate.
        public void Remove(Predicate<UComponent> match)
        {
            UComponent toRemove = null;
            foreach (var script in this)
            {
                if (match.Invoke(script))
                {
                    toRemove = script;
                    break;
                }
            }
            RootScript.RemoveChildComponent(toRemove);
            return;
        }

        // Remove all scripts matching the predicate.
        public int RemoveAll(Predicate<UComponent> match)
        {
            List<UComponent> toRemove = new List<UComponent>();
            foreach (var script in this)
            {
                if (match.Invoke(script))
                {
                    toRemove.Add(script);
                }
            }
            foreach (var script in toRemove)
            {
                RootScript.RemoveChildComponent(script);
            }
            return toRemove.Count;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #region ISystem

        public void Draw(TComponentGraph graph, GameTime gameTime)
        {
            //throw new NotImplementedException();
        }

        public void Update(TComponentGraph graph, GameTime gameTime)
        {
            bool graphIsDirty = false;

            foreach (UComponent component in graph)
            {
                if (component is IUpdateComponent updater && component.IsActive && component.bCanUpdate)
                {
                    updater.Update(gameTime);
                }

                component.bCanUpdate = component.bIsActive;

                graphIsDirty = graphIsDirty || component.IsDirty;
            }

            if (graphIsDirty)
            {
                UpdateGraph(graph);
            }
        }

        private static void UpdateGraph(TComponentGraph graph)
        {
            List<FDeferredRegistrationInputs> allRegistrations = new List<FDeferredRegistrationInputs>();
            List<FDeferredRegistrationInputs> allUnregistrations = new List<FDeferredRegistrationInputs>();

            UpdateGraphRecursive(graph, allRegistrations, allUnregistrations);

            foreach (var r in allRegistrations)
            {
                r.parent.OnRegisterChildComponent?.Invoke(r.child);
                r.child.OnRegisterComponent?.Invoke();
            }
            foreach (var u in allUnregistrations)
            {
                u.child.OnUnregisterComponent?.Invoke();
                u.parent.OnUnregisterChildComponent?.Invoke(u.child);
            }
        }

        private static void UpdateGraphRecursive(TComponentGraph graph, List<FDeferredRegistrationInputs> allRegistrations, List<FDeferredRegistrationInputs> allUnregistrations)
        {
            List<FDeferredRegistrationInputs> registrations = new List<FDeferredRegistrationInputs>();
            List<FDeferredRegistrationInputs> unregistrations = new List<FDeferredRegistrationInputs>();

            foreach (UComponent component in graph)
            {
                registrations.AddRange(component.deferredRegistrations);
                component.deferredRegistrations.Clear();

                unregistrations.AddRange(component.deferredUnregistrations);
                component.deferredUnregistrations.Clear();
            }

            foreach (var r in registrations)
            {
                r.parent.DeferredAddChildComponent(r.child, r.at);
            }
            foreach (var u in unregistrations)
            {
                u.parent.DeferredRemoveChildComponent(u.child);
            }

            allRegistrations.AddRange(registrations);
            allUnregistrations.AddRange(unregistrations);

            if (registrations.Count > 0 || unregistrations.Count > 0)
            {
                UpdateGraphRecursive(graph, allRegistrations, allUnregistrations);
            }
        }

        #endregion

        private class ARootScript : UComponent
        {
            public TComponentGraph RootParent = null;
            public override TComponentGraph ComponentGraph => RootParent;
        }
    }
}