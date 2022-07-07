﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Tide.XMLSchema;

namespace Tide.Core
{
    public delegate void OnEvent();

    public delegate void OnGraphEvent(UComponent child);

    public class UComponentGraph : IEnumerable<UComponent>
    {
        public UComponentGraph()
        {
            RootScript = new ARootScript
            {
                RootParent = this
            };
        }

        public static IEnumerable<UComponent> _GetScriptsRecursive(UComponent script)
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

        public UComponent Add(UComponent script, UComponent parent = null, int at = -1)
        {
            if (parent == null)
            {
                parent = RootScript;
            }
            parent.RegisterChildComponent(script, at);
            return script;
        }

        public void Remove(UComponent script)
        {
            RootScript.UnregisterChildComponent(script);
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
            RootScript.UnregisterChildComponent(toRemove);
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
                RootScript.UnregisterChildComponent(script);
            }
            return toRemove.Count;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private ARootScript RootScript { get; }

        private class ARootScript : UComponent
        {
            public UComponentGraph RootParent = null;
            public override UComponentGraph ScriptGraph => RootParent;
        }
    }
}