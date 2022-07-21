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
    }
}