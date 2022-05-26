using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using Tide.XMLSchema;

namespace Tide.Core
{
    public interface ISerialisableComponent
    {
        public void Load(UContentManager content, string serialisedDataPath);

        public string Serialise(string path, ref Dictionary<string, ISerialisedInstanceData> serialisedSet);

        public static string GetSerialisableID(object obj, string path, ref Dictionary<string, ISerialisedInstanceData> serialisedSet)
        {
            int intPID = path.GetHashCode();
            int intID = obj.GetHashCode();
            string ID = string.Format("{0}_{1}", intPID, intID);

            while (serialisedSet.ContainsKey(ID))
            {
                intID++;
                ID = string.Format("{0}", intID);
            }
            return ID;
        }
    }
}
