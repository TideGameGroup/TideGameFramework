using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Tide.XMLSchema;

namespace Tide.Core
{
    public class ASpritesRenderer : UComponent, IDrawableComponent, ISerialisableComponent
    {
        private readonly Dictionary<string, FAnimationData> animations = new Dictionary<string, FAnimationData>();
        private readonly List<bool> bIsFinished = new List<bool>();
        private readonly List<bool> bShouldDraw = new List<bool>();
        private readonly List<Color> colors = new List<Color>();
        private readonly List<float> elapsedTimes = new List<float>();
        private readonly List<int> endFrames = new List<int>();
        private readonly List<float> frameRates = new List<float>();
        private readonly List<bool> loops = new List<bool>();
        private readonly List<string> names = new List<string>();
        private readonly Dictionary<string, Texture2D> nameTextureMap = new Dictionary<string, Texture2D>();
        private readonly List<float> scales = new List<float>();
        private readonly List<Rectangle> sources = new List<Rectangle>();
        private readonly List<int> startFrames = new List<int>();
        private readonly List<string> texIDs = new List<string>();
        private readonly ATransform transforms;
        private readonly List<int> widths = new List<int>();

        // flags
        public bool bViewAlignedSprites = false;

        public float globalScale = 1.0f;

        public ASpritesRenderer(ATransform transforms)
        {
            this.transforms = transforms;
        }

        public float FrameRate => 16.0f;

        public int Add(string defaultAnimation = "", float scale = 1.0f)
        {
            // get index
            int index = names.Count;

            // add empty entity data to lists
            bIsFinished.Add(false);
            bShouldDraw.Add(true);

            colors.Add(Color.White);
            names.Add("");
            startFrames.Add(0);
            texIDs.Add("");
            loops.Add(false);
            scales.Add(scale);
            endFrames.Add(0);
            widths.Add(0);
            frameRates.Add(FrameRate);
            sources.Add(new Rectangle());
            elapsedTimes.Add(0);

            // play animation on entity if animation is provided
            if (defaultAnimation != "")
            {
                Play(index, defaultAnimation, 0, false);
            }

            return index;
        }

        public void LoadSpriteAnimations(UContentManager content, string textureAnimationDataName)
        {
            FTextureAnimationData textureData = content.Load<FTextureAnimationData>(textureAnimationDataName);

            foreach (FSpriteAnimationData data in textureData.animations)
            {
                Texture2D textureAtlas = content.Load<Texture2D>(data.texture);
                AddSpriteAnimation(data, textureAtlas);
            }
        }

        public void AddSpriteAnimation(FSpriteAnimationData data, Texture2D texture)
        {
            nameTextureMap.TryAdd(texture.Name, texture);
            animations[data.name] = new FAnimationData
            {
                texID = texture.Name,
                frames = data.frame,
                frameRate = data.framerate == -1 ? FrameRate : data.framerate,
                width = data.width,
                loops = data.loop,
                source = data.source,
            };
        }

        public void AddSpriteAnimation(string name, int frames, int width, Texture2D texture, Rectangle source, bool loop = false, float framerate = -1.0f)
        {
            nameTextureMap.TryAdd(texture.Name, texture);
            animations[name] = new FAnimationData
            {
                texID = texture.Name,
                frames = frames,
                frameRate = framerate == -1 ? FrameRate : framerate,
                width = width,
                loops = loop,
                source = source,
            };
        }

        public void ClearFinished(Func<int, int> callback)
        {
            int i = 0;
            while (i < bIsFinished.Count)
            {
                if (bIsFinished[i])
                {
                    RemoveAt(i);
                    callback.Invoke(i);
                }
                else
                {
                    i++;
                }
            }
        }

