using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tide.Core
{
    public class FLineRendererComponentConstructorArgs
    {
        public UContentManager content;
        public ICoordinateSystem coordinateSystem;
        public Texture2D texture;
    }

    public class ALineRendererComponent : UComponent, IDrawableComponent
    {
        private readonly Texture2D lineTexture = null;
        private readonly List<List<Vector2>> points = new List<List<Vector2>>();
        private readonly List<Color> colors = new List<Color>();
        private readonly List<float> thicknesses = new List<float>();

        public ALineRendererComponent(FLineRendererComponentConstructorArgs args)
        {
            NullCheck(args.content);
            lineTexture = args.texture ?? args.content.GenerateNullTexture(Color.White);
            CoordinateSystem = args.coordinateSystem ?? ATransformComponent.defaultCoordinateSystem;
        }

        public int Count => points.Count;
        public ICoordinateSystem CoordinateSystem { get; private set; }

        public void DrawLine(SpriteBatch spriteBatch, Vector2 origin, Vector2 destination, Color color, FView view2D, float thickness = 1f)
        {
            Vector2 origin2d = CoordinateSystem.ConvertTo(origin);
            Vector2 destin2d = CoordinateSystem.ConvertTo(destination);

            origin2d *= new Vector2(1f, -1f);
            destin2d *= new Vector2(1f, -1f);

            float dist = Vector2.Distance(origin2d, destin2d);
            float angle = (float)Math.Atan2(destin2d.Y - origin2d.Y, destin2d.X - origin2d.X);
            Vector2 centre = new Vector2(0f, 0.5f);
            Vector2 scale = new Vector2(dist, thickness);

            spriteBatch.Draw(lineTexture, origin2d, null, color, angle, centre, scale, SpriteEffects.None, 0);
        }

        public void SetPoint(int line, int pt, Vector2 point)
        {
            points[line][pt] = point;
        }

        public void AddPoint(int line, Vector2 point)
        {
            points[line].Add(point);
        }

        public int AddLine(Color color, float thickness = 1f)
        {
            points.Add(new List<Vector2>());
            colors.Add(color);
            thicknesses.Add(thickness);
            return points.Count - 1;
        }

        public void Draw(FView view2D, SpriteBatch spriteBatch, GameTime gameTime)
        {
            for (int l = 0; l < points.Count; l++)
            {
                for (int p = 0; p < points[l].Count - 1; p++)
                {
                    DrawLine(spriteBatch, points[l][p], points[l][p + 1], colors[l], view2D, thicknesses[l]);
                }
            }
        }

        public Vector2 GetPoint(int line, int i)
        {
            return points[line][i];
        }

        public void RemoveLine(int line)
        {
            points.RemoveAt(line);
            colors.RemoveAt(line);
        }
    }
}
