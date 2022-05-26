using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tide.Core
{
    public class ALineRendererComponent : UComponent, IDrawableComponent2D
    {
        private readonly Texture2D lineTexture = null;
        private List<List<Vector3>> points = new List<List<Vector3>>();
        private List<Color> colors = new List<Color>();

        public ALineRendererComponent(UContentManager Content)
        {
            lineTexture = Content.Load<Texture2D>("Line");
        }

        public int Count => points.Count;

        public void DrawLine(SpriteBatch spriteBatch, Vector3 origin, Vector3 destination, Color color, UView3D view3D)
        {
            Vector2 origin2d = view3D.WorldToScreen(origin, out _);
            Vector2 destin2d = view3D.WorldToScreen(destination, out _);

            float dist = Vector2.Distance(origin2d, destin2d);
            float angle = (float)Math.Atan2(destin2d.Y - origin2d.Y, destin2d.X - origin2d.X);
            Vector2 centre = new Vector2(0f, 0.5f);
            Vector2 scale = new Vector2(dist, 1.0f);

            spriteBatch.Draw(lineTexture, origin2d, null, color, angle, centre, scale, SpriteEffects.None, 0);
        }

        public void SetPoint(int line, int pt, Vector3 point)
        {
            points[line][pt] = point;
        }

        public void AddPoint(int line, Vector3 point)
        {
            points[line].Add(point);
        }

        public int AddLine(Color red)
        {
            points.Add(new List<Vector3>());
            colors.Add(red);
            return points.Count - 1;
        }

        public void Draw2D(UView3D view3D, SpriteBatch spriteBatch, GameTime gameTime)
        {
            for (int l = 0; l < points.Count; l++)
            {
                for (int p = 0; p < points[l].Count - 1; p++)
                {
                    DrawLine(spriteBatch, points[l][p], points[l][p + 1], colors[l], view3D);
                }
            }
        }

        public Vector3 GetPoint(int line, int i)
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
