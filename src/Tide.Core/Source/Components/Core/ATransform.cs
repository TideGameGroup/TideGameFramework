using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using Tide.XMLSchema;

namespace Tide.Core
{
    public class ATransform : UComponent, ISerialisableComponent
    {
        public List<float> angles       = new List<float>();
        public List<float> scales       = new List<float>();
        public List<Vector2> positions  = new List<Vector2>();
        public int Count => positions.Count;

        public Matrix this[int i]
        {
            get { return Matrix.CreateScale(scales[i]) * Matrix.CreateRotationZ(angles[i]) * Matrix.CreateTranslation(FVectorHelper.ToVector3(positions[i])); }
        }

        public void Add(float angle, Vector2 position, float scale = 1.0f)
        {
            angles.Add(angle);
            positions.Add(position);
            scales.Add(scale);
        }

        public void RemoveAt(int i)
        {
            angles.RemoveAt(i);
            positions.RemoveAt(i);
            scales.RemoveAt(i);
        }

        #region ISerialisableComponent


        public void Load(UContentManager content, string serialisedScriptPath)
        {
            if (serialisedScriptPath == "") { return; }

            FTransform instance = content.Load<FTransform>(serialisedScriptPath);

            List<Vector2> truePositions = new List<Vector2>();

            for (int i = 0; i < instance.positions.Length; i++)
            {
                instance.positions[i].Y = -instance.positions[i].Z;
                instance.positions[i].Z = 0f;
                instance.positions[i] = instance.positions[i] * 32f;
            }

            for (int i = 0; i < truePositions.Count; i++)
            {
                Add(instance.angles[i], truePositions[i], instance.scales[i]);
            }
        }

        public string Serialise(string path, ref Dictionary<string, ISerialisedInstanceData> serialisedSet)
        {
            string ID = ISerialisableComponent.GetSerialisableID(this, path, ref serialisedSet);

            serialisedSet.Add(ID,
                new FTransform
                {
                    //positions = positions.ToArray(),
                    angles = angles.ToArray(),
                    scales = scales.ToArray()
                }
            );

            return ID;
        }

        #endregion
    }
}