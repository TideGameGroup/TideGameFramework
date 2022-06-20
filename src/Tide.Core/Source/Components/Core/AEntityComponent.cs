using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tide.XMLSchema;

namespace Tide.Core
{
    public delegate void EntityDelegate(int i);
    public delegate void EntityUpdateDelegate(int i, GameTime gameTime);

    public class AEntityComponent : UComponent, ISerialisableComponent, IUpdateComponent
    {
        protected List<double> timestamps = new List<double>();
        protected GameTime gameTime = null;

        protected List<int> removeList = new List<int>();

        public AEntityComponent()
        {
            Transforms = new ATransform2D();
            SpriteRenderer = new ASpritesRenderer(Transforms);

            RegisterChildComponent(Transforms);
            RegisterChildComponent(SpriteRenderer);
        }

        public int Count => Transforms.Count;
        public ASpritesRenderer SpriteRenderer { get; protected set; }
        public ATransform2D Transforms { get; protected set; }
        public EntityDelegate OnAddEntity { get; set; }
        public EntityDelegate OnRemoveEntity { get; set; }
        public EntityUpdateDelegate OnUpdateEntity { get; set; }

        public int Add(Vector3 position, string animation, float scale = 1f)
        {
            Transforms.Add(0, position, scale);
            SpriteRenderer.Add(animation, scale);
            timestamps.Add(gameTime != null ? gameTime.TotalGameTime.TotalSeconds : 0.0f);

            OnAddEntity?.Invoke(Transforms.Count - 1);

            return Transforms.Count - 1;
        }

        public bool RemoveAt(int i)
        {
            OnRemoveEntity?.Invoke(i);

            Transforms.RemoveAt(i);
            SpriteRenderer.RemoveAt(i);
            timestamps.RemoveAt(i);

            return true;
        }

        public bool RemoveAtSafe(int i)
        {
            OnRemoveEntity?.Invoke(i);

            Transforms.RemoveAt(i);
            SpriteRenderer.RemoveAt(i);
            timestamps.RemoveAt(i);

            return true;
        }

        public double GetLifeTime(int i)
        {
            return gameTime.TotalGameTime.TotalSeconds - timestamps[i];
        }

        #region ISerialisableScript
        public void Load(UContentManager content, string serialisedScriptPath)
        {
            if (serialisedScriptPath == "" || serialisedScriptPath == null) { return; }

            FEntities entities = content.Load<FEntities>(serialisedScriptPath);

            Transforms.Load(content, entities.transform2D);
            SpriteRenderer.Load(content, entities.spriteRenderer);

            for (int i = 0; i < Transforms.positions.Count; i++)
            {
                timestamps.Add(gameTime != null ? gameTime.TotalGameTime.TotalSeconds : 0.0f);
                OnAddEntity?.Invoke(i);
            }
        }

        public string Serialise(string path, ref Dictionary<string, ISerialisedInstanceData> serialisedSet)
        {
            string ID = ISerialisableComponent.GetSerialisableID(this, path, ref serialisedSet);
            string tID = Transforms.Serialise(path, ref serialisedSet);
            string sID = SpriteRenderer.Serialise(path, ref serialisedSet);

            serialisedSet.Add(ID, new FEntities
            {
                transform2D = tID,
                spriteRenderer = sID,
            }
            );
            return ID;
        }
        #endregion

        #region IUpdateComponent
        public void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;
        }
        #endregion
    }
}
