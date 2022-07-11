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
            ProcessedContent = default;
            PipelineManager PM = new PipelineManager(projectDir, projectDir + "\\tempBin", "Path\\tempBin");

            bool Worked = false;
            while (!Worked)
                try
                {
                    var BuiltContent = PM.BuildContent(filename);
                    ProcessedContent = (T)PM.ProcessContent(BuiltContent);
                    Worked = true;
                    File.Delete(projectDir + "\\tempBin\\" + filename.TrimEnd(".xml".ToCharArray()) + ".xnb");
                }
                catch (InvalidContentException E)
                {
                    Debug.Write("CompilerException");
                    Debug.Write(E.Message);
                    Worked = true;
                }
                catch (IOException E)
                {
                    Debug.Write("Most likely a leftover file when exiting. Check .\\tempBin");
                    Debug.Write(E.Message);
                    continue;
                }
                catch (Exception E)
                {
                    Debug.Write(E.Message);
                    continue;
                }

            return Worked;
        }
    }
}
