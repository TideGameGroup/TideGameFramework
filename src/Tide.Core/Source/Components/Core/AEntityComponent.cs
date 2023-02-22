using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Tide.XMLSchema;

namespace Tide.Core
{
    public delegate void EntityDelegate(int i);

    public delegate void EntityUpdateDelegate(int i, GameTime gameTime);

    public class AEntityComponent : UComponent, ISerialisableComponent, IUpdateComponent
    {
        protected GameTime gameTime = null;
        protected List<int> removeList = new List<int>();
        protected List<double> timestamps = new List<double>();

        public AEntityComponent()
        {
            Transforms = new ATransformComponent();
            SpriteRenderer = new ASpritesRenderComponent(Transforms);

            AddChildComponent(Transforms);
            AddChildComponent(SpriteRenderer);
        }

        public ICoordinateSystem CoordinateSystem => Transforms.CoordinateSystem;
        public int Count => Transforms.Count;
        public EntityDelegate OnAddEntity { get; set; }
        public EntityDelegate OnRemoveEntity { get; set; }
        public EntityUpdateDelegate OnUpdateEntity { get; set; }
        public ASpritesRenderComponent SpriteRenderer { get; protected set; }
        public ATransformComponent Transforms { get; protected set; }

        public int Add(Vector2 position, string animation, float scale = 1f)
        {
            Transforms.Add(0, position, scale);
            SpriteRenderer.Add(animation, scale);
            timestamps.Add(gameTime != null ? gameTime.TotalGameTime.TotalSeconds : 0.0f);

            OnAddEntity?.Invoke(Transforms.Count - 1);

            return Transforms.Count - 1;
        }

        public double GetLifeTime(int i)
        {
            return gameTime == null ? 0.0 : gameTime.TotalGameTime.TotalSeconds - timestamps[i];
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

        #region ISerialisableScript

        public void Load(UContentManager content, string serialisedScriptPath)
        {
            if (serialisedScriptPath == "" || serialisedScriptPath == null) { return; }

            FEntities entities = content.Load<FEntities>(serialisedScriptPath);

            Transforms.Load(content, entities.transform);
            SpriteRenderer.Load(content, entities.spriteRenderer);

            for (int i = 0; i < Transforms.Count; i++)
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
                transform = tID,
                spriteRenderer = sID,
            }
            );
            return ID;
        }

        #endregion ISerialisableScript

        #region IUpdateComponent

        public void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;
        }

        #endregion IUpdateComponent
    }
}