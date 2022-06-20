using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Tide.Core
{
    public class AImposterSpritesRendererComponent : UComponent, IUpdateComponent
    {
        private readonly int angleCount;
        private List<int> angleList;
        private ATransform2D transforms;

        public AImposterSpritesRendererComponent(ATransform2D transforms, int angles)
        {
            SpriteRenderer = new ASpritesRenderer(transforms);

            RegisterChildComponent(SpriteRenderer);

            this.transforms = transforms;

            angleCount = angles;
            angleList = new List<int>();
        }

        public int Count => transforms.Count;
        public ASpritesRenderer SpriteRenderer { get; private set; }

        public int AddSprite(string defaultAnimation = "")
        {
            return SpriteRenderer.Add(defaultAnimation);
        }

        public void AddSpriteAnimation(string name, int frames, int width, Texture2D texture, Rectangle source, bool shouldLoop = false)
        {
            SpriteRenderer.AddSpriteAnimation(name, frames, width, texture, source, shouldLoop);
        }

        public void Play(int i, string name, int startFrame = 0, bool forceLoop = false, float exactStart = 0.0f, bool clampToFrameCount = true)
        {
            SpriteRenderer.Play(i, name, startFrame, forceLoop, exactStart, clampToFrameCount);
        }

        public void SetShouldDraw(int i, bool val)
        {
            SpriteRenderer.SetShouldDraw(i, val);
        }

        public void Update(GameTime gameTime)
        {
            while (angleList.Count < Count)
            {
                angleList.Add(0);
            }
            for (int i = 0; i < Count; i++)
            {
                float angle = MathHelper.WrapAngle(transforms.angles[i]) + MathHelper.Pi;
                int a = (int)MathHelper.Lerp(0, angleCount - 1, angle / MathHelper.TwoPi);

                if (a != angleList[i])
                {
                    angleList[i] = a;

                    SpriteRenderer.Play(
                        i,
                        SpriteRenderer.GetPlaying(i),
                        SpriteRenderer.GetAnimationData(i).frames * a,
                        SpriteRenderer.GetLooping(i),
                        SpriteRenderer.GetElapsedTime(i),
                        false
                        );
                }
            }
        }
    }
}