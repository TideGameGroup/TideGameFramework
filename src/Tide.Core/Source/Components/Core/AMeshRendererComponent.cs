using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Tide.XMLSchema;

namespace Tide.Core
{
    internal class AMeshRendererComponent : UComponent, IDrawableComponent, ISerialisableComponent
    {
        private List<Model>     models      = new List<Model>();
        private List<Texture2D> textures    = new List<Texture2D>();
        private readonly UContentManager content;

        // methods
        public AMeshRendererComponent(UContentManager content, ATransform2D transforms)
        {
            this.content = content ?? throw new ArgumentNullException(nameof(content));
            Transforms = transforms ?? throw new ArgumentNullException(nameof(transforms));
        }

        public int Count => Transforms.Count;
        public ATransform2D Transforms { get; protected set; }

        public void Add(Model model, Texture2D texture)
        {
            //todo default mesh like source?
            models.Add(model);
            textures.Add(texture);
        }

        public void Add(string model, string texture)
        {
            Model _model = content.Load<Model>(model);
            Texture2D _texture = content.Load<Texture2D>(texture);

            Add(_model, _texture);
        }

        #region ISerialisableComponent
        public void Load(UContentManager content, string serialisedDataPath)
        {
            if (serialisedDataPath != null && serialisedDataPath != null)
            {
                FMeshData meshData = content.Load<FMeshData>(serialisedDataPath);

                foreach (var model in meshData.models)
                {
                    models.Add(content.Load<Model>(model));
                }

                foreach (var texture in meshData.textures)
                {
                    textures.Add(content.Load<Texture2D>(texture));
                }
            }
        }

        public string Serialise(string path, ref Dictionary<string, ISerialisedInstanceData> serialisedSet)
        {
            return "";
        }

        #endregion
        #region IDrawableComponent
        public void Draw(UView3D view3D, GameTime gameTime)
        {
            for (int i = 0; i < models.Count; i++)
            {
                foreach (ModelMesh mesh in models[i].Meshes)
                {
                    //todo fix frustum culling
                    //if (!view.Frustum.Intersects(mesh.BoundingSphere)) { continue; }// view frustum culling 

                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = Transforms[i];
                        effect.View = view3D.ViewMatrix;
                        effect.Projection = view3D.Projection;
                        effect.TextureEnabled = true;
                        effect.Texture = textures[i];
                    }

                    mesh.Draw();
                }
            }
        }
        #endregion
    }
}
