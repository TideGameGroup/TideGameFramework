using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace Tide.Core
{
    public class TAudio : ISystem
    {
        private readonly UContentManager content;

        private readonly USettings settings;

        private readonly Dictionary<SoundEffectInstance, settingChangedEvent> soundEventTable
            = new Dictionary<SoundEffectInstance, settingChangedEvent>();

        private readonly Dictionary<string, SoundEffect> soundTable
            = new Dictionary<string, SoundEffect>();

        public TAudio(UContentManager content, USettings settings)
        {
            this.content = content;
            this.settings = settings;
        }

        private SoundEffect Get(string sound)
        {
            if (soundTable.ContainsKey(sound) == false)
            {
                Cache(sound);
            }

            if (soundTable.ContainsKey(sound))
            {
                return soundTable[sound];
            }

            return null;
        }

        public void Cache(string sound)
        {
            try
            {
                SoundEffect soundEffect = content.Load<SoundEffect>(sound);
                soundTable.TryAdd(sound, soundEffect);
            }
            catch (ContentLoadException)
            {
                return;
            }
        }

        public void Draw(TComponentGraph graph, GameTime gameTime)
        { }

        public SoundEffectInstance Play(string sound)
        {
            SoundEffectInstance instance = Get(sound).CreateInstance();
            instance.Play();

            settingChangedEvent volumeEvent = new settingChangedEvent(() =>
            {
                instance.Volume = settings["volume"].f;
            });

            settings.SetOnChangedCallback("volume", volumeEvent);
            soundEventTable[instance] = volumeEvent;

            volumeEvent.Invoke();
            return instance;
        }

        public void PlaySingle(string sound)
        {
            SoundEffect effect = Get(sound);
            if (effect != null)
            {
                effect.Play(settings["volume"].f, 0.0f, 0.0f);
            }
        }

        public void Stop(SoundEffectInstance instance)
        {
            if (soundEventTable.ContainsKey(instance))
            {
                settings.RemoveOnChangedCallback("volume", soundEventTable[instance]);
                instance.Stop();
            }
        }

        public void Update(TComponentGraph graph, GameTime gameTime)
        { }
    }
}