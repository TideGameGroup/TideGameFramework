using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Text;
using Tide.XMLSchema;

namespace Tide.Core
{
    public class AScene : UComponent
    {
        public AScene(ContentManager Content, string levelToLoad)
        {
            /*
            // load level data
            FScene leveldata = Content.Load<FScene>(levelToLoad);
            
            // combine into a single list
            List<FSerialisedScript> prefabs = leveldata.serialisedClasses;
            foreach (var scriptName in leveldata.scripts)
            {
                var script = new FSerialisedScript();
                script.typename = scriptName;
                script.data = null;
                prefabs.Add(script);
            }

            // iterate over list add scripts,
            // skip over those that depend on later scripts 
            int i = 0;

            // exit if index exceeds the length of the list
            while (i < prefabs.Count)
            {
                if (ScriptGraph.Emplace(prefabs[i], this) != null)
                {
                    // script was initialised succesfully, remove it from the list
                    prefabs.RemoveAt(i);

                    // go back to start of the list
                    i = 0;
                }
                else
                {
                    // unable to initialise script
                    // try the next one
                    i++;
                };
            }
            */
        }
    }
}
