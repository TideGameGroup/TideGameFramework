using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tide.Core
{
    public class UStatistics : UComponent
    {
        public Dictionary<string, string> stats = new Dictionary<string, string>();

        public static UStatistics Get;

        public string this[string stat]
        {
            get { return stats[stat]; }
        }

        public void Set(string stat, string value)
        {
            stats[stat] = value;
        }
    }
}