        public void Draw2D(UViewport view, SpriteBatch spriteBatch, GameTime gameTime)
        {
            float ymod = view.position.Y + view.Scale / 2;
            float yalpha = 1 / view.Scale;

            for (int i = 0; i < transforms.Count; i++)
            {
                if (texIDs[i] == "" || !bShouldDraw[i]) { continue; }

                if (!nameTextureMap.TryGetValue(texIDs[i], out Texture2D tex))
                {
                    continue;
                }

                int frame = startFrames[i] + (int)(elapsedTimes[i] * frameRates[i]);

                // step frame?
                if (frame > endFrames[i])
                {
                    if (loops[i])
                    {
                        frame = startFrames[i];
                        elapsedTimes[i] = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    else
                    {
                        frame = endFrames[i]; // rest on last frame
                        bIsFinished[i] = true;
                    }
                }

                elapsedTimes[i] = elapsedTimes[i] + (float)gameTime.ElapsedGameTime.TotalSeconds;
                int x = frame % widths[i];
                int y = frame / widths[i];

                Rectangle source = sources[i];
                Rectangle centre = new Rectangle(0, 0, source.Width, source.Height);
                Rectangle offset = source;

                offset.Offset(source.Width * x, source.Height * y);

                spriteBatch.Draw(
                    tex,
                    transforms.positions[i] * new Vector2(1f, -1f),
                    offset,
                    colors[i],
                    bViewAlignedSprites ? 0 : transforms.angles[i],
                    centre.Center.ToVector2(),
                    1f,
                    SpriteEffects.None,
                    0f //(transforms.positions[i].Y + ymod) * yalpha
                );
            }
        }

        public FAnimationData GetAnimationData(int i)
        {
            return animations[names[i]];
        }

        public float GetElapsedTime(int i)
        {
            return elapsedTimes[i];
        }

        public int GetEndFrame(int i)
        {
            return endFrames[i];
        }

        public bool GetIsPlaying(int i)
        {
            int frame = startFrames[i] + (int)(elapsedTimes[i] * FrameRate);
            return frame <= endFrames[i];
        }

        public bool GetLooping(int i)
        {
            return loops[i];
        }

        public string GetPlaying(int i)
        {
            return names[i];
        }

        public int GetStartFrame(int i)
        {
            return startFrames[i];
        }

        public void Load(UContentManager content, string serialisedScriptPath)
        {
            if (serialisedScriptPath == "") { return; }

            FSprites spriteData = content.Load<FSprites>(serialisedScriptPath);

            for (int i = 0; i < spriteData.names.Length; i++)
            {
                Add(spriteData.names[i], spriteData.scales[i]);

                // overrides
                colors[i] = spriteData.colors[i];
                bShouldDraw[i] = spriteData.bShouldDraw[i];
                colors[i] = spriteData.colors[i];
                colors[i] = spriteData.colors[i];
            }
        }

        public void Play(int i, string name, int startFrame = 0, bool forceLoop = false, float exactStart = 0.0f, bool clampToFrameCount = true)
        {
            FAnimationData animdata = animations[name];

            names[i] = name;
            texIDs[i] = animdata.texID;
            startFrames[i] = startFrame;
            endFrames[i] = clampToFrameCount ? (animdata.frames - 1) : startFrame + (animdata.frames - 1);
            widths[i] = animdata.width;
            loops[i] = forceLoop || animdata.loops;
            frameRates[i] = animdata.frameRate;
            sources[i] = animdata.source;
            elapsedTimes[i] = exactStart;
        }

        public void RemoveAt(int i)
        {
            bIsFinished.RemoveAt(i);
            bShouldDraw.RemoveAt(i);

            colors.RemoveAt(i);
            elapsedTimes.RemoveAt(i);
            endFrames.RemoveAt(i);
            frameRates.RemoveAt(i);
            loops.RemoveAt(i);
            names.RemoveAt(i);
            scales.RemoveAt(i);
            sources.RemoveAt(i);
            startFrames.RemoveAt(i);
            texIDs.RemoveAt(i);
            widths.RemoveAt(i);
        }

        public string Serialise(string path, ref Dictionary<string, ISerialisedInstanceData> serialisedSet)
        {
            string ID = ISerialisableComponent.GetSerialisableID(this, path, ref serialisedSet);
            List<string> keys = new List<string>(animations.Keys);

            serialisedSet.Add(ID,
                new FSprites
                {
                    bShouldDraw = bShouldDraw.ToArray(),
                    colors = colors.ToArray(),
                    names = names.ToArray(),
                    scales = scales.ToArray()
                }
            );

            return ID;
        }

        public void OffsetSource(int i, int x, int y)
        {
            Rectangle r = sources[i];
            r.Offset(x, y);
            sources[i] = r;
        }

        public void SetColor(int i, Color color)
        {
            colors[i] = color;
        }

        public void SetShouldDraw(int i, bool val)
        {
            bShouldDraw[i] = val;
        }
    }
}