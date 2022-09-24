using MonoGame.Framework.Content.Pipeline.Builder;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Tide.Core
{
    public class TSystemGraph : IEnumerable<ISystem>
    {
        private readonly List<ISystem> systems = new List<ISystem>();

        public void Add(ISystem system)
        {
            systems.Add(system);
        }

        public IEnumerator<ISystem> GetEnumerator()
        {
            return systems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return systems.GetEnumerator();
        }

        public T Find<T>()
        {
            return (T) systems.Find((item) => item is T);
        }

        // 
        public void Replace(ISystem system)
        {
            systems.RemoveAll((item) => item.GetType() == system.GetType());
            systems.Add(system);
        }
    }
}