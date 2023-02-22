using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Tide.XMLSchema;

namespace Tide.Core
{

    public class ATransformComponent : UComponent, ISerialisableComponent
    {
        internal readonly List<float> worldAngles = new List<float>();
        internal readonly List<Vector2> worldPositions = new List<Vector2>();
        internal readonly List<float> worldScales = new List<float>();
        internal readonly List<float> angles = new List<float>();
        internal readonly List<Vector2> positions = new List<Vector2>();
        internal readonly List<float> scales = new List<float>();
        internal readonly List<float> layers = new List<float>();
        public static ICoordinateSystem defaultCoordinateSystem = new FCartesianCoordinates();

        public ATransformComponent(ICoordinateSystem coordinateSystem = null)
        {
            CoordinateSystem = coordinateSystem ?? defaultCoordinateSystem;
        }

        public ReadOnlyCollection<float> Angles => worldAngles.AsReadOnly();
        public ICoordinateSystem CoordinateSystem { get; private set; }
        public int Count => positions.Count;
        public ReadOnlyCollection<Vector2> Positions => worldPositions.AsReadOnly();
        public ReadOnlyCollection<float> Scales => worldScales.AsReadOnly();

        public Matrix this[int i]
        {
            get { return Matrix.CreateScale(worldScales[i]) * Matrix.CreateRotationZ(worldAngles[i]) * Matrix.CreateTranslation(FStaticVectorFunctions.ToVector3(worldPositions[i])); }
        }

        public void Add(float angle, Vector2 position, float scale = 1.0f, float layer = 0.0f)
        {
            angles.Add(CoordinateSystem.ConvertAngleTo(angle));
            positions.Add(CoordinateSystem.ConvertTo(position));
            scales.Add(CoordinateSystem.ConvertAngleTo(scale));

            layers.Add(layer);

            worldAngles.Add(angle);
            worldPositions.Add(position);
            worldScales.Add(scale);
        }

        public float GetAngle(int i)
        {
            return worldAngles[i];
        }

        public Vector2 GetPosition(int i)
        {
            return worldPositions[i];
        }

        public float GetScale(int i)
        {
            return worldScales[i];
        }

        public void RemoveAt(int i)
        {
            angles.RemoveAt(i);
            positions.RemoveAt(i);
            scales.RemoveAt(i);
            worldAngles.RemoveAt(i);
            worldPositions.RemoveAt(i);
            worldScales.RemoveAt(i);
        }

        public void SetAngle(int i, float angle)
        {
            worldAngles[i] = angle;
            angles[i] = CoordinateSystem.ConvertAngleTo(angle);
        }

        public void SetLayer(int i, float layer)
        {
            layers[i] = layer;
        }

        public void SetPosition(int i, Vector2 position)
        {
            worldPositions[i] = position;
            positions[i] = CoordinateSystem.ConvertTo(position);
        }

        public void SetScale(int i, float scale)
        {
            worldScales[i] = scale;
            scales[i] = CoordinateSystem.ConvertScaleTo(scale);
        }

        #region ISerialisableComponent

        public void Load(UContentManager content, string serialisedScriptPath)
        {
            if (serialisedScriptPath == "") { return; }

            FTransform instance = content.Load<FTransform>(serialisedScriptPath);

            for (int i = 0; i < instance.positions.Length; i++)
            {
                Add(instance.angles[i], instance.positions[i], instance.scales[i]);
            }
        }

        public string Serialise(string path, ref Dictionary<string, ISerialisedInstanceData> serialisedSet)
        {
            string ID = ISerialisableComponent.GetSerialisableID(this, path, ref serialisedSet);

            serialisedSet.Add(ID,
                new FTransform
                {
                    positions = positions.ToArray(),
                    angles = angles.ToArray(),
                    scales = scales.ToArray()
                }
            );

            return ID;
        }

        #endregion ISerialisableComponent
    }
}