using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Tide.Core;
using Tide.XMLSchema;
using MonoGame.Framework.Content.Pipeline.Builder;
using Microsoft.Xna.Framework.Content.Pipeline;
using System.Diagnostics;

namespace Tide.Tools
{
    public class UImportTools : UComponent
    {
        public UImportTools() { }

        public static bool ImportSerialisedData<T>(string projectDir, string filename, out T ProcessedContent)
        {
            string tempDir = Path.Combine(projectDir, "tempBin");
            ProcessedContent = default;
            PipelineManager PM = new PipelineManager(projectDir, tempDir, "Path\\tempBin");

            bool Worked = false;
            try
            {
                var BuiltContent = PM.BuildContent(filename);
                ProcessedContent = (T)PM.ProcessContent(BuiltContent);
                Worked = true;
            }
            catch (Exception E)
            {
                Debug.Write(E.Message);
            }

            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }

            return Worked;
        }
    }
}
